using System.IO.Compression;
using System.Text;
using eXtensionSharp;
using FluentValidation;
using Jennifer.External.OAuth;
using Jennifer.Jwt.Application.Auth;
using Jennifer.Jwt.Application.Auth.Services;
using Jennifer.Jwt.Application.Endpoints;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Hubs;
using Jennifer.Jwt.Infrastructure.Consts;
using Jennifer.Jwt.Infrastructure.Email;
using Jennifer.Jwt.Infrastructure.Session;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.Jwt.Services.AuthServices;
using Jennifer.Jwt.Services.UserServices;
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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Jennifer.Jwt;

/// <summary>
/// Provides extension methods for integrating Jennifer's functionalities
/// within an application, including database setup, caching, authentication,
/// email services, and middleware setup.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configures and adds Jennifer.Jwt services to the specified dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to which Jennifer.Jwt services will be added.</param>
    /// <param name="jenniferOptions">The configuration options used to setup Jennifer features and behavior.</param>
    /// <param name="dbContextOptions">A delegate to configure the DbContext options for Jennifer's database context.</param>
    /// <param name="identityOptions">A delegate to configure options for ASP.NET Core Identity.</param>
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
    public static void AddJennifer(this IServiceCollection services,
        JenniferOptions jenniferOptions,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
        Action<IdentityOptions> identityOptions)
    {
        JenniferOptionSingleton.Attach(jenniferOptions);
        
        services.AddDbContext<JenniferDbContext>(dbContextOptions);
        
        if (identityOptions.xIsEmpty())
        {
            identityOptions = (options) =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.Tokens.AuthenticatorTokenProvider = null!; // optional     
                options.User.RequireUniqueEmail = true;
            };
        }
        
        services.AddIdentity<User, Role>(identityOptions)
            .AddEntityFrameworkStores<JenniferDbContext>()
            .AddDefaultTokenProviders();

        /* [as cookie]
         *         builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
         */
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
        
        services.AddAuthService();
        services.AddUserService();
        services.AddCacheResolver();
        
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IExternalSignService, ExternalSignService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IUserClaimService, UserClaimService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleClaimService, RoleClaimService>();
        
        services.AddValidatorsFromAssemblyContaining<UserDtoValidator>(); // 자동 검증 필터 추가
        services.AddExternalOAuthHandler();

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
        if (cacheOptions.xIsEmpty())
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
        
        if (redisOptions.xIsNotEmpty())
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
        if(redisOptions.xIsEmpty())
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
    }
}