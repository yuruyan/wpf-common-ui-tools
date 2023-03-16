using System.Collections;

namespace CommonTools.Model;

public interface ISortable {
    /// <summary>
    /// 升序
    /// </summary>
    public ICollection Ascend();

    /// <summary>
    /// 升序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection AscendBy<T>(T sorter);

    /// <summary>
    /// 降序
    /// </summary>
    public ICollection Descend();

    /// <summary>
    /// 降序
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection DescendBy<T>(T sorter);
}

public interface ISortable<TResult> {
    /// <summary>
    /// 升序
    /// </summary>
    public ICollection<TResult> Ascend();

    /// <summary>
    /// 升序
    /// </summary>
    /// <typeparam name="Sorter"></typeparam>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection<TResult> AscendBy<Sorter>(Sorter sorter);

    /// <summary>
    /// 降序
    /// </summary>
    public ICollection<TResult> Descend();

    /// <summary>
    /// 降序
    /// </summary>
    /// <typeparam name="Sorter"></typeparam>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection<TResult> DescendBy<Sorter>(Sorter sorter);
}

public interface ISortable<TSorter, TResult> {
    /// 升序
    /// </summary>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection<TResult> AscendBy(TSorter sorter);

    /// <summary>
    /// 降序
    /// </summary>
    /// <param name="sorter"></param>
    /// <returns></returns>
    public ICollection<TResult> DescendBy(TSorter sorter);
}