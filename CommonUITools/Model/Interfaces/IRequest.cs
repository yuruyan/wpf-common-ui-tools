namespace CommonUITools.Model.Interfaces;

public interface IRequest {
    public event EventHandler Requested;
}

public interface IRequest<T> {
    public event EventHandler<T> Requested;
}
