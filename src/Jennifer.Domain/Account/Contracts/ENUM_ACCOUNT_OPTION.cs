using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Domain.Account.Contracts;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_ACCOUNT_OPTION, string>))]
public class ENUM_ACCOUNT_OPTION : SmartEnum<ENUM_ACCOUNT_OPTION, string>
{
    public static ENUM_ACCOUNT_OPTION WELCOME_MESSAGE_SUBJECT = new("WELCOME_EMAIL_SUBJECT", "WELCOME_EMAIL_SUBJECT");
    public static ENUM_ACCOUNT_OPTION WELCOME_MESSAGE_BODY = new("WELCOME_EMAIL_BODY", "WELCOME_EMAIL_BODY");
    public static ENUM_ACCOUNT_OPTION PASSWORD_RESET_SUBJECT = new("PASSWORD_RESET_EMAIL_SUBJECT", "PASSWORD_RESET_EMAIL_SUBJECT");
    public static ENUM_ACCOUNT_OPTION PASSWORD_RESET_BODY = new("PASSWORD_RESET_EMAIL_BODY", "PASSWORD_RESET_EMAIL_BODY");
    public static ENUM_ACCOUNT_OPTION OTP_URI = new("OTP_URI", "OTP_URI");
    
    public ENUM_ACCOUNT_OPTION(string name, string value) : base(name, value)
    {
    }
}