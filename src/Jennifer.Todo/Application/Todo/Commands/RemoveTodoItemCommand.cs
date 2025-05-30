using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Todo.Application.Todo.Commands;

public sealed record RemoveTodoItemCommand(Guid Id):ICommand<Result>;
public sealed class RemoveTodoItemCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
):ICommandHandler<RemoveTodoItemCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveTodoItemCommand command, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var exists = await dbContext.TodoItems.FirstOrDefaultAsync(m => m.Id == command.Id && m.UserId == user.Id, cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        dbContext.TodoItems.Remove(exists!);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();       
    }
}