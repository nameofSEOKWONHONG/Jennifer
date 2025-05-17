using System.Net.Http.Headers;
using System.Net.Http.Json;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public class GoogleOAuthHandler(IHttpClientFactory httpClientFactory) : ExternalOAuthHandler(httpClientFactory, "google")
{
    public override async Task<IExternalOAuthResult> Verify(string providerToken, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", providerToken);
        var res = await client.GetAsync("/oauth2/v3/userinfo", ct);
        if (!res.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get google user");
        
        var content = await res.Content.ReadFromJsonAsync<GoogleOAuthResult>(cancellationToken: ct);
        if (content is null) return ExternalOAuthResult.Fail("fail to get google user");
        
        return ExternalOAuthResult.Success(content.Sub, content.Email, content.Name);
    }
}