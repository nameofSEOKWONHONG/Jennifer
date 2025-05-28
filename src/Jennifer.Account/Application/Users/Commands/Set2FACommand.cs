using eXtensionSharp;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed record Set2FACommand(Guid UserId, bool enable) : ICommand<Result>;

internal class Set2FACommandCommandHandler(UserManager<User> userManager): ICommandHandler<Set2FACommand, Result>
{
    public async ValueTask<Result> Handle(Set2FACommand command, CancellationToken cancellationToken)
    {
        var exists = await userManager.FindByIdAsync(command.UserId.ToString());
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found user");
        
        await userManager.SetTwoFactorEnabledAsync(exists, command.enable);
        
        return await Result.SuccessAsync();
    }
}