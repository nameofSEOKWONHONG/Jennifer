using System.IO.Compression;
using System.Net;
using System.Text;
using AspNetCoreRateLimit;
using AspNetCoreRateLimit.Redis;
using Confluent.Kafka;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Grpc.Net.Client;
using Jennifer.Account.Hubs;
using Jennifer.External.OAuth;
using Jennifer.Infrastructure.Email;
using Jennifer.Account.Application.Auth;
using Jennifer.Account.Application.Menus;
using Jennifer.Account.Application.Options;
using Jennifer.Account.Application.Roles;
using Jennifer.Account.Application.Tests;
using Jennifer.Account.Application.Users;
using Jennifer.Account.Grpc;
using Jennifer.Account.GrpcServices;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Common;
using Jennifer.External.OAuth.Contracts;
using Jennifer.Infrastructure.AppConfigurations;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extensions;
using Jennifer.Infrastructure.Middlewares;
using Jennifer.Infrastructure.Session;
using Jennifer.SharedKernel;
using Jennifer.Todo.Grpc;
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
        //services.RemoveAll<IUserValidator<User>>();
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
        services.AddGrpc();
        
        services.AddSingleton(provider =>
        {
            var channel = GrpcChannel.ForAddress("https://localhost:7288", new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(5),
                    KeepAlivePingDelay = TimeSpan.FromSeconds(30),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(10),
                    EnableMultipleHttp2Connections = true
                }
            });

            return new TodoService.TodoServiceClient(channel);
        });
        
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
            options.ApplyConfiguration(new ExternalOAuthDocumentConfiguration());
            options.ApplyConfiguration(new EmailSendResultDocumentConfiguration());
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
        app.MapMenuEndpoint();

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
        
        app.UseJMongoDb();
    }
}

/*
 * [수평적 확장 모놀리식에 대한 해설]
 * 모놀리식은 모든 프로젝트에 대한 출발점이다.
 * 모든 코드를 하나의 프로젝트에 묶어서 개발하는 것으로 대부분 당신이 작성하는 코드일 것이다.
 * 수평적 확장 모놀리식은 이러한 모놀리식을 각각의 프로젝트로 분리하는 것이다.
 * 이 프로젝트에서는 Account와 Todo로 분리된다.
 * Account의 DB 연결은 분리될 수 있다.
 * Todo의 DB 연결은 분리될 수 있다.
 *
 * 현재는 infra에 db context가 있지만 여기서도 선택지가 있다.
 * 현재처럼 같은 db에 종속된다면 infrastructure에 구현할 수 있지만
 * db를 분리한다면 현재의 DbContext는 모두 각각의 프로젝트로 간다.
 * Domain역시 마찬가지이다.
 *
 * 그럼 처음 시작할 때 모놀리식으로 개발하지 않고 현재의 구조로 개발한다면 향후에
 * 확장에 따라 하나씩 마이크로서비스로 이전하면 되겠다.
 *
 * MSA의 결과는 분리된 DB, 분리된 Endpoint이다.
 *
 * 따라서 현재의 Account가 하나의 API 프로젝트가 되어야 한다.
 * 따라서 현재의 Todo가 하나의 API 프로젝트가 되어야 한다.
 *
 * 실시간 연동을 위한 부분도 하나의 프로젝트가 되어야 한다.
 *
 * MSA를 할 수 있는 규모는 개발자 100명 이상의 프로젝트에 (DB, 인프라 포함)
 * 평균 접속자 10만
 * 일일 DAU 5만
 * 이상일 경우 시도해 볼 수 있겠다.
 *
 * 하지만, 한국 기준 서비스로 해당 규모는 극히 일부이므로 대부분 겪을 수 없는 시나리오다.
 * 따라서, MSA가 정말 필요하냐고 묻는다면 필요하지 않다고 말할 수 있겠다.
 *
 * [CQRS는?]
 * CQRS는 매우 추천하는 방식으로 도메인에 대한 분리와 Aggregate Root 구현으로
 * 관심사를 절대적으로 분리할 수 있다.
 * 이 방법을 권장하는 것은 이것이 절대적으로 논리 구조를 분리해 준다는 것에 있다.
 * 백엔드는 논리 구조의 단순성이 매우 중요하다.
 * UI 개발은 실제 UI를 추적하며 개발할 수 있지만 백엔드는 요청과 결과에 대한 논리 구현이므로
 * 코드가 길어지면 추적하기 매우 힘들어 진다.
 * 따라서, 대부분의 경우에 CQRS를 권장한다.
 * CQRS의 경우 아무리 복잡한 논리라도 대부분의 경우 500줄 이상 될 수 없다.
*/