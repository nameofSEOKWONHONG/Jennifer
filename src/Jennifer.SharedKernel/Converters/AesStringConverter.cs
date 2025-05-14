using eXtensionSharp;
using Jennifer.SharedKernel.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Jennifer.SharedKernel.Converters;

public class AesStringConverter: ValueConverter<string, string>
{
    public AesStringConverter() : base(
        v => v.xIsEmpty() ? null : v.ToAesEncrypt(),
        v => v.xIsEmpty() ? null : v.ToAesDecrypt())
    { }
}