namespace CommonTools.Model;

public enum CommonStatus {
    SUCCESS, FAILED, ERROR
}

public class DataTransferResult<T> {
    public CommonStatus Status { get; set; } = CommonStatus.FAILED;
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; }

    /// <summary>
    /// 根据 result 创建 DataTransferResult
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    /// <typeparam name="Destination"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static DataTransferResult<T> FromResult<Source>(DataTransferResult<Source> result) {
        return new DataTransferResult<T> {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DataTransferResult<T> Failed() {
        return new DataTransferResult<T>();
    }

    public static DataTransferResult<T> Success() {
        return new DataTransferResult<T>() { Status = CommonStatus.SUCCESS };
    }

    public static DataTransferResult<T> Success(T data) {
        return new DataTransferResult<T>() { Status = CommonStatus.SUCCESS, Data = data };
    }

    public static DataTransferResult<T> Error() {
        return new DataTransferResult<T>() { Status = CommonStatus.ERROR };
    }

    public override string ToString() {
        return $"{{{nameof(Status)}={Status.ToString()}, {nameof(Message)}={Message}, {nameof(Data)}={Data}}}";
    }
}

public class DataTransferResults<T> {
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
    public static DataTransferResults<T> FromResult<Source>(DataTransferResults<Source> result) {
        return new DataTransferResults<T> {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DataTransferResults<T> Failed() {
        return new DataTransferResults<T>();
    }

    public static DataTransferResults<T> Success() {
        return new DataTransferResults<T>() { Status = CommonStatus.SUCCESS };
    }

    public static DataTransferResults<T> Error() {
        return new DataTransferResults<T>() { Status = CommonStatus.ERROR };
    }

    public override string ToString() {
        return $"{{{nameof(Status)}={Status.ToString()}, {nameof(Message)}={Message}, {nameof(Data)}={Data}}}";
    }
}

public class DataTransferResult {
    public CommonStatus Status { get; set; } = CommonStatus.FAILED;
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 根据 result 创建 DataTransferResult
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static DataTransferResult FromResult(DataTransferResult result) {
        return new DataTransferResult {
            Status = result.Status,
            Message = result.Message
        };
    }

    public static DataTransferResult Failed() {
        return new DataTransferResult();
    }

    public static DataTransferResult Success() {
        return new DataTransferResult { Status = CommonStatus.SUCCESS };
    }

    public static DataTransferResult Error() {
        return new DataTransferResult { Status = CommonStatus.ERROR };
    }

    public override string ToString() {
        return $"{{{nameof(Status)}={Status.ToString()}, {nameof(Message)}={Message}}}";
    }
}
