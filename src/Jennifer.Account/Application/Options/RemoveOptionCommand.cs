using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options;

public sealed record RemoveOptionCommand(int Id): ICommand<Result>;
public sealed class RemoveOptionCommandHandler(JenniferDbContext dbContext): ICommandHandler<RemoveOptionCommand, Result>
{
    public async ValueTask<Result> Handle(RemoveOptionCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Options.Where(m => m.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if (exists.xIsEmpty()) return await Result.FailureAsync("not found");

        dbContext.Options.Remove(exists);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}