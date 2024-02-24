using CommonTools.Model;
using System.Text.Json;

namespace CommonTools.Utils;

/// <summary>
/// 资源工具
/// </summary>
public static class ResourceUtils {
    /// <summary>
    /// 中文汉字拼音
    /// </summary>
    public static readonly IReadOnlyDictionary<char, string> ChineseCharacterPinYinDict = JsonSerializer.Deserialize(
        Encoding.UTF8.GetString(Resources.Resource.ChineseCharacterPinYin), SourceGenerationContext.Default.DictionaryCharString
    )!;
    /// <summary>
    /// 声调对应字母
    /// </summary>
    public static readonly IReadOnlyDictionary<char, char> ToneLetterDict = new Dictionary<char, char>() {
        {'ā', 'a'},
        {'á', 'a'},
        {'ǎ', 'a'},
        {'à', 'a'},
        {'ō', 'o'},
        {'ó', 'o'},
        {'ǒ', 'o'},
        {'ò', 'o'},
        {'ē', 'e'},
        {'é', 'e'},
        {'ě', 'e'},
        {'è', 'e'},
        {'ī', 'i'},
        {'í', 'i'},
        {'ǐ', 'i'},
        {'ì', 'i'},
        {'ū', 'u'},
        {'ú', 'u'},
        {'ǔ', 'u'},
        {'ù', 'u'},
        {'ǖ', 'v'},
        {'ǘ', 'v'},
        {'ǚ', 'v'},
        {'ǜ', 'v'},
        {'ɡ', 'g'},
    };
    /// <summary>
    /// 获取汉字拼音
    /// </summary>
    /// <param name="character">汉字</param>
    /// <returns>找不到返回空</returns>
    public static string GetChineseCharacterPinYin(char character) {
        return ChineseCharacterPinYinDict.TryGetValue(character, out var chineseCharacterPinYin)
            ? chineseCharacterPinYin : string.Empty;
    }
}
