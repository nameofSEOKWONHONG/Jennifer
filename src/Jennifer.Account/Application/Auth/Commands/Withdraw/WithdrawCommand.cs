using eXtensionSharp;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.Withdraw;

public sealed record WithdrawCommand():ICommand<Result>;
public sealed class WithdrawCommandHandler(
    JenniferDbContext dbContext,
    ISessionContext session
    ): ICommandHandler<WithdrawCommand, Result>
{
    public async ValueTask<Result> Handle(WithdrawCommand request, CancellationToken cancellationToken)
    {
        var user = await session.User.GetAsync();
        var exists = await dbContext.Users.Where(m => m.Id == user.Id)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found");
        
        exists.Delete(user.Id.ToString());

        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}