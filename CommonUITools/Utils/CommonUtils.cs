using Newtonsoft.Json;
using NLog;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonUITools.Utils;

public static class CommonUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static DateTime Epoch => new(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 当前时间戳(ms)
    /// </summary>
    public static long CuruentMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// 当前时间戳(s)
    /// </summary>
    public static long CuruentSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// 拷贝对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T? Copy<T>(T obj) {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }

    /// <summary>
    /// DateTime 转 Timestamp
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ConvertToTimestamp(DateTime value) {
        return (long)(value - Epoch).TotalMilliseconds;
    }

    /// <summary>
    /// string 日期转 DateTime
    /// </summary>
    /// <param name="value"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(string value, string pattern = "yyyy-MM-dd HH:mm:ss") {
        return Convert.ToDateTime(
            value,
            new DateTimeFormatInfo() { ShortDatePattern = pattern }
        );
    }

    /// <summary>
    /// 时间戳转 DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(long timestamp, bool milliseconds = true) {
        if (milliseconds) {
            return Epoch.AddMilliseconds(timestamp);
        }
        return Epoch.AddSeconds(timestamp);
    }

    /// <summary>
    /// 获取集合相同前缀
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetSamePrefix(IEnumerable<string> list) {
        if (!list.Any()) {
            return "";
        }
        var sb = new StringBuilder();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        foreach (var s in list.FirstOrDefault()) {
            foreach (var item in list) {
                if (item[sb.Length] != s) {
                    return sb.ToString();
                }
            }
            sb.Append(s);
        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        return sb.ToString();
    }

    /// <summary>
    /// 标准化多行输入文本
    /// 将 "\r\n" 替换为 '\n'
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string NormalizeMultipleLineText(string text) {
        return text.Replace("\r\n", "\n");
    }

    /// <summary>
    /// 单例模式对象 Dict
    /// </summary>
    private static readonly IDictionary<Type, object> SingletonDict = new Dictionary<Type, object>();

    /// <summary>
    /// 获取单例对象
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Exception">对象创建失败</exception>
    public static object GetSingletonInstance(Type type) {
        if (SingletonDict.ContainsKey(type)) {
            return SingletonDict[type];
        }
        lock (type) {
            if (SingletonDict.ContainsKey(type)) {
                return SingletonDict[type];
            }
            object? obj = Activator.CreateInstance(type, true);
            if (obj is null) {
                throw new Exception($"Create object {type} failed");
            }
            SingletonDict[type] = obj;
            return obj;
        }
    }

    /// <summary>
    /// 获取单例对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="Exception">对象创建失败</exception>
    public static T GetSingletonInstance<T>(Type type) {
        return (T)GetSingletonInstance(type);
    }

    /// <summary>
    /// 根据 init 函数创建单例对象
    /// </summary>
    /// <param name="type"></param>
    /// <param name="init"></param>
    /// <returns></returns>
    public static object GetSingletonInstance(Type type, Func<object> init) {
        if (SingletonDict.ContainsKey(type)) {
            return SingletonDict[type];
        }
        lock (type) {
            if (SingletonDict.ContainsKey(type)) {
                return SingletonDict[type];
            }
            object obj = init();
            SingletonDict[type] = obj;
            return obj;
        }
    }

    /// <summary>
    /// 根据 init 函数创建单例对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="init"></param>
    /// <returns></returns>
    public static T GetSingletonInstance<T>(Type type, Func<object> init) {
        return (T)GetSingletonInstance(type, init);
    }

    /// <summary>
    /// 路径分隔符正则
    /// </summary>
    private static readonly Regex PathSpliterRegex = new(@"[/\\]");

    /// <summary>
    /// 移除路径分隔符(\, /)
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RemovePathSpliter(string path) {
        return PathSpliterRegex.Replace(path, "");
    }

    /// <summary>
    /// 检查 null，用以消除 warning
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T NullCheck<T>(T? value) {
        ArgumentNullException.ThrowIfNull(value);
        return value;
    }

    /// <summary>
    /// 检查 Range 是否有效
    /// </summary>
    /// <returns>无效返回 null</returns>
    public static Range? CheckRange(double minInclusive, double maxExclusive) {
        int min = (int)minInclusive;
        int max = (int)maxExclusive;
        if (min > max) {
            return null;
        }
        return new Range(new(min), new(max));
    }

    /// <summary>
    /// 判断文件是否可能是二进制文件
    /// 
    /// 最多读取前 64kb 字节，判断是否包含 '\0'
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsLikelyBinaryFile(string path) {
        using var readStream = File.OpenRead(path);
        using var reader = new BinaryReader(readStream);
        byte nullByte = (byte)'\0';
        // 先读取前 1kb 字节
        if (Array.IndexOf(reader.ReadBytes(1024), nullByte) >= 0) {
            return true;
        }
        // 读取前 64kb 字节
        return Array.IndexOf(reader.ReadBytes(63 * 1024), nullByte) >= 0;
    }
}
