﻿using System.Security.Claims;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Extensions;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed record TokenGenerateCommand(User User):ICommand<Result<TokenResponse>>;
public sealed class TokenGenerateCommandHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    ITokenService tokenService,
    IDistributedCache cache
    ):ICommandHandler<TokenGenerateCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(TokenGenerateCommand command, CancellationToken cancellationToken)
    {
        var userClaims = await userManager.GetClaimsAsync(command.User);
        var roles = await userManager.GetRolesAsync(command.User);
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

        var refreshToken = tokenService.GenerateRefreshToken();
        var refreshTokenObj = new Services.Implements.RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, command.User.Id.ToString());
        
        var result = await userManager.SetAuthenticationTokenAsync(command.User, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        var error = new ValidationError(result.Errors.Select(x => new Error(x.Code, x.Description))
            .ToArray());
        if (!result.Succeeded) return await Result<TokenResponse>.FailureAsync(error);

        var sid = UlidGenerator.Instance.GenerateString();
        var encodedRefreshToken = tokenService.ObjectToTokenString(refreshTokenObj);
        var token = new TokenResponse(tokenService.GenerateJwtToken(sid, command.User, userClaims.ToList(), roleClaims), encodedRefreshToken, isTwoFactor:command.User.TwoFactorEnabled);

        await cache.SetCacheUserSid(CachingConsts.SidCacheKey(sid), command.User.Id.ToString(), TimeSpan.FromMinutes(30), cancellationToken);
        
        return await Result<TokenResponse>.SuccessAsync(token);
    }
}