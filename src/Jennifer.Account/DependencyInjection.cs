using System.IO.Compression;
using System.Text;
using FluentValidation;
using Jennifer.Account.Application.Auth.Commands.SignUp;
using Jennifer.Account.Data;
using Jennifer.Account.Hubs;
using Jennifer.Account.Models;
using Jennifer.Account.Session;
using Jennifer.External.OAuth;
using Jennifer.Infrastructure;
using Jennifer.Infrastructure.Data;
using Jennifer.Infrastructure.Email;
using Jennifer.Infrastructure.Options;
using Jennifer.Account.Application.Auth;
using Jennifer.Account.Application.Users;
using Jennifer.Account.Behaviors;
using Jennifer.Account.Services;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Role = Jennifer.Account.Models.Role;

namespace Jennifer.Account;

/// <summary>
/// Provides extension methods for integrating Jennifer's functionalities
/// within an application, including database setup, caching, authentication,
/// email services, and middleware setup.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures and adds Jennifer.Account services to the specified dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which Jennifer.Account services will be added.</param>
    /// <param name="jenniferOptions">The configuration options used to setup Jennifer features and behavior.</param>
    /// <param name="dbContextSetup">A delegate to configure the DbContext options for Jennifer's database context.</param>
    /// <param name="identitySetup">A delegate to configure options for ASP.NET Core Identity.</param>
    /// <remarks>
    /// identityOptions default value
    /// <code>
    /// options.Password.RequiredLength = 8;
    /// options.Password.RequireNonAlphanumeric = true;
    /// options.SignIn.RequireConfirmedAccount = false;
    /// options.SignIn.RequireConfirmedEmail = false;
    /// options.SignIn.RequireConfirmedPhoneNumber = false;
    /// options.User.RequireUniqueEmail = true;
    /// </code>
    /// </remarks>
    public static IServiceCollection AddJennifer(this IServiceCollection services,
        JenniferOptions jenniferOptions,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextSetup,
        Action<IdentityOptions> identitySetup)
    {
        JenniferOptionSingleton.Attach(jenniferOptions);

        ArgumentNullException.ThrowIfNull(dbContextSetup);
        ArgumentNullException.ThrowIfNull(identitySetup);
        
        services.AddDbContext<JenniferDbContext>(dbContextSetup)
            .AddScoped<IApplicationDbContext, JenniferDbContext>();
        services.AddScoped<IJenniferSqlConnection, JenniferSqlConnection>();
        
        services.AddIdentity<User, Role>(identitySetup)
            .AddEntityFrameworkStores<JenniferDbContext>()
            .AddDefaultTokenProviders();
        services.AddTransient<IUserValidator<User>, UserNameValidator<User>>();
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = JenniferOptionSingleton.Instance.Options.Jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = JenniferOptionSingleton.Instance.Options.Jwt.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JenniferOptionSingleton.Instance.Options.Jwt.Key)),
                ValidateIssuerSigningKey = true
            };
            options.Events = new JwtBearerEvents()
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        context.Response.Headers["Token-Invalid"] = "true"; 
                    }

                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // SignalR 요청인지 확인
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        path.StartsWithSegments("/jenniferHub"))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
        
        services.AddAuthorization();
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream", "application/json"]);
        });
        services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        services.Configure<BrotliCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
        services.AddApiVersioning();

        #region [register feature's]

        services.AddAuthService();
        services.AddUserService();
        services.AddSessionService();
        services.AddExternalOAuthHandler();        

        #endregion


        #if DEBUG
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        #else 
        services.AddCors(op =>
        {
            op.AddPolicy("AllowSpecificOrigin", policy =>
            {
                policy.WithOrigins("https://example.com") // 허용할 도메인
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        #endif

        #region [setting mediator]

        services.AddMediator(options =>
        {
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        //Registered behaviors are executed in order (e.g., IpBlockBehavior, ValidationBehavior, TransactionBehavior)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.IpBlockBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(Behaviors.TransactionBehavior<,>));
        services.AddValidatorsFromAssemblyContaining<SignUpAdminCommandValidator>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        services.AddSingleton<IpBlockService>();

        #endregion
        
        return services;
    }

    public static IServiceCollection WithJenniferCache(this IServiceCollection services, string redisConnectionString, Action<RedisCacheOptions> setup)
    {
        ArgumentNullException.ThrowIfNull(redisConnectionString);
        ArgumentNullException.ThrowIfNull(setup);
        
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConnectionString));
        
        try
        {
            services.AddStackExchangeRedisCache(setup);
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }
        services.AddMemoryCache();
        
        return services;
    }

    // /// <summary>
    // /// Adds Jennifer.Account's hybrid caching services to the specified dependency injection container.
    // /// </summary>
    // /// <param name="services">The service collection to which the hybrid caching services will be added.</param>
    // /// <param name="cacheOptions">A delegate to configure hybrid cache options. If null, the following default settings will be applied:
    // /// - MaximumPayloadBytes: 1MB (1024 * 1024)
    // /// - MaximumKeyLength: 1024
    // /// - DefaultEntryOptions:
    // /// - Expiration: 30 minutes
    // /// - LocalCacheExpiration: 5 minutes</param>
    // /// <param name="redisOptions">An optional delegate to configure Redis cache options. If provided, Redis will be configured as part of the hybrid caching mechanism.</param>
    // public static IServiceCollection WithJenniferHybridCache(this IServiceCollection services,
    //     Action<HybridCacheOptions> cacheOptions,
    //     Action<RedisCacheOptions> redisOptions)
    // {
    //     if (cacheOptions.xIsEmpty())
    //     {
    //         cacheOptions = (options) =>
    //         {
    //             options.MaximumPayloadBytes = 1024 * 1024;
    //             options.MaximumKeyLength = 1024;
    //             options.DefaultEntryOptions = new HybridCacheEntryOptions
    //             {
    //                 Expiration = TimeSpan.FromMinutes(30),
    //                 LocalCacheExpiration = TimeSpan.FromMinutes(5)
    //             };
    //         };
    //     }
    //     services.AddHybridCache(cacheOptions);
    //     
    //     if (redisOptions.xIsEmpty())
    //     {
    //         redisOptions = (options) =>
    //         {
    //             options.Configuration = "localhost";
    //             options.InstanceName = "Jennifer";
    //             
    //
    //         };
    //     }
    //     
    //     services.AddStackExchangeRedisCache(redisOptions);
    //
    //     return services;
    // }

    /// <summary>
    /// Adds the Jennifer.Account Auth Hub services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the SignalR and related Jennifer.Account.Hub services will be added.</param>
    /// <param name="backplaneOptions">An optional delegate to configure Redis options for SignalR. If null, the default SignalR setup will be used without Redis integration.</param>
    public static IServiceCollection WithJenniferSignalr(this IServiceCollection services, Action<RedisOptions> backplaneOptions)
    {
        try
        {
            services.AddSignalR().AddStackExchangeRedis(backplaneOptions);
        }
        catch (Exception ex)
        {
            services.AddSignalR();
        }
        
        services.AddSingleton<IUserIdProvider, SubUserIdProvider>();

        return services;
    }

    /// <summary>
    /// Adds the Jennifer.Mail services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the Jennifer.Mail services will be added.
    /// This includes a singleton registration for email queuing and a hosted service for email sending operations.</param>
    public static void WithJenniferMailService(this IServiceCollection services)
    {
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddHostedService<EmailSenderService>();
    }

    /// <summary>
    /// Adds the Jennifer.Account endpoint mappings to the application's endpoint route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder instance where the Jennifer.Account endpoints will be mapped.</param>
    private static void UseJenniferEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoint();
        app.MapUserEndpoint();
        // app.MapUserRoleEndpoint();
        // app.MapUserClaimEndpoint();
        // app.MapRoleEndpoint();
    }

    /// <summary>
    /// Configures Jennifer.Account middleware and SignalR hub for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance where the middleware and SignalR hub will be configured.</param>
    public static void UseJennifer(this WebApplication app)
    {
#if DEBUG
        app.UseCors(); // 이름 지정 없이 default policy 사용
#else
        app.UseCors("AllowSpecificOrigin");
#endif
        
        // set account route api endpoint jennifer manager
        app.UseJenniferEndpoints();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseMiddleware<JenniferSessionContextMiddleware>(); // 반드시 인증 이후에 실행

        app.MapHub<JenniferHub>("/jenniferHub");
        
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IpBlockService>();
        service.SubscribeToUpdates();
    }
}