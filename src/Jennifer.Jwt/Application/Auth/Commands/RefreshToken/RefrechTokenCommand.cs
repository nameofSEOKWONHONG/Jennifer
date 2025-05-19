using System.Security.Claims;
using FluentValidation;
using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string Token):ICommand<IResult>;

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService):ICommandHandler<RefreshTokenCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshTokenObj = jwtService.TokenStringToObject(command.Token);
        if(refreshTokenObj is null) 
            return TypedResults.BadRequest(TokenResponse.Fail("Not found"));
        
        if(refreshTokenObj.Expiry < DateTime.UtcNow) 
            return TypedResults.BadRequest(TokenResponse.Fail("Expired"));
        
        var user = await userManager.FindByIdAsync(refreshTokenObj.UserId);
        if(user is null) 
            return TypedResults.BadRequest(TokenResponse.Fail("Not found"));
        
        var token = await userManager.GetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(token is null) 
            return TypedResults.BadRequest(TokenResponse.Fail("Not found"));
        
        if(refreshTokenObj.Token != token) 
            return TypedResults.BadRequest(TokenResponse.Fail("Not found"));
        
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
        var newToken = TokenResponse.Success(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), jwtService.ObjectToTokenString(newRefreshTokenObj));
        return TypedResults.Ok(newToken); 
    }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(m => m.Token).NotEmpty();
    }
}