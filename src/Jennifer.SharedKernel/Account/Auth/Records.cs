namespace Jennifer.SharedKernel.Account.Auth;

public record TokenResponse(string AccessToken, string RefreshToken, bool isTwoFactor);

public record SignInRequest(string Email, string Password);


public record PasswordChangeRequest(string OldPassword, string NewPassword);

public class PasswordForgotChangeRequest
{
    public string Email { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
    public string VerifyType { get; set; }
}

/// <summary>
/// Represents a registration request for creating a new user account.
/// </summary>
/// <see cref="ENUM_EMAIL_VERIFY_TYPE"/>
public class RegisterRequest
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
    /// <see cref="ENUM_EMAIL_VERIFY_TYPE"/> which defines the allowed verification types.
    /// </remarks>
    public string VerifyType { get; set; }
}

public class RegisterAdminRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
}

/// <summary>
/// Represents a request for verifying an email address using a verification code.
/// </summary>
/// <see cref="ENUM_EMAIL_VERIFY_TYPE"/>
public class EmailConfirmRequest
{
    public EmailConfirmRequest(string email, string code, string verifyType)
    {
        Email = email;
        Code = code;
        VerifyType = verifyType;
    }

    public string Email { get; set; }
    public string Code { get; set; }
    public string VerifyType { get; set; }
}

public class VerifyCodeResponse
{
    public VerifyCodeResponse(string verityStatus, string message)
    {
        VerityStatus = verityStatus;
        Message = message;
    }

    public string VerityStatus { get; set; }
    public string Message { get; set; }   
}

public record EmailConfirmSendRequest(string Email, string UserName, string VerityStatus);

public record ExternalSignInRequest(string Provider, string AccessToken);