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

internal class ProcessResultMessageConverter : IValueConverter {
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

internal class ProcessResultIconForegroundConverter : IValueConverter {
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
    public static readonly DependencyProperty FinishedCountProperty = DependencyProperty.Register("FinishedCount", typeof(int), typeof(FileProcessStatusWidget), new PropertyMetadata(0));
    public static readonly DependencyProperty HasTaskRunningProperty = DependencyProperty.Register("HasTaskRunning", typeof(bool), typeof(FileProcessStatusWidget), new PropertyMetadata(false));

    /// <summary>
    /// FileProcessStatusList
    /// </summary>
    public ObservableCollection<FileProcessStatus> FileProcessStatusList {
        get { return (ObservableCollection<FileProcessStatus>)GetValue(FileProcessStatusListProperty); }
        set { SetValue(FileProcessStatusListProperty, value); }
    }
    /// <summary>
    /// 完成任务数
    /// </summary>
    public int FinishedCount {
        get { return (int)GetValue(FinishedCountProperty); }
        set { SetValue(FinishedCountProperty, value); }
    }
    /// <summary>
    /// 是否有任务正在运行
    /// </summary>
    public bool HasTaskRunning {
        get { return (bool)GetValue(HasTaskRunningProperty); }
        set { SetValue(HasTaskRunningProperty, value); }
    }
    /// <summary>
    /// 更新状态 Timer
    /// </summary>
    private readonly System.Timers.Timer UpdateStatusTimer = new(1500);

    public FileProcessStatusWidget() {
        FileProcessStatusList = new();
        UpdateStatusTimer.Elapsed += (_, _) => {
            Dispatcher.Invoke(() => {
                HasTaskRunning = FileProcessStatusList.Any(f => f.Status == ProcessResult.Processing);
                FinishedCount = FileProcessStatusList.Count(f => f.Status switch {
                    ProcessResult.Interrupted or ProcessResult.Successful or ProcessResult.Failed => true,
                    _ => false
                });
            });
        };
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

    /// <summary>
    /// 开启定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UpdateStatusTimer.Start();
    }

    /// <summary>
    /// 暂停定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewUnloadedHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UpdateStatusTimer.Stop();
    }
}
