using Jennifer.Account.Application.Auth.Commands.SignIn;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    IJwtService jwtService,
    ISender sender):ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshTokenObj = jwtService.TokenStringToObject(command.Token);
        if(refreshTokenObj is null) 
            return await Result<TokenResponse>.FailureAsync("not valid token.");
        
        if(refreshTokenObj.Expiry < DateTime.UtcNow) 
            return await Result<TokenResponse>.FailureAsync("expired");
        
        var user = await userManager.FindByIdAsync(refreshTokenObj.UserId);
        if(user is null) 
            return await Result<TokenResponse>.FailureAsync("not found user.");
        
        var token = await userManager.GetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(token is null) 
            return await Result<TokenResponse>.FailureAsync("not found refreshToken.");
        
        if(refreshTokenObj.Token != token) 
            return await Result<TokenResponse>.FailureAsync("not valid token.");
        
        return await sender.Send(new TokenGenerateCommand(user), cancellationToken);
    }
}