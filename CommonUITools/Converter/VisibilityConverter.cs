using System.Globalization;

namespace CommonUITools.Converter;

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
/// 相等则隐藏，比较的是 ToString
/// </summary>
public class HideIfEuqalConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value?.ToString() == parameter?.ToString() ? Visibility.Collapsed : Visibility.Visible;
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
        return value?.ToString() != parameter?.ToString() ? Visibility.Collapsed : Visibility.Visible;
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
            return System.Convert.ToInt64(value) == 0 ? Visibility.Collapsed : Visibility.Visible;
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
            return System.Convert.ToInt64(value) != 0 ? Visibility.Collapsed : Visibility.Visible;
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
