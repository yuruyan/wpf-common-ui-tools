namespace CommonUITools.Widget;

public partial class DragDropTextBox : UserControl, IDisposable {
    public static readonly DependencyProperty TextBoxProperty = DependencyProperty.Register("TextBox", typeof(TextBox), typeof(DragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty FileViewProperty = DependencyProperty.Register("FileView", typeof(object), typeof(DragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(DragDropTextBox), new PropertyMetadata(false));

    /// <summary>
    /// DragDropEvent，参数为 FileData
    /// </summary>
    public event EventHandler<object>? DragDropEvent;

    /// <summary>
    /// 输入框
    /// </summary>
    public TextBox TextBox {
        get { return (TextBox)GetValue(TextBoxProperty); }
        set {
            // 释放引用
            if (GetValue(TextBoxProperty) is TextBox oldValue) {
                DragDropHelper.Dispose(oldValue);
                oldValue.PreviewDragOver -= FileDragOverHandler;
            }
            SetValue(TextBoxProperty, value);
        }
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
        UIUtils.SetLoadedOnceEventHandler(this, static (sender, _) => {
            if (sender is DragDropTextBox box) {
                box.Content = box.TextBox;
            }
        });
        DependencyPropertyDescriptor
            .FromProperty(TextBoxProperty, this.GetType())
            .AddValueChanged(this, TextBoxPropertyChangedHandler);
        DependencyPropertyDescriptor
            .FromProperty(HasFileProperty, this.GetType())
            .AddValueChanged(this, HasFilePropertyChangedHandler);
        InitializeComponent();
    }

    /// <summary>
    /// TextBox 变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TextBoxPropertyChangedHandler(object? sender, EventArgs e) {
        if (TextBox == null) {
            return;
        }
        TextBox.AllowDrop = true;
        // 先清除
        TextBox.PreviewDragOver -= FileDragOverHandler;
        TextBox.PreviewDragOver += FileDragOverHandler;
        DragDropHelper.SetIsEnabled(TextBox, true);
        DragDropHelper.SetBackgroundProperty(TextBox, BackgroundProperty);
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
    /// HasFile 变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HasFilePropertyChangedHandler(object? sender, EventArgs e) {
        Content = HasFile ? FileView : TextBox;
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

    public void Dispose() {
        #region Events
        PreviewDrop -= PreviewDropHandler;
        DependencyPropertyDescriptor
            .FromProperty(TextBoxProperty, this.GetType())
            .RemoveValueChanged(this, TextBoxPropertyChangedHandler);
        DependencyPropertyDescriptor
            .FromProperty(HasFileProperty, this.GetType())
            .RemoveValueChanged(this, HasFilePropertyChangedHandler);
        TextBox.PreviewDragOver -= FileDragOverHandler;
        DragDropHelper.Dispose(TextBox);
        DragDropHelper.Dispose(this);
        DragDropEvent = null;
        #endregion

        ClearValue(DataContextProperty);
        ClearValue(ContentProperty);
        Clear();
        GC.SuppressFinalize(this);
    }

}
