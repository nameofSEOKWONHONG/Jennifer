using Jennifer.Account.Session.Abstracts;
using Jennifer.Account.Session.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Session;

public static class DependencyInjection
{
    public static void AddSessionService(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ISessionContext, SessionContext>();
        services.AddScoped<IUserRoleFetcher, UserRoleFetcher>();
        //services.Decorate<IUserRoleFetcher, CachedUserRoleFetcher>();
    }
}