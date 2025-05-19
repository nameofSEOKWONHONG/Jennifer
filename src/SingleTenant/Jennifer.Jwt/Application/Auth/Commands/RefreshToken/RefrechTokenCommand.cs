using System.Security.Claims;
using FluentValidation;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token):ICommand<TokenResponse>;

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService):ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshTokenObj = jwtService.TokenStringToObject(command.Token);
        if(refreshTokenObj is null) 
            return Result<TokenResponse>.Failure(Error.NotFound(string.Empty, "Not found"));
        
        if(refreshTokenObj.Expiry < DateTime.UtcNow) 
            return Result<TokenResponse>.Failure(Error.Failure(string.Empty, "Expired"));
        
        var user = await userManager.FindByIdAsync(refreshTokenObj.UserId);
        if(user is null) 
            return Result<TokenResponse>.Failure(Error.NotFound(string.Empty, "Not found"));
        
        var token = await userManager.GetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(token is null) 
            return Result<TokenResponse>.Failure(Error.NotFound(string.Empty, "Not found"));
        
        if(refreshTokenObj.Token != token) 
            return Result<TokenResponse>.Failure(Error.NotFound(string.Empty, "Not found"));
        
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
        await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:newRefreshToken);
        return TokenResponse.Success(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), jwtService.ObjectToTokenString(newRefreshTokenObj));
    }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(m => m.Token).NotEmpty();
    }
}