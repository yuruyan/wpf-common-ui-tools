using System.Globalization;

namespace CommonUITools.Converter;
/// <summary>
/// Color 转 Brush
/// </summary>
public class ColorToBrushConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return new SolidColorBrush((Color)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return ((SolidColorBrush)value).Color;
    }
}

/// <summary>
/// Brush 转 Color
/// </summary>
public class BrushToColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return ((SolidColorBrush)value).Color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return new SolidColorBrush((Color)value);
    }
}

/// <summary>
/// string 转 Brush
/// </summary>
public class StringToBrushConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value.ToString()!.ToBrush();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        return ((SolidColorBrush)value).ToString();
    }
}

/// <summary>
/// string 转 Color
/// </summary>
public class StringToColorConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        return value.ToString()!.ToColor();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
