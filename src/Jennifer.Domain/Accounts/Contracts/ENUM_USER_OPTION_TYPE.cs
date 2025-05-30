using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Domain.Accounts.Contracts;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_USER_OPTION_TYPE, string>))]
// ReSharper disable once InconsistentNaming
public class ENUM_USER_OPTION_TYPE(string name, string value) : SmartEnum<ENUM_USER_OPTION_TYPE, string>(name, value)
{
    public static readonly ENUM_USER_OPTION_TYPE LANGUAGE = new(nameof(LANGUAGE), "LANGUAGE");
    public static readonly ENUM_USER_OPTION_TYPE TIMEZONE = new(nameof(TIMEZONE), "TIMEZONE");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION = new(nameof(NOTIFICATION), "NOTIFICATION");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_EMAIL = new(nameof(NOTIFICATION_EMAIL), "NOTIFICATION_EMAIL");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_SMS = new(nameof(NOTIFICATION_SMS), "NOTIFICATION_SMS");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_PUSH = new(nameof(NOTIFICATION_PUSH), "NOTIFICATION_PUSH");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_WECHAT = new(nameof(NOTIFICATION_WECHAT), "NOTIFICATION_WECHAT");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_LINE = new(nameof(NOTIFICATION_LINE), "NOTIFICATION_LINE");
    public static readonly ENUM_USER_OPTION_TYPE NOTIFICATION_KAKAO = new(nameof(NOTIFICATION_KAKAO), "NOTIFICATION_KAKAO");
}