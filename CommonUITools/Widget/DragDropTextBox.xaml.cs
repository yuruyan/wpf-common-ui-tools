namespace CommonUITools.Widget;

public partial class DragDropTextBox : UserControl {
    public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register("TextBox", typeof(TextBox), typeof(DragDropTextBox), new PropertyMetadata(TextBoxPropertyChangedHandler));
    public static readonly DependencyProperty FileViewProperty = DependencyProperty.Register("FileView", typeof(object), typeof(DragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(DragDropTextBox), new PropertyMetadata(false, HasFilePropertyChangedHandler));

    /// <summary>
    /// DragDropEvent，参数为 FileData
    /// </summary>
    public event EventHandler<object>? DragDropEvent;

    /// <summary>
    /// 输入框
    /// </summary>
    public TextBox TextBox {
        get { return (TextBox)GetValue(TextBoxProperty); }
        set { SetValue(TextBoxProperty, value); }
    }
    /// <summary>
    /// 文件界面
    /// </summary>
    public object FileView {
        get { return (object)GetValue(FileViewProperty); }
        set { SetValue(FileViewProperty, value); }
    }
    /// <summary>
    /// 是否有文件
    /// </summary>
    public bool HasFile {
        get { return (bool)GetValue(HasFileProperty); }
        set { SetValue(HasFileProperty, value); }
    }
    /// <summary>
    /// 文件数据
    /// </summary>
    public object? FileData { get; private set; }

    public DragDropTextBox() {
        // Set content to TextBox
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is DragDropTextBox box) {
                box.Content = box.TextBox;
            }
        });
        InitializeComponent();
    }

    private static void TextBoxPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not DragDropTextBox self) {
            return;
        }

        // Clear reference
        if (e.OldValue is TextBox oldTextBox) {
            oldTextBox.PreviewDragOver -= self.FileDragOverHandler;
            DragDropHelper.Dispose(oldTextBox);
        }
        if (e.NewValue is not TextBox textBox) {
            return;
        }
        textBox.AllowDrop = true;
        // 先清除
        textBox.PreviewDragOver -= self.FileDragOverHandler;
        textBox.PreviewDragOver += self.FileDragOverHandler;
        DragDropHelper.SetIsEnabled(textBox, true);
        DragDropHelper.SetBackgroundProperty(textBox, BackgroundProperty);
    }

    private static void HasFilePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not DragDropTextBox self) {
            return;
        }
        self.Content = e.NewValue is true ? self.FileView : self.TextBox;
    }

    /// <summary>
    /// 更改 TextBox 默认拖拽行为
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileDragOverHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        e.Effects = DragDropEffects.Copy;
    }

    /// <summary>
    /// Drop 事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PreviewDropHandler(object sender, DragEventArgs e) {
        FileData = e.Data.GetData(DataFormats.FileDrop);
        if (FileData != null) {
            HasFile = true;
            DragDropEvent?.Invoke(sender, FileData);
        }
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    public void Clear() {
        TextBox.Clear();
        HasFile = false;
        FileData = null;
    }

    private void RootLoadedHandler(object sender, RoutedEventArgs e) {
        DragDropHelper.SetBackgroundProperty(this, BackgroundProperty);
        DragDropHelper.SetIsEnabled(this, true);
        PreviewDrop += PreviewDropHandler;
        if (TextBox is not null) {
            TextBox.PreviewDragOver -= FileDragOverHandler;
            TextBox.PreviewDragOver += FileDragOverHandler;
            DragDropHelper.SetIsEnabled(TextBox, true);
            DragDropHelper.SetBackgroundProperty(TextBox, BackgroundProperty);
        }
    }

    private void RootUnloadedHandler(object sender, RoutedEventArgs e) {
        DragDropHelper.Dispose(this);
        PreviewDrop -= PreviewDropHandler;
        if (TextBox is not null) {
            TextBox.PreviewDragOver -= FileDragOverHandler;
            DragDropHelper.Dispose(TextBox);
        }
    }
}
