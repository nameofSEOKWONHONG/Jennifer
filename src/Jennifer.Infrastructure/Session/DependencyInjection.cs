using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.Infrastructure.Session.Contracts;
using Jennifer.Infrastructure.Session.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Infrastructure.Session;

public static class DependencyInjection
{
    public static void AddSessionService(this IServiceCollection services)
    {
        //add fetcher
        services.AddScoped<IUserCacheProvider, UserCacheProvider>();
        services.AddScoped<IOptionCacheProvider, OptionCacheProvider>();
        services.AddScoped<IUserOptionCacheProvider, UserOptionCacheProvider>();
        
        //add context
        services.AddKeyedScoped<IUnifiedContext<UserCacheResult>, UserContext>(SessionContextKeyedServiceName.User);
        services.AddKeyedScoped<IUnifiedContext<UserOptionCacheResult[]>, UserOptionContext>(SessionContextKeyedServiceName.UserOption);
        services.AddKeyedScoped<IUnifiedContext<OptionCacheResult[]>, OptionContext>(SessionContextKeyedServiceName.Option);
        
        services.AddScoped<ISessionContext, SessionContext>();
    }
}