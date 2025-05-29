using eXtensionSharp;
using Jennifer.Account.Application.Auth.Commands.SignIn;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Domain.Account;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.ExternalOAuth;

internal sealed class ExternalOAuthHandler(
    UserManager<User> userManager,
    IExternalOAuthHandlerFactory externalOAuthHandlerFactory,
    ISender sender): ICommandHandler<ExternalOAuthCommand, Result<TokenResponse>>
{

    public async ValueTask<Result<TokenResponse>> Handle(ExternalOAuthCommand command, CancellationToken cancellationToken)
    {
        // 1. 외부 서비스에 access_token을 전달하여 사용자 정보 확인
        var instance = externalOAuthHandlerFactory.Resolve(command.Provider);
        var verified = await instance.Verify(command.AccessToken, cancellationToken);
        if (!verified.IsSuccess)
            return await Result<TokenResponse>.FailureAsync("Verify failed.");

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

        return await sender.Send(new TokenGenerateCommand(user), cancellationToken);
    }
}