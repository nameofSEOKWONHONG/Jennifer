using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Application.Auth.Services.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Auth;

internal static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IVerifyCodeSendEmailService, VerifyCodeSendEmailService>();
        services.AddScoped<IVerifyCodeConfirmService, VerifyCodeConfirmService>();
        // services.AddScoped<IConfigurationAddService, ConfigurationAddService>();
    }
}