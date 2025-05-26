using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Tests;

public static class DependencyInjection
{
    public static void AddTestService(this IServiceCollection services)
    {
        services.AddScoped<IRegisterUserService, RegisterUserService>();
        services.AddScoped<INodifyService, NodifyService>();
    }
}