using System.Globalization;

namespace CommonUITools.Converter;

/// <summary>
/// 转 double，不抛异常，失败返回 0
/// </summary>
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
/// 转为负数
/// </summary>
public class ToNegativeConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return -Math.Abs(System.Convert.ToDouble(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}

/// <summary>
/// 转为正数
/// </summary>
public class ToPositiveConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return Math.Abs(System.Convert.ToDouble(value));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}