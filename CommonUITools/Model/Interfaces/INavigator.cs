namespace CommonUITools.Model;

public interface INavigator {
    public void Navigate(object view);
}

public interface INavigator<T> {
    public void Navigate(T view);
}
