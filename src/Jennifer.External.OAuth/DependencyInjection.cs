using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;
using Jennifer.External.OAuth.Implements;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.External.OAuth;

public static class DependencyInjection
{
    /// <summary>
    /// Registers external OAuth providers and their dependencies into the service container.
    /// This includes HTTP clients for each supported OAuth provider (Kakao, Google, Facebook, Apple, Naver)
    /// and their corresponding provider implementations.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to add the services to.
    /// </param>
    /// <param name="configure">
    /// An optional action to configure provider-specific options via <see cref="ExternalOAuthOption.Instance"/>.
    /// Keys are expected to be provider names (e.g., "google", "kakao") and values the associated client IDs.
    /// </param>
    /// <remarks>
    /// This method wires up:
    /// <list type="bullet">
    /// <item>
    /// <description><see cref="IHttpClientFactory"/> clients for each provider, with base addresses preconfigured.</description>
    /// </item>
    /// <item>
    /// <description><see cref="IExternalOAuthProvider"/> implementations for each provider, using <c>AddTransient</c>.</description>
    /// </item>
    /// <item>
    /// <description>A singleton factory <see cref="IExternalOAuthProviderFactory"/> to resolve the appropriate provider by key.</description>
    /// </item>
    /// <item>
    /// <description>Calls <paramref name="configure"/> to populate the shared options dictionary, if provided. e.g, AppleClientId</description>
    /// </item>
    /// </list>
    /// </remarks>
    public static void AddExternalOAuthHandler(this IServiceCollection services, Action<Dictionary<string, string>> configure = null)
    {
        services.AddHttpContextAccessor();
        
        services.AddHttpClient("kakao", client =>
        {
            client.BaseAddress = new Uri("https://kapi.kakao.com");
        });
        services.AddHttpClient("google", client =>
        {
            client.BaseAddress = new Uri("https://www.googleapis.com");
        });
        services.AddHttpClient("facebook", client =>
        {
            client.BaseAddress = new Uri("https://graph.facebook.com/v18.0");
        });
        services.AddHttpClient("apple", client =>
        {
            client.BaseAddress = new Uri("https://appleid.apple.com");
        });
        services.AddHttpClient("naver", client =>
        {
            client.BaseAddress = new Uri("https://openapi.naver.com");
        });
        services.AddHttpClient("line", client =>
        {
            client.BaseAddress = new Uri("https://api.line.me");
        });
        
        services.AddTransient<IExternalOAuthProvider, FacebookOAuthProvider>();
        services.AddTransient<IExternalOAuthProvider, GoogleOAuthProvider>();
        services.AddTransient<IExternalOAuthProvider, KakaoOAuthProvider>();
        services.AddTransient<IExternalOAuthProvider, AppleOAuthProvider>();
        services.AddTransient<IExternalOAuthProvider, NaverOAuthProvider>();
        services.AddTransient<IExternalOAuthProvider, LineOAuthProvider>();
        services.AddSingleton<IExternalOAuthProviderFactory, ExternalOAuthProviderFactory>();
        
        configure?.Invoke(ExternalOAuthOption.Instance.Options);
    }
}