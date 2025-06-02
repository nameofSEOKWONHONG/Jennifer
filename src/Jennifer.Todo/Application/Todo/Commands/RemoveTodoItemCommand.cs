using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

/// <summary>
/// Command to remove a todo item
/// </summary>
public sealed record RemoveTodoItemCommand(Guid Id):ICommand<Result>;

/// <summary>
/// Handler for removing todo items
/// </summary>
public sealed class RemoveTodoItemCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
):ICommandHandler<RemoveTodoItemCommand, Result>
{
    /// <summary>
    /// Handles the removal of a todo item
    /// </summary>
    /// <param name="command">Command containing the todo item ID to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result if item was removed, Failure if not found</returns>
    public async ValueTask<Result> Handle(RemoveTodoItemCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.Current.GetAsync();
        var exists = await dbContext.TodoItems.FirstOrDefaultAsync(m => m.Id == command.Id && m.UserId == user.Id, cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        dbContext.TodoItems.Remove(exists!);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();       
    }
}