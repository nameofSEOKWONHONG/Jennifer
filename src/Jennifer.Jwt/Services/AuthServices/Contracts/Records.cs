using Jennifer.Jwt.Models;

namespace Jennifer.Jwt.Services.AuthServices.Contracts;


public record TokenResponse(string AccessToken, string RefreshToken);

public record SignInRequest(string Email, string Password);

public record PasswordChangeRequest(string OldPassword, string NewPassword);

public record PasswordForgotChangeRequest(string Email, string Code, string NewPassword, string Type);

public record RegisterRequest(string Email, string Password, string UserName, string PhoneNumber, string VerifyCode, string Type);

/// <summary>
/// Represents a request to verify a code for authentication or validation purposes.
/// </summary>
/// <param name="email">The email address associated with the request.</param>
/// <param name="Code">The verification code provided by the user.</param>
/// <param name="Type">The type of verification being performed.</param>
/// <see cref="ENUM_EMAIL_VERIFICATION_TYPE"/>
public record VerifyCodeRequest(string email, string Code, string Type);

public record VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS Status, string Message);