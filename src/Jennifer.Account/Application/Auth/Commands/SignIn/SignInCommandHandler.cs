using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

internal sealed class SignInCommandHandler(        
    UserManager<User> userManager,
    IServiceExecutionBuilderFactory factory): ICommandHandler<SignInCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user is null) return await Result<TokenResponse>.FailureAsync("not found user");

        var locked = await userManager.IsLockedOutAsync(user);
        if(locked) return await Result<TokenResponse>.FailureAsync("locked");
        
        if(!await userManager.CheckPasswordAsync(user, command.Password))
            return await Result<TokenResponse>.FailureAsync("wrong password");

        if (user.TwoFactorEnabled)
        {
            return await Result<TokenResponse>.SuccessAsync(new TokenResponse(user.Id.ToString(), null, true));
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