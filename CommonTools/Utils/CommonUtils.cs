using NLog;
using System.Text.RegularExpressions;

namespace CommonTools.Utils;

public static partial class CommonUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private const string PathSpliterPattern = @"[/\\]";
    private static readonly string _ProcessPath = Environment.ProcessPath!;
    private static readonly string _ProcessDirectory = Path.GetDirectoryName(Environment.ProcessPath!)!;
    /// <summary>
    /// 路径分隔符正则
    /// </summary>
    private static readonly Regex PathSpliterRegex = GetPathSpliterRegex();

    /// <summary>
    /// Current process path
    /// </summary>
    public static string ProcessPath => _ProcessPath;
    /// <summary>
    /// Current process directory
    /// </summary>
    public static string ProcessDirectory => _ProcessDirectory;

#if NET7_0_OR_GREATER
    [GeneratedRegex(PathSpliterPattern)]
    private static partial Regex GetPathSpliterRegex();
#else
    private static Regex GetPathSpliterRegex() => new(PathSpliterPattern);
#endif

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
    /// <returns></returns>
    /// <exception cref="Exception">对象创建失败</exception>
    public static T GetSingletonInstance<T>() => (T)GetSingletonInstance(typeof(T));

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
    /// <param name="init"></param>
    /// <returns></returns>
    public static T GetSingletonInstance<T>(Func<object> init) => (T)GetSingletonInstance(typeof(T), init);

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
    /// <returns>如果 <paramref name="value"/> 为 null，则抛出异常，否则返回</returns>
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

    /// <summary>
    /// 获取随机唯一文件名
    /// </summary>
    /// <param name="dirPath">随机文件所在路径，默认为 <see cref="Environment.CurrentDirectory"/></param>
    /// <returns></returns>
    public static string? GetUniqueRandomFileName(string? dirPath = null) {
        dirPath = dirPath ?? Environment.CurrentDirectory;
        int count = 0;
        while (count++ < 16) {
            var name = Path.GetRandomFileName();
            if (!File.Exists(Path.Combine(dirPath, name))) {
                return name;
            }
        }
        return null;
    }

    /// <summary>
    /// 获取唯一文件名称，如果已存在文件，则自动重命名
    /// </summary>
    /// <param name="filepath"></param>
    /// <param name="autoRenameCount">重命名最大次数</param>
    /// <returns></returns>
    /// <exception cref="Exception">自动重命名超过最大次数</exception>
    public static string GetUniqueFileNameFor(string filepath, uint autoRenameCount = uint.MaxValue) {
        if (!File.Exists(filepath)) {
            return filepath;
        }
        var dir = Path.GetDirectoryName(filepath) ?? throw new ArgumentException($"Path '{filepath}' is not a file path");
        var name = Path.GetFileNameWithoutExtension(filepath);
        var ext = Path.GetExtension(filepath);
        uint i = 1;
        while (i <= autoRenameCount) {
            // Rename
            filepath = Path.Combine(dir, $"{name}({i++}){ext}");
            if (!File.Exists(filepath)) {
                return filepath;
            }
        }
        throw new Exception($"File name conflicts for {autoRenameCount} times");
    }
}
