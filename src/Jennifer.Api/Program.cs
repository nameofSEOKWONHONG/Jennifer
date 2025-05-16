using Jennifer.Api;
using Jennifer.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
    options.AddDocumentTransformer<OpenApiSecuritySchemeTransformer>();
});

// Add jennifer account manager
builder.AddJennifer("account", 
    (provider, optionsBuilder) =>
    {
        var con = builder.Configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(con);
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