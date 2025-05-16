using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Services.AuthServices;

internal static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<ICheckEmailService, CheckEmailService>();
        services.AddScoped<IPasswordChangeService, PasswordChangeService>();
        services.AddScoped<IPasswordForgotChangeService, PasswordForgotChangeService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ISignInService, SignInService>();
        services.AddScoped<ISignOutService, SignOutService>();
        services.AddScoped<ISignUpService, SignUpService>();
        services.AddScoped<ISignUpAdminService, SignUpAdminService>();
        services.AddScoped<IVerifyCodeByEmailSendService, VerifyCodeByEmailSendService>();
        services.AddScoped<IVerifyCodeService, VerifyCodeService>();
    }
}