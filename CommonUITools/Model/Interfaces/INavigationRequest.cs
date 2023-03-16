namespace CommonUITools.Model;

public interface INavigationRequest {
    public event EventHandler NavigationRequested;
}

public interface INavigationRequest<Args> {
    public event EventHandler<Args> NavigationRequested;
}
