namespace CommonUITools.Model;

public class JsonResponse<T> {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;
    public T? data { get; set; }

    public override string ToString() {
        return $"{{{nameof(code)}={code.ToString()}, {nameof(message)}={message}, {nameof(data)}={data}}}";
    }
}

public class JsonResponse {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;

    public override string ToString() {
        return $"{{{nameof(code)}={code.ToString()}, {nameof(message)}={message}}}";
    }
}
