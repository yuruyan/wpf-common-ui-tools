using System.Globalization;

namespace CommonUITools.Converter;

/// <summary>
/// DateTime 转 DateTime String Converter
/// </summary>
public class DateTimeToDateTimeStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string delimiter = (string)(parameter ?? "-");
        return ((DateTime)value).ToString($"yyyy{delimiter}MM{delimiter}dd HH:mm:ss");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// DateTime 转 Date String Converter
/// </summary>
public class DateTimeToDateStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string delimiter = (string)(parameter ?? "-");
        return ((DateTime)value).ToString($"yyyy{delimiter}MM{delimiter}dd");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 时间戳转字符串
/// </summary>
public class TimeStampStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return DateTimeUtils.ToDateTime(System.Convert.ToInt64(value)).ToString("G");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// DateTime 转年龄
/// </summary>
public class AgeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is DateTime dateTime) {
            return (int)((DateTime.Now - dateTime).TotalDays / 365);
        }
        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
