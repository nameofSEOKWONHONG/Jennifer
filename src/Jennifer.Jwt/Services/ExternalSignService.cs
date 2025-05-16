using System.Security.Claims;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Infrastructure.SignHandlers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Jennifer.Jwt.Services;

public class ExternalSignService: IExternalSignService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly JwtService _jwtService;
    private readonly IHttpClientFactory _httpClientFactory;

    public ExternalSignService(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<JwtService> options, IHttpClientFactory httpClientFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtService = options.Value;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TokenResponse> SignIn(string provider, string providerToken, CancellationToken ct)
    {
        // 1. 외부 서비스에 access_token을 전달하여 사용자 정보 확인
        var instance = ExternalSignHandlerFactory.Create(provider, _httpClientFactory);
        var verified = await instance.Verify(providerToken, ct);
        if (verified is null) return null;

        // 2. provider별 ID 가져오기
        string providerId = verified.ProviderId;

        // 3. 이미 연결된 외부 로그인 사용자 확인
        var user = await _userManager.FindByLoginAsync(provider, providerId);
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
            var loginInfo = new UserLoginInfo(provider, providerId, provider);
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

public record ExternalSignInRequest(string Provider, string ProviderToken);