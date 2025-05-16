using System.Security.Claims;
using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Services.Abstracts;

public interface IJwtService
{
    /// <summary>
    /// Generates a JWT (JSON Web Token) based on the provided user, user-specific claims, and role-specific claims.
    /// </summary>
    /// <param name="user">The user for whom the JWT is being generated.</param>
    /// <param name="userClaims">The list of claims specific to the user.</param>
    /// <param name="roleClaims">The list of claims associated with the user's roles.</param>
    /// <returns>A string containing the generated JWT.</returns>
    string GenerateJwtToken(User user, List<Claim> userClaims, List<Claim> roleClaims);
    
    /// <summary>
    /// Generates a new refresh token as a cryptographically secure random value and its encrypted counterpart.
    /// </summary>
    /// <returns>A tuple containing the base64-encoded refresh token and its encrypted version.</returns>
    string GenerateRefreshToken();    

    /// <summary>
    /// Converts the provided <see cref="RefreshToken"/> object into a base64-encoded string representation.
    /// </summary>
    /// <param name="refreshToken">The refresh token object to be serialized and encoded.</param>
    /// <returns>A base64-encoded string representation of the refresh token.</returns>
    string ObjectToTokenString(RefreshToken refreshToken);

    /// <summary>
    /// Generates a new refresh token based on the provided string representation of an existing refresh token.
    /// </summary>
    /// <param name="refreshToken">The string representation of the existing refresh token.</param>
    /// <returns>A deserialized RefreshToken object containing the token's data, including token value, expiry, creation date, and user ID.</returns>
    RefreshToken TokenStringToObject(string refreshToken);
}