namespace CommonTools.Model;

public record class JsonResponse<T> {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;

    public static JsonResponse<T> Success => new() { Code = 200, Message = nameof(Success) };

    public static JsonResponse<T> Error => new() { Code = 400, Message = nameof(Error) };

    public static JsonResponse<T> Forbidden => new() { Code = 403, Message = nameof(Forbidden) };

    public static JsonResponse<T> NotFound => new() { Code = 404, Message = nameof(NotFound) };

    public static JsonResponse<T> InternalError => new() { Code = 500, Message = nameof(InternalError) };
}

public record class JsonResponse {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public static JsonResponse Success => new() { Code = 200, Message = nameof(Success) };

    public static JsonResponse Error => new() { Code = 400, Message = nameof(Error) };

    public static JsonResponse Forbidden => new() { Code = 403, Message = nameof(Forbidden) };

    public static JsonResponse NotFound => new() { Code = 404, Message = nameof(NotFound) };

    public static JsonResponse InternalError => new() { Code = 500, Message = nameof(InternalError) };
}
