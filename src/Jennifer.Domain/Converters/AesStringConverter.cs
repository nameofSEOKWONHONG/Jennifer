using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Jennifer.Domain.Converters;

/// <summary>
/// Provides a conversion mechanism for string properties using AES encryption and decryption.
/// </summary>
/// <remarks>
/// The <see cref="AesStringConverter"/> class is used primarily to secure sensitive string values
/// by transforming them to an encrypted format when saving to the database and decrypting them when
/// retrieving from the database.
/// </remarks>
public class AesStringConverter: ValueConverter<string, string>
{
    public AesStringConverter() : base(
        v => string.IsNullOrWhiteSpace(v) ? null : v.ToAesEncrypt(),
        v => string.IsNullOrWhiteSpace(v) ? null : v.ToAesDecrypt())
    { }
}