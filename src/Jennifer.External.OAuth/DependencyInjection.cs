using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;
using Jennifer.External.OAuth.Implements;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Jennifer.External.OAuth;

public static class DependencyInjection
{
    public static void AddExternalOAuthHandler(this IServiceCollection services)
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
        
        services.AddTransient<IExternalOAuthHandler, FacebookOAuthHandler>();
        services.AddTransient<IExternalOAuthHandler, GoogleOAuthHandler>();
        services.AddTransient<IExternalOAuthHandler, KakaoOAuthHandler>();
        services.AddTransient<IExternalOAuthHandler, AppleOAuthHandler>();
        services.AddSingleton<IExternalOAuthHandlerFactory, ExternalOAuthHandlerFactory>();
    }
}