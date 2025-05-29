using System.IO.Compression;
using System.Text;
using eXtensionSharp.Mongo;
using FluentValidation;
using Jennifer.Account.Application.Auth.Commands.SignUp;
using Jennifer.Account.Hubs;
using Jennifer.Account.Session;
using Jennifer.External.OAuth;
using Jennifer.Infrastructure;
using Jennifer.Infrastructure.Email;
using Jennifer.Account.Application.Auth;
using Jennifer.Account.Application.Options;
using Jennifer.Account.Application.Roles;
using Jennifer.Account.Application.Tests;
using Jennifer.Account.Application.Users;
using Jennifer.Domain.Account;
using Jennifer.Domain.Common;
using Jennifer.Domain.Database;
using Jennifer.External.OAuth.Contracts;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Abstractions.DomainEvents;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Middlewares;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Role = Jennifer.Domain.Account.Role;

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

        services.AddDbContext<JenniferDbContext>(dbContextSetup);
        services.AddDbContext<JenniferReadOnlyDbContext>(dbContextSetup);
        services.AddScoped<ITransactionDbContext, JenniferDbContext>();
        
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
        services.AddRoleService();
        services.AddTestService();
        services.AddSessionService();
        services.AddExternalOAuthHandler();
        services.AddJMongoDb(JenniferOptionSingleton.Instance.Options.MongodbConnectionString, config =>
        {
            config.AddInitializer<ExternalOAuthDocumentConfiguration>();
            config.AddInitializer<EmailSendResultDocumentConfiguration>();
        });

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
        //Registered behaviors are executed in order (e.g., IpBlockBehavior, DomainEventBehavior, ValidationBehavior, TransactionBehavior)
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DomainEventBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        services.AddValidatorsFromAssemblyContaining<SignUpAdminCommandValidator>();
        services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();
        services.AddSingleton<IIpBlockService, IpBlockService>();
        #endregion

        services.AddScoped<ISlimSender, SlimSender>();
        
        return services;
    }

    public static IServiceCollection WithJenniferCache(this IServiceCollection services, string redisConnectionString)
    {
        ArgumentNullException.ThrowIfNull(redisConnectionString);

        try
        {
            IConnectionMultiplexer connectionMultiplexer =
                ConnectionMultiplexer.Connect(redisConnectionString);

            services.AddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () =>
                    Task.FromResult(connectionMultiplexer));
        }
        finally
        {
            services.AddHybridCache(options =>
            {
                // Maximum size of cached items
                options.MaximumPayloadBytes = 1024 * 1024 * 10; // 10MB
                options.MaximumKeyLength = 512;
                
                // Default timeouts
                options.DefaultEntryOptions = new HybridCacheEntryOptions
                {
                    Expiration = TimeSpan.FromMinutes(JenniferOptionSingleton.Instance.Options.Jwt.ExpireMinutes),
                    LocalCacheExpiration = TimeSpan.FromMinutes(JenniferOptionSingleton.Instance.Options.Jwt.ExpireMinutes)
                };
            });            
        }
        
        return services;
    }

    /// <summary>
    /// Configures and adds SignalR services to the specified dependency injection container for Jennifer.Account,
    /// enabling optional communication through a Redis backplane.
    /// </summary>
    /// <param name="services">The service collection to which SignalR services will be added.</param>
    /// <param name="backplaneConnectionString">The connection string for the Redis backplane used to coordinate SignalR messages across servers in a distributed environment.</param>
    /// <returns>The updated service collection with SignalR and Redis backplane services registered.</returns>
    public static IServiceCollection WithJenniferSignalr(this IServiceCollection services,
        string backplaneConnectionString)
    {
        ArgumentNullException.ThrowIfNull(backplaneConnectionString);
        
        services.AddSingleton<IUserIdProvider, SubUserIdProvider>();
        
        try
        {
            IConnectionMultiplexer connectionMultiplexer =
                ConnectionMultiplexer.Connect(backplaneConnectionString);
            
            services.AddSignalR().AddStackExchangeRedis(o =>
            {
                o.ConnectionFactory = writer => Task.FromResult(connectionMultiplexer);
            });
        }
        catch
        {
            services.AddSignalR();
        }
        
        

        return services;
    }

    public static IServiceCollection WithJenniferMongodb(this IServiceCollection services,
        string mongodbConnectionString)
    {
        services.AddJMongoDb(mongodbConnectionString, options =>
        {
            options.AddInitializer<ExternalOAuthDocumentConfiguration>();
            options.AddInitializer<EmailSendResultDocumentConfiguration>();
        });

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
        services.AddHostedService<EmailSendService>();
    }

    /// <summary>
    /// Adds the Jennifer.Account endpoint mappings to the application's endpoint route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder instance where the Jennifer.Account endpoints will be mapped.</param>
    private static void UseJenniferEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoint();
        app.MapOptionEndpoint();
        app.MapRoleEndpoint();
        app.MapTestEndpoint();
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

        app.UseMiddleware<IpBlockMiddleware>();
        app.UseMiddleware<ProblemDetailsMiddleware>();
        app.UseMiddleware<SessionContextMiddleware>();

        app.MapHub<JenniferHub>("/jenniferHub");
        
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IIpBlockService>();
        service.SubscribeToUpdates();

        app.UseJMongoDbAsync();
    }
}