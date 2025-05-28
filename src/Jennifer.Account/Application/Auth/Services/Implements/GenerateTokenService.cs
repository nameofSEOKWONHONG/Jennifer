using System.Security.Claims;
using FluentValidation;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Models;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Services.Implements;

internal sealed class GenerateTokenService(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IJwtService jwtService    
): ServiceBase<User, Result<TokenResponse>>, IGenerateTokenService
{
    protected override async Task<Result<TokenResponse>> HandleAsync(User request, CancellationToken cancellationToken)
    {
        var userClaims = await userManager.GetClaimsAsync(request);
        var roles = await userManager.GetRolesAsync(request);
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
        var refreshTokenObj = new Services.Implements.RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, request.Id.ToString());
        
        var result = await userManager.SetAuthenticationTokenAsync(request, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        if(!result.Succeeded) throw new ValidationException(result.Errors.Select(m => m.Description).First());
        
        var encodedRefreshToken = jwtService.ObjectToTokenString(refreshTokenObj);
        var token = new TokenResponse(jwtService.GenerateJwtToken(request, userClaims.ToList(), roleClaims), encodedRefreshToken, isTwoFactor:request.TwoFactorEnabled);
        return await Result<TokenResponse>.SuccessAsync(token);
    }
} 