using System.Security.Cryptography;
using System.Text;
using Jennifer.Infrastructure.Options;

namespace Jennifer.Infrastructure.Extenstions;

public static class EncryptionExtensions
{
    public static string ToAesEncrypt(this string value)
    {
        var key = Convert.FromBase64String(JenniferOptionSingleton.Instance.Options.Crypto.AesKey);
        var iv  = Convert.FromBase64String(JenniferOptionSingleton.Instance.Options.Crypto.AesIV);
        byte[] data = Encoding.UTF8.GetBytes(value);

        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        using var encryptor = aes.CreateEncryptor(key, iv);
        byte[] encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

        return Convert.ToBase64String(encrypted);
    }

    public static string ToAesDecrypt(this string value)
    {
        var key = Convert.FromBase64String(JenniferOptionSingleton.Instance.Options.Crypto.AesKey);
        var iv  = Convert.FromBase64String(JenniferOptionSingleton.Instance.Options.Crypto.AesIV);
        byte[] data = Convert.FromBase64String(value);

        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(key, iv);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}