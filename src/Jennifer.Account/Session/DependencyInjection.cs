using Jennifer.Account.Session.Abstracts;
using Jennifer.Account.Session.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Account.Session;

internal static class DependencyInjection
{
    internal static void AddSessionService(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ISessionContext, SessionContext>();
        services.AddScoped<IUserFetcher, UserFetcher>();
    }
}