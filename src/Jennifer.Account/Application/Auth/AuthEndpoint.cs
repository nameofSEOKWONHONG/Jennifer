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
            Validates if the provided email address is available for registration.
            Returns true if the email can be used for a new account, false if it's already registered.
            The endpoint helps prevent duplicate registrations and ensures email uniqueness in the system.
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
            Creates a new user account with the provided information.
            Returns a token response containing JWT and refresh tokens upon successful registration.
            Email verification will be sent to complete the registration process.
")
            .WithName("SignUp");
        
        group.MapPost("/signup/verify", 
                async (SignUpVerifyRequest request, ISender sender, CancellationToken ct) => 
                    await sender.Send(new SignUpVerifyCommand(request.UserId, request.Code), ct))
            .WithName("SignUpVerify");
        
        // group.MapGet("/signup/verify/{id}/{code}", 
        //         async (Guid id, string code, ICommandHandler<SignUpVerifyCommand, IResult> handler, CancellationToken ct) => 
        //             await handler.HandleAsync(new SignUpVerifyCommand(id, code), ct))
        //     .WithName("SignUpVerify");

        group.MapPost("/signup/retry",
            async (SignUpRetryRequest request, ISender sender,
                    CancellationToken ct) =>
                await sender.Send(new SignUpRetryCommand(request.Email), ct))
            .WithName("SignUpRetry");

        #endregion
        
        #region [admin sign up process]

        group.MapPost("/signup/admin",
                async (RegisterAdminRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new SignUpAdminCommand(request.Email, request.Password, request.UserName, request.PhoneNumber);
                    return await sender.Send(command, ct);
                })
            .WithName("SignUpAdmin");
        
        #endregion
        
        group.MapPost("/signin", 
            async (SignInRequest request, ISender sender, CancellationToken ct) => 
                await sender.Send(new SignInCommand(request.Email, request.Password), ct))
            .WithName("SignIn")
            .WithDescription(@"
Sign in with email and password. 
Returns a JWT token and refresh token. 
Use the refresh token to obtain a new JWT token. 
The refresh token expires after 7 days.");
        
        group.MapPost("/signin/external", 
                async (ExternalSignInRequest request, ISender sender, CancellationToken ct) =>
                    await sender.Send(new ExternalOAuthCommand(request.Provider, request.AccessToken), ct))
            .WithName("ExternalSignIn");        
        
        group.MapPost("/signout",
            async (ISender sender, CancellationToken ct) => 
                await sender.Send(new SignOutCommand(true), ct))
            .WithName("SignOut")
            .Produces(StatusCodes.Status200OK)
            .WithDescription("Signs out the currently authenticated user and invalidates their tokens.")
            .RequireAuthorization();

        group.MapPost("/refreshtoken",
                async (string refreshToken, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new RefreshTokenCommand(refreshToken), ct))
            .WithName("RefreshToken")
            .Produces<TokenResponse>(StatusCodes.Status200OK)
            .WithDescription("Generates a new JWT token using a valid refresh token.");
        
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
            ;
        
        group.MapPost("/password/forgot/verify",
                async (VerifyCodeRequest request, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new PasswordForgotVerifyCommand(request.Email, request.Code), ct))
            .WithName("PasswordForgotVerify")
            ;

        group.MapPost("/password/forgot/change",
                async (PasswordForgotChangeRequest request, ISender sender,
                        CancellationToken ct) =>
                    await sender.Send(new PasswordForgotChangeCommand(request.Email,  request.Code, request.NewPassword), ct))
            .WithName("PasswordForgotChange")
            ;

        #endregion
        
        group.MapPost("/password/change",
            async (PasswordChangeRequest request, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new PasswordChangeCommand(request.OldPassword, request.NewPassword), ct);
            })
            .WithName("PasswordChange")
            .RequireAuthorization()
            ;

        /*
         * [2fa login]
         * 1. 로그인 상태에서 계정 -> 2fa 활성화 -> QR 코드 발급 -> Auth 앱 등록 -> 강제 로그 아웃 후 로그인 재시도
         * 2. 로그인 -> (2FA 활성화시)OTP 코드 입력 -> 로그인 완료
         */
        group.MapPost("/2fa/setup", async (Setup2FACommand command, ISender sender) =>
                await sender.Send(command))
            .WithName("Setup2FACommand")
            .WithDescription(@"
            Sets up two-factor authentication (2FA) for a user account.
            Generates a unique secret key and QR code for configuring authenticator apps.
            Returns the secret key and QR code image in base64 format.
            The user must verify the 2FA setup by providing a valid code before 2FA is enabled.
            ");
            

        group.MapPost("/2fa/verify", async (Verify2FACommand command, ISender sender) =>
            await sender.Send(command))
            .WithName("Verify2FACommand")
            .WithDescription(@"
            Verifies and enables two-factor authentication for a user account.
            Validates the provided 2FA code against the user's secret key.
            Upon successful verification, 2FA is enabled for the account.
            Returns success if verification passes, failure if the code is invalid.
            ");
    }
}


