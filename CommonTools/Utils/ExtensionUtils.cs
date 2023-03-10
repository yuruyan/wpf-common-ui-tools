using System.Collections;
using System.Collections.ObjectModel;

namespace CommonTools.Utils;

/// <summary>
/// 扩展方法
/// </summary>
public static class ExtensionUtils {
    /// <summary>
    /// 从 collection 中移除符合 predicate 条件的第一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="predicate"></param>
    /// <returns>
    /// true if item was successfully removed from the System.Collections.Generic.ICollection;
    /// otherwise, false. This method also returns false if item is not found in the
    /// original System.Collections.Generic.ICollection
    /// </returns>
    public static bool Remove<T>(this ICollection<T> collection, Func<T, bool> predicate) {
        if (collection.FirstOrDefault(predicate) is T obj) {
            return collection.Remove(obj);
        }
        return false;
    }

    /// <summary>
    /// 从 collection 中移除符合 predicate 条件的所有元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="predicate"></param>
    /// <returns>移除的元素</returns>
    public static IList<T> RemoveAll<T>(this ICollection<T> enumerable, Func<T, bool> predicate) {
        List<T> removedList = new();
        foreach (var item in enumerable.Where(predicate).ToArray()) {
            if (enumerable.Remove(item)) {
                removedList.Add(item);
            }
        }
        return removedList;
    }

    /// <summary>
    /// 移除多个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thisCollection"></param>
    /// <param name="toBeRemovedItems"></param>
    public static void RemoveList<T>(this ICollection<T> thisCollection, IEnumerable<T> toBeRemovedItems) {
        foreach (var item in toBeRemovedItems) {
            thisCollection.Remove(item);
        }
    }

    /// <summary>
    /// 打印集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="count">打印输出个数，-1 代表打印所有</param>
    public static void Print<T>(this IEnumerable<T> values, int count = -1) {
        if (count == -1) {
            foreach (var item in values) {
                Console.WriteLine(item);
            }
            return;
        }
        int index = 1;
        foreach (var item in values) {
            Console.WriteLine(item);
            if (index++ >= count) {
                return;
            }
        }
    }

    /// <summary>
    /// 打印数组
    /// </summary>
    /// <param name="array"></param>
    /// <param name="count">打印输出个数，-1 代表打印所有</param>
    public static void PrintArray(this Array array, int count = -1) {
        if (count == -1) {
            foreach (var item in array) {
                Console.WriteLine(item);
            }
            return;
        }
        int index = 1;
        foreach (var item in array) {
            Console.WriteLine(item);
            if (index++ >= count) {
                return;
            }
        }
    }

    /// <summary>
    /// 更新现有集合，不替换元素，更新后长度为 <typeparamref name="Source"/> 的长度
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    /// <typeparam name="Source"></typeparam>
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="updater">更新原数据</param>
    /// <param name="converter">转换器</param>
    public static void UpdateWithCollection<Target, Source>(
        this ObservableCollection<Target> target,
        ICollection<Source> source,
        Action<Source, Target> updater,
        Func<Source, Target> converter
    ) {
        int oldLength = target.Count, newLength = source.Count;
        if (oldLength <= newLength) {
            // 更新
            foreach (var (s, t) in source.Zip(target)) {
                updater(s, t);
            }
            var addData = source
                .Skip(oldLength)
                .Take(newLength - oldLength)
                .Select(converter);
            // 添加剩余元素
            foreach (var item in addData) {
                target.Add(item);
            }
            return;
        }
        // 删除多余元素
        for (int i = oldLength - 1; i >= newLength; i--) {
            target.RemoveAt(i);
        }
        // 更新
        foreach (var (s, t) in source.Zip(target)) {
            updater(s, t);
        }
    }

    /// <summary>
    /// 用新数据替换现有数据，更新后长度为 <typeparamref name="Source"/> 的长度
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    /// <typeparam name="Source"></typeparam>
    /// <param name="target"></param>
    /// <param name="source"></param>
    /// <param name="converter">转换器</param>
    public static void ReplaceWithCollection<Target, Source>(
        this ObservableCollection<Target> target,
        ICollection<Source> source,
        Func<Source, Target> converter
    ) {
        int oldLength = target.Count, newLength = source.Count;
        var enumerator = source.GetEnumerator();
        if (oldLength <= newLength) {
            // 替换
            for (int i = 0; i < oldLength; i++) {
                enumerator.MoveNext();
                target[i] = converter(enumerator.Current);
            }
            // 添加剩余元素
            while (enumerator.MoveNext()) {
                target.Add(converter(enumerator.Current));
            }
            return;
        }
        // 删除多余元素
        for (int i = oldLength - 1; i >= newLength; i--) {
            target.RemoveAt(i);
        }
        // 替换
        for (int i = 0; i < newLength; i++) {
            enumerator.MoveNext();
            target[i] = converter(enumerator.Current);
        }
    }

    /// <summary>
    /// 替换 '/' 为 '\\'
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ReplaceSlashWithBackSlash(this string source) => source.Replace('/', '\\');

    /// <summary>
    /// 替换 '\\' 为 '/'
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ReplaceBackSlashWithSlash(this string source) => source.Replace('\\', '/');

    /// <summary>
    /// ForEach
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <param name="source"></param>
    /// <param name="action"></param>
    public static void ForEach<Source>(this IEnumerable<Source> source, Action<Source> action) {
        foreach (var item in source) {
            action(item);
        }
    }

    /// <summary>
    /// ForEach
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <param name="source"></param>
    /// <param name="action">第一个参数为索引</param>
    public static void ForEach<Source>(this IEnumerable<Source> source, Action<int, Source> action) {
        int index = 0;
        foreach (var item in source) {
            action(index++, item);
        }
    }

    /// <summary>
    /// 获取元素所在位置
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static int IndexOf<Source>(this IEnumerable<Source> source, Source target) {
        int index = 0;
        foreach (var item in source) {
            if (object.Equals(item, target)) {
                return index;
            }
            index++;
        }
        return -1;
    }

    /// <summary>
    /// 获取元素所在位置
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static int IndexOf<Source>(this IEnumerable<Source> source, Predicate<Source> predicate) {
        int index = 0;
        foreach (var item in source) {
            if (predicate(item)) {
                return index;
            }
            index++;
        }
        return -1;
    }

    /// <summary>
    /// 转换
    /// </summary>
    /// <typeparam name="Target"></typeparam>
    /// <param name="source"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static IList<Target> Cast<Target>(this IList source, Func<object, Target> func) {
        var results = new List<Target>(source.Count);
        foreach (var item in source) {
            results.Add(func(item));
        }
        return results;
    }

    /// <summary>
    /// 扩充集合到指定长度
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="length"></param>
    /// <param name="defaultValue">填充默认值</param>
    /// <returns></returns>
#pragma warning disable CS8601 // Possible null reference assignment.
    public static ICollection<T> ResizeToLength<T>(this ICollection<T> source, int length, T defaultValue = default) {
#pragma warning restore CS8601 // Possible null reference assignment.
        // 不需要扩充
        if (length <= source.Count) {
            return source;
        }
        int resizeCount = length - source.Count;
        for (int i = 0; i < resizeCount; i++) {
            source.Add(defaultValue);
        }
        return source;
    }

    /// <summary>
    /// 扩充集合 <paramref name="count"/> 个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <param name="defaultValue">填充默认值</param>
    /// <returns></returns>
#pragma warning disable CS8601 // Possible null reference assignment.
    public static ICollection<T> ResizeByCount<T>(this ICollection<T> source, int count, T defaultValue = default) {
#pragma warning restore CS8601 // Possible null reference assignment.
        if (count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count), "Cannot be less than 0");
        }
        for (int i = 0; i < count; i++) {
            source.Add(defaultValue);
        }
        return source;
    }
}
