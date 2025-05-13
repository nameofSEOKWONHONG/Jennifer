using Jennifer.Jwt.Domains;

namespace Jennifer.Jwt.Services;

public interface IExternalSignService
{
    /// <summary>
    /// Signs in a user using an external provider, verifies the provider token, and creates or links the user's account.
    /// </summary>
    /// <param name="provider">The name of the external provider (e.g., kakao, google, apple, naver).</param>
    /// <param name="providerToken">The token provided by the external provider to verify the user's credentials.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the access and refresh tokens if the sign-in is successful; otherwise, null.</returns>
    Task<TokenResponse> SignIn(string provider, string providerToken, CancellationToken ct);
}