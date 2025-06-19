using System.Text.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public class FacebookOAuthProvider: ExternalOAuthProvider
{
    public FacebookOAuthProvider(IHttpClientFactory httpClientFactory, IJMongoFactory factory) : base(httpClientFactory, factory, "facebook")
    {
    }

    public override async Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Clear();
        var response = await client.GetAsync($"/me?fields=id,name,email&access_token={providerToken}", ct);
        if (!response.IsSuccessStatusCode) return ExternalOAuthResult.Fail("fail to get facebook user");
        
        var content = await response.Content.ReadAsStringAsync(ct);

        // 2. JSON 파싱
        var fbUser = JsonSerializer.Deserialize<FacebookUserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (fbUser is null || string.IsNullOrEmpty(fbUser.Id)) return ExternalOAuthResult.Fail("fail to get facebook user");
        
        var collection = this.mongoFactory.Create<ExternalOAuthDocument>();
        await collection.InsertOneAsync(new ExternalOAuthDocument()
        {
            Result = fbUser.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken: ct);        
        
        return ExternalOAuthResult.Success(fbUser.Id, fbUser.Email, fbUser.Name);
    }
}

