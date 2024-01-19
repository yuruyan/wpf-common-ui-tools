using System.Security.Cryptography;

namespace CommonTools.Utils;

public static class DigestUtils {
    private static string StreamDigest(Func<Stream, byte[]> digest, Stream data) => Convert.ToHexString(digest(data));

    private static string BytesDigest(Func<byte[], byte[]> digest, byte[] data) => Convert.ToHexString(digest(data));

    private static string StringDigest(Func<byte[], string> digest, string data) => digest(Encoding.UTF8.GetBytes(data));

    /// <summary>
    /// Md5
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Md5Digest(Stream data) => StreamDigest(MD5.HashData, data);

    /// <summary>
    /// Sha1
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha1Digest(Stream data) => StreamDigest(SHA1.HashData, data);

    /// <summary>
    /// Sha256
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256Digest(Stream data) => StreamDigest(SHA256.HashData, data);

    /// <summary>
    /// Sha384
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha384Digest(Stream data) => StreamDigest(SHA384.HashData, data);

    /// <summary>
    /// Sha512
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha512Digest(Stream data) => StreamDigest(SHA512.HashData, data);

    /// <summary>
    /// Md5
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Md5Digest(byte[] data) => BytesDigest(MD5.HashData, data);

    /// <summary>
    /// Sha1
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha1Digest(byte[] data) => BytesDigest(SHA1.HashData, data);

    /// <summary>
    /// Sha256
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha256Digest(byte[] data) => BytesDigest(SHA256.HashData, data);

    /// <summary>
    /// Sha384
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha384Digest(byte[] data) => BytesDigest(SHA384.HashData, data);

    /// <summary>
    /// Sha512
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string Sha512Digest(byte[] data) => BytesDigest(SHA512.HashData, data);

    /// <summary>
    /// Md5
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Md5Digest(string input) => StringDigest(Md5Digest, input);

    /// <summary>
    /// Sha1
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sha1Digest(string input) => StringDigest(Sha1Digest, input);

    /// <summary>
    /// Sha256
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sha256Digest(string input) => StringDigest(Sha256Digest, input);

    /// <summary>
    /// Sha384
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sha384Digest(string input) => StringDigest(Sha384Digest, input);

    /// <summary>
    /// Sha512
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Sha512Digest(string input) => StringDigest(Sha512Digest, input);
}
