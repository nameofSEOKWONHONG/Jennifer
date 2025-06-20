using eXtensionSharp;
using FluentValidation;
using Jennifer.Api;
using Jennifer.Account;
using Jennifer.Account.Application.Auth.Commands.SignUp;
using Jennifer.Domain.Common;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Middlewares;
using Jennifer.SharedKernel;
using Jennifer.Todo;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer<OpenApiSecuritySchemeTransformer>();
});

builder.Host.UseSerilog((context, services, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration);
});

#region [setting mediator]
builder.Services.AddScoped<DomainEventDispatcher>();
builder.Services.AddValidatorsFromAssemblyContaining<SignUpAdminCommandValidator>();

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
    options.Assemblies = [
        typeof(Jennifer.Account.DependencyInjection).Assembly,
        typeof(Jennifer.Todo.DependencyInjection).Assembly,
        typeof(Jennifer.Domain.DependencyInjection).Assembly
    ];
    options.PipelineBehaviors =
    [
        typeof(TransactionBehavior<,>),
        typeof(ValidationBehavior<,>)
    ];
});

#endregion

var cryptoOptions = new CryptoOptions(
    builder.Configuration["AES_KEY"],
    builder.Configuration["AES_IV"]);
var jwtOptions = new JwtOptions(builder.Configuration["JWT_KEY"],
    builder.Configuration["JWT_ISSUER"],
    builder.Configuration["JWT_AUDIANCE"],
    builder.Configuration["JWT_EXPIRYMINUTES"].xValue<int>(),
    builder.Configuration["JWT_REFRESHEXPIRYMINUTES"].xValue<int>());
var smtpOptions = new EmailSmtpOptions(
    builder.Configuration["SMTP_HOST"],
    builder.Configuration["SMTP_PORT"].xValue<int>(),
    builder.Configuration["SMTP_USERNAME"],
    builder.Configuration["SMTP_PASSWORD"]
);

var jenniferOptions = new JenniferOptions(
    cryptoOptions, 
    jwtOptions,
    smtpOptions);

// Add jennifer account manager
builder.Services.AddJennifer(jenniferOptions,
        (provider, optionsBuilder) =>
        {
            optionsBuilder.UseNpgsql(builder.Configuration["SQLSERVER_CONNECTION"])
            //optionsBuilder.UseSqlServer(builder.Configuration["SQLSERVER_CONNECTION"])
                .AddInterceptors(new AuditInterceptor());
            if (builder.Environment.IsDevelopment())
            {
                optionsBuilder.EnableSensitiveDataLogging()
                    .EnableThreadSafetyChecks()
                    .EnableDetailedErrors();
            }
        }, identityOptions =>
        {
            identityOptions.Password.RequiredLength = 8;
            identityOptions.Password.RequireNonAlphanumeric = true;
            identityOptions.SignIn.RequireConfirmedAccount = false;
            identityOptions.SignIn.RequireConfirmedEmail = false;
            identityOptions.SignIn.RequireConfirmedPhoneNumber = false;
            identityOptions.Tokens.AuthenticatorTokenProvider = null!; // optional
            identityOptions.User.RequireUniqueEmail = true;
            identityOptions.User.AllowedUserNameCharacters = null!;
        })
    // Add jennifer cache
    .WithJenniferDistributeCache(builder.Configuration["REDIS_AUTH_CONNECTION"])
    // Add jennifer ip block, need redis IConnectionMultiplexer, use WithJenniferCache
    .WithJenniferIpRateLimit()    
    // Add jennifer signalr
    .WithJenniferSignalr(builder.Configuration["REDIS_HUB_BACKPLANE"])
    // Add jennifer mongodb
    .WithJenniferMongodb(builder.Configuration["MONGODB_CONNECTION"])
    // Add jennifer email
    .WithJenniferMailService(builder.Configuration["KAFKA_CONNECTION"]);

builder.Services.AddTodo((provider, optionsBuilder) =>
{
    optionsBuilder.UseNpgsql(builder.Configuration["SQLSERVER_CONNECTION"])
        //optionsBuilder.UseSqlServer(builder.Configuration["SQLSERVER_CONNECTION"])
        .AddInterceptors(new AuditInterceptor());
    if (builder.Environment.IsDevelopment())
    {
        optionsBuilder.EnableSensitiveDataLogging()
            .EnableThreadSafetyChecks()
            .EnableDetailedErrors();
    }
});

builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(res => res.AddService("jennifer-api"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddEntityFrameworkCoreInstrumentation()
            .AddRedisInstrumentation()
            .AddNpgsql()
            ;

        tracing.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(builder.Configuration["OTEL_EXPORTER_ENDPOINT"].xValue<string>());
        });
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.None;
        options.Authentication = 
            new ScalarAuthenticationOptions
            {
                PreferredSecuritySchemes = ["Bearer"]
            };        
    });
}

app.UseHttpsRedirection();

app.UseJennifer();
app.UseTodo();

await app.RunAsync();