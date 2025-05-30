using Jennifer.Account.Application.Users.Filters;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Users;

public static class DependencyInjection
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserQueryFilter, UserQueryFilter>();
        services.AddScoped<IServiceExecutionBuilderFactory, ServiceExecutionBuilderFactory>();
        // services.AddScoped<IGetUserService, GetUserService>();
        // services.AddScoped<IUserService, UserService>();
        // services.AddScoped<IUserRoleService, UserRoleService>();
        // services.AddScoped<IUserClaimService, UserClaimService>();
        // services.AddScoped<IRoleService, RoleService>();
        // services.AddScoped<IRoleClaimService, RoleClaimService>();        
    }
}