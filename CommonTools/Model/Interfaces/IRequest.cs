namespace CommonTools.Model;

public interface IRequest {
    public event EventHandler Requested;
}

public interface IRequest<T> {
    public event EventHandler<T> Requested;
}
