using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Handler for processing UpdateTodoCommand
/// </summary>
public sealed class UpdateTodoItemCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
): ICommandHandler<UpdateTodoItemCommand, Result>
{
    /// <summary>
    /// Updates an existing todo item with new values from the command
    /// </summary>
    /// <param name="itemCommand">Command containing updated todo item details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result if update successful, Failure if item not found</returns>
    public async ValueTask<Result> Handle(UpdateTodoItemCommand itemCommand, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.FirstOrDefaultAsync(m => m.Id == itemCommand.Item.Id && m.UserId == user.Id, cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");

        exists.Description = itemCommand.Item.Description; 
        exists.DueDate = itemCommand.Item.DueDate;  
        exists.Labels = itemCommand.Item.Labels;  
        exists.IsCompleted = itemCommand.Item.IsCompleted;  
        exists.CompletedAt = itemCommand.Item.CompletedAt;  
        exists.Priority = itemCommand.Item.Priority;
        
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();       
    }
}