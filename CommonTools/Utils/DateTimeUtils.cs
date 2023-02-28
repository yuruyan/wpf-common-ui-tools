using System.Globalization;

namespace CommonTools.Utils;

public static class DateTimeUtils {
    private static DateTime Epoch => new(1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// 当前时间戳(ms)
    /// </summary>
    public static long CuruentMilliseconds => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// 当前时间戳(s)
    /// </summary>
    public static long CuruentSeconds => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    /// <summary>
    /// DateTime 转 Timestamp
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static long ConvertToTimestamp(DateTime value) {
        return (long)(value - Epoch).TotalMilliseconds;
    }

    /// <summary>
    /// string 日期转 DateTime
    /// </summary>
    /// <param name="value"></param>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(string value, string pattern = "yyyy-MM-dd HH:mm:ss") {
        return Convert.ToDateTime(
            value,
            new DateTimeFormatInfo() { ShortDatePattern = pattern }
        );
    }

    /// <summary>
    /// 时间戳转 DateTime
    /// </summary>
    /// <param name="timestamp"></param>
    /// <param name="milliseconds"></param>
    /// <returns></returns>
    public static DateTime ConvertToDateTime(long timestamp, bool milliseconds = true) {
        if (milliseconds) {
            return Epoch.AddMilliseconds(timestamp);
        }
        return Epoch.AddSeconds(timestamp);
    }

    /// <summary>
    /// 获取前一天的 DateTime
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime GetYesterdayTime(DateTime date) {
        return ConvertToDateTime(ConvertToTimestamp(date) - ConstantUtils.OneDayMillisecond);
    }

    /// <summary>
    /// 获取后一天的 DateTime
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static DateTime GetTomorrowTime(DateTime date) {
        return ConvertToDateTime(ConvertToTimestamp(date) + ConstantUtils.OneDayMillisecond);
    }

    /// <summary>
    /// 获取指定日期一天内的 (Start, End) Range，End 不包括
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static (DateTime, DateTime) GetDayRangeTime(DateTime date) {
        return (
            new DateTime(date.Year, date.Month, date.Day),
            new DateTime(date.Year, date.Month, date.Day).AddDays(1)
        );
    }
}