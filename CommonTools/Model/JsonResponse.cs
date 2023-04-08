namespace CommonTools.Model;

public static class ResponseCodes {
    public const int Success = 200;
    public const int Error = 400;
    public const int Failed = 401;
    public const int Forbidden = 403;
    public const int NotFound = 404;
    public const int InternalError = 500;
}

public record class JsonResponse<T> : JsonResponse {
    public T Data { get; set; } = default!;

    public new static JsonResponse<T> Success(T data = default!) => new() { Data = data, Code = ResponseCodes.Success, Message = nameof(Success) };

    public new static JsonResponse<T> Error(T data = default!) => new() { Data = data, Code = ResponseCodes.Error, Message = nameof(Error) };

    public new static JsonResponse<T> Failed(T data = default!) => new() { Data = data, Code = ResponseCodes.Failed, Message = nameof(Failed) };

    public new static JsonResponse<T> Forbidden(T data = default!) => new() { Data = data, Code = ResponseCodes.Forbidden, Message = nameof(Forbidden) };

    public new static JsonResponse<T> NotFound(T data = default!) => new() { Data = data, Code = ResponseCodes.NotFound, Message = nameof(NotFound) };

    public new static JsonResponse<T> InternalError(T data = default!) => new() { Data = data, Code = ResponseCodes.InternalError, Message = nameof(InternalError) };

    /// <summary>
    /// 成功返回 <see cref="Success(T)"/>，失败返回 <see cref="Failed(T)"/>
    /// </summary>
    /// <param name="success"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static JsonResponse<T> From(bool success, T data = default!, string? message = null) {
        return new JsonResponse<T> {
            Code = success ? ResponseCodes.Success : ResponseCodes.Failed,
            Data = data,
            Message = message ?? (success ? nameof(Success) : nameof(Failed)),
        };
    }

    /// <summary>
    /// <paramref name="data"/> 不为 null 返回 <see cref="Success(T)"/>，否则返回 <see cref="Failed(T)"/>
    /// </summary>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static JsonResponse<T> From(T data, string? message = null) {
        return new JsonResponse<T> {
            Code = data is null ? ResponseCodes.Failed : ResponseCodes.Success,
            Data = data,
            Message = message ?? (data is null ? nameof(Failed) : nameof(Success)),
        };
    }
}

public record class JsonResponse {
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;

    public static JsonResponse Success => new() { Code = ResponseCodes.Success, Message = nameof(Success) };

    public static JsonResponse Error => new() { Code = ResponseCodes.Error, Message = nameof(Error) };

    public static JsonResponse Failed => new() { Code = ResponseCodes.Failed, Message = nameof(Failed) };

    public static JsonResponse Forbidden => new() { Code = ResponseCodes.Forbidden, Message = nameof(Forbidden) };

    public static JsonResponse NotFound => new() { Code = ResponseCodes.NotFound, Message = nameof(NotFound) };

    public static JsonResponse InternalError => new() { Code = ResponseCodes.InternalError, Message = nameof(InternalError) };

    /// <summary>
    /// 成功返回 <see cref="Success"/>，是否返回 <see cref="Failed"/>
    /// </summary>
    /// <param name="success"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static JsonResponse From(bool success, string? message = null) {
        return new JsonResponse {
            Code = success ? ResponseCodes.Success : ResponseCodes.Failed,
            Message = message ?? (success ? nameof(Success) : nameof(Failed)),
        };
    }
}
