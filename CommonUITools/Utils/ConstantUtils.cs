namespace CommonUITools.Utils;

public static class ConstantUtils {
    // 1 MB 大小，以 byte 为单位
    public const long OneMbSize = 1024 * 1024;
    // 1 GB 大小，以 byte 为单位
    public const long OneGbSize = 1024 * 1024 * 1024;
    // 半分钟的秒数
    public const int HalfMinuteSecond = 30;
    // 半分钟的毫秒数
    public const int HalfMinuteMillisecond = HalfMinuteSecond * 1000;
    // 1分钟的秒数
    public const int OneMinuteSecond = HalfMinuteSecond << 1;
    // 1分钟的毫秒数
    public const int OneMinuteMillisecond = OneMinuteSecond * 1000;
    // 半小时的秒数
    public const int HalfHourSecond = 1800;
    // 半小时的毫秒数
    public const int HalfHourMillisecond = HalfHourSecond * 1000;
    // 1小时的秒数
    public const int OneHourSecond = HalfHourSecond << 1;
    // 1小时的毫秒数
    public const int OneHourMillisecond = OneHourSecond * 1000;
    // 1天的秒数
    public const int OneDaySecond = OneHourSecond * 24;
    // 1天的毫秒数
    public const int OneDayMillisecond = OneDaySecond * 1000;
}
