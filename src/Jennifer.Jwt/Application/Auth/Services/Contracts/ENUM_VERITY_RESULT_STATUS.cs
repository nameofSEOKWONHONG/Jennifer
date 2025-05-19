using System.Text.Json.Serialization;
using Ardalis.SmartEnum;
using Ardalis.SmartEnum.EFCore;

namespace Jennifer.Jwt.Application.Auth.Services.Contracts;

[JsonConverter(typeof(SmartEnumConverter<ENUM_VERITY_RESULT_STATUS, int>))]
public class ENUM_VERITY_RESULT_STATUS: SmartEnum<ENUM_VERITY_RESULT_STATUS, int>
{
    public static readonly ENUM_VERITY_RESULT_STATUS NOT_FOUND = new(nameof(NOT_FOUND), 0);
    public static readonly ENUM_VERITY_RESULT_STATUS FAILED_COUNT_LIMIT = new(nameof(FAILED_COUNT_LIMIT), -1);
    public static readonly ENUM_VERITY_RESULT_STATUS WRONG_CODE = new(nameof(WRONG_CODE), -2);

    public static readonly ENUM_VERITY_RESULT_STATUS EMAIL_CONFIRM = new(nameof(EMAIL_CONFIRM), 1);

    public ENUM_VERITY_RESULT_STATUS(string name, int value) : base(name, value)
    {
    }
}