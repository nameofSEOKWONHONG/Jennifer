using eXtensionSharp;
using Jennifer.Api;
using Jennifer.Infrastructure.Options;
using Jennifer.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using JwtOptions = Jennifer.Infrastructure.Options.JwtOptions;

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

var options = new JenniferOptions("account", 
    Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION"), 
    cryptoOptions, 
    jwtOptions,
    smtpOptions);

// Add jennifer account manager
builder.Services.AddJennifer(options,
        (provider, optionsBuilder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            optionsBuilder.UseSqlServer(configuration["SQLSERVER_CONNECTION"]);
            if (builder.Environment.IsDevelopment())
            {
                optionsBuilder.EnableSensitiveDataLogging()
                    .EnableThreadSafetyChecks()
                    .EnableDetailedErrors();
            }
        }, null)
    // Add jennifer hybrid cache
    .WithJenniferHybridCache(null, null)
    // Add jennifer signalr hub
    // options =>
    // {
    //     options.Configuration = 
    //         builder.Configuration.GetConnectionString("RedisConnectionString");
    // }
    .WithJenniferAuthHub(null)
    // Add jennifer email
    .WithJenniferMailService();



builder.Services.AddSwaggerGen();

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

app.UseSwagger();

app.UseHttpsRedirection();

app.UseJennifer();

app.Run();