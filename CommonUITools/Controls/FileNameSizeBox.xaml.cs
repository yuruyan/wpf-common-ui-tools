using ModernWpf.Controls;

namespace CommonUITools.Controls;

public partial class FileNameSizeBox : SimpleStackPanel {
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileNameSizeBox), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }

    public FileNameSizeBox() {
        InitializeComponent();
    }

    /// <summary>
    /// 打开文件点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        FileName.OpenFileWithAsync();
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenInFileExplorerHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        FileName.OpenFileInExplorerAsync();
    }
}
