using eXtensionSharp;
using Jennifer.Domain.Account;
using Jennifer.Domain.Account.Contracts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Options.Commands;

public sealed record UpdateOptionCommand(int Id, ENUM_ACCOUNT_OPTION Type, string Value): ICommand<Result>;
public sealed class UpdateOptionCommandHandler(JenniferDbContext dbContext): ICommandHandler<UpdateOptionCommand, Result>
{
    public async ValueTask<Result> Handle(UpdateOptionCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Options.Where(m => m.Id == command.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        exists.Type = command.Type;
        exists.Value = command.Value;
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();       
    }
}