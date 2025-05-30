using eXtensionSharp;
using Jennifer.Account.Application.Auth.Commands.SignIn;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

public sealed class Verify2FaCommandHandler(
    UserManager<User> userManager,
    ISender sender
) : ICommandHandler<Verify2FaCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(Verify2FaCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(command.UserId.ToString());
        if (user.xIsEmpty()) 
            return await Result<TokenResponse>.FailureAsync("User not found");
        
        if (user!.AuthenticatorKey.xIsEmpty()) 
            return await Result<TokenResponse>.FailureAsync("No secret configured");

        var isValid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            TokenOptions.DefaultAuthenticatorProvider,
            command.Code
        );
        if (!isValid) return await Result<TokenResponse>.FailureAsync("Invalid 2FA code");

        if (!user.TwoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            await userManager.UpdateAsync(user);
        }

        return await sender.Send(new TokenGenerateCommand(user), cancellationToken);
    }
}
