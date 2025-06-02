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
        services.AddScoped<IUserCacheProvider, Implements.UserCacheProvider>();
        services.AddScoped<IOptionCacheProvider, Implements.OptionCacheProvider>();
        services.AddScoped<IUserOptionCacheProvider, Implements.UserOptionCacheProvider>();
        
        //add context
        services.AddKeyedScoped<IUnifiedCacheProvider<UserCacheResult>, UserCacheProvider>(SessionContextKeyedServiceName.User);
        services.AddKeyedScoped<IUnifiedCacheProvider<UserOptionCacheResult[]>, UserOptionCacheProvider>(SessionContextKeyedServiceName.UserOption);
        services.AddKeyedScoped<IUnifiedCacheProvider<OptionCacheResult[]>, OptionCacheProvider>(SessionContextKeyedServiceName.Option);
        
        services.AddScoped<ISessionContext, SessionContext>();
        services.AddScoped<IUserContext, UserContext>();
    }
}