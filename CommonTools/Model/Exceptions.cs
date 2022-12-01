using System.Runtime.Serialization;

namespace CommonTools.Model;

/// <summary>
/// 解析错误
/// </summary>
public class ParseException : Exception {
    public ParseException() { }

    public ParseException(string? message) : base(message) { }

    public ParseException(string? message, Exception? innerException) : base(message, innerException) { }

    protected ParseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

/// <summary>
/// 加载错误
/// </summary>
public class LoadException : Exception {
    public LoadException() { }

    public LoadException(string? message) : base(message) { }

    public LoadException(string? message, Exception? innerException) : base(message, innerException) { }

    protected LoadException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
