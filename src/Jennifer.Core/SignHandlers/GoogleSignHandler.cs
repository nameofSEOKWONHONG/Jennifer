using System.Net.Http.Headers;
using System.Net.Http.Json;
using Jennifer.Core.Domains;

namespace Jennifer.Core.SignHandlers;

public class GoogleSignHandler(IHttpClientFactory httpClientFactory) : ExternalSignHandler(httpClientFactory)
{
    public override async Task<IExternalSignResult> Verify(string providerToken, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("google");
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", providerToken);
        var res = await client.GetAsync("/oauth2/v3/userinfo", ct);
        res.EnsureSuccessStatusCode();
        
        var content = await res.Content.ReadFromJsonAsync<GoogleSignResult>(cancellationToken: ct);
        if (content is null) return null;
        
        return content;
    }
}