using System.Collections;

namespace CommonTools.Model;

public interface IFilterable {
    public ICollection Filter<T>(T filter);
}

public interface IFilterable<TResult> {
    public ICollection<TResult> Filter<TFilter>(TFilter filter);
}

public interface IFilterable<TFilter, TResult> {
    public ICollection<TResult> Filter(TFilter filter);
}
