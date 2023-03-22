namespace CommonUITools.Model;

public enum ProcessResult {
    /// <summary>
    /// 未开始
    /// </summary>
    NotStarted,
    /// <summary>
    /// 处理中
    /// </summary>
    Processing,
    /// <summary>
    /// 暂停
    /// </summary>
    Paused,
    /// <summary>
    /// 终止
    /// </summary>
    Interrupted,
    /// <summary>
    /// 成功
    /// </summary>
    Successful,
    /// <summary>
    /// 失败
    /// </summary>
    Failed,
}