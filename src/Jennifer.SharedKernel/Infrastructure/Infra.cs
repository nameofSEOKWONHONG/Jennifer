using System.Security.Cryptography;
using System.Text;

namespace Jennifer.SharedKernel.Infrastructure;

public static class Infra
{
    public static string ToAesEncrypt(this string value)
    {
        var key = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_KEY")!);
        var iv  = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_IV")!);
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
        var key = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_KEY")!);
        var iv  = Convert.FromBase64String(Environment.GetEnvironmentVariable("AES_IV")!);
        byte[] data = Encoding.UTF8.GetBytes(value);

        using Aes aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor(key, iv);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(data, 0, data.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }
}