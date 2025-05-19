using System.Security.Claims;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignIn;

public sealed record SignInCommand(string Email, string Password):ICommand<TokenResponse>;

public class SignInCommandHandler(        
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService): ICommandHandler<SignInCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(SignInCommand command, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(command.Email);
        if(user is null) return TokenResponse.Fail("Not found");

        var locked = await userManager.IsLockedOutAsync(user);
        if(locked) return TokenResponse.Fail("Locked");
        
        if(!await userManager.CheckPasswordAsync(user, command.Password))
            return TokenResponse.Fail("Password is wrong");

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
        user.SecurityStamp = Guid.NewGuid().ToString();
        await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        var encodedRefreshToken = jwtService.ObjectToTokenString(refreshTokenObj);
        return TokenResponse.Success(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
    }
}