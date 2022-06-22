using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace CommonUITools.Utils;

public class CryptoUtils {
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key">32位16进制字符串</param>
    /// <param name="iv">32位16进制字符串</param>
    /// <returns></returns>
    public static byte[] AESEncode(byte[] message, string key, string iv) {
        KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode(key));
        IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        inCipher.Init(true, new ParametersWithIV(keyParam, Hex.Decode(iv)));
        return inCipher.DoFinal(message);
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
        KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode(key));
        IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        inCipher.Init(false, new ParametersWithIV(keyParam, Hex.Decode(iv)));
        return inCipher.DoFinal(message);
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
