using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;

namespace Jennifer.Jwt.Services.AuthServices.Contracts;


public record TokenResponse(string AccessToken, string RefreshToken);

public record SignInRequest(string Email, string Password);

public record PasswordChangeRequest(string OldPassword, string NewPassword);

public class PasswordForgotChangeRequest(
    string Email,
    string Code,
    string NewPassword,
    ENUM_EMAIL_VERIFICATION_TYPE Type)
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }
}

/// <summary>
/// Represents a registration request for creating a new user account.
/// </summary>
/// <param name="Email">The email address provided by the user for registration.</param>
/// <param name="Password">The password chosen by the user for account security.</param>
/// <param name="UserName">The username selected by the user for the account.</param>
/// <param name="PhoneNumber">The phone number associated with the user account.</param>
/// <param name="VerifyCode">The verification code used for account validation.</param>
/// <param name="Type">The type of email verification being applied.</param>
/// <see cref="ENUM_EMAIL_VERIFICATION_TYPE"/>
public class RegisterRequest(
    string Email,
    string Password,
    string UserName,
    string PhoneNumber,
    string VerifyCode,
    ENUM_EMAIL_VERIFICATION_TYPE Type)
{
    /// <summary>
    /// Gets or sets the email address associated with the user or registration process.
    /// </summary>
    /// <remarks>
    /// This property is used as a primary identifier for user communication and validation during the registration and authentication processes.
    /// It is also utilized for sending verification codes and confirming the user's email address.
    /// </remarks>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the password chosen by the user for account security.
    /// </summary>
    /// <remarks>
    /// The password is used during user authentication to verify the identity of the requesting user.
    /// It should meet the required security criteria, such as having a minimum length
    /// and including a combination of alphanumeric and special characters.
    /// </remarks>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the username selected by the user for the account.
    /// </summary>
    /// <remarks>
    /// The <c>UserName</c> property represents the unique identifier chosen by the user,
    /// typically used during registration to personalize their account.
    /// This property is required and is commonly utilized for authentication or display purposes.
    /// </remarks>
    public string UserName { get; set; }

    /// <summary>
    /// Gets or sets the phone number associated with the user account.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the verification code used for account validation during the registration
    /// process or other authentication-related operations.
    /// </summary>
    public string VerifyCode { get; set; }

    /// <summary>
    /// Represents the type of email verification being applied.
    /// </summary>
    /// <remarks>
    /// This property is used to specify the email verification method
    /// when handling a registration request. It is associated with
    /// <see cref="ENUM_EMAIL_VERIFICATION_TYPE"/> which defines the allowed verification types.
    /// </remarks>
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }   
}

/// <summary>
/// Represents a request to verify a code for authentication or validation purposes.
/// </summary>
/// <param name="Email">The email address associated with the request.</param>
/// <param name="Code">The verification code provided by the user.</param>
/// <param name="Type">The type of verification being performed.</param>
/// <see cref="ENUM_EMAIL_VERIFICATION_TYPE"/>
public class VerifyCodeRequest(string Email, string Code, ENUM_EMAIL_VERIFICATION_TYPE Type)
{
    public string Email { get; set; }
    public string Code { get; set; }
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }   
}

public class VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS Status, string Message)
{
    public ENUM_VERITY_RESULT_STATUS Status { get; set; }
    public string Message { get; set; }   
}

public record VerifyCodeByEmailSendRequest(string Email, ENUM_EMAIL_VERIFICATION_TYPE Type);