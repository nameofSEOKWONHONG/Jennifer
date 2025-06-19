using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Domain.Accounts.Contracts;

[JsonConverter(typeof(SmartEnumValueConverter<ENUM_USER_TYPE, int>))]
// ReSharper disable once InconsistentNaming
public class ENUM_USER_TYPE(string name, int value) : SmartEnum<ENUM_USER_TYPE, int>(name, value)
{
    public static readonly ENUM_USER_TYPE ADMIN = new ENUM_USER_TYPE("ADMIN", 1);
    public static readonly ENUM_USER_TYPE CUSTOMER = new ENUM_USER_TYPE("CUSTOMER", 2);
    public static readonly ENUM_USER_TYPE DELEVER = new ENUM_USER_TYPE("DELEVER", 3);
    public static readonly ENUM_USER_TYPE MANAGER = new ENUM_USER_TYPE("MANAGER", 4);
    public static readonly ENUM_USER_TYPE SUPERVISOR = new ENUM_USER_TYPE("SUPERVISOR", 5);
    public static readonly ENUM_USER_TYPE WITHDRAW = new ENUM_USER_TYPE("WITHDRAW", 99);
}