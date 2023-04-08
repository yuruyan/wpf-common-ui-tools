namespace CommonUITools.Model;

public interface INavigationRequest {
    public event EventHandler NavigationRequested;
}

public interface INavigationRequest<Args> : INavigationRequest {
    public new event EventHandler<Args> NavigationRequested;
}
