using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Jennifer.Web.Admin.Auth;
using Jennifer.Web.Admin.Client.Auth.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddHttpClient();

await builder.Build().RunAsync();
