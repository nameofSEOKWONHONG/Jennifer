using Jennifer.Account.Application.Users.Queries;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Application.Users;

public static class DependencyInjection
{
    public static void AddUserService(this IServiceCollection services)
    {
        services.AddScoped<IUserQueryFilter, UserQueryFilter>();
        services.AddScoped<IServiceExecutionBuilderFactory, ServiceExecutionBuilderFactory>();
    }
}