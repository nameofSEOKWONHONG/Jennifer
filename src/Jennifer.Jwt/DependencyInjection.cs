using System.Text;
using FluentValidation;
using Jennifer.SharedKernel.Consts;
using Jennifer.SharedKernel.Infrastructure;
using Jennifer.Jwt.Endpoints;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Hubs;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.SharedKernel.Infrastructure.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Jennifer.Jwt;

public static class DependencyInjection
{
    /// <summary>
    /// Adds the Jennifer.Jwt services to the dependency injection container.
    /// </summary>
    /// <param name="builder">The service collection to which the Jennifer.Jwt.Tenant services will be added.</param>
    /// <param name="schema">The database schema to be used by Jennifer.Jwt.Tenant.</param>
    /// <param name="dbContextOptions">A delegate to configure database context options. Accepts a service provider and a database context options builder.</param>
    /// <param name="identityOptions">A delegate to configure ASP.NET Core Identity options. Allows configuration of password policies, user lockout settings, login settings, and other Identity-related options.
    /// Default values:
    /// - Password.RequiredLength = 8
    /// - Password.RequireNonAlphanumeric = true
    /// - SignIn.RequireConfirmedAccount = false
    /// - SignIn.RequireConfirmedEmail = false
    /// - SignIn.RequireConfirmedPhoneNumber = false
    /// - Tokens.AuthenticatorTokenProvider = null
    /// - User.RequireUniqueEmail = true</param>
    /// <exception cref="ArgumentNullException">Thrown when the provided schema is null, empty, or consists only of whitespace.</exception>
    public static void AddJennifer(this WebApplicationBuilder builder, string schema,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
        Action<IdentityOptions> identityOptions)
    {
        if (string.IsNullOrWhiteSpace(schema)) throw new ArgumentNullException(nameof(schema));
        
        DotNetEnv.Env.Load();
        
        builder.Host.UseSerilog((context, services, config) =>
        {
            config
                .ReadFrom.Configuration(context.Configuration);
        });
        
        EntitySettings.Schema = schema;
        builder.Services.AddDbContext<JenniferDbContext>(dbContextOptions);
        if (identityOptions is null)
        {
            identityOptions = (options) =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Tokens.AuthenticatorTokenProvider = null!; // 선택적     
                options.User.RequireUniqueEmail = true;
            };
        }
        builder.Services.AddIdentity<User, Role>(identityOptions)
            .AddEntityFrameworkStores<JenniferDbContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection("JwtOptions"))
            .ValidateDataAnnotations();
        
        var jwtSection = builder.Configuration.GetSection("JwtOptions");
        var jwtOptions = jwtSection.Get<JwtOptions>();

        /* [as cookie]
         *         builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
         */
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions!.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
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
                        path.StartsWithSegments("/authHub")) // Hub 경로와 일치해야 함
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
        
        builder.Services.AddAuthorization();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHttpClient("kakao", client =>
        {
            client.BaseAddress = new Uri("https://kapi.kakao.com");
        });
        builder.Services.AddHttpClient("google", client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com");
        });
        builder.Services.AddHttpClient("naver", client =>
        {
            client.BaseAddress = new Uri("https://openapi.naver.com");
        });
        
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<GzipCompressionProvider>();
            options.Providers.Add<BrotliCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/octet-stream", "application/json" });
        });
        builder.Services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);
        builder.Services.Configure<BrotliCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Fastest);     
        
        builder.Services.AddScoped<ISessionContext, SessionContext>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IExternalSignService, ExternalSignService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRoleService, UserRoleService>();
        builder.Services.AddScoped<IUserClaimService, UserClaimService>();
        builder.Services.AddScoped<IRoleService, RoleService>();
        builder.Services.AddScoped<IRoleClaimService, RoleClaimService>();
        
        builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>(); // 자동 검증 필터 추가

    }

    /// <summary>
    /// Adds Jennifer.Jwt's hybrid caching services to the specified dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the hybrid caching services will be added.</param>
    /// <param name="cacheOptions">A delegate to configure hybrid cache options. If null, the following default settings will be applied:
    /// - MaximumPayloadBytes: 1MB (1024 * 1024)
    /// - MaximumKeyLength: 1024
    /// - DefaultEntryOptions:
    /// - Expiration: 30 minutes
    /// - LocalCacheExpiration: 5 minutes</param>
    /// <param name="redisOptions">An optional delegate to configure Redis cache options. If provided, Redis will be configured as part of the hybrid caching mechanism.</param>
    public static void AddJenniferHybridCache(this IServiceCollection services,
        Action<HybridCacheOptions> cacheOptions,
        Action<RedisCacheOptions> redisOptions)
    {
        if (cacheOptions is null)
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
        }
        services.AddHybridCache(cacheOptions);
        
        if (redisOptions is not null)
        {
            services.AddStackExchangeRedisCache(redisOptions);    
        }
    }

    /// <summary>
    /// Adds the Jennifer.Jwt Auth Hub services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the SignalR and related Jennifer.Jwt.Hub services will be added.</param>
    /// <param name="redisOptions">An optional delegate to configure Redis options for SignalR. If null, the default SignalR setup will be used without Redis integration.</param>
    public static void AddJenniferAuthHub(this IServiceCollection services, Action<RedisOptions> redisOptions)
    {
        if(redisOptions is null)
            services.AddSignalR();
        else
            services.AddSignalR().AddStackExchangeRedis(redisOptions);
        
        services.AddSingleton<IUserIdProvider, SubUserIdProvider>();
    }

    /// <summary>
    /// Adds the Jennifer.Mail services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which the Jennifer.Mail services will be added.
    /// This includes a singleton registration for email queuing and a hosted service for email sending operations.</param>
    public static void AddJenniferMailService(this IServiceCollection services)
    {
        services.AddSingleton<IEmailQueue, EmailQueue>();
        services.AddHostedService<EmailSenderService>();
    }

    /// <summary>
    /// Adds the Jennifer.Jwt endpoint mappings to the application's endpoint route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder instance where the Jennifer.Jwt endpoints will be mapped.</param>
    private static void UseJenniferEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoint();
        app.MapUserEndpoint();
        app.MapUserRoleEndpoint();
        app.MapUserClaimEndpoint();
        app.MapRoleEndpoint();
    }

    /// <summary>
    /// Configures Jennifer.Jwt middleware and SignalR hub for the application.
    /// </summary>
    /// <param name="app">The WebApplication instance where the middleware and SignalR hub will be configured.</param>
    public static void UseJennifer(this WebApplication app)
    {
        // set account route api endpoint jennifer manager
        app.UseJenniferEndpoints();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseMiddleware<SessionContextMiddleware>(); // 반드시 인증 이후에 실행

        app.MapHub<AuthHub>("/authHub");
    }
}