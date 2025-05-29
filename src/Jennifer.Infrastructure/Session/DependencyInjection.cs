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
        services.AddScoped<IUserFetcher, UserFetcher>();
        services.AddScoped<IOptionFetcher, OptionFetcher>();
        services.AddScoped<IUserOptionFetcher, UserOptionFetcher>();
        
        //add context
        services.AddKeyedScoped<IUnifiedContext<UserFetchResult>, UserContext>(SessionContextKeyedServiceName.User);
        services.AddKeyedScoped<IUnifiedContext<UserOptionFetchResult[]>, UserOptionContext>(SessionContextKeyedServiceName.UserOption);
        services.AddKeyedScoped<IUnifiedContext<OptionFetchResult[]>, OptionContext>(SessionContextKeyedServiceName.Option);
        
        services.AddScoped<ISessionContext, SessionContext>();
    }
}