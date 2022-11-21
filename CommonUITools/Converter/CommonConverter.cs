using CommonUITools.Utils;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CommonUITools.Converter;

public class SafeDoubleConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        try {
            return System.Convert.ToDouble(value);
        } catch (Exception) {
            return 0;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 除以 1.5 Converter
/// </summary>
public class DivideHalfThreeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) / 1.5;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 除以 2 Converter
/// </summary>
public class DivideTwoConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) / 2;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 除以 3 Converter
/// </summary>
public class DivideThreeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) / 3;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为 0 则返回 ""
/// </summary>
public class HideZeroConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        int v = System.Convert.ToInt32(value);
        return v <= 0 ? "" : v;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为 0 则隐藏
/// </summary>
public class HideIfZeroConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        try {
            return System.Convert.ToUInt64(value) == 0 ? Visibility.Collapsed : Visibility.Visible;
        } catch {
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 不为 0 则隐藏
/// </summary>
public class HideIfNotZeroConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        try {
            return System.Convert.ToUInt64(value) != 0 ? Visibility.Collapsed : Visibility.Visible;
        } catch {
            return Visibility.Collapsed;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为空则隐藏
/// </summary>
public class HideIfEmptyConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (value.ToString() ?? string.Empty).Any() ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 不为空则隐藏
/// </summary>
public class HideIfNotEmptyConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return (value.ToString() ?? string.Empty).Any() ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为True则隐藏
/// </summary>
public class HideIfTrueConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为False则隐藏
/// </summary>
public class HideIfFalseConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return !System.Convert.ToBoolean(value) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 为 Null 则隐藏
/// </summary>
public class HideIfNullConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value != null ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 不为 Null 则隐藏
/// </summary>
public class HideIfNotNullConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value != null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 相等则返回 True
/// </summary>
public class EqualConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value == parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 不相等则返回 True
/// </summary>
public class NotEqualConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value != parameter;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 相等则隐藏，比较的是 ToString
/// </summary>
public class HideIfEuqalConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value.ToString() == parameter.ToString() ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 不相等则隐藏，比较的是 ToString
/// </summary>
public class HideIfNotEuqalConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value.ToString() != parameter.ToString() ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 加法 Convert，value + parameter
/// </summary>
public class AddConvert : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) + (parameter == null ? 0 : System.Convert.ToDouble(parameter));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 减法 Converter，value - parameter
/// </summary>
public class SubtractConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return Math.Abs(System.Convert.ToDouble(value) - (parameter == null ? 0 : System.Convert.ToDouble(parameter)));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 减法 Converter，parameter - value
/// </summary>
public class SubtractInvertConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) {
            return System.Convert.ToDouble(parameter);
        }
        return System.Convert.ToDouble(parameter) - System.Convert.ToDouble(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 乘法 Converter，value * parameter
/// </summary>
public class MultiplyConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) * (parameter == null ? 1 : System.Convert.ToDouble(parameter));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 除法 Converter，value / parameter
/// </summary>
public class DivideConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return System.Convert.ToDouble(value) / (parameter == null ? 1 : System.Convert.ToDouble(parameter));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 除法 Converter，parameter / value
/// </summary>
public class DivideInvertConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) {
            return System.Convert.ToDouble(parameter);
        }
        return System.Convert.ToDouble(parameter) / System.Convert.ToDouble(value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// StringAppend Converter，前后用 '||' 分开
/// </summary
public class StringAppendConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) {
            return value;
        }
        string para = (string)parameter;
        string[] vs = para.Split("||");
        if (vs.Length == 0) {
            return value;
        }
        return $"{vs[0]}{value}{vs[1]}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

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
/// value 与 parameter 内容相同，返回 parameter 指定的 Visibility
/// 不满足则返回 Visible
/// parameter 格式：value|Visibility(大小写均可)
/// </summary>
public class VisibilityEqualConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) {
            throw new ArgumentNullException($"parameter is null");
        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        string[] ls = parameter.ToString().Split("|");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (ls.Length != 2) {
            throw new ArgumentException("content of parameter is invalid");
        }
        if (value.ToString() == ls[0]) {
            // 遍历 Visibility，找到相同的
            foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                if (item.ToString().ToLower() == ls[1].ToLower()) {
                    return item;
                }
            }
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// value 与 parameter 包括的内容匹配，返回 parameter 指定的 Visibility
/// 不满足则返回 Visible
/// parameter 格式：[a b c]|Visibility(大小写均可)
/// </summary>
public class VisibilityIncludesConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(parameter);
        string[] ls = (parameter.ToString() ?? "|").Split("|");
        if (ls.Length != 2) {
            throw new ArgumentException("content of parameter is invalid");
        }
        var values = ls[0][1..^1].Split(" ").Select(v => v.ToLower()).ToHashSet();
        if (values.Contains((value.ToString() ?? "").ToLower())) {
            // 遍历 Visibility，找到相同的
            foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                if (item.ToString().ToLower() == ls[1].ToLower()) {
                    return item;
                }
            }
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// value 与 parameter 包括的内容不匹配，返回 parameter 指定的 Visibility
/// 不满足则返回 Visible
/// parameter 格式：[a b c]|Visibility(大小写均可)
/// </summary>
public class VisibilityNotIncludesConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(parameter);
        string[] ls = (parameter.ToString() ?? "|").Split("|");
        if (ls.Length != 2) {
            throw new ArgumentException("content of parameter is invalid");
        }
        var values = ls[0][1..^1].Split(" ").Select(v => v.ToLower()).ToHashSet();
        if (!values.Contains((value.ToString() ?? "").ToLower())) {
            // 遍历 Visibility，找到相同的
            foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                if (item.ToString().ToLower() == ls[1].ToLower()) {
                    return item;
                }
            }
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// value 与 parameter 内容不相同，返回 parameter 指定的 Visibility
/// 不满足则返回 Visible
/// parameter 格式：value|Visibility(大小写均可)
/// </summary>
public class VisibilityNotEqualConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (parameter == null) {
            throw new ArgumentNullException($"parameter is null");
        }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        string[] ls = parameter.ToString().Split("|");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        if (ls.Length != 2) {
            throw new ArgumentException("content of parameter is invalid");
        }
        if (value.ToString() != ls[0]) {
            // 遍历 Visibility，找到相同的
            foreach (Visibility item in Enum.GetValues(typeof(Visibility))) {
                if (item.ToString().ToLower() == ls[1].ToLower()) {
                    return item;
                }
            }
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 显示文件名 Converter
/// </summary>
public class FileNameConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string path) {
            return Path.GetFileName(path);
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 文件大小 Converter，自动显示相应大小
/// </summary>
public class FileSizeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var size = System.Convert.ToUInt64(value);
        return size switch {
            < 1024 => string.Format("{0} B", size),
            < ConstantUtils.OneMbSize => (size / (double)1024).ToString("#.## KB"),
            < ConstantUtils.OneGbSize => (size / (double)0x100000).ToString("#.## MB"),
            _ => (size / (double)0x40000000).ToString("#.## GB"),
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

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

public class TimeStampStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return CommonUtils.ConvertToDateTime(System.Convert.ToInt64(value)).ToString("G");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Bool 条件转 String
/// parameter 格式：trueValue|falseValue
/// </summary>
public class BoolToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        string sParam = parameter.ToString() ?? "|";
        string[] values = sParam.Split('|');
        if (System.Convert.ToBoolean(value)) {
            return values[0];
        }
        return values[1];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 文件图标 Converter，返回 string，以 '/' 开头
/// </summary>
public class FileIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return FileIconUtils.GetIcon(value.ToString() ?? string.Empty);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 文件图标 Converter
/// </summary>
public class FilePngIconConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = File.OpenRead(new(
            Path.Combine(Path.GetDirectoryName(Environment.ProcessPath)!, FileIconUtils.GetPngIcon(value.ToString() ?? string.Empty)[1..])
        ));
        image.EndInit();
        image.Freeze();
        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}