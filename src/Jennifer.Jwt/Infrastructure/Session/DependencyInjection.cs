using Jennifer.Jwt.Infrastructure.Session.Abstracts;
using Jennifer.Jwt.Infrastructure.Session.Implements;
using Jennifer.SharedKernel;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Jwt.Infrastructure.Session;

public static class DependencyInjection
{
    public static void AddCacheResolver(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
                                                         services.AddScoped<ISessionContext, SessionContext>();
        services.AddScoped<IUserRoleFetcher, UserRoleFetcher>();
        services.Decorate<IUserRoleFetcher, CachedUserRoleFetcher>();
    }
}