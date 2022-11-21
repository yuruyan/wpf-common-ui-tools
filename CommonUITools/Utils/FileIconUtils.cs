using Newtonsoft.Json;
using NLog;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CommonUITools.Utils;

public static class FileIconUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 文件后缀-图标路径
    /// [extension, filepathWithoutExtension]
    /// </summary>
    private static readonly IDictionary<string, string> IconDict;
    /// <summary>
    /// 根据文件后缀长度降序排列 Icon List
    /// </summary>
    private static readonly IList<KeyValuePair<string, string>> SortedIconList = new List<KeyValuePair<string, string>>();
    /// <summary>
    /// 默认 Icon Key
    /// </summary>
    private const string DefaultIconKey = "*";
    /// <summary>
    /// 图标压缩文件路径
    /// </summary>
    private const string CompressedIconFile = DeCompressedIconFolder + "/icon.zip";
    /// <summary>
    /// 图标解压缩目录
    /// </summary>
    private const string DeCompressedIconFolder = "Resource/image/FileIcon";
    private const string Svg = ".svg";
    private const string Png = ".png";

    static FileIconUtils() {
        var iconDict = JsonConvert.DeserializeObject<IDictionary<string, string>>(
            Encoding.UTF8.GetString(Resource.Resource.Icon).ToLowerInvariant()
        );
        if (iconDict is null) {
            throw new JsonSerializationException($"解析 Icon 配置文件失败");
        }
        ConvertToImagePath(iconDict);
        IconDict = iconDict;
        Logger.Debug("加载 FileIcon 配置文件成功");
        // 对 SortedIconList 进行降序排序
        var list = new List<KeyValuePair<string, string>>(iconDict);
        list.Sort((x, y) => y.Key.Length - x.Key.Length);
        SortedIconList = list;
        CheckAndDecompressFileIcons();
        Logger.Debug("加载 FileIcon 文件成功");
    }

    /// <summary>
    /// 转换为 Image Path
    /// </summary>
    /// <param name="pairs"></param>
    private static void ConvertToImagePath(IDictionary<string, string> pairs) {
        foreach (var pair in pairs) {
            pairs[pair.Key] = $"/{DeCompressedIconFolder}/{Path.GetFileNameWithoutExtension(pair.Value)}";
        }
    }

    /// <summary>
    /// 检查 Icon 文件是否已解压
    /// </summary>
    private static void CheckAndDecompressFileIcons() {
        var existFileIconNames = Directory
            .GetFiles(DeCompressedIconFolder)
            .Select(p => Path.GetFileName(p).ToLowerInvariant())
            .ToHashSet();
        // 实际目录 Icon 数量小于配置文件配置 Icon 数量
        var iconNameSet = IconDict.Values.Select(Path.GetFileNameWithoutExtension).ToHashSet();
        var iconNameList = iconNameSet.ToList();
        // 设置 svg 后缀
        for (int i = 0; i < iconNameList.Count; i++) {
            iconNameList[i] = $"{iconNameList[i]}{Svg}";
        }
        // 添加 png 文件
        iconNameList.AddRange(iconNameSet.Select(f => $"{f}.png"));
        if (iconNameList.Any(f => !existFileIconNames.Contains(f!))) {
            // 解压文件
            ZipFile.ExtractToDirectory(CompressedIconFile, DeCompressedIconFolder, true);
            Logger.Debug("解压文件 icon 完毕");
        }
    }

    /// <summary>
    /// 显式初始化
    /// </summary>
    /// <remarks>不调用该方法，程序会自动初始化</remarks>
    public static void InitializeExplicitly() {
        _ = GetPngIcon("a.txt");
    }

    /// <summary>
    /// 返回匹配 Icon 文件名，以 '/' 开头
    /// </summary>
    /// <param name="fileName">文件名或绝对路径</param>
    /// <returns></returns>
    public static string GetIcon(string fileName) {
        fileName = fileName.ToLowerInvariant();
        string ext = Path.GetExtension(fileName);
        // 完全匹配
        if (IconDict.ContainsKey(ext)) {
            return $"{IconDict[ext]}{Svg}";
        }
        // 根据
        foreach (var dict in IconDict) {
            if (fileName.EndsWith(dict.Key)) {
                return $"{dict.Value}{Svg}";
            }
        }
        // 返回默认 Icon
        return $"{IconDict[DefaultIconKey]}{Svg}";
    }

    /// <summary>
    /// 返回匹配 PngIcon 文件名，以 '/' 开头
    /// </summary>
    /// <param name="fileName">文件名或绝对路径</param>
    /// <returns></returns>
    public static string GetPngIcon(string fileName) {
        return $"{GetIcon(fileName)[0..^4]}{Png}";
    }
}
