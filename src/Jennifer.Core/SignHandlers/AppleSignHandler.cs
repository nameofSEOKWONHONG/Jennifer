using System.IdentityModel.Tokens.Jwt;
using Jennifer.Core.Domains;
using Jennifer.Jwt.Services.Handlers;
using Microsoft.IdentityModel.Tokens;

namespace Jennifer.Core.SignHandlers;

public class AppleSignHandler : ExternalSignHandler
{
    public AppleSignHandler(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
    {
    }

    public override async Task<IExternalSignResult> Verify(string providerToken, CancellationToken ct)
    {
        var client = this._httpClientFactory.CreateClient("apple");
        var json = await client.GetStringAsync("https://appleid.apple.com/auth/keys", ct);

        var jwks = new JsonWebKeySet(json);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = "https://appleid.apple.com",
            ValidAudience = "com.your.bundle.id", // ← Apple Developer에서 등록한 Client ID
            IssuerSigningKeys = jwks.Keys, // JWKS에서 가져온 키
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true
        };
        var principal = tokenHandler.ValidateToken(providerToken, validationParameters, out var validatedToken);
        var providerId = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var email = principal.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        return new AppleSignResult
        {
            ProviderId = providerId ?? throw new Exception("Missing sub"),
            Email = email ?? throw new Exception("Missing email"),
            Name = "" // Apple은 이름을 클레임에 넣지 않음
        };
    }
}