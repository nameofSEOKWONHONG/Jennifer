using System.Security.Cryptography;
using System.Text;
using Jennifer.SharedKernel.Consts;

namespace Jennifer.SharedKernel.Extenstions;

public static class EncryptionExtensions
{
    public static string ToAesEncrypt(this string value)
    {
        var key = Convert.FromBase64String(JenniferSetting.AesKey);
        var iv  = Convert.FromBase64String(JenniferSetting.AesIV);
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
        var key = Convert.FromBase64String(JenniferSetting.AesKey);
        var iv  = Convert.FromBase64String(JenniferSetting.AesIV);
        byte[] data = Convert.FromBase64String(value);

        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(key, iv);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}