using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;
using Microsoft.IdentityModel.Tokens;

namespace Jennifer.External.OAuth.Implements;

public sealed class LineOAuthProvider(
    IHttpClientFactory httpClientFactory,
    IJMongoFactory mongoFactory
) : ExternalOAuthProvider(httpClientFactory, mongoFactory, "line")
{
    public override async Task<IExternalOAuthResult> AuthenticateAsync(string idToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        var channelId = ExternalOAuthOption.Instance.Options["OAuth:Line:ClientId"];

        var tokenHandler = new JwtSecurityTokenHandler();
        var keys = await GetLineSigningKeysAsync(client, ct);

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://access.line.me",
            ValidAudience = channelId,
            IssuerSigningKeys = keys,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };

        try
        {
            var principal = tokenHandler.ValidateToken(idToken, validationParameters, out _);
            var userId = principal.FindFirst("sub")?.Value;
            var email = principal.FindFirst("email")?.Value;
            var name = principal.FindFirst("name")?.Value;
            var picture = principal.FindFirst("picture")?.Value;

            var result = new LineIdTokenClaims
            {
                UserId = userId,
                Email = email,
                Name = name,
                PictureUrl = picture
            };

            var collection = this.mongoFactory.Create<ExternalOAuthDocument>().GetCollection();
            await collection.InsertOneAsync(new ExternalOAuthDocument
            {
                Result = result.xSerialize(),
                CreatedAt = DateTimeOffset.UtcNow
            }, cancellationToken: ct);

            return ExternalOAuthResult.Success(userId, email, name);
        }
        catch (SecurityTokenException ex)
        {
            return ExternalOAuthResult.Fail("LINE id_token validation failed: " + ex.Message);
        }
    }
    
    private static async Task<IEnumerable<SecurityKey>> GetLineSigningKeysAsync(HttpClient client, CancellationToken ct)
    {
        var response = await client.GetFromJsonAsync<JsonWebKeySet>("/oauth2/v2.1/certs", ct);
        return response.Keys.Select(k => new JsonWebKey(k.ToString()));
    }
}
