using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Converters;
using Jennifer.SharedKernel;
using Microsoft.IdentityModel.Tokens;

namespace Jennifer.Account.Application.Auth.Services.Implements;

public sealed class TokenService : ITokenService
{
    public TokenService()
    {
    }
    
    public string GenerateJwtToken(string sid, User user, List<Claim> userClaims, List<Claim> roleClaims)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, sid),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim("emailConfirmed", user.EmailConfirmed.ToString()),
            new Claim("cs", user.ConcurrencyStamp!)
        };
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        var jwtOptions = JenniferOptionSingleton.Instance.Options.Jwt;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.ExpireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string ObjectToTokenString(RefreshToken refreshToken)
    {
        var json = JsonSerializer.Serialize(refreshToken);
        var src = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return src.ToAesEncrypt();
    }

    public RefreshToken TokenStringToObject(string refreshToken)
    {
        var src = refreshToken.ToAesDecrypt();
        var json = Encoding.UTF8.GetString(Convert.FromBase64String(src));
        return JsonSerializer.Deserialize<RefreshToken>(json);
    }

    public string GenerateRefreshToken()
    { 
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

public record RefreshToken(string Token, DateTime Expiry, DateTime CreateOn, string UserId);
