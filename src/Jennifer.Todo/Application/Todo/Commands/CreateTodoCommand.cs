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
public sealed record CreateTodoCommand(TodoItemDto Item): ICommand<Result<Guid>>;

/// <summary>
/// Handler for creating todo items.
/// Validates that the item doesn't already exist and creates a new one if valid.
/// </summary>
public sealed class CreateTodoCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
    ): ICommandHandler<CreateTodoCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var exists = await dbContext.TodoItems.AnyAsync(m => m.Id == command.Item.Id && m.UserId == user.Id, cancellationToken: cancellationToken);
        if(exists == true) return await Result<Guid>.FailureAsync("already exists");
        
        var newTodoItem = TodoItem.Create(user.Id, 
            command.Item.Description, 
            command.Item.DueDate, 
            command.Item.Labels, 
            command.Item.IsCompleted, 
            command.Item.CompletedAt, 
            command.Item.Priority);
        await dbContext.TodoItems.AddAsync(newTodoItem, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Result<Guid>.SuccessAsync(newTodoItem.Id);       
    }
}