using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Domain.Accounts.Contracts;

/// <summary>
/// Represents the type of email verification for user operations such as registration and password recovery.
/// </summary>
/// <remarks>
/// This class is a smart enumeration that provides predefined values representing different email verification types.
/// It utilizes the Ardalis.SmartEnum library to simplify working with constants and ensure type safety.
/// </remarks>
[JsonConverter(typeof(SmartEnumNameConverter<ENUM_EMAIL_VERIFY_TYPE, int>))]
// ReSharper disable once InconsistentNaming
public class ENUM_EMAIL_VERIFY_TYPE(string name, int value) : SmartEnum<ENUM_EMAIL_VERIFY_TYPE, int>(name, value)
{
    public static readonly ENUM_EMAIL_VERIFY_TYPE SIGN_UP_BEFORE = new(nameof(SIGN_UP_BEFORE), 1);
    public static readonly ENUM_EMAIL_VERIFY_TYPE PASSWORD_FORGOT = new(nameof(PASSWORD_FORGOT), 2);
}
