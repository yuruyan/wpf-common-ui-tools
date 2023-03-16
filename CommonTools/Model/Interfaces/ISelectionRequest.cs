namespace CommonTools.Model;

public interface ISelectionRequest {
    public event EventHandler SelectionRequest;
}

public interface ISelectionRequest<Args> {
    public event EventHandler<Args> SelectionRequest;
}
