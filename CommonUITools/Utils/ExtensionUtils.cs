using AutoMapper;
using CommonUITools.Model;
using System.Collections.ObjectModel;

namespace CommonUITools.Utils;

public static class ExtensionUtils {
    /// <summary>
    /// 平均拆分集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="splitListCount">拆分为多少个集合</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int splitListCount) {
        if (splitListCount < 1) {
            throw new OverflowException($"{nameof(splitListCount)} cannot be less than 0");
        }
        var sourceList = source.ToList();
        // 每个列表最大元素个数
        int perListCount = (int)Math.Ceiling(sourceList.Count / (double)splitListCount);
        var destList = new List<T>[splitListCount];
        // 初始化数组
        for (int i = 0; i < splitListCount; i++) {
            destList[i] = new(perListCount);
        }
        int currentListIndex = -1;
        // 分配
        foreach (var item in sourceList) {
            int tempIndex = currentListIndex + 1;
            currentListIndex = tempIndex >= splitListCount ? 0 : tempIndex;
            destList[currentListIndex].Add(item);
        }
        return destList;
    }

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
    public static IEnumerable<T> RemoveAll<T>(this ICollection<T> enumerable, Func<T, bool> predicate) {
        List<T> removedList = new();
        foreach (var item in enumerable.Where(predicate).ToArray()) {
            if (enumerable.Remove(item)) {
                removedList.Add(item);
            }
        }
        return removedList;
    }

    /// <summary>
    /// 打印集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="count">打印输出个数，-1 代表打印所有</param>
    public static void PrintArray<T>(this IEnumerable<T> values, int count = -1) {
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
}
