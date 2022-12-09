using System.Globalization;
using System.Windows.Data;

namespace CommonUITools.Converter;

/// <summary>
/// ResultIcon
/// </summary>
public class ProcessResultIconConverter : IValueConverter {
    /// <summary>
    /// Convert
    /// </summary>
    /// <param name="value">ProcessResult</param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ProcessResult result) {
            return result switch {
                ProcessResult.NotStarted => "\ue642",
                ProcessResult.Interrupted => "\ue76f",
                ProcessResult.Paused => "\ue662",
                ProcessResult.Successful => "\ue641",
                ProcessResult.Failed => "\ue6c6",
                _ => "\ue602"
            };
        }
        return "\ue602";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ResultMessage
/// </summary>
public class ProcessResultMessageConverter : IValueConverter {
    /// <inheritdoc cref="ProcessResultIconConverter.Convert(object, Type, object, CultureInfo)"/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ProcessResult result) {
            return result switch {
                ProcessResult.NotStarted => "未开始",
                ProcessResult.Processing => "处理中",
                ProcessResult.Paused => "暂停",
                ProcessResult.Interrupted => "终止",
                ProcessResult.Successful => "成功",
                ProcessResult.Failed => "失败",
                _ => string.Empty
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ResultIconForeground
/// </summary>
public class ProcessResultIconForegroundConverter : IValueConverter {
    /// <inheritdoc cref="ProcessResultIconConverter.Convert(object, Type, object, CultureInfo)"/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ProcessResult result) {
            return result switch {
                ProcessResult.Interrupted => UIUtils.StringToBrush("#f2a416"),
                ProcessResult.Successful => UIUtils.StringToBrush("#39b54d"),
                ProcessResult.Failed => UIUtils.StringToBrush("#cf3736"),
                _ => new SolidColorBrush(Colors.Black)
            };
        }
        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// ResultVisibility
/// </summary>
public class ProcessResultVisibilityConverter : IValueConverter {
    /// <inheritdoc cref="ProcessResultIconConverter.Convert(object, Type, object, CultureInfo)"/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ProcessResult result) {
            return result switch {
                ProcessResult.Processing or ProcessResult.Paused => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
