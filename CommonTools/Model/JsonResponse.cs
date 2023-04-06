namespace CommonTools.Model;

public record class JsonResponse<T> : JsonResponse {
    public T Data { get; set; } = default!;

    public new static JsonResponse<T> Success(T data = default!) => new() { Data = data, Code = 200, Message = nameof(Success) };

    public new static JsonResponse<T> Error(T data = default!) => new() { Data = data, Code = 400, Message = nameof(Error) };

    public new static JsonResponse<T> Failed(T data = default!) => new() { Data = data, Code = 401, Message = nameof(Failed) };

    public new static JsonResponse<T> Forbidden(T data = default!) => new() { Data = data, Code = 403, Message = nameof(Forbidden) };

    public new static JsonResponse<T> NotFound(T data = default!) => new() { Data = data, Code = 404, Message = nameof(NotFound) };

    public new static JsonResponse<T> InternalError(T data = default!) => new() { Data = data, Code = 500, Message = nameof(InternalError) };

    /// <summary>
    /// 成功返回 <see cref="Success(T)"/>，失败返回 <see cref="Failed(T)"/>
    /// </summary>
    /// <param name="success"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static JsonResponse<T> From(bool success, T data = default!, string message = "") {
        return new JsonResponse<T> {
            Code = success ? 200 : 401,
            Data = data,
            Message = message,
        };
    }
}

public record class JsonResponse {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public static JsonResponse Success => new() { Code = 200, Message = nameof(Success) };

    public static JsonResponse Error => new() { Code = 400, Message = nameof(Error) };

    public static JsonResponse Failed => new() { Code = 401, Message = nameof(Failed) };

    public static JsonResponse Forbidden => new() { Code = 403, Message = nameof(Forbidden) };

    public static JsonResponse NotFound => new() { Code = 404, Message = nameof(NotFound) };

    public static JsonResponse InternalError => new() { Code = 500, Message = nameof(InternalError) };

    /// <summary>
    /// 成功返回 <see cref="Success"/>，是否返回 <see cref="Failed"/>
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static JsonResponse From(bool success, string message = "") {
        return new JsonResponse {
            Message = message,
            Code = success ? 200 : 401
        };
    }
}
