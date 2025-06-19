using System.Net.Http.Headers;
using System.Net.Http.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public class GoogleOAuthProvider(IHttpClientFactory httpClientFactory, IJMongoFactory factory) : ExternalOAuthProvider(httpClientFactory, factory, "google")
{
    public override async Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", providerToken);
        var res = await client.GetAsync("/oauth2/v3/userinfo", ct);
        if (!res.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get google user");
        
        var content = await res.Content.ReadFromJsonAsync<GoogleOAuthResult>(cancellationToken: ct);
        if (content is null) return ExternalOAuthResult.Fail("fail to get google user");
        
        var collection = this.mongoFactory.Create<ExternalOAuthDocument>();
        await collection.InsertOneAsync(new ExternalOAuthDocument()
        {
            Result = content.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken: ct);   
        
        return ExternalOAuthResult.Success(content.Sub, content.Email, content.Name);
    }
}