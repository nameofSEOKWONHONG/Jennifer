using eXtensionSharp;
using Jennifer.Api;
using Jennifer.Account;
using Jennifer.Account.Data;
using Jennifer.Infrastructure.Options;
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
    builder.Configuration["SQLSERVER_CONNECTION"], 
    builder.Configuration["MONGODB_CONNECTION"], 
    cryptoOptions, 
    jwtOptions,
    smtpOptions);



// Add jennifer account manager
builder.Services.AddJennifer(options,
        (provider, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(builder.Configuration["SQLSERVER_CONNECTION"])
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
            identityOptions.User.AllowedUserNameCharacters = null;
        })
    // Add jennifer cache
    .WithJenniferCache(builder.Configuration["REDIS_AUTH_CONNECTION"])
    // Add jennifer signalr
    .WithJenniferSignalr(builder.Configuration["REDIS_HUB_BACKPLANE"])
    // Add jennifer mongodb
    .WithJenniferMongodb(builder.Configuration["MONGODB_CONNECTION"])
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