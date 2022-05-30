using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System.Text;

namespace CommonTools.Utils;

public class CryptoUtils {
    /// <summary>
    /// 加密
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
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
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] AESEncode(string message, string key, string iv) {
        KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode(key));
        IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        inCipher.Init(true, new ParametersWithIV(keyParam, Hex.Decode(iv)));
        return inCipher.DoFinal(Encoding.UTF8.GetBytes(message));
    }

    /// <summary>
    /// 解密
    /// </summary>
    /// <param name="message"></param>
    /// <param name="key"></param>
    /// <param name="iv"></param>
    /// <returns></returns>
    public static byte[] AESDecode(byte[] message, string key, string iv) {
        KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", Hex.Decode(key));
        IBufferedCipher inCipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
        inCipher.Init(false, new ParametersWithIV(keyParam, Hex.Decode(iv)));
        return inCipher.DoFinal(message);
    }
}

