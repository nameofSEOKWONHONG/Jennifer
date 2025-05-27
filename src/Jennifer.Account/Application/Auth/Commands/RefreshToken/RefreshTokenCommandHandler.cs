using System.Security.Claims;
using eXtensionSharp;
using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Account.Application.Auth.Commands.RefreshToken;

internal sealed class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService,
    IDistributedCache cache):ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
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
        
        var userClaims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    roleClaims.Add(claim); // ClaimType/Value 그대로
                }
            }
        }
        
        var newRefreshToken = jwtService.GenerateRefreshToken();
        var newRefreshTokenObj = new Services.Implements.RefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        
        var result = await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:newRefreshToken);
        if(!result.Succeeded) throw new ValidationException(result.Errors.Select(m => m.Description).First());

        var exists = await cache.GetAsync(user.Id.ToString(), cancellationToken);
        if(exists.xIsNotEmpty()) await cache.RefreshAsync(user.Id.ToString(), cancellationToken);
        
        await cache.SetStringAsync(user.Id.ToString(), user.xSerialize(), new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(JenniferOptionSingleton.Instance.Options.Jwt.ExpireMinutes)
        }, token: cancellationToken);
        
        var newToken = new TokenResponse(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), jwtService.ObjectToTokenString(newRefreshTokenObj));
        return await Result<TokenResponse>.SuccessAsync(newToken);
    }
}