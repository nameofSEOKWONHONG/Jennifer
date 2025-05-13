using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Jennifer.Core.Infrastructure;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Jennifer.Jwt.Services;

public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtOptions;
    public JwtService(IOptions<JwtOptions> options)
    {
        _jwtOptions = options.Value;   
    }
    
    public string GenerateJwtToken(User user, List<Claim> userClaims, List<Claim> roleClaims)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        };
        claims.AddRange(userClaims);
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshTokenString(RefreshToken refreshToken)
    {
        var json = JsonSerializer.Serialize(refreshToken);
        var src = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return src.ToAesEncrypt();
    }

    public RefreshToken GenerateRefreshToken(string refreshToken)
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
