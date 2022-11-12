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
    /// [extension, filepath]
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
            pairs[pair.Key] = $"/{DeCompressedIconFolder}/{pair.Value}";
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
        if (IconDict.Values.Any(f => !existFileIconNames.Contains(Path.GetFileName(f)))) {
            // 解压文件
            ZipFile.ExtractToDirectory(CompressedIconFile, DeCompressedIconFolder, true);
            Logger.Debug("解压文件 icon 完毕");
        }
    }

    /// <summary>
    /// 返回匹配 Icon 文件名，以 '/' 开头
    /// </summary>
    /// <param name="fileName">文件名/绝对路径</param>
    /// <returns></returns>
    public static string GetIcon(string fileName) {
        fileName = fileName.ToLowerInvariant();
        string ext = Path.GetExtension(fileName);
        // 完全匹配
        if (IconDict.ContainsKey(ext)) {
            return IconDict[ext];
        }
        // 根据
        foreach (var dict in IconDict) {
            if (fileName.EndsWith(dict.Key)) {
                return dict.Value;
            }
        }
        // 返回默认 Icon
        return IconDict[DefaultIconKey];
    }
}
