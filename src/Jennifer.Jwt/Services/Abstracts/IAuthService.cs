using Jennifer.Jwt.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace Jennifer.Jwt.Services.Abstracts;

/// <summary>
/// Provides services for user authentication and authorization processes,
/// including user registration, sign-in, sign-out, token refresh, and password management.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user in the system with the provided registration details.
    /// </summary>
    /// <param name="request">The request object containing the user registration details, including the email and password.</param>
    /// <returns>An <see cref="IResult"/> indicating the outcome of the registration process, such as success or any errors encountered.</returns>
    Task<IResult> Register(RegisterRequest request);

    /// <summary>
    /// Signs in a user using the provided email and password, and generates an access token and refresh token if successful.
    /// </summary>
    /// <param name="email">The email address of the user attempting to sign in.</param>
    /// <param name="password">The password of the user attempting to sign in.</param>
    /// <returns>A <see cref="TokenResponse"/> containing the generated access token and refresh token, or null if authentication fails.</returns>
    Task<TokenResponse> Signin(string email, string password);

    /// <summary>
    /// Authenticates a user using their email and password, and signs them in using cookie-based authentication.
    /// </summary>
    /// <param name="email">The email address of the user attempting to sign in.</param>
    /// <param name="password">The password of the user attempting to sign in.</param>
    /// <returns>A boolean value indicating whether the sign-in was successful.</returns>
    Task<bool> CookieSignIn(string email, string password);

    /// <summary>
    /// Signs out the currently authenticated user by removing their authentication token.
    /// </summary>
    /// <returns>A <see cref="Task{TResult}"/> containing a boolean value indicating whether the sign-out operation was successful.</returns>
    Task<bool> SignOut();

    /// <summary>
    /// Refreshes the user's authentication token using the provided refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token issued to the user for renewing their session.</param>
    /// <returns>A <see cref="TokenResponse"/> containing a new access token and a new refresh token, or null if the operation fails.</returns>
    Task<TokenResponse> RefreshToken(string refreshToken);

    /// <summary>
    /// Generates a password reset token for the user associated with the provided email address.
    /// </summary>
    /// <param name="email">The email address of the user requesting the password reset token.</param>
    /// <returns>A string containing the password reset token if the user exists; otherwise, null.</returns>
    Task<string> RequestChangePasswordToken(string email);

    /// <summary>
    /// Resets the password for a user based on a valid reset token, email address, and a new password.
    /// </summary>
    /// <param name="email">The email address of the user requesting the password reset.</param>
    /// <param name="token">The password reset token generated and associated with the user.</param>
    /// <param name="newPassword">The new password to be set for the user's account.</param>
    /// <returns>A <see cref="bool"/> indicating whether the password reset operation was successful.</returns>
    Task<bool> ChangePasswordWithToken(string email, string token, string newPassword);

    Task<bool> RequestConfirmEmailToken(string url, string email);

    Task<bool> ConfirmEmail(string userId, string code);

    Task<bool> UpdateUserInfo(string email, string name, string phoneNumber);
}