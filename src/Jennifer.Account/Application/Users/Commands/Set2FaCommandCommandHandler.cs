using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Users.Commands;

public class Set2FaCommandCommandHandler(UserManager<User> userManager,
    ISessionContext session): ICommandHandler<Set2FaCommand, Result>
{
    public async ValueTask<Result> Handle(Set2FaCommand command, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByIdAsync(command.UserId.ToString());
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found user");
        
        await userManager.SetTwoFactorEnabledAsync(exists, command.enable);

        await session.User.ClearAsync();
        
        return await Result.SuccessAsync();
    }
}