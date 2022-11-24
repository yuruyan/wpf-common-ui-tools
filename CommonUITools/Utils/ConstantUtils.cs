namespace CommonUITools.Utils;

public static class ConstantUtils {
    #region 大小
    /// <summary>
    /// 1 KB 大小，以 byte 为单位
    /// </summary>
    public const long OneKbSize = 1024;
    /// <summary>
    /// 1 MB 大小，以 byte 为单位
    /// </summary>
    public const long OneMbSize = 1024 * 1024;
    /// <summary>
    /// 1 GB 大小，以 byte 为单位
    /// </summary>
    public const long OneGbSize = 1024 * 1024 * 1024;
    /// <summary>
    /// 1 TB 大小，以 byte 为单位
    /// </summary>
    public const long OneTbSize = 1024 * 1024 * 1024;
    /// <summary>
    /// 默认文件缓冲区大小，以 byte 为单位
    /// </summary>
    public const long DefaultFileBufferSize = 4096;
    #endregion

    #region 时间
    /// <summary>
    /// 半分钟的秒数
    /// </summary>
    public const int HalfMinuteSecond = 30;
    /// <summary>
    /// 半分钟的毫秒数
    /// </summary>
    public const int HalfMinuteMillisecond = HalfMinuteSecond * 1000;
    /// <summary>
    /// 1分钟的秒数
    /// </summary>
    public const int OneMinuteSecond = HalfMinuteSecond << 1;
    /// <summary>
    /// 1分钟的毫秒数
    /// </summary>
    public const int OneMinuteMillisecond = OneMinuteSecond * 1000;
    /// <summary>
    /// 半小时的秒数
    /// </summary>
    public const int HalfHourSecond = 1800;
    /// <summary>
    /// 半小时的毫秒数
    /// </summary>
    public const int HalfHourMillisecond = HalfHourSecond * 1000;
    /// <summary>
    /// 1小时的秒数
    /// </summary>
    public const int OneHourSecond = HalfHourSecond << 1;
    /// <summary>
    /// 1小时的毫秒数
    /// </summary>
    public const int OneHourMillisecond = OneHourSecond * 1000;
    /// <summary>
    /// 1天的秒数
    /// </summary>
    public const int OneDaySecond = OneHourSecond * 24;
    /// <summary>
    /// 1天的毫秒数
    /// </summary>
    public const int OneDayMillisecond = OneDaySecond * 1000;
    #endregion
}
