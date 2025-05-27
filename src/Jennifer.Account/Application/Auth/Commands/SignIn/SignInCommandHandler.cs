using System.Security.Claims;
using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.SignIn;

internal sealed class SignInCommandHandler(        
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService): ICommandHandler<SignInCommand, Result<TokenResponse>>
{
    public async ValueTask<Result<TokenResponse>> Handle(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user is null) return await Result<TokenResponse>.FailureAsync("not found user");

        var locked = await userManager.IsLockedOutAsync(user);
        if(locked) return await Result<TokenResponse>.FailureAsync("locked");
        
        if(!await userManager.CheckPasswordAsync(user, command.Password))
            return await Result<TokenResponse>.FailureAsync("wrong password");

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
        
        var encodedRefreshToken = jwtService.ObjectToTokenString(refreshTokenObj);
        var token = new TokenResponse(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
        return await Result<TokenResponse>.SuccessAsync(token);
    }
}