namespace CommonUITools.Controls;

public partial class LoadingBox : UserControl {
    public const double DefaultSize = 30;
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(LoadingBox), new PropertyMetadata(DefaultSize));
    private readonly Storyboard LoadStoryboard;
    private readonly Storyboard UnLoadStoryboard;

    /// <summary>
    /// ProcessRing 大小
    /// </summary>
    public double Size {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public LoadingBox() {
        InitializeComponent();
        LoadStoryboard = (Storyboard)Resources["LoadStoryboard"];
        UnLoadStoryboard = (Storyboard)Resources["UnLoadStoryboard"];
        UnLoadStoryboard.Completed += (_, _) => {
            Opacity = 0;
            Visibility = Visibility.Collapsed;
        };
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show() {
        if (!IsVisible) {
            Visibility = Visibility.Visible;
            LoadStoryboard.Begin();
        }
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close() {
        if (IsVisible) {
            UnLoadStoryboard.Begin();
        }
    }
}
