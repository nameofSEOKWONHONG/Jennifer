using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Commands.CheckEmail;
using Jennifer.Jwt.Application.Auth.Commands.ExternalOAuth;
using Jennifer.Jwt.Application.Auth.Commands.Password;
using Jennifer.Jwt.Application.Auth.Commands.RefreshToken;
using Jennifer.Jwt.Application.Auth.Commands.SignIn;
using Jennifer.Jwt.Application.Auth.Commands.SignOut;
using Jennifer.Jwt.Application.Auth.Commands.SignUp;
using Jennifer.Jwt.Application.Auth.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Application.Auth;

/// <summary>
/// Provides functionality for mapping authentication-related API endpoints
/// to an <see cref="IEndpointRouteBuilder"/>. These endpoints include user
/// authentication operations such as signup, signin, signout, password management,
/// and token refresh.
/// </summary>
public static class AuthEndpoint
{
    /// <summary>
    /// Maps the sign-related API endpoints to the specified endpoint route builder.
    /// These endpoints include operations for user signup, signin, signout, and identity verification.
    /// </summary>
    /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> used to define the API routes.</param>
    public static void MapAuthEndpoint(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/auth")
                .WithGroupName("v1")
                .WithTags("Auth");
        
        group.MapPost("/check", 
                async (string email, IQueryHandler<CheckByEmailQuery, bool> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new CheckByEmailQuery(email), ct))
            .WithName("CheckEmail");

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
                        ICommandHandler<SignUpCommand, Guid> handler, CancellationToken ct) => 
                    await handler.HandleAsync(
                        new SignUpCommand(request.Email, request.Password, request.UserName, request.PhoneNumber, request.Type), ct))
            .WithName("SignUp");
        
        group.MapPost("/signup/verify", 
                async (SignUpVerifyRequest request, ICommandHandler<SignUpVerifyCommand, bool> handler, CancellationToken ct) => 
                    await handler.HandleAsync(new SignUpVerifyCommand(request.UserId, request.Code), ct))
            .WithName("SignUpVerify");
        
        // group.MapGet("/signup/verify/{id}/{code}", 
        //         async (Guid id, string code, ICommandHandler<SignUpVerifyCommand, IResult> handler, CancellationToken ct) => 
        //             await handler.HandleAsync(new SignUpVerifyCommand(id, code), ct))
        //     .WithName("SignUpVerify");

        group.MapPost("/signup/retry",
            async (SignUpRetryRequest request, ICommandHandler<SignUpRetryCommand, bool> handler,
                    CancellationToken ct) =>
                await handler.HandleAsync(new SignUpRetryCommand(request.Email), ct))
            .WithName("SignUpRetry");

        #endregion
        
        #region [admin sign up process]

        group.MapPost("/signup/admin",
                async (RegisterAdminRequest request, ICommandHandler<SignUpAdminCommand, Guid> handler, CancellationToken ct) =>
                {
                    var command = new SignUpAdminCommand(request.Email, request.Password, request.UserName, request.PhoneNumber);
                    var result = await handler.HandleAsync(command, ct);
                    return result.IsSuccess ? Results.Ok() : Results.Problem(result.Error.Description);
                })
            .WithName("SignUpAdmin");
        
        #endregion
        
        
        
        group.MapPost("/signin", 
            async (SignInRequest request, ICommandHandler<SignInCommand, TokenResponse> handler, CancellationToken ct) => 
                await handler.HandleAsync(new SignInCommand(request.Email, request.Password), ct))
            .WithName("SignIn")
            .WithDescription(@"
Sign in with email and password. 
Returns a JWT token and refresh token. 
Use the refresh token to obtain a new JWT token. 
The refresh token expires after 7 days. 
Use the refresh token to obtain a new JWT token. The refresh token expires after 7 days.");
        
        group.MapPost("/external/signin", 
                async (ExternalSignInRequest request, ICommandHandler<ExternalOAuthCommand, TokenResponse> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new ExternalOAuthCommand(request.Provider, request.AccessToken), ct))
            .WithName("ExternalSignIn");        
        
        group.MapPost("/signout",
            async (ICommandHandler<SignOutCommand, bool> handler, CancellationToken ct) => 
                await handler.HandleAsync(new SignOutCommand(true), ct))
            .WithName("SignOut")
            .RequireAuthorization();

        group.MapPost("/refreshtoken",
                async (string refreshToken, ICommandHandler<RefreshTokenCommand, TokenResponse> handler,
                        CancellationToken ct) =>
                    await handler.HandleAsync(new RefreshTokenCommand(refreshToken), ct))
            .WithName("RefreshToken");
        
        #region [none login state change password]
        
        /*
         * 1. 인증코드 생성 및 메일 전송 : /password/forgot
         * 2. 인증코드 확인 : /password/forgot/verify
         * 3. 인증코드 변경 : /password/forgot/change
         */
        
        group.MapPost("/password/forgot",
                async (PasswordForgotRequest request, ICommandHandler<PasswordForgotCommand, IResult> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new PasswordForgotCommand(request.Email, request.UserName), ct))
            .WithName("PasswordForgot")
            ;
        
        group.MapPost("/password/forgot/verify",
                async (VerifyCodeRequest request, ICommandHandler<PasswordForgotVerifyCommand, IResult> handler,
                        CancellationToken ct) =>
                    await handler.HandleAsync(new PasswordForgotVerifyCommand(request.Email, request.Code), ct))
            .WithName("PasswordForgotVerify")
            ;

        group.MapPost("/password/forgot/change",
                async (PasswordForgotChangeRequest request, ICommandHandler<PasswordForgotChangeCommand, IResult> handler,
                        CancellationToken ct) =>
                    await handler.HandleAsync(new PasswordForgotChangeCommand(request.Email,  request.Code, request.NewPassword), ct))
            .WithName("PasswordForgotChange")
            ;

        #endregion
        
        group.MapPost("/password/change", 
                async (PasswordChangeRequest request, ICommandHandler<PasswordChangeCommand, bool> handler, CancellationToken ct) => 
                    await handler.HandleAsync(new PasswordChangeCommand(request.OldPassword, request.NewPassword), ct))
            .WithName("PasswordChange")
            .RequireAuthorization()
            ;
    }
}


