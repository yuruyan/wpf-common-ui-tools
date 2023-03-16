namespace CommonTools.Model;

public interface IFilterRequest {
    public event EventHandler FilterRequest;
}

public interface IFilterRequest<Args> {
    public event EventHandler<Args> FilterRequest;
}
