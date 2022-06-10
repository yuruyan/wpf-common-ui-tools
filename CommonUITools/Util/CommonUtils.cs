using Newtonsoft.Json;
using NLog;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonUITools.Utils;

public class CommonUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly Random Random = new();

    /// <summary>
    /// 当前时间戳(ms)
    /// </summary>
    public static long CuruentMilliseconds => DateTimeOffset.Now.ToUnixTimeMilliseconds();

    /// <summary>
    /// 当前时间戳(s)
    /// </summary>
    public static long CuruentSeconds => DateTimeOffset.Now.ToUnixTimeSeconds();

    /// <summary>
    /// 拷贝对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T Copy<T>(T obj) {
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
    }

    /// <summary>
    /// 比较两个对象是否相同
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static bool Equals<T>(T? x, T? y) {
        if (x != null) {
            return x.Equals(y);
        } else if (y != null) {
            return y.Equals(x);
        }
        return true;
    }

    /// <summary>
    /// DateTime 转 Timestamp
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ConvertToTimestamp(DateTime value) {
        TimeSpan elapsedTime = value - Epoch;
        return (long)elapsedTime.TotalMilliseconds;
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
            timestamp /= 1000;
        }
        long lTime = long.Parse(timestamp + "0000000");
        var toNow = new TimeSpan(lTime);
        return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local).Add(toNow);
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
    /// 简化 try 代码块
    /// </summary>
    /// <param name="action"></param>
    public static void Try(Action action) {
        try {
            action();
        } catch { }
    }

    /// <summary>
    /// 获取本机 IP 地址
    /// </summary>
    /// <returns></returns>
    public static string? GetLocalIpAddress() {
        try {
            using Socket socket = new(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            socket.Connect("8.8.8.8", 65530);
            if (socket.LocalEndPoint is IPEndPoint endPoint) {
                return endPoint.Address.ToString();
            }
        } catch (Exception e) {
            Logger.Info(e);
        }
        return null;
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
    /// 等待 predicate 返回 true，然后执行 action
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="action"></param>
    /// <param name="interval">检查间隔时间(ms)</param>
    public static void WaitFor(Func<bool> predicate, Action action, int interval = 50) {
        Task.Factory.StartNew(() => {
            while (!predicate()) {
                Thread.Sleep(interval);
            }
            action();
        });
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
}

