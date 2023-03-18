using ModernWpf.Controls;

namespace CommonUITools.Widget;

public partial class FileNameSizeWidget : SimpleStackPanel, IDisposable {
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileNameSizeWidget), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }

    public FileNameSizeWidget() {
        InitializeComponent();
    }

    /// <summary>
    /// 打开文件点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UIUtils.OpenFileWithAsync(FileName);
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenInFileExplorerHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UIUtils.OpenFileInExplorerAsync(FileName);
    }

    public void Dispose() {
        //FileNameIconWidget.Dispose();
        //FileSizeWidget.Dispose();
        BindingOperations.ClearAllBindings(FileNameSizeWidgetRoot);
        BindingOperations.ClearAllBindings(FileSizeWidget);
        BindingOperations.ClearAllBindings(this);
        FileName = string.Empty;
        Children.Clear();
        Resources.Clear();
        GC.SuppressFinalize(this);
    }
}
