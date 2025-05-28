using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.TwoFactor;

internal sealed class Verify2FACommandHandler(
    UserManager<User> userManager,
    IServiceExecutionBuilderFactory factory
) : ICommandHandler<Verify2FACommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(Verify2FACommand command, CancellationToken cancellationToken)
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

        Result<TokenResponse> result = null;
        
        var builder = factory.Create();
        await builder.Register<IGenerateTokenService, User, Result<TokenResponse>>()
            .Request(user)
            .When(() => user.xIsNotEmpty())
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);

        return result;
    }
}
