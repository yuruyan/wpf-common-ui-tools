namespace CommonUITools.Widget;

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
    private readonly DispatcherTimer UpdateStatusTimer = new() {
        Interval = TimeSpan.FromMilliseconds(1500)
    };

    public FileProcessStatusWidget() {
        FileProcessStatusList = new();
        UpdateStatusTimer.Tick += (_, _) => {
            HasTaskRunning = FileProcessStatusList.Any(f => f.Status == ProcessResult.Processing);
            FinishedCount = FileProcessStatusList.Count(f => f.Status switch {
                ProcessResult.Interrupted or ProcessResult.Successful or ProcessResult.Failed => true,
                _ => false
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
