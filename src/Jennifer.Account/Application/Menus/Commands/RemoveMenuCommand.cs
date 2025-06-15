using System.Data;
using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Menus.Commands;

[UseTransaction(IsolationLevel.ReadUncommitted)]
public sealed record RemoveMenuCommand(Guid MenuId) : ICommand<Result>;

public sealed class RemoveMenuCommandHandler(JenniferDbContext dbContext):ICommandHandler<RemoveMenuCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveMenuCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Menus.FirstOrDefaultAsync(m => m.Id == command.MenuId, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        dbContext.Menus.Remove(exists);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}