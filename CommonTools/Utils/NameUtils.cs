using System.Runtime.CompilerServices;

namespace CommonTools.Utils;

public static class NameUtils {
    /// <summary>
    /// 获取全名称表达式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_"></param>
    /// <param name="fullName"></param>
    /// <returns></returns>
    public static string FullNameOf<T>(T _, [CallerArgumentExpression(nameof(_))] string fullName = "") {
        return fullName;
    }
}

public static class NameUtilsExtension {
    /// <inheritdoc cref="NameUtils.FullNameOf{T}(T, string)"/>
    public static string FullNameOf<T>(this T _, [CallerArgumentExpression(nameof(_))] string fullName = "") {
        return fullName;
    }
}