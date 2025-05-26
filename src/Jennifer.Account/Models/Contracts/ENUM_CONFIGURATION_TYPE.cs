using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Ardalis.SmartEnum;

namespace Jennifer.Account.Models.Contracts;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_CONFIGURATION_TYPE, string>))]
public class ENUM_CONFIGURATION_TYPE : SmartEnum<ENUM_CONFIGURATION_TYPE, string>
{
    public static ENUM_CONFIGURATION_TYPE WELCOME_MESSAGE_SUBJECT = new("WELCOME_EMAIL_SUBJECT", "WELCOME_EMAIL_SUBJECT");
    public static ENUM_CONFIGURATION_TYPE WELCOME_MESSAGE_BODY = new("WELCOME_EMAIL_BODY", "WELCOME_EMAIL_BODY");
    
    public ENUM_CONFIGURATION_TYPE(string name, string value) : base(name, value)
    {
    }
}