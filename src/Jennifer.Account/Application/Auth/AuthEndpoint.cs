using Jennifer.Account.Application.Auth.Commands.CheckEmail;
using Jennifer.Account.Application.Auth.Commands.ExternalOAuth;
using Jennifer.Account.Application.Auth.Commands.Password;
using Jennifer.Account.Application.Auth.Commands.RefreshToken;
using Jennifer.Account.Application.Auth.Commands.SignIn;
using Jennifer.Account.Application.Auth.Commands.SignOut;
using Jennifer.Account.Application.Auth.Commands.SignUp;
using Jennifer.Account.Application.Auth.Commands.TwoFactor;
using Jennifer.Account.Application.Auth.Contracts;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Account.Application.Auth;

/// <summary>
/// Provides functionality for mapping authentication-related API endpoints
/// to an <see cref="IEndpointRouteBuilder"/>. These endpoints include user
/// authentication operations such as signup, signin, signout, password management,
/// and token refresh.
/// </summary>
public static class AuthEndpoint
{
    /// <summary>
    /// Configures the authentication-related API endpoints to the provided endpoint route builder.
    /// These endpoints facilitate operations such as user authentication, session management, and identity verification.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> used to define and organize the API routes.</param>
    public static void MapAuthEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth")
            .WithGroupName("v1")
            .WithTags("Auth");

        group.MapPost("/check",
                async (CheckByEmailRequest request, ISender sender, CancellationToken ct) =>
                    await sender.Send(new CheckByEmailQuery(request.Email), ct))
            .WithName("CheckEmail")
            .Produces<bool>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .WithDescription(@"
            제공된 이메일 주소가 회원가입이 가능한지 확인합니다.
            이메일이 신규 계정으로 사용 가능한 경우 true를 반환하고, 이미 등록된 경우 false를 반환합니다.
            이 엔드포인트는 중복 등록을 방지하고 시스템의 이메일 고유성을 보장합니다.
")
            ;

        #region [sign up process]
        
        /*
         * [메일 주소 증명]
         * 1. 회원 가입 및 가입 확인 메일 발송 : /signup (인증 코드 포함)
         * 2. 회원 메일 확인에 따라 주소 클릭 후 페이지 접근에 따라 확정 : /signup/verify
         * 
         * [메일 코드 증명]
         * 1. 회원 가입 및 가입 확인 메일 발송 : /signup (인증 코드 포함)
         * 2. 회원 메일 확인에 따라 코드 확인 후 페이지 입력 및 확정 : /signup/verify
         */

        group.MapPost("/signup", 
                async (RegisterRequest request,
                        ISender sender, CancellationToken ct) => 
                    await sender.Send(
                        new SignUpCommand(request.Email, request.Password, request.UserName, request.PhoneNumber, request.Type), ct))
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .WithDescription(@"
            제공된 정보로 새 사용자 계정을 생성합니다.
            성공적인 등록 시 JWT 및 갱신 토큰이 포함된 토큰 응답을 반환합니다.
            등록 절차를 완료하기 위한 이메일 인증이 발송됩니다.
")
            .WithName("SignUp");
        
        group.MapPost("/signup/verify", 
                async (SignUpVerifyRequest request, ISender sender, CancellationToken ct) => 
                    await sender.Send(new SignUpVerifyCommand(request.UserId, request.Code), ct))
            .WithName("SignUpVerify")
            .WithDescription(@"
            회원가입시 발급된 인증코드를 확인합니다.
            인증코드가 일치하면 계정이 활성화됩니다.
            인증코드가 일치하지 않으면 실패를 반환합니다.
");
        
        // group.MapGet("/signup/verify/{id}/{code}", 
        //         async (Guid id, string code, ICommandHandler<SignUpVerifyCommand, IResult> handler, CancellationToken ct) => 
        //             await handler.HandleAsync(new SignUpVerifyCommand(id, code), ct))
        //     .WithName("SignUpVerify");

        group.MapPost("/signup/retry",
            async (SignUpRetryRequest request, ISender sender,
                    CancellationToken ct) =>
                await sender.Send(new SignUpRetryCommand(request.Email), ct))
            .WithName("SignUpRetry")
            .WithDescription(@"
            회원가입 인증 메일을 재발송합니다.
            인증 메일이 도착하지 않았거나 만료된 경우 사용됩니다.
            등록된 이메일 주소로 새로운 인증 코드가 발송됩니다.
");

        #endregion
        
        #region [admin sign up process]

        group.MapPost("/signup/admin",
                async (RegisterAdminRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new SignUpAdminCommand(request.Email, request.Password, request.UserName, request.PhoneNumber);
                    return await sender.Send(command, ct);
                })
            .WithName("SignUpAdmin")
            .WithDescription(@"
            관리자 계정을 생성합니다.
            관리자 권한이 있는 사용자만 접근 가능합니다.
            관리자 계정 생성 후 즉시 활성화되며 별도의 인증이 필요하지 않습니다.
");
        
        #endregion
        
        group.MapPost("/signin", 
            async (SignInRequest request, ISender sender, CancellationToken ct) => 
                await sender.Send(new SignInCommand(request.Email, request.Password), ct))
            .WithName("SignIn")
            .WithDescription(@"
이메일과 비밀번호로 로그인합니다.
JWT 토큰과 갱신 토큰을 반환합니다.
갱신 토큰을 사용하여 새로운 JWT 토큰을 얻을 수 있습니다.
갱신 토큰은 7일 후 만료됩니다.");
        
        group.MapPost("/signin/external", 
                async (ExternalSignInRequest request, ISender sender, CancellationToken ct) =>
                    await sender.Send(new ExternalOAuthCommand(request.Provider, request.AccessToken), ct))
            .WithName("ExternalSignIn")
            .WithDescription(@"
            외부 OAuth 공급자를 통해 로그인합니다.
            지원되는 공급자의 액세스 토큰으로 인증합니다.
            성공시 JWT 토큰과 갱신 토큰을 반환합니다.
");        
        
        group.MapPost("/signout",
            async (ISender sender, CancellationToken ct) => 
                await sender.Send(new SignOutCommand(true), ct))
            .WithName("SignOut")
            .Produces(StatusCodes.Status200OK)
            .WithDescription("현재 인증된 사용자를 로그아웃하고 토큰을 무효화합니다.")
            .RequireAuthorization();

        group.MapPost("/refreshtoken",
                async (string refreshToken, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new RefreshTokenCommand(refreshToken), ct))
            .WithName("RefreshToken")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .WithDescription("유효한 갱신 토큰을 사용하여 새로운 JWT 토큰을 생성합니다.");
        
        #region [none login state change password]
        
        /*
         * 1. 인증코드 생성 및 메일 전송 : /password/forgot
         * 2. 인증코드 확인 : /password/forgot/verify
         * 3. 인증코드 변경 : /password/forgot/change
         */
        
        group.MapPost("/password/forgot",
                async (PasswordForgotRequest request, ISender sender, CancellationToken ct) =>
                    await sender.Send(new PasswordForgotCommand(request.Email, request.UserName), ct))
            .WithName("PasswordForgot")
            .WithDescription(@"
            비밀번호 재설정을 위한 인증 코드를 생성하고 이메일로 발송합니다.
            이메일과 사용자 이름이 일치하는 계정이 있어야 합니다.
            인증 코드는 제한된 시간 동안만 유효합니다.
");
        
        group.MapPost("/password/forgot/verify",
                async (VerifyCodeRequest request, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new PasswordForgotVerifyCommand(request.Email, request.Code), ct))
            .WithName("PasswordForgotVerify")
            .WithDescription(@"
            비밀번호 재설정을 위해 발급된 인증 코드를 확인합니다.
            이메일 주소와 인증 코드가 일치해야 합니다.
            인증에 성공하면 비밀번호를 변경할 수 있습니다.
");

        group.MapPost("/password/forgot/change",
                async (PasswordForgotChangeRequest request, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new PasswordForgotChangeCommand(request.Email,  request.Code, request.NewPassword), ct))
            .WithName("PasswordForgotChange")
            .WithDescription(@"
            인증된 코드로 새 비밀번호로 변경합니다.
            이메일 주소와 인증 코드가 일치해야 합니다.
            새 비밀번호는 보안 요구사항을 충족해야 합니다.
");

        #endregion
        
        group.MapPost("/password/change",
            async (PasswordChangeRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new PasswordChangeCommand(request.OldPassword, request.NewPassword), ct);
            })
            .WithName("PasswordChange")
            .RequireAuthorization()
            .WithDescription(@"
            로그인된 사용자의 비밀번호를 변경합니다.
            현재 비밀번호와 새 비밀번호가 필요합니다.
            새 비밀번호는 보안 요구사항을 충족해야 합니다.
");

        /*
         * [2fa login]
         * 1. 로그인 상태에서 계정 -> 2fa 활성화 -> QR 코드 발급 -> Auth 앱 등록 -> 강제 로그 아웃 후 로그인 재시도
         * 2. 로그인 -> (2FA 활성화시)OTP 코드 입력 -> 로그인 완료
         */
        group.MapPost("/2fa/setup", async (Setup2FaCommand command, ISender sender) =>
                await sender.Send(command))
            .WithName("Setup2FACommand")
            .WithDescription(@"
            사용자 계정에 대한 2단계 인증(2FA)을 설정합니다.
            인증 앱 구성을 위한 고유 보안 키와 QR 코드를 생성합니다.
            보안 키와 QR 코드 이미지를 base64 형식으로 반환합니다.
            사용자는 2FA가 활성화되기 전에 유효한 코드를 제공하여 2FA 설정을 확인해야 합니다.
            ");
            

        group.MapPost("/2fa/verify", async (Verify2FaCommand command, ISender sender) =>
            await sender.Send(command))
            .WithName("Verify2FACommand")
            .WithDescription(@"
            사용자 계정에 대한 2단계 인증을 확인하고 활성화합니다.
            제공된 2FA 코드를 사용자의 보안 키와 대조하여 확인합니다.
            확인이 성공하면 계정에 대해 2FA가 활성화됩니다.
            확인이 성공하면 성공을, 코드가 유효하지 않으면 실패를 반환합니다.
            ");
    }
}