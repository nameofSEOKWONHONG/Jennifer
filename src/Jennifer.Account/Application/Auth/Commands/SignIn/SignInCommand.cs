﻿using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Infrastructure.Options;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Hybrid;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password):ICommand<Result<TokenResponse>>;

internal sealed class SignInCommandHandler(        
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService,
    IDistributedCache cache): ICommandHandler<SignInCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user is null) return Result<TokenResponse>.Failure("not found user");

        var locked = await userManager.IsLockedOutAsync(user);
        if(locked) return Result<TokenResponse>.Failure("locked");
        
        if(!await userManager.CheckPasswordAsync(user, command.Password))
            return Result<TokenResponse>.Failure("wrong password");

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

        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenObj = new Services.Implements.RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        
        var result = await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        if(!result.Succeeded) throw new ValidationException(result.Errors.Select(m => m.Description).First());

        var exists = await cache.GetAsync(user.Id.ToString(), cancellationToken);
        if (exists.xIsNotEmpty()) await cache.RemoveAsync(user.Id.ToString(), cancellationToken);
        await cache.SetStringAsync(user.Id.ToString(), user.xSerialize(), new DistributedCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromMinutes(JenniferOptionSingleton.Instance.Options.Jwt.ExpireMinutes)
        }, token: cancellationToken);    
        
        var encodedRefreshToken = jwtService.ObjectToTokenString(refreshTokenObj);
        var token = new TokenResponse(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
        return Result<TokenResponse>.Success(token);
    }
}