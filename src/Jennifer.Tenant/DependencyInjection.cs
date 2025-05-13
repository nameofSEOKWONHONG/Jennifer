using Jennifer.SharedKernel.Consts;
using Jennifer.Endpoints;

using Jennifer.Tenant.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.Tenant;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the Jennifer Tenant services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the Jennifer.Tenant services will be added.</param>
    /// <param name="schema">The database schema to be used by Jennifer.Tenant.</param>
    /// <param name="dbContextOptions">A delegate to configure database context options. Accepts a service provider and a database context options builder.</param>
    /// <param name="cacheOptions">An optional delegate to configure hybrid cache options. If null, the following default settings will be applied:
    /// - MaximumPayloadBytes: 1MB (1024 * 1024)
    /// - MaximumKeyLength: 1024
    /// - DefaultEntryOptions:
    ///   - Expiration: 30 minutes
    ///   - LocalCacheExpiration: 5 minutes</param>
    /// <param name="redisOptions">An optional delegate to configure Redis cache options.</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided schema is null, empty, or consists only of whitespace.</exception>
    public static void AddJenniferTenant(this IServiceCollection services, string schema,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
        Action<HybridCacheOptions> cacheOptions,
        Action<RedisCacheOptions> redisOptions)
    {
        if (string.IsNullOrWhiteSpace(schema)) throw new ArgumentNullException(nameof(schema));
        EntitySettings.Schema = schema;
        services.AddDbContext<TenantJenniferDbContext>(dbContextOptions.Invoke);
        if (cacheOptions is not null)
        {
            cacheOptions = (options) =>
            {
                options.MaximumPayloadBytes = 1024 * 1024;
                options.MaximumKeyLength = 1024;
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(30),
                    LocalCacheExpiration = TimeSpan.FromMinutes(5)
                };
            };
            services.AddHybridCache(cacheOptions);    
        }
        if (redisOptions is not null)
        {
            services.AddStackExchangeRedisCache(redisOptions);    
        }
    }

    
    public static void UseJenniferTenant(this IEndpointRouteBuilder app)
    {
        app.MapUserEndpoint();
        app.MapUserRoleEndpoint();
        app.MapRoleEndpoint();
        app.MapRoleClaimEndpoint();
    }
}