using eXtensionSharp;
using Jennifer.Api;
using Jennifer.Jwt;
using Jennifer.Jwt.Infrastructure.Consts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using JwtOptions = Jennifer.Jwt.Infrastructure.Consts.JwtOptions;

DotNetEnv.Env.Load();

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

var cryptoOptions = new CryptoOptions(Environment.GetEnvironmentVariable("AES_KEY"),
    Environment.GetEnvironmentVariable("AES_IV"));
var jwtOptions = new JwtOptions(Environment.GetEnvironmentVariable("JWT_KEY"),
    Environment.GetEnvironmentVariable("JWT_ISSUER"),
    Environment.GetEnvironmentVariable("JWT_AUDIANCE"),
    Environment.GetEnvironmentVariable("JWT_EXPIRYMINUTES").xValue<int>(),
    Environment.GetEnvironmentVariable("JWT_REFRESHEXPIRYMINUTES").xValue<int>());
var smtpOptions = new EmailSmtpOptions(
    Environment.GetEnvironmentVariable("SMTP_HOST"),
    Environment.GetEnvironmentVariable("SMTP_PORT").xValue<int>(),
    Environment.GetEnvironmentVariable("SMTP_USERNAME"),
    Environment.GetEnvironmentVariable("SMTP_PASSWORD")
);

var options = new JenniferOptions("account", 
    Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION"), 
    cryptoOptions, 
    jwtOptions,
    smtpOptions);

// Add jennifer account manager
builder.Services.AddJennifer(options,
    (provider, optionsBuilder) =>
    {
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION"));
        if (builder.Environment.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging()
                .EnableThreadSafetyChecks()
                .EnableDetailedErrors();
        }
    }, null);
// Add jennifer hybrid cache
builder.Services.AddJenniferHybridCache(null, null);

// Add jennifer signalr hub
builder.Services.AddJenniferAuthHub(null);

// Add jennifer email
builder.Services.AddJenniferMailService();

// options =>
// {
//     options.Configuration = 
//         builder.Configuration.GetConnectionString("RedisConnectionString");
// }

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
                PreferredSecurityScheme = "Bearer"
            };        
    });
}

app.UseHttpsRedirection();

app.UseJennifer();

app.Run();