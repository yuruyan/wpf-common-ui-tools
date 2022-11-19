using CommonUITools.Model;
using CommonUITools.Utils;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CommonUITools.Widget;

internal class ProcessResultIconConverter : IValueConverter {
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

internal class ProcessResultIconForegroundConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is ProcessResult result) {
            return result switch {
                ProcessResult.Interrupted => UIUtils.StringToBrush("#1296db"),
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

internal class ProcessVisibilityConverter : IValueConverter {
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

public partial class FileProcessStatusWidget : UserControl {
    public static readonly DependencyProperty FileProcessStatusListProperty = DependencyProperty.Register("FileProcessStatusList", typeof(ObservableCollection<FileProcessStatus>), typeof(FileProcessStatusWidget), new PropertyMetadata());

    /// <summary>
    /// FileProcessStatusList
    /// </summary>
    public ObservableCollection<FileProcessStatus> FileProcessStatusList {
        get { return (ObservableCollection<FileProcessStatus>)GetValue(FileProcessStatusListProperty); }
        set { SetValue(FileProcessStatusListProperty, value); }
    }

    public FileProcessStatusWidget() {
        FileProcessStatusList = new();
        InitializeComponent();
    }

    /// <summary>
    /// 打开文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is FileProcessStatus status) {
            UIUtils.OpenFileWithAsync(status.FileName);
        }
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is FileProcessStatus status) {
            UIUtils.OpenFileInExplorerAsync(status.FileName);
        }
    }

    /// <summary>
    /// OpenFileMenuItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileMenuItemLoadedHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is FileProcessStatus status) {
            element.Visibility = status.Status switch {
                ProcessResult.Successful or ProcessResult.Interrupted or ProcessResult.Failed => Visibility.Visible,
                _ => Visibility.Collapsed
            };
        }
    }
}
