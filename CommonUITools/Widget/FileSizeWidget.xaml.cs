namespace CommonUITools.Widget;

public partial class FileSizeWidget : UserControl, IDisposable {
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileSizeWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty FileSizeProperty = DependencyProperty.Register("FileSize", typeof(long), typeof(FileSizeWidget), new PropertyMetadata(0L));
    public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register("Prefix", typeof(string), typeof(FileSizeWidget), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(string), typeof(FileSizeWidget), new PropertyMetadata(string.Empty));

    /// <summary>
    /// 文件名
    /// </summary>
    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }
    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize {
        get { return (long)GetValue(FileSizeProperty); }
        private set { SetValue(FileSizeProperty, value); }
    }
    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix {
        get { return (string)GetValue(PrefixProperty); }
        set { SetValue(PrefixProperty, value); }
    }
    /// <summary>
    /// 后缀
    /// </summary>
    public string Suffix {
        get { return (string)GetValue(SuffixProperty); }
        set { SetValue(SuffixProperty, value); }
    }

    public FileSizeWidget() {
        DependencyPropertyDescriptor
            .FromProperty(FileNameProperty, this.GetType())
            .AddValueChanged(this, FileNamePropertyChangedHandler);
        InitializeComponent();
    }

    /// <summary>
    /// FileNamePropertyChanged
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileNamePropertyChangedHandler(object? sender, EventArgs e) {
        if (!File.Exists(FileName)) {
            FileSize = 0;
            return;
        }
        FileSize = new FileInfo(FileName).Length;
    }

    public void Dispose() {
        DependencyPropertyDescriptor
            .FromProperty(FileNameProperty, this.GetType())
            .RemoveValueChanged(this, FileNamePropertyChangedHandler);
        ClearValue(ContentProperty);
        ClearValue(DataContextProperty);
        FileName = Prefix = Suffix = string.Empty;
        GC.SuppressFinalize(this);
    }
}
