using System.Net.Http.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public class KakaoOAuthProvider(IHttpClientFactory httpClientFactory, IJMongoFactory factory) : ExternalOAuthProvider(httpClientFactory, factory, "kakao")
{
    public override async Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"bearer {providerToken}");
        var authResponse = await client.GetAsync("/v1/user/access_token_info", ct);
        if (!authResponse.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get kakao user");
        
        var kakaoAuthResult = await authResponse.Content.ReadFromJsonAsync<KakaoTokenInfoResult>(cancellationToken: ct);
        if (kakaoAuthResult.Id <= 0) return ExternalOAuthResult.Fail("fail to get kakao user");

        var info = await client.GetAsync("/v2/user/me", ct);
        if (!info.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get kakao user");

        var result = await info.Content.ReadFromJsonAsync<KakaoUserResult>(cancellationToken: ct);
        if (result is null) return ExternalOAuthResult.Fail("fail to get kakao user");
        
        var collection = this.mongoFactory.Create<ExternalOAuthDocument>().GetCollection();
        await collection.InsertOneAsync(new ExternalOAuthDocument()
        {
            Result = result.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken: ct);  
        
        return ExternalOAuthResult.Success(result.Id.ToString(), result.KakaoAccount.Email ?? "", result.KakaoAccount.Profile?.Nickname ?? "");
    }
}