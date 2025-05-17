using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.SignUpAdmin;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

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
                async (string email, ICheckEmailService service, CancellationToken ct) =>
                    await service.HandleAsync(email, ct))
            .WithName("CheckEmail");
        
        group.MapPost("/verify", 
            async (string email, IVerifyCodeByEmailSendService service, CancellationToken ct) => 
                await service.HandleAsync(new VerifyCodeByEmailSendRequest(email, ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE), ct))
            .WithName("VerifyEmail");
        
        group.MapPost("/signup", 
            async (RegisterRequest request, ISignUpService service, CancellationToken ct) => 
                await service.HandleAsync(request,ct))
            .WithName("SignUp");

        // group.MapPost("/signup/admin", 
        //         async (Microsoft.AspNetCore.Identity.Data.RegisterRequest request, ISignUpAdminService service, CancellationToken ct) => 
        //             await service.HandleAsync(request, ct))
        //     .WithName("SignUpAdmin");
        
        group.MapPost("/signup/admin",
            async (Microsoft.AspNetCore.Identity.Data.RegisterRequest request,
                ICommandHandler<SignUpAdminCommand, Guid> handler,
                CancellationToken ct) =>
            {
                var command = new SignUpAdminCommand(request.Email, request.Password);
                var result = await handler.Handle(command, ct);
                return result.IsSuccess ? Results.Ok() : Results.Problem();
            })
            .WithName("SignUpAdmin");
        
        group.MapPost("/signin", 
            async (SignInRequest request, ISignInService service, CancellationToken ct) => 
                await service.HandleAsync(request, ct))
            .WithName("SignIn");
        
        group.MapPost("/signout", 
            async (ISignOutService authService, CancellationToken ct) => 
                await authService.HandleAsync(false, ct))
            .WithName("SignOut")
            .RequireAuthorization();
        
        group.MapPost("/refreshtoken", 
            async (string refreshToken, IRefreshTokenService service, CancellationToken ct) =>
                await service.HandleAsync(refreshToken, ct))
            .WithName("RefreshToken");
        
        group.MapPost("/password/forgot",
            async (string email, IVerifyCodeByEmailSendService service, CancellationToken ct) => 
                await service.HandleAsync(new VerifyCodeByEmailSendRequest(email, ENUM_EMAIL_VERIFICATION_TYPE.PASSWORD_FORGOT), ct))
            .WithName("PasswordForgot");

        group.MapPost("/password/forgot/change", 
                async (PasswordForgotChangeRequest request, IPasswordForgotChangeService service, CancellationToken ct) => 
                    await service.HandleAsync(request, ct))
            .WithName("PasswordForgotChange");

        group.MapPost("/password/change",
            async (PasswordChangeRequest request, IPasswordChangeService service, CancellationToken ct) =>
                await service.HandleAsync(request, ct))
            .WithName("PasswordChange")
            .RequireAuthorization();
        
        group.MapPost("/external/signin", 
            async (ExternalSignInRequest request, IExternalSignService service, CancellationToken ct) =>
                await service.HandleAsync(request, ct))
            .WithName("ExternalSignIn");
    }
}


