using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Jennifer.Todo.Application.Todo.Contracts;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

public sealed record UpdateTodoCommand(TodoItemDto Item): ICommand<Result>;
public sealed class UpdateTodoCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
): ICommandHandler<UpdateTodoCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var exists = await dbContext.TodoItems.FirstOrDefaultAsync(m => m.Id == command.Item.Id && m.UserId == user.Id, cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");

        exists.Description = command.Item.Description; 
        exists.DueDate = command.Item.DueDate;  
        exists.Labels = command.Item.Labels;  
        exists.IsCompleted = command.Item.IsCompleted;  
        exists.CompletedAt = command.Item.CompletedAt;  
        exists.Priority = command.Item.Priority;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();       
    }
}