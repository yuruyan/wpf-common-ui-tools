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

}
