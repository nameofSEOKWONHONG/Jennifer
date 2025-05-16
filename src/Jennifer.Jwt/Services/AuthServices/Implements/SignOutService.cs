using System.IdentityModel.Tokens.Jwt;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.Bases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class SignOutService: ServiceBase<SignInService, bool, IResult>, ISignOutService
{
    private readonly UserManager<User> _userManager;
    private readonly HttpContext _httpContext;

    public SignOutService(ILogger<SignInService> logger,
        UserManager<User> userManager,
        IHttpContextAccessor accessor) : base(logger)
    {
        _userManager = userManager;
        _httpContext = accessor.HttpContext;
    }

    public async Task<IResult> HandleAsync(bool request, CancellationToken cancellationToken)
    {
        var id = _httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrWhiteSpace(id)) return Results.Unauthorized();
        
        var user = await _userManager.FindByIdAsync(id);
        if(user is null) return Results.Unauthorized();
        
        var result = await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        return Results.Ok(result.Succeeded);
    }
}