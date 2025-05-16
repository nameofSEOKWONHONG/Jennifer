using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.Jwt.Services.UserServices.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Services.UserServices;

internal static class DependencyInjection
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IGetUserService, GetUserService>();
    }
}