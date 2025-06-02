using eXtensionSharp;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Command to create a new todo item
/// </summary>
public sealed record CreateTodoItemCommand(TodoItemDto Item): ICommand<Result<Guid>>;

/// <summary>
/// Handler for creating todo items.
/// Validates that the item doesn't already exist and creates a new one if valid.
/// </summary>
public sealed class CreateTodoItemCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
    ): ICommandHandler<CreateTodoItemCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateTodoItemCommand itemCommand, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.AnyAsync(m => m.Id == itemCommand.Item.Id && m.UserId == user.Id, cancellationToken: cancellationToken);
        if(exists == true) return await Result<Guid>.FailureAsync("already exists");
        
        var newTodoItem = TodoItem.Create(user.Id, 
            itemCommand.Item.Description, 
            itemCommand.Item.DueDate, 
            itemCommand.Item.Labels, 
            itemCommand.Item.IsCompleted, 
            itemCommand.Item.CompletedAt, 
            itemCommand.Item.Priority);
        await dbContext.TodoItems.AddAsync(newTodoItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        if (itemCommand.Item.SharedUsers.xIsNotEmpty())
        {
            var shareItems = new List<TodoItemShare>();
            foreach (var itemSharedUser in itemCommand.Item.SharedUsers)
            {
                shareItems.Add(TodoItemShare.Create(user.Id, newTodoItem.Id, itemSharedUser));
            }
            await dbContext.TodoItemShares.AddRangeAsync(shareItems, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return await Result<Guid>.SuccessAsync(newTodoItem.Id);       
    }
}