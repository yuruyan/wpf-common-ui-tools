namespace CommonTools.Model;

public interface ISortRequest {
    public event EventHandler SortRequest;
}

public interface ISortRequest<Args> {
    public event EventHandler<Args> SortRequest;
}
