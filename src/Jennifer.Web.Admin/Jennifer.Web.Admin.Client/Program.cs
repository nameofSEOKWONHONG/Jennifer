using Jennifer.Web.Admin.Client;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddFluentUIComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();



await builder.Build().RunAsync();

public class ClientLoginModel
{
    [Required(ErrorMessage = "Username is Required")]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Input Invalid")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    public string Password { get; set; }
}