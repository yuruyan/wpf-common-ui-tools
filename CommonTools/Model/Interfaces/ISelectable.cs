using System.Collections;

namespace CommonTools.Model;

public interface ISelectable {
    public ICollection Select();

    public ICollection SelectBy<T>(T selector);
}

public interface ISelectable<T> {
    public ICollection<T> Select();

    public ICollection<T> SelectBy<S>(S selector);
}

public interface ISelectable<Selector, TResult> {
    public ICollection<TResult> Select();

    public ICollection<TResult> SelectBy(Selector selector);
}