using System.IO.Compression;
using System.Net;
using System.Text;
using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Confluent.Kafka;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.Account.Hubs;
using Jennifer.External.OAuth;
using Jennifer.Infrastructure.Email;
using Jennifer.Account.Application.Auth;
using Jennifer.Account.Application.Options;
using Jennifer.Account.Application.Roles;
using Jennifer.Account.Application.Tests;
using Jennifer.Account.Application.Users;
using Jennifer.Account.GrpcServices;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Common;
using Jennifer.External.OAuth.Contracts;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Infrastructure.AppConfigurations;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extensions;
using Jennifer.Infrastructure.Middlewares;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Role = Jennifer.Domain.Accounts.Role;

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
    /// <param name="jenniferOptions">The configuration options used to set up Jennifer features and behavior.</param>
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

        //read, write dbcontext
        services.AddDbContext<JenniferDbContext>(dbContextSetup);
        //readonly dbcontext
        services.AddDbContextPool<JenniferReadOnlyDbContext>((sp, op) =>
        {
            op.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            dbContextSetup.Invoke(sp, op);
        });
        //abstraction transaction dbcontext
        services.AddScoped<ITransactionDbContext, JenniferDbContext>();
        //use dapper executor
        services.AddScoped<IDapperExecutor, DapperExecutor>();
        
        services.AddIdentity<User, Role>(identitySetup)
            .AddEntityFrameworkStores<JenniferDbContext>()
            .AddDefaultTokenProviders();
        services.RemoveAll<IUserValidator<User>>();
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

        services.AddScoped<ISlimSender, SlimSender>();
        services.AddGrpc();
        
        return services;
    }

    /// <summary>
    /// Configures and adds distributed caching using Redis for Jennifer services.
    /// </summary>
    /// <param name="services">The service collection to which the distributed cache services will be added.</param>
    /// <param name="redisConnectionString">The connection string for the Redis server used to configure the distributed cache.</param>
    /// <returns>
    /// The updated service collection with the distributed cache services configured.
    /// </returns>
    public static IServiceCollection WithJenniferDistributeCache(this IServiceCollection services,
        string redisConnectionString)
    {
        ArgumentNullException.ThrowIfNull(redisConnectionString);

        try
        {
            IConnectionMultiplexer connectionMultiplexer =
                ConnectionMultiplexer.Connect(redisConnectionString);

            services.AddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () =>
                    Task.FromResult(connectionMultiplexer);
            });
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

        WithOptions.Instance.WithJenniferDistributeCache = true;
        
        return services;
    }


    /// <summary>
    /// Configures and adds IP rate limiting using Redis as the backend storage for rate limit rules and throttling,
    /// along with additional services required for custom IP blocking in the specified dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which IP rate limiting and related services will be added.</param>
    /// <returns>The same instance of <see cref="IServiceCollection"/> to allow method chaining.</returns>
    public static IServiceCollection WithJenniferIpRateLimit(this IServiceCollection services)
    {
        services.AddDistributedRateLimiting<RedisProcessingStrategy>();
        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.RealIpHeader = "X-Forwarded-For";
            options.ClientIdHeader = "X-ClientId";
            options.HttpStatusCode = (int)HttpStatusCode.TooManyRequests;
            options.GeneralRules =
            [
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "10s", // 10초
                    Limit = 30 // 최대 30회
                }
            ];
            options.RequestBlockedBehaviorAsync = async (context, identity, rule, counter) =>
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                if(ip.xIsEmpty()) return;
                
                var redis = context.RequestServices.GetRequiredService<IConnectionMultiplexer>().GetDatabase();
                var ipBlockService = context.RequestServices.GetRequiredService<IIpBlockTtlService>();
                var db = context.RequestServices.GetRequiredService<JenniferDbContext>();
                
                // 위반 횟수 Redis 카운터 증가
                var violationKey = $"ratelimit:violation:{ip}";
                var count = await redis.StringIncrementAsync(violationKey);

                // 누적 위반 TTL 설정 (예: 24시간)
                if (count == 1)
                    await redis.KeyExpireAsync(violationKey, TimeSpan.FromHours(24));

                if (count > 10)
                {
                    // 차단 등록 (TTL 포함, Redis + 메모리)
                    await ipBlockService.BlockIpAsync(ip);
                    await redis.KeyDeleteAsync(violationKey);          // 누적 제거
                    // RDB 이력 기록
                    await db.IpBlockLogs.AddAsync(new IpBlockLog
                    {
                        IpAddress = ip,
                        BlockedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddHours(1), // 예: 1시간 임시 차단
                        Reason = "RateLimit exceeded",
                        CreatedBy = "SYSTEM",
                        IsPermanent = false
                    });
                    await db.SaveChangesAsync();
                }
                
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\":\"Too many requests. Your IP has been temporarily blocked.\"}");
            };
        });
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IIpBlockTtlService, IpBlockTtlService>();
        services.AddSingleton<IIpBlockStaticService, IpBlockStaticService>();
        WithOptions.Instance.WithJenniferIpRateLimit = true;
        
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
                o.ConnectionFactory = _ => Task.FromResult(connectionMultiplexer);
            });
        }
        catch
        {
            services.AddSignalR();
        }
        
        WithOptions.Instance.WithJenniferSignalr = true;

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
        
        WithOptions.Instance.WithJenniferMongodb = true;

        return services;
    }

    /// <summary>
    /// Configures and adds Jennifer mail service components to the specified dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which Jennifer mail service components will be added.</param>
    /// <param name="kafkaConnectionString">The Kafka connection string used to configure producer and consumer for mailing services.</param>
    public static void WithJenniferMailService(this IServiceCollection services, string kafkaConnectionString)
    {
        // services.AddSingleton<IEmailQueue, EmailQueue>();
        // services.AddHostedService<EmailSendService>();
        services.AddHostedService<EmailConsumerProcessor>();
        services.AddSingleton(_ =>
        {
            var config = new ProducerConfig
            {
                BootstrapServers = kafkaConnectionString,
                ClientId = Dns.GetHostName(),
                Acks = Acks.All, // 메시지 손실 방지
                EnableIdempotence = true, // 중복 방지 (선택 사항)
            };
            return new ProducerBuilder<string, string>(config).Build();
        });
        services.AddSingleton(_ =>
        {
            var config = new ConsumerConfig()
            {
                BootstrapServers = kafkaConnectionString,
                GroupId = "jennifer-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
                ClientId = Dns.GetHostName(),
                SecurityProtocol = SecurityProtocol.Plaintext
            };
            return new ConsumerBuilder<string, string>(config).Build();
        });
        
        WithOptions.Instance.WithJenniferMailService = true;
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
        if(WithOptions.Instance.WithJenniferIpRateLimit)
            app.UseIpRateLimiting();
        
#if DEBUG
        app.UseCors(); // 이름 지정 없이 default policy 사용
#else
        app.UseCors("AllowSpecificOrigin");
#endif
        
        // set account route api endpoint jennifer manager
        app.UseJenniferEndpoints();

        app.UseAuthentication();
        app.UseAuthorization();

        if(WithOptions.Instance.WithJenniferIpRateLimit)
            app.UseMiddleware<IpBlockMiddleware>();
        
        app.UseMiddleware<ProblemDetailsMiddleware>();
        app.UseMiddleware<SessionMiddleware>();
        
        app.MapGrpcService<AccountServiceImpl>();

        app.MapHub<JenniferHub>("/jenniferHub");

        using (var scope = app.Services.CreateScope())
        {
            if (WithOptions.Instance.WithJenniferDistributeCache)
            {
                var service = scope.ServiceProvider.GetRequiredService<IIpBlockTtlService>();
                service.SubscribeToUpdates();            
            }

            //TODO: Here need test.
            if (WithOptions.Instance.WithJenniferIpRateLimit)
            {
                var service = scope.ServiceProvider.GetRequiredService<IIpBlockStaticService>();
                var task = Task.Run(async() => await service.Setup());
                Task.WaitAll(task);
            }
        }
        
        app.UseJMongoDbAsync();
    }
}

