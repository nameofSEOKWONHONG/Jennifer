using System.Security.Claims;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class RefreshTokenService: ServiceBase<RefreshTokenService, string, IResult>, IRefreshTokenService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;

    public RefreshTokenService(ILogger<RefreshTokenService> logger,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService) : base(logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }

    public async Task<IResult> HandleAsync(string request, CancellationToken cancellationToken)
    {
        var refreshTokenObj = _jwtService.TokenStringToObject(request);
        if(refreshTokenObj is null) return Results.Unauthorized();
        
        if(refreshTokenObj.Expiry < DateTime.UtcNow) return Results.Unauthorized();
        
        var user = await _userManager.FindByIdAsync(refreshTokenObj.UserId);
        if(user is null) return Results.Unauthorized();
        
        var token = await _userManager.GetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(token is null) return Results.Unauthorized();
        
        if(refreshTokenObj.Token != token) return Results.Unauthorized();
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    roleClaims.Add(claim); // ClaimType/Value 그대로
                }
            }
        }
        
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenObj = new RefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:newRefreshToken);
        var newToken = new TokenResponse(_jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), _jwtService.ObjectToTokenString(newRefreshTokenObj));
        return Results.Ok(newToken);
    }
}