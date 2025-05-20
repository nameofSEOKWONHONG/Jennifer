using Jennifer.Jwt.Application.Users.Commands;
using Jennifer.Jwt.Services;
using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.Jwt.Services.UserServices.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Application.Users;

public static class DependencyInjection
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserQueryFilter, UserQueryFilter>();
        services.AddScoped<IGetUserService, GetUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserClaimService, UserClaimService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleClaimService, RoleClaimService>();        
    }
}