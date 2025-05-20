using System.Security.Claims;
using eXtensionSharp;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Session;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jennifer.Jwt.Application.Auth.Services.Implements;

/// <summary>
/// Represents a service that handles external sign-in operations using third-party providers.
/// </summary>
public class ExternalOAuthService: ServiceBase<ExternalOAuthService, ExternalSignInRequest, TokenResponse>, IExternalOAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IExternalOAuthHandlerFactory _externalOAuthHandlerFactory;
    private readonly JwtService _jwtService;

    /// <summary>
    /// Handles external sign-in operations using third-party authentication providers.
    /// </summary>
    /// <remarks>
    /// This service processes the external authentication requests by validating the provided token
    /// with the third-party provider, retrieves user and role information,
    /// and generates necessary authentication tokens for the application.
    /// </remarks>
    public ExternalOAuthService(ILogger<ExternalOAuthService> logger,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IOptions<JwtService> options,
        IExternalOAuthHandlerFactory externalOAuthHandlerFactory)
        : base(logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _externalOAuthHandlerFactory = externalOAuthHandlerFactory;
        _jwtService = options.Value;
    }

    /// <summary>
    /// Handles external authentication requests by validating tokens with third-party providers,
    /// associating users with external login information, and generating authentication tokens for the application.
    /// </summary>
    /// <param name="request">The external sign-in request containing the provider and provider token.</param>
    /// <param name="cancellationToken">Cancellation token for asynchronous operation handling.</param>
    /// <returns>A result containing authentication tokens or null if the process fails.</returns>
    public async Task<TokenResponse> HandleAsync(ExternalSignInRequest request, CancellationToken cancellationToken)
    {
        // 1. 외부 서비스에 access_token을 전달하여 사용자 정보 확인
        var instance = _externalOAuthHandlerFactory.Resolve(request.Provider);
        var verified = await instance.Verify(request.AccessToken, cancellationToken);
        if (!verified.IsSuccess) return null;

        // 2. provider별 ID 가져오기
        string providerId = verified.ExternalId;
        if (providerId.xIsEmpty()) return null;

        // 3. 이미 연결된 외부 로그인 사용자 확인
        var user = await _userManager.FindByLoginAsync(request.Provider, providerId);
        if (user is null)
        {
            // 4. 동일 이메일의 기존 사용자 확인
            user = await _userManager.FindByEmailAsync(verified.Email);

            if (user is null)
            {
                // 5. 새 사용자 생성
                user = new User
                {
                    UserName = verified.Name,
                    Email = verified.Email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded) return null;
            }

            // 6. 외부 로그인 정보 연결
            var loginInfo = new UserLoginInfo(request.Provider, providerId, request.Provider);
            var linkResult = await _userManager.AddLoginAsync(user, loginInfo);
            if (!linkResult.Succeeded) return null;
        }

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
        return new TokenResponse(_jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), _jwtService.ObjectToTokenString(refreshTokenObj));
    }
}