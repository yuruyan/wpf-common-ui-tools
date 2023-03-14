using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System.Collections.Concurrent;

namespace CommonTools.Core;

public static class DigestUtils {
    /// <summary>
    /// 默认读取缓冲区大小
    /// </summary>
    private const int ReadBufferSize = 1024 * 8;

    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="text"></param>
    /// <param name="digest"></param>
    /// <returns></returns>
    private static string GeneralDigest(string text, IDigest digest) {
        byte[] sourceBuffer = Encoding.UTF8.GetBytes(text);
        byte[] resultBuffer = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(sourceBuffer, 0, sourceBuffer.Length);
        digest.DoFinal(resultBuffer, 0);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <returns></returns>
    private static string GeneralDigest(FileStream stream, IDigest digest) {
        byte[] resultBuffer = new byte[digest.GetDigestSize()];
        var readBuffer = new byte[ReadBufferSize];
        int readCount;
        while ((readCount = stream.Read(readBuffer, 0, readBuffer.Length)) > 0) {
            digest.BlockUpdate(readBuffer, 0, readCount);
        }
        digest.DoFinal(resultBuffer, 0);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string MD5Digest(string text) => GeneralDigest(text, new MD5Digest());

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string MD5Digest(FileStream stream) => GeneralDigest(stream, new MD5Digest());

    /// <summary>
    /// Sha256Digest 摘要
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Sha256Digest(string text) => GeneralDigest(text, new Sha256Digest());

    /// <summary>
    /// Sha256Digest 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string Sha256Digest(FileStream stream) => GeneralDigest(stream, new Sha256Digest());

    /// <summary>
    /// Sha512Digest 摘要
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string Sha512Digest(string text) => GeneralDigest(text, new Sha512Digest());

    /// <summary>
    /// Sha512Digest 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string Sha512Digest(FileStream stream) => GeneralDigest(stream, new Sha512Digest());
}
