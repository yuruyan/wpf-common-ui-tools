namespace CommonTools.Model;

public enum CommonStatus {
    SUCCESS, FAILED, ERROR
}

public record DTResult<T> {
    public CommonStatus Status { get; set; } = CommonStatus.FAILED;
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;

    /// <summary>
    /// 根据 result 创建 DTResult，不拷贝 data
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static DTResult<T> FromResult<Source>(DTResult<Source> result) {
        return new DTResult<T> {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DTResult<T> Failed(string message = "") {
        return new DTResult<T>();
    }

    public static DTResult<T> Success() {
        return new DTResult<T>() { Status = CommonStatus.SUCCESS };
    }

    public static DTResult<T> Success(T data) {
        return new DTResult<T>() { Status = CommonStatus.SUCCESS, Data = data };
    }

    public static DTResult<T> Error(string message = "") {
        return new DTResult<T>() { Status = CommonStatus.ERROR };
    }
}

public record DTResults<T> {
    public CommonStatus Status { get; set; } = CommonStatus.FAILED;
    public string Message { get; set; } = string.Empty;
    public IList<T> Data { get; set; } = new List<T>();

    /// <summary>
    /// 根据 result 创建 DataTransferResults
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <typeparam name="Destination"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static DTResults<T> FromResult<Source>(DTResults<Source> result) {
        return new DTResults<T> {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DTResults<T> Failed() {
        return new DTResults<T>();
    }

    public static DTResults<T> Success() {
        return new DTResults<T>() { Status = CommonStatus.SUCCESS };
    }

    public static DTResults<T> Error() {
        return new DTResults<T>() { Status = CommonStatus.ERROR };
    }
}

public record DTResult {
    public CommonStatus Status { get; set; } = CommonStatus.FAILED;
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 根据 result 创建 DTResult
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static DTResult FromResult(DTResult result) {
        return new DTResult {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DTResult Failed() {
        return new DTResult();
    }

    public static DTResult Failed(string message) {
        return new DTResult() { Message = message };
    }

    public static DTResult Success() {
        return new DTResult { Status = CommonStatus.SUCCESS };
    }

    public static DTResult Error() {
        return new DTResult { Status = CommonStatus.ERROR };
    }

    public static DTResult Error(string message) {
        return new DTResult { Status = CommonStatus.ERROR, Message = message };
    }
}
