using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed class SignInCommandHandler(        
    UserManager<User> userManager,
    ISender sender): ICommandHandler<SignInCommand, Result<TokenResponse>>
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
        
        return await sender.Send(new TokenGenerateCommand(user), cancellationToken);
    }
}