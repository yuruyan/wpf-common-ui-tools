using System.Globalization;

namespace CommonUITools.Converter;

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
        var size = System.Convert.ToInt64(value);
        if (size < 0) {
            size = 0;
        }
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
/// 数值转 GridLength
/// </summary>
public class NumberToGridLengthConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return new GridLength(System.Convert.ToDouble(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 数值转 Thickness
/// </summary>
public class NumberToThicknessConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return new Thickness(System.Convert.ToDouble(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 数值转 CornerRadius
/// </summary>
public class NumberToCornerRadiusConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return new CornerRadius(System.Convert.ToDouble(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 对象转字符串
/// </summary>
public class ObjectToStringConvert : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value?.ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}