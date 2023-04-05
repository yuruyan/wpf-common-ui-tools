using System.Globalization;
using System.Windows.Media.Imaging;

namespace CommonUITools.Converters;

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
        image.UriSource = new(
            Path.Combine(
                Path.GetDirectoryName(Environment.ProcessPath)!,
                FileIconUtils.GetPngIcon(value.ToString() ?? string.Empty)[1..]
            ),
            UriKind.Absolute
        );
        image.EndInit();
        image.Freeze();
        return image;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}