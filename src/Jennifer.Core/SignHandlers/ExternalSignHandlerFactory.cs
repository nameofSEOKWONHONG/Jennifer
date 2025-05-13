using Jennifer.Core.SignHandlers;

namespace Jennifer.Jwt.Services.Handlers;

public class ExternalSignHandlerFactory
{
    public static ExternalSignHandler Create(string provider, IHttpClientFactory httpClientFactory)
    {
        return provider switch
        {
            "kakao" => new KakaoSignHandler(httpClientFactory),
            "google" => new GoogleSignHandler(httpClientFactory),
            _ => null
        };
    }
}