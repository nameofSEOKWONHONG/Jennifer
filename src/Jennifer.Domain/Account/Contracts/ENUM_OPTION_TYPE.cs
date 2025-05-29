using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Domain.Account.Contracts;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_OPTION_TYPE, string>))]
// ReSharper disable once InconsistentNaming
public class ENUM_OPTION_TYPE(string name, string value) : SmartEnum<ENUM_OPTION_TYPE, string>(name, value)
{
    public static ENUM_OPTION_TYPE WELCOME_MESSAGE_SUBJECT = new("WELCOME_MESSAGE_SUBJECT", "WELCOME_MESSAGE_SUBJECT");
    public static ENUM_OPTION_TYPE WELCOME_MESSAGE_BODY = new("WELCOME_MESSAGE_BODY", "WELCOME_MESSAGE_BODY");
    public static ENUM_OPTION_TYPE PASSWORD_RESET_SUBJECT = new("PASSWORD_RESET_SUBJECT", "PASSWORD_RESET_SUBJECT");
    public static ENUM_OPTION_TYPE PASSWORD_RESET_BODY = new("PASSWORD_RESET_BODY", "PASSWORD_RESET_BODY");
    public static ENUM_OPTION_TYPE OTP_URI = new("OTP_URI", "OTP_URI");
}