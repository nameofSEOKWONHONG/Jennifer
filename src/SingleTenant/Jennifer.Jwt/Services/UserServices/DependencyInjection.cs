using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.Jwt.Services.UserServices.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Services.UserServices;

internal static class DependencyInjection
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IGetUserService, GetUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserClaimService, UserClaimService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleClaimService, RoleClaimService>();        
    }
}