namespace CommonTools.Utils;

/// <summary>
/// 资源工具
/// </summary>
public static class ResourceUtils {
    /// <summary>
    /// 中文汉字拼音
    /// </summary>
    public static readonly IReadOnlyDictionary<char, string> ChineseCharacterPinYinDict = JsonConvert.DeserializeObject<Dictionary<char, string>>(
        Encoding.UTF8.GetString(Resource.Resource.ChineseCharacterPinYin)
    )!;

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
