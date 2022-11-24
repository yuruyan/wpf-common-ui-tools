using CommonUITools.Utils;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace CommonUITools.Model;

public class ExtendedObservableCollection<T> : ObservableCollection<T> {
    public ExtendedObservableCollection() { }

    public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) {
    }

    public ExtendedObservableCollection(List<T> list) : base(list) {
    }

    public void AddRange(IEnumerable<T> collection) {
        CheckReentrancy();
        collection.ForEach(Items.Add);
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }
}
