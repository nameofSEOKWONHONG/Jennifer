using System.Security.Claims;
using eXtensionSharp;
using FluentValidation;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.ExternalOAuth;

public sealed record ExternalOAuthCommand(string Provider, string AccessToken):ICommand<TokenResponse>;

public class ExternalOAuthHandler(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IExternalOAuthHandlerFactory externalOAuthHandlerFactory,
    IJwtService jwtService): ICommandHandler<ExternalOAuthCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(ExternalOAuthCommand command, CancellationToken cancellationToken)
    {
        // 1. 외부 서비스에 access_token을 전달하여 사용자 정보 확인
        var instance = externalOAuthHandlerFactory.Resolve(command.Provider);
        var verified = await instance.Verify(command.AccessToken, cancellationToken);
        if (!verified.IsSuccess)
            return Result<TokenResponse>.Failure(Error.NotFound(string.Empty, "Not found"));

        // 2. provider별 ID 가져오기
        string providerId = verified.ExternalId;
        if (providerId.xIsEmpty()) return null;

        // 3. 이미 연결된 외부 로그인 사용자 확인
        var user = await userManager.FindByLoginAsync(command.Provider, providerId);
        if (user is null)
        {
            // 4. 동일 이메일의 기존 사용자 확인
            user = await userManager.FindByEmailAsync(verified.Email);

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

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded) return null;
            }

            // 6. 외부 로그인 정보 연결
            var loginInfo = new UserLoginInfo(command.Provider, providerId, command.Provider);
            var linkResult = await userManager.AddLoginAsync(user, loginInfo);
            if (!linkResult.Succeeded) return null;
        }

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
        await userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        return TokenResponse.Success(jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), jwtService.ObjectToTokenString(refreshTokenObj));
    }
}

public class ExternalOAuthCommandValidator : AbstractValidator<ExternalOAuthCommand>
{
    public ExternalOAuthCommandValidator()
    {
        RuleFor(m => m.Provider).NotEmpty();
        RuleFor(m => m.AccessToken).NotEmpty();
    }
}