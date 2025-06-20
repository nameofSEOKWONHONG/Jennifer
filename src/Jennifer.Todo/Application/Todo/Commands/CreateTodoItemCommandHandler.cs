using eXtensionSharp;
using Jennifer.Domain.Todos;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Handler for creating todo items.
/// Validates that the item doesn't already exist and creates a new one if valid.
/// </summary>
public sealed class CreateTodoItemCommandHandler(
    TodoDbContext dbContext,
    ISessionContext session
): ICommandHandler<CreateTodoItemCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateTodoItemCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.AnyAsync(m => m.Id == command.Item.Id && m.UserId == user.Id, cancellationToken: cancellationToken);
        if(exists == true) return await Result<Guid>.FailureAsync("already exists");
        
        var newTodoItem = TodoItem.Create(user.Id, 
            command.Item.Title,
            command.Item.Description, 
            command.Item.DueDate, 
            command.Item.Labels, 
            command.Item.IsCompleted, 
            command.Item.CompletedAt, 
            (Priority)command.Item.Priority);
        await dbContext.TodoItems.AddAsync(newTodoItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        if (command.Item.SharedUsers.xIsNotEmpty())
        {
            var shareItems = new List<TodoItemShare>();
            foreach (var itemSharedUser in command.Item.SharedUsers)
            {
                shareItems.Add(TodoItemShare.Create(user.Id, newTodoItem.Id, itemSharedUser));
            }
            await dbContext.TodoItemShares.AddRangeAsync(shareItems, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        
        return await Result<Guid>.SuccessAsync(newTodoItem.Id);       
    }
}