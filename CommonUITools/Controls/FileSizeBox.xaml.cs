namespace CommonUITools.Controls;

public partial class FileSizeBox : UserControl {
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileSizeBox), new PropertyMetadata(string.Empty, FileNamePropertyChangedHandler));
    public static readonly DependencyProperty FileSizeProperty = DependencyProperty.Register("FileSize", typeof(long), typeof(FileSizeBox), new PropertyMetadata(0L));
    public static readonly DependencyProperty PrefixProperty = DependencyProperty.Register("Prefix", typeof(string), typeof(FileSizeBox), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty SuffixProperty = DependencyProperty.Register("Suffix", typeof(string), typeof(FileSizeBox), new PropertyMetadata(string.Empty));

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

    public FileSizeBox() {
        InitializeComponent();
    }

    private static void FileNamePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FileSizeBox self) {
            return;
        }

        if (!File.Exists(self.FileName)) {
            self.FileSize = 0;
            return;
        }
        self.FileSize = new FileInfo(self.FileName).Length;
    }
}
