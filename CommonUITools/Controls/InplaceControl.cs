namespace CommonUITools.Controls;

public class InplaceControl : Control {
    public static readonly DependencyProperty ReaderProperty = DependencyProperty.Register("Reader", typeof(object), typeof(InplaceControl), new PropertyMetadata());
    public static readonly DependencyProperty WriterProperty = DependencyProperty.Register("Writer", typeof(object), typeof(InplaceControl), new PropertyMetadata());
    public static readonly DependencyProperty IsWriterVisibleProperty = DependencyProperty.Register("IsWriterVisible", typeof(bool), typeof(InplaceControl), new PropertyMetadata(false, IsWriterVisiblePropertyChangedHandler));
    public event EventHandler<bool>? IsWriterVisibleChanged;
    private bool IsMouseClick;

    /// <summary>
    /// 只读 Control
    /// </summary>
    public object Reader {
        get { return (object)GetValue(ReaderProperty); }
        set { SetValue(ReaderProperty, value); }
    }
    /// <summary>
    /// 读写 Control
    /// </summary>
    public object Writer {
        get { return (object)GetValue(WriterProperty); }
        set { SetValue(WriterProperty, value); }
    }
    /// <summary>
    /// Write 是否可见
    /// </summary>
    public bool IsWriterVisible {
        get { return (bool)GetValue(IsWriterVisibleProperty); }
        set { SetValue(IsWriterVisibleProperty, value); }
    }
    private Window CurrentWindow = Application.Current.MainWindow;

    static InplaceControl() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(InplaceControl), new FrameworkPropertyMetadata(typeof(InplaceControl)));
    }

    private static void IsWriterVisiblePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is InplaceControl self) {
            self.IsWriterVisibleChanged?.Invoke(self, (bool)e.NewValue);
        }
    }

    public InplaceControl() {
        // Click Reader to show writer
        this.SetLoadedOnceEventHandler((_, _) => {
            CurrentWindow = Window.GetWindow(this);
            if (Template.FindName("ReaderControl", this) is FrameworkElement element) {
                element.MouseUp += (_, _) => IsWriterVisible = true;
            }
        });
        PreviewMouseDown += (_, _) => IsMouseClick = true;
        // Place after 'SetLoadedOnceEventHandler'
        Loaded += (_, _) => CurrentWindow.PreviewMouseUp += WindowPreviewMouseUpHandler;
        Unloaded += (_, _) => CurrentWindow.PreviewMouseUp -= WindowPreviewMouseUpHandler;
    }

    private void WindowPreviewMouseUpHandler(object sender, MouseButtonEventArgs e) {
        // Click outside
        if (!IsMouseClick) {
            IsWriterVisible = false;
        }
        IsMouseClick = false;
    }
}
