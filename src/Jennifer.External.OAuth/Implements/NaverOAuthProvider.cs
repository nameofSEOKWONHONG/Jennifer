using System.Net.Http.Headers;
using System.Net.Http.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;

namespace Jennifer.External.OAuth.Implements;

public sealed class NaverOAuthProvider(
    IHttpClientFactory httpClientFactory,
    IJMongoFactory mongoFactory
) : ExternalOAuthProvider(httpClientFactory, mongoFactory, "naver")
{
    public override async Task<IExternalOAuthResult> AuthenticateAsync(string accessToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var info = await client.GetAsync("/v1/nid/me", ct);
        if (!info.IsSuccessStatusCode)
            return ExternalOAuthResult.Fail("fail to get naver user");

        var result = await info.Content.ReadFromJsonAsync<NaverUserResponse>(cancellationToken: ct);
        if (result?.Response == null || string.IsNullOrEmpty(result.Response.Id))
            return ExternalOAuthResult.Fail("invalid naver user response");

        var collection = this.mongoFactory.Create<ExternalOAuthDocument>();
        await collection.InsertOneAsync(new ExternalOAuthDocument
        {
            Result = result.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow
        }, cancellationToken: ct);

        return ExternalOAuthResult.Success(result.Response.Id, result.Response.Email ?? string.Empty, result.Response.Name ?? result.Response.Nickname ?? string.Empty);
    }
}
