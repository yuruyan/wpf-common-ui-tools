using System.Security.Cryptography;

namespace CommonTools.Utils;

public static class CryptoUtils {
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key">32位16进制字符串</param>
    /// <param name="iv">32位16进制字符串</param>
    /// <returns></returns>
    public static byte[] AESEncode(byte[] message, string key, string iv) {
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Convert.FromHexString(key);
        aes.IV = Convert.FromHexString(iv);
        using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return encryptor.TransformFinalBlock(message, 0, message.Length);
    }

    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="message">使用 UTF8 编码</param>
    /// <param name="key">32位16进制字符串</param>
    /// <param name="iv">32位16进制字符串</param>
    /// <returns></returns>
    public static byte[] AESEncode(string message, string key, string iv) {
        return AESEncode(Encoding.UTF8.GetBytes(message), key, iv);
    }

    /// <summary>
    /// 加密后编码为 base64 字符串
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static string AESEncodeToBase64(string message, string key, string iv) {
        return Convert.ToBase64String(AESEncode(Encoding.UTF8.GetBytes(message), key, iv));
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key">32位16进制字符串</param>
    /// <param name="iv">32位16进制字符串</param>
    /// <returns></returns>
    public static byte[] AESDecode(byte[] message, string key, string iv) {
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = Convert.FromHexString(key);
        aes.IV = Convert.FromHexString(iv);
        using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        return decryptor.TransformFinalBlock(message, 0, message.Length);
    }

    /// <summary>
    /// 解密 base64 编码的数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key">32位16进制字符串</param>
    /// <param name="iv">32位16进制字符串</param>
    /// <returns></returns>
    public static byte[] AESDecodeFromBase64(string data, string key, string iv) {
        return AESDecode(Convert.FromBase64String(data), key, iv);
    }
}
