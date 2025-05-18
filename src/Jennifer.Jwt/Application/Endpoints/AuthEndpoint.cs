using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Commands.CheckEmail;
using Jennifer.Jwt.Application.Auth.Commands.ExternalOAuth;
using Jennifer.Jwt.Application.Auth.Commands.Password;
using Jennifer.Jwt.Application.Auth.Commands.RefreshToken;
using Jennifer.Jwt.Application.Auth.Commands.SignIn;
using Jennifer.Jwt.Application.Auth.Commands.SignOut;
using Jennifer.Jwt.Application.Auth.Commands.SignUp;
using Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Application.Endpoints;

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
                async (string email, IQueryHandler<CheckByEmailQuery, IResult> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new CheckByEmailQuery(email), ct))
            .WithName("CheckEmail");

        #region [sign up process]
        
        /*
         * 1. 회원 가입 및 가입 확인 메일 발송 : /signup (페이지 주소 포함)
         * 2. 회원 메일 확인에 따라 페이지 주소로 연결 후 POST 인증 코드 확인 : /signup/verify
         */

        group.MapPost("/signup", 
                async (RegisterRequest request,
                        ICommandHandler<SignUpCommand, IResult> handler, CancellationToken ct) => 
                    await handler.HandleAsync(
                        new SignUpCommand(request.Email, request.Password, request.UserName, request.PhoneNumber, request.VerifyCode, request.Type), ct))
            .WithName("SignUp");
        
        group.MapPost("/signup/verify", 
                async (SignUpVerifyRequest request, ICommandHandler<SignUpVerifyCommand, IResult> handler, CancellationToken ct) => 
                    await handler.HandleAsync(new SignUpVerifyCommand(request.UserId, request.Code), ct))
            .WithName("SignUpVerify");
        

        #endregion
        
        #region [admin sign up process]

        group.MapPost("/signup/admin",
                async (Microsoft.AspNetCore.Identity.Data.RegisterRequest request,
                    ICommandHandler<SignUpAdminCommand, Guid> handler,
                    CancellationToken ct) =>
                {
                    var command = new SignUpAdminCommand(request.Email, request.Password);
                    var result = await handler.HandleAsync(command, ct);
                    return result.IsSuccess ? Results.Ok() : Results.Problem();
                })
            .WithName("SignUpAdmin");
        
        #endregion
        
        
        
        group.MapPost("/signin", 
            async (SignInRequest request, ICommandHandler<SignInCommand, IResult> handler, CancellationToken ct) => 
                await handler.HandleAsync(new SignInCommand(request.Email, request.Password), ct))
            .WithName("SignIn")
            .WithDescription(@"
Sign in with email and password. 
Returns a JWT token and refresh token. 
Use the refresh token to obtain a new JWT token. 
The refresh token expires after 7 days. 
Use the refresh token to obtain a new JWT token. The refresh token expires after 7 days.");
        
        group.MapPost("/external/signin", 
                async (ExternalSignInRequest request, ICommandHandler<ExternalOAuthCommand, IResult> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new ExternalOAuthCommand(request.Provider, request.AccessToken), ct))
            .WithName("ExternalSignIn");        
        
        group.MapPost("/signout", 
            async (ICommandHandler<SignOutCommand, IResult> handler, CancellationToken ct) => 
                await handler.HandleAsync(new SignOutCommand(true), ct))
            .WithName("SignOut")
            .RequireAuthorization();
        
        group.MapPost("/refreshtoken", 
            async (string refreshToken, ICommandHandler<RefreshTokenCommand, IResult> handler, CancellationToken ct) =>
                await handler.HandleAsync(new RefreshTokenCommand(refreshToken), ct))
            .WithName("RefreshToken");
        
        #region [none login state change password]
        
        /*
         * 1. 인증코드 생성 및 메일 전송 : /password/forgot
         * 2. 인증코드 확인 : /password/forgot/verify
         * 3. 인증코드 변경 : /password/forgot/change
         */
        
        group.MapPost("/password/forgot",
                async (string email, ICommandHandler<PasswordForgotCommand, IResult> handler, CancellationToken ct) =>
                    await handler.HandleAsync(new PasswordForgotCommand(email), ct))
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
                async (PasswordChangeRequest request, ICommandHandler<PasswordChangeCommand, IResult> handler, CancellationToken ct) => 
                    await handler.HandleAsync(new PasswordChangeCommand(request.OldPassword, request.NewPassword), ct))
            .WithName("PasswordChange")
            .RequireAuthorization()
            ;
    }
}


