using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Application.Auth.Services.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Auth;

public static class DependencyInjection
{
    public static void AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailConfirmSendService, EmailConfirmSendService>();
        services.AddScoped<IEmailConfirmService, EmailConfirmService>();
    }
}