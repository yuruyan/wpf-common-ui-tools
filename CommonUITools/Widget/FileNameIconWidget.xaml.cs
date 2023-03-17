using CommonUITools.Converter;

namespace CommonUITools.Widget;

/// <summary>
/// 文件名、图标 widget
/// </summary>
public partial class FileNameIconWidget : UserControl, IDisposable {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileNameIconWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(FileNameIconWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(FileNameIconWidget), new PropertyMetadata(20.0));
    public static readonly DependencyProperty AutoIconProperty = DependencyProperty.Register("AutoIcon", typeof(bool), typeof(FileNameIconWidget), new PropertyMetadata(true));

    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }
    /// <summary>
    /// 图标，用于显示
    /// </summary>
    public string IconPath {
        get { return (string)GetValue(IconPathProperty); }
        set { SetValue(IconPathProperty, value); }
    }
    /// <summary>
    /// 图标大小，默认 20
    /// </summary>
    public double IconSize {
        get { return (double)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }
    /// <summary>
    /// 是否自动获取 Icon，默认 true
    /// </summary>
    public bool AutoIcon {
        get { return (bool)GetValue(AutoIconProperty); }
        set { SetValue(AutoIconProperty, value); }
    }
    private readonly IValueConverter FileIconConverter = new FileIconConverter();

    public FileNameIconWidget() {
        DependencyPropertyDescriptor
            .FromProperty(AutoIconProperty, typeof(FileNameIconWidget))
            .AddValueChanged(this, AutoIconPropertyChangedHandler);
        InitializeComponent();
    }

    /// <summary>
    /// AutoIconProperty 改变
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AutoIconPropertyChangedHandler(object? sender, EventArgs e) {
        // 自动获取
        if (!AutoIcon) {
            FileIconImage.SetBinding(Image.SourceProperty, new Binding(nameof(IconPath)));
            return;
        }
        FileIconImage.SetBinding(Image.SourceProperty, new Binding(nameof(FileName)) {
            Converter = FileIconConverter
        });
    }

    public void Dispose() {
        DataContext = null;
        FileIconImage.ClearValue(Image.SourceProperty);
        DependencyPropertyDescriptor
            .FromProperty(AutoIconProperty, typeof(FileNameIconWidget))
            .RemoveValueChanged(this, AutoIconPropertyChangedHandler);
        GC.SuppressFinalize(this);
    }
}
