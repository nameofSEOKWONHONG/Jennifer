using Jennifer.Core.Domains;
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
        
        group.MapPost("/signup", async (RegisterRequest request, ISignService signService) => await signService.Register(request));
        
        group.MapPost("/signin", async (SignInRequest request, ISignService signService) =>
        {
            var result = await signService.Signin(request.Email, request.Password);
            if(result is null) return Results.Unauthorized();
            return Results.Ok(result);
        });
        group.MapPost("/cookie/signin", async (SignInRequest request, ISignService signService) => Results.Ok(await signService.CookieSignIn(request.Email, request.Password)));
        
        group.MapPost("/signout", (ISignService signService) => Results.Ok(signService.SignOut())).RequireAuthorization();
        
        group.MapPost("/refreshtoken", async (string refreshToken, ISignService signService) =>
        {
            var result = await signService.RefreshToken(refreshToken);
            if(result is null) return Results.Unauthorized();
            return Results.Ok(result);
        });

        group.MapGet("/password/forgot",
            async (string email, ISignService signService) => Results.Ok(await signService.RequestPasswordResetToken(email)));
        group.MapPost("/password/reset", 
            async (PasswordResetRequest request, ISignService signService) => 
            Results.Ok(await signService.ResetPassword(request.ResetToken, request.Password, request.NewPassword)));

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