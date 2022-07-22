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

}
