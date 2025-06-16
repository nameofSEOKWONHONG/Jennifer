using Blazored.SessionStorage;
using Jennifer.Wasm.Admin;
using Jennifer.Wasm.Admin.Auth;
using Jennifer.Wasm.Admin.Auth.Services;
using Jennifer.Wasm.Admin.Services.Weathers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7288") });
builder.Services.AddHttpClient<IWeatherService, WeatherService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
builder.Services.AddFluentUIComponents();

builder.Services.AddBlazoredSessionStorageAsSingleton();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddSingleton<JwtAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider, JwtAuthenticationStateProvider>();

builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();

