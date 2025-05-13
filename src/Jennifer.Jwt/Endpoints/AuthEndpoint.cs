using Jennifer.SharedKernel.Domains;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;

namespace Jennifer.Jwt.Endpoints;

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
        
        group.MapPost("/signup", async (RegisterRequest request, IAuthService authService) => await authService.Register(request));
        
        group.MapPost("/signin", async (SignInRequest request, IAuthService authService) =>
        {
            var result = await authService.Signin(request.Email, request.Password);
            if(result is null) return Results.Unauthorized();
            return Results.Ok(result);
        });
        group.MapPost("/cookie/signin", async (SignInRequest request, IAuthService authService) => Results.Ok(await authService.CookieSignIn(request.Email, request.Password)));
        
        group.MapPost("/signout", (IAuthService authService) => Results.Ok(authService.SignOut())).RequireAuthorization();
        
        group.MapPost("/refreshtoken", async (string refreshToken, IAuthService authService) =>
        {
            var result = await authService.RefreshToken(refreshToken);
            if(result is null) return Results.Unauthorized();
            return Results.Ok(result);
        });

        group.MapGet("/password/forgot",
            async (string email, IAuthService authService) => Results.Ok(await authService.RequestChangePasswordToken(email)));
        group.MapPost("/password/reset", 
            async (PasswordResetRequest request, IAuthService authService) => 
            Results.Ok(await authService.ChangePasswordWithToken(request.ResetToken, request.Password, request.NewPassword)));

        group.MapPost("/external/signin", async (ExternalSignInRequest request, IExternalSignService service, CancellationToken ct) 
            =>
        {
            var result = await service.SignIn(request.Provider, request.ProviderToken, ct);
            if(result is null) return Results.Unauthorized();
            return Results.Ok(result);
        });
        group.MapPost("/external/signin/apple", () => Results.Ok("apple"));
    }
}