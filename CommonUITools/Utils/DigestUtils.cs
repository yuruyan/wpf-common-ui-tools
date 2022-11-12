using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System.Collections.Concurrent;
using System.IO;
using System.Text;

namespace CommonUITools.Core;

public static class DigestUtils {
    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="s"></param>
    /// <param name="digest"></param>
    /// <returns></returns>
    private static string GeneralDigest(string s, IDigest digest) {
        byte[] sourceBuffer = Encoding.UTF8.GetBytes(s);
        byte[] resultBuffer = new byte[digest.GetDigestSize()];
        digest.BlockUpdate(sourceBuffer, 0, sourceBuffer.Length);
        digest.DoFinal(resultBuffer, 0);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// 读取缓冲区队列
    /// </summary>
    private static readonly ConcurrentQueue<byte[]> ReadBufferQueue = new();
    /// <summary>
    /// 默认读取缓冲区个数
    /// </summary>
    private static readonly int DefaultReadBufferSize = 2;
    /// <summary>
    /// 默认读取缓冲区大小
    /// </summary>
    private static readonly int FileReadBuffer = 16 * 1024 * 1024;

    static DigestUtils() {
        // 初始化队列
        for (int i = 0; i < DefaultReadBufferSize; i++) {
            ReadBufferQueue.Enqueue(new byte[FileReadBuffer]);
        }
    }

    /// <summary>
    /// 摘要算法
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="digest"></param>
    /// <param name="callback">回调，参数为总读取的大小</param>
    /// <returns></returns>
    private static string GeneralDigest(FileStream stream, IDigest digest, Action<long>? callback = null) {
        // 从队列中获取缓存;
        ReadBufferQueue.TryDequeue(out var buffer);
        if (buffer is null) {
            ReadBufferQueue.Enqueue(buffer = new byte[FileReadBuffer]);
        }
        byte[] resultBuffer = new byte[digest.GetDigestSize()];
        int read;
        long totalRead = 0;
        while ((read = stream.Read(buffer, 0, FileReadBuffer)) > 0) {
            digest.BlockUpdate(buffer, 0, read);
            totalRead += read;
            callback?.Invoke(totalRead);
        }
        digest.DoFinal(resultBuffer, 0);
        // 添加到队列中
        ReadBufferQueue.Enqueue(buffer);
        return Hex.ToHexString(resultBuffer);
    }

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string MD5Digest(string s) {
        return GeneralDigest(s, new MD5Digest());
    }

    /// <summary>
    /// md5 摘要
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string MD5Digest(FileStream stream, Action<long>? callback = null) {
        return GeneralDigest(stream, new MD5Digest(), callback);
    }
}
