namespace CommonUITools.Model;

public record class JsonResponse<T> {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;
}

public record class JsonResponse {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
}
