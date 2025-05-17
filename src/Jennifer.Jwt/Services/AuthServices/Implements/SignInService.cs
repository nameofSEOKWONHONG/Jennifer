using System.Security.Claims;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class SignInService: ServiceBase<SignInService, SignInRequest, IResult>, ISignInService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;

    public SignInService(ILogger<SignInService> logger, 
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService) : base(logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }

    public async Task<IResult> HandleAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if(user is null) Results.Unauthorized();

        var locked = await _userManager.IsLockedOutAsync(user);
        if(locked) return Results.Unauthorized();
        
        if(!await _userManager.CheckPasswordAsync(user, request.Password)) return Results.Unauthorized();

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

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenObj = new RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        var encodedRefreshToken = _jwtService.ObjectToTokenString(refreshTokenObj);
        var token = new TokenResponse(_jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
        return Results.Ok(token);
    }
}