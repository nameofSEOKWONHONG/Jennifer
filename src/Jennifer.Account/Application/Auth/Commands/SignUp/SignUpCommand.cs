using Jennifer.Account.Behaviors;
using Jennifer.Account.Models.Contracts;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.SharedKernel;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed class SignUpCommand : ITransactionCommand<Result<Guid>>
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
    /// Represents the type of email verification being applied.
    /// </summary>
    /// <remarks>
    /// This property is used to specify the email verification method
    /// when handling a registration request. It is associated with
    /// <see cref="ENUM_EMAIL_VERIFICATION_TYPE"/> which defines the allowed verification types.
    /// </remarks>
    public ENUM_EMAIL_VERIFICATION_TYPE Type { get; set; }

    public SignUpCommand(string email, string password, string userName, string phoneNumber, ENUM_EMAIL_VERIFICATION_TYPE type)
    {
        Email = email;
        Password = password;
        UserName = userName;
        PhoneNumber = phoneNumber;
        Type = type;
    }
}