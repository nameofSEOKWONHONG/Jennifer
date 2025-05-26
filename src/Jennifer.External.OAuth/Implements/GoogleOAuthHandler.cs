using System.Net.Http.Headers;
using System.Net.Http.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public class GoogleOAuthHandler(IHttpClientFactory httpClientFactory, IJMongoFactory factory) : ExternalOAuthHandler(httpClientFactory, factory, "google")
{
    public override async Task<IExternalOAuthResult> Verify(string providerToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", providerToken);
        var res = await client.GetAsync("/oauth2/v3/userinfo", ct);
        if (!res.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get google user");
        
        var content = await res.Content.ReadFromJsonAsync<GoogleOAuthResult>(cancellationToken: ct);
        if (content is null) return ExternalOAuthResult.Fail("fail to get google user");
        
        var collection = this.mongoFactory.Create<ExternalOAuthDocument>().GetCollection();
        await collection.InsertOneAsync(new ExternalOAuthDocument()
        {
            Result = content.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken: ct);   
        
        return ExternalOAuthResult.Success(content.Sub, content.Email, content.Name);
    }
}