using System.Net.Http.Json;
using Jennifer.Core.Domains;

namespace Jennifer.Core.SignHandlers;

public class KakaoSignHandler(IHttpClientFactory httpClientFactory) : ExternalSignHandler(httpClientFactory)
{
    public override async Task<IExternalSignResult> Verify(string providerToken, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("kakao");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("Authorization", $"bearer {providerToken}");
        var authResponse = await client.GetAsync("/v1/user/access_token_info", ct);
        if (!authResponse.IsSuccessStatusCode) return null;
        var kakaoAuthResult = await authResponse.Content.ReadFromJsonAsync<KakaoTokenInfoResult>(cancellationToken: ct);
        if (kakaoAuthResult.Id <= 0) return null;

        var info = await client.GetAsync("/v2/user/me", ct);
        if (!info.IsSuccessStatusCode) return null;

        var result = await info.Content.ReadFromJsonAsync<KakaoUserResult>(cancellationToken: ct);
        if (result is null) return null;

        return new KakaoSignResult()
        {
            ProviderId = result.Id.ToString(),
            Email = result.KakaoAccount.Email ?? "",
            Name = result.KakaoAccount.Profile?.Nickname ?? ""
        };
    }
}