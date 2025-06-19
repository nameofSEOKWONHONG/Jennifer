using System.IdentityModel.Tokens.Jwt;
using eXtensionSharp;
using eXtensionSharp.Mongo;
using Jennifer.External.OAuth.Abstracts;
using Jennifer.External.OAuth.Contracts;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Jennifer.External.OAuth.Implements;

public class AppleOAuthProvider : ExternalOAuthProvider
{
    public AppleOAuthProvider(IHttpClientFactory httpClientFactory, IJMongoFactory factory) : base(httpClientFactory, factory, "apple")
    {
    }

    public override async Task<IExternalOAuthResult> AuthenticateAsync(string providerToken, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient(this.Provider);
        var json = await client.GetStringAsync("/auth/keys", ct);

        var jwks = new JsonWebKeySet(json);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://appleid.apple.com",
            ValidAudience = ExternalOAuthOption.Instance.Options["AppleClientId"], //"com.your.bundle.id", // ← Apple Developer에서 등록한 Client ID
            IssuerSigningKeys = jwks.Keys, // JWKS에서 가져온 키
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
        var principal = tokenHandler.ValidateToken(providerToken, validationParameters, out var validatedToken);
        var providerId = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var email = principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        if (providerId.xIsEmpty()) return ExternalOAuthResult.Fail("providerId is empty");

        var collection = this.mongoFactory.Create<ExternalOAuthDocument>();
        await collection.InsertOneAsync(new ExternalOAuthDocument()
        {
            Result = principal.xSerialize(),
            CreatedAt = DateTimeOffset.UtcNow,
        }, cancellationToken: ct);

        return ExternalOAuthResult.Success(providerId, email, string.Empty);
    }
}