using ModernWpf.Controls;
using System.Windows;

namespace CommonUITools.Widget;

public partial class FileNameSizeWidget : SimpleStackPanel {
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
}
