using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.SystemTextJson;

namespace Jennifer.Tenant.Models.Contracts;

[JsonConverter(typeof(SmartEnumNameConverter<ENUM_EMAIL_VERIFICATION_TYPE, int>))]
public class ENUM_EMAIL_VERIFICATION_TYPE: SmartEnum<ENUM_EMAIL_VERIFICATION_TYPE, int> 
{
    public static readonly ENUM_EMAIL_VERIFICATION_TYPE SIGN_UP_BEFORE = new(nameof(SIGN_UP_BEFORE), 1);
    public static readonly ENUM_EMAIL_VERIFICATION_TYPE PASSWORD_FORGOT = new(nameof(PASSWORD_FORGOT), 2);
    public ENUM_EMAIL_VERIFICATION_TYPE(string name, int value) : base(name, value)
    {
    }
}
