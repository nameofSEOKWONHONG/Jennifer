using Jennifer.Tenant.Session.Abstracts;
using Jennifer.Tenant.Session.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Tenant.Session;

public static class DependencyInjection
{
    public static void AddCacheResolver(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ITenantSessionContext, TenantSessionContext>();
        services.AddScoped<IUserRoleFetcher, UserRoleFetcher>();
        services.Decorate<IUserRoleFetcher, CachedUserRoleFetcher>();
    }
}