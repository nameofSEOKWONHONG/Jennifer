using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Commands;

public sealed class UpdateOptionCommandHandler(
    ISessionContext session,
    JenniferDbContext dbContext): ICommandHandler<UpdateOptionCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateOptionCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Options.Where(m => m.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        exists.Type = command.Type;
        exists.Value = command.Value;
        await dbContext.SaveChangesAsync(cancellationToken);

        await session.Option.ClearAsync();
        
        return await Result.SuccessAsync();       
    }
}