﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CommonTools.Utils;

/// <summary>
/// 扩展方法
/// </summary>
public static partial class ExtensionUtils {
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
    public static bool Remove<T>(this ICollection<T> collection, Predicate<T> predicate) {
        if (collection.FirstOrDefault(item => predicate(item)) is T obj) {
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
    public static List<T> RemoveAll<T>(this ICollection<T> enumerable, Predicate<T> predicate) {
        var tobeRemovedItems = enumerable.Where(item => predicate(item)).ToList();
        foreach (var item in tobeRemovedItems) {
            enumerable.Remove(item);
        }
        return tobeRemovedItems;
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
    /// 移除 Dictionary 多个 Key
    /// </summary>
    /// <typeparam name="Key"></typeparam>
    /// <typeparam name="Value"></typeparam>
    /// <param name="source"></param>
    /// <param name="keys"></param>
    public static void RemoveKeyList<Key, Value>(this IDictionary<Key, Value> source, IEnumerable<Key> keys) {
        foreach (var key in keys) {
            source.Remove(key);
        }
    }

    /// <summary>
    /// 打印集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="values"></param>
    /// <param name="count">打印输出个数，-1 代表打印所有</param>
    /// <param name="separator">分隔符</param>
    public static void Print<T>(this IEnumerable<T> values, int count = -1, string separator = "\n") {
        if (count == -1) {
            Console.WriteLine(string.Join(separator, values));
            return;
        }
        Console.WriteLine(string.Join(separator, values.Take(count)));
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
    /// <returns>失败返回 -1</returns>
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
    /// <returns>失败返回 -1</returns>
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
    /// <returns>A new list which contains elements that returned from <paramref name="func"/></returns>
    public static IList<Target> Cast<Target>(this ICollection source, Func<object, Target> func) {
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
    /// <returns><paramref name="source"/></returns>
    public static ICollection<T> ResizeToLength<T>(this ICollection<T> source, int length, T defaultValue = default!) {
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
    /// <returns><paramref name="source"/></returns>
    public static ICollection<T> ResizeByCount<T>(this ICollection<T> source, int count, T defaultValue = default!) {
        if (count < 0) {
            throw new ArgumentOutOfRangeException(nameof(count), "Cannot be less than 0");
        }
        for (int i = 0; i < count; i++) {
            source.Add(defaultValue);
        }
        return source;
    }

    /// <summary>
    /// 数组转置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns>A new array</returns>
    public static T[,] Transpose<T>(this T[,] source) {
        int rowLength = source.GetLength(0);
        int columnLength = source.GetLength(1);
        var data = new T[columnLength, rowLength];
        for (int i = 0; i < rowLength; i++) {
            for (int j = 0; j < columnLength; j++) {
                data[j, i] = source[i, j];
            }
        }
        return data;
    }

    /// <summary>
    /// 集合转置，第二维长度必须相同
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns>A new list</returns>
    public static List<IList<T>> Transpose<T>(this ICollection<IList<T>> source) {
        int rowLength = source.Count;
        int columnLength = source.First().Count;
        var data = new List<IList<T>>(columnLength);
        // Initialize
        for (int i = 0; i < columnLength; i++) {
            data.Add(new List<T>(rowLength));
        }
        foreach (var row in source) {
            int currentRow = 0;
            foreach (var columnElem in row) {
                data[currentRow++].Add(columnElem);
            }
        }
        return data;
    }

    /// <summary>
    /// 批量添加数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="data"></param>
    /// <returns><paramref name="source"/></returns>
    public static ICollection<T> AddRange<T>(this ICollection<T> source, params IEnumerable<T>[] data) {
        foreach (var list in data) {
            foreach (var item in list) {
                source.Add(item);
            }
        }
        return source;
    }

    /// <inheritdoc cref="AddRange{T}(ICollection{T}, IEnumerable{T}[])"/>
    public static ICollection<T> AddRange<T>(this ICollection<T> source, params T[] data) {
        foreach (var item in data) {
            source.Add(item);
        }
        return source;
    }

    /// <summary>
    /// 按顺序集合多个数据集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="data"></param>
    /// <returns>A new list which contains all data from <paramref name="source"/> and <paramref name="data"/></returns>
    public static List<T> Join<T>(this IEnumerable<T> source, params IEnumerable<T>[] data) {
        var results = new List<T>(source);
        foreach (var item in data) {
            results.AddRange(item);
        }
        return results;
    }

    /// <summary>
    /// 查看是否包含指定 Predicate 的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static bool Contains<T>(this IEnumerable<T> source, Predicate<T> predicate) {
        foreach (var item in source) {
            if (predicate(item)) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取一个符合 <paramref name="predicate"/> 的元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="predicate"></param>
    /// <param name="result"></param>
    /// <returns>成功返回 true，失败返回 false</returns>
    public static bool TryGet<T>(this IEnumerable<T> source, Predicate<T> predicate, out T result) {
        result = default!;
        foreach (var item in source) {
            if (predicate(item)) {
                result = item;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 获取集合相同前缀
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    public static string GetSamePrefix(this IEnumerable<string> list) {
        var shortest = list.Min()!;
        var sb = new StringBuilder();
        for (int i = 0; i < shortest.Length; i++) {
            var same = list.All(item => item[i] == shortest[i]);
            if (!same) {
                return sb.ToString();
            }
            sb.Append(shortest[i]);
        }
        return sb.ToString();
    }

    /// <summary>
    /// 随机排序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(this IEnumerable<T> list) {
        var data = list.ToList();
        for (int i = 0, total = data.Count; i < total; i++) {
            var index = Random.Shared.Next(total - i);
            var replaceIndex = total - i - 1;
            (data[index], data[replaceIndex]) = (data[replaceIndex], data[index]);
        }
        return data;
    }

    /// <summary>
    /// 移除重复项，保证元素顺序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static List<T> RemoveDuplicates<T>(this IEnumerable<T> source) {
        var list = new List<T>();
        var set = new HashSet<T>();
        foreach (var item in source) {
            if (set.Add(item)) {
                list.Add(item);
            }
        }
        set.Clear();
        return list;
    }

    /// <summary>
    /// Int32 序列
    /// </summary>
    /// <param name="start">起始值</param>
    /// <param name="count">个数</param>
    /// <param name="step">间隔</param>
    /// <returns></returns>
    public static IEnumerable<int> Range(this int start, int count, int step = 1) {
        for (int i = 0; i < count; i++) {
            yield return start += step;
        }
    }

    /// <summary>
    /// Int64 序列
    /// </summary>
    /// <param name="start">起始值</param>
    /// <param name="count">个数</param>
    /// <param name="step">间隔</param>
    /// <returns></returns>
    public static IEnumerable<long> Range(this long start, int count, long step = 1) {
        for (int i = 0; i < count; i++) {
            yield return start += step;
        }
    }

    /// <summary>
    /// Double 序列
    /// </summary>
    /// <param name="start">起始值</param>
    /// <param name="count">个数</param>
    /// <param name="step">间隔</param>
    /// <returns></returns>
    public static IEnumerable<double> Range(this double start, int count, double step = 1) {
        for (int i = 0; i < count; i++) {
            yield return start += step;
        }
    }

    /// <summary>
    /// Double 序列
    /// </summary>
    /// <param name="start">起始值</param>
    /// <param name="count">个数</param>
    /// <param name="step">间隔</param>
    /// <returns></returns>
    public static IEnumerable<float> Range(this float start, int count, float step = 1) {
        for (int i = 0; i < count; i++) {
            yield return start += step;
        }
    }
}

/// <summary>
/// for string
/// </summary>
public static partial class ExtensionUtils {
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
    /// 将换行符替换为 Linux 样式 (\n)
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string ReplaceLineFeedWithLinuxStyle(this string source) => source.Replace("\r\n", "\n");
}