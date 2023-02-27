using CommonTools.Model;

namespace CommonTools.Utils;

public static class RandomUtils {
    /// <summary>
    /// 数字
    /// </summary>
    private const string Numbers = "0123456789";
    /// <summary>
    /// 字母
    /// </summary>
    private const string LowerCaseAlphabet = "abcdefghijklmnopqrstuvwxyz";
    /// <summary>
    /// 数字和字母
    /// </summary>
    private const string Letters = Numbers + LowerCaseAlphabet;

    /// <summary>
    /// 随机双精度浮点数
    /// </summary>
    /// <param name="min">最小值，包括</param>
    /// <param name="max">最大值，不包括</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static double RandomDouble(double min, double max) {
        #region Handle exceptions
        if (min < 0 || max < 0) {
            throw new ArgumentException($"{nameof(min)} and {(nameof(max))} must not be negative");
        }
        if (min > max) {
            throw new ArgumentException($"{nameof(min)} must less than or equals {nameof(max)}");
        }
        #endregion

        return (max - min) * Random.Shared.NextDouble() + min;
    }

    /// <summary>
    /// 随机单精度浮点数
    /// </summary>
    /// <param name="min">最小值，包括</param>
    /// <param name="max">最大值，不包括</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static float RandomSingle(float min, float max) {
        #region Handle exceptions
        if (min < 0 || max < 0) {
            throw new ArgumentException($"{nameof(min)} and {(nameof(max))} must not be negative");
        }
        if (min > max) {
            throw new ArgumentException($"{nameof(min)} must less than or equals {nameof(max)}");
        }
        #endregion

        return (max - min) * Random.Shared.NextSingle() + min;
    }

    /// <summary>
    /// 处理字符大小写
    /// </summary>
    /// <param name="sb"></param>
    /// <param name="characterCase"></param>
    /// <returns></returns>
    private static string HandleCharacterCase(StringBuilder sb, CharacterCase characterCase) {
        if (characterCase == CharacterCase.UpperCase) {
            return sb.ToString().ToUpperInvariant();
        }
        if (characterCase == CharacterCase.LowerCase) {
            return sb.ToString();
        }
        for (int i = 0, length = sb.Length; i < length; i++) {
            var ch = sb[i];
            sb[i] = (Random.Shared.Next() & 1) == 0 ? char.ToUpperInvariant(ch) : ch;
        }
        return sb.ToString();
    }

    /// <summary>
    /// 随机数字字母
    /// </summary>
    /// <param name="length">长度</param>
    /// <param name="characterCase">大小写</param>
    /// <returns></returns>
    public static string RandomLetter(int length, CharacterCase characterCase = CharacterCase.LowerCase) {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            sb.Append(Letters[Random.Shared.Next(Letters.Length)]);
        }
        return HandleCharacterCase(sb, characterCase);
    }

    /// <summary>
    /// 随机字母
    /// </summary>
    /// <param name="length">长度</param>
    /// <param name="characterCase">大小写</param>
    /// <returns></returns>
    public static string RandomAlphabet(int length, CharacterCase characterCase = CharacterCase.LowerCase) {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            sb.Append(LowerCaseAlphabet[Random.Shared.Next(LowerCaseAlphabet.Length)]);
        }
        return HandleCharacterCase(sb, characterCase);
    }

    /// <summary>
    /// 随机数字字符串
    /// </summary>
    /// <param name="length">长度</param>
    /// <returns></returns>
    public static string RandomStringNumber(int length) {
        var sb = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            sb.Append(Numbers[Random.Shared.Next(Numbers.Length)]);
        }
        return sb.ToString();
    }
}
