using System.Collections.Specialized;

namespace CommonUITools.Widget;

public partial class MultiDragDropTextBox : UserControl {
    private const string FileListInfoBorderFadeOutStoryboardName = "FileListInfoBorderFadeOutStoryboard";
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(MultiDragDropTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(MultiDragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(MultiDragDropTextBox), new PropertyMetadata(false, HasFilePropertyChangedHandler));
    private static readonly DependencyPropertyKey FileNameListPropertyKey = DependencyProperty.RegisterReadOnly("FileNameList", typeof(ExtendedObservableCollection<string>), typeof(MultiDragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty FileNameListProperty = FileNameListPropertyKey.DependencyProperty;
    private static readonly DependencyProperty FirstFileNameProperty = DependencyProperty.Register("FirstFileName", typeof(string), typeof(MultiDragDropTextBox), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsSupportDirectoryProperty = DependencyProperty.Register("IsSupportDirectory", typeof(bool), typeof(MultiDragDropTextBox), new PropertyMetadata(false));

    /// <summary>
    /// DragDropEvent，参数为 本次拖拽的 FileNames
    /// </summary>
    public event EventHandler<IList<string>>? DragDropEvent;
    /// <summary>
    /// 文件名集合
    /// </summary>
    private readonly ISet<string> FileNameSet = new HashSet<string>();
    /// <summary>
    /// 文件数据
    /// </summary>
    public object? FileData { get; private set; }
    /// <summary>
    /// 获取完整文件名列表
    /// </summary>
    public IList<string> FileNames => FileNameList.ToList();
    /// <summary>
    /// FileListInfoBorderFadeOut 动画
    /// </summary>
    private Storyboard FileListInfoBorderFadeOutStoryboard = default!;

    /// <summary>
    /// 输入文本，默认双向绑定
    /// </summary>
    public string InputText {
        get { return (string)GetValue(InputTextProperty); }
        set { SetValue(InputTextProperty, value); }
    }
    /// <summary>
    /// 文本框样式，默认 MultilineTextBoxStyle
    /// </summary>
    public Style TextBoxStyle {
        get { return (Style)GetValue(TextBoxStyleProperty); }
        set { SetValue(TextBoxStyleProperty, value); }
    }
    /// <summary>
    /// 是否有文件
    /// </summary>
    public bool HasFile {
        get { return (bool)GetValue(HasFileProperty); }
        private set { SetValue(HasFileProperty, value); }
    }
    /// <summary>
    /// 文件名列表
    /// </summary>
    private ExtendedObservableCollection<string> FileNameList => (ExtendedObservableCollection<string>)GetValue(FileNameListProperty);
    /// <summary>
    /// 第一个文件名
    /// </summary>
    private string FirstFileName {
        get { return (string)GetValue(FirstFileNameProperty); }
        set { SetValue(FirstFileNameProperty, value); }
    }
    /// <summary>
    /// 是否支持文件夹，默认 false
    /// </summary>
    public bool IsSupportDirectory {
        get { return (bool)GetValue(IsSupportDirectoryProperty); }
        set { SetValue(IsSupportDirectoryProperty, value); }
    }

    public MultiDragDropTextBox() {
        SetValue(FileNameListPropertyKey, new ExtendedObservableCollection<string>());
        // 更新 FirstFileName
        FileNameList.CollectionChanged += FileNameListCollectionChangedHandler;
        this.SetLoadedOnceEventHandler(static (sender, _) => {
            if (sender is MultiDragDropTextBox textBox) {
                if (textBox.TryFindResource("MultilineTextBoxStyle") is Style style) {
                    textBox.TextBoxStyle ??= style;
                }
                textBox.FileListInfoBorderFadeOutStoryboard = (Storyboard)textBox.Resources[FileListInfoBorderFadeOutStoryboardName];
            }
        });
        InitializeComponent();
    }

    private static void HasFilePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not MultiDragDropTextBox self) {
            return;
        }
        if (e.NewValue is false) {
            self.FileData = null;
            self.FileNameSet.Clear();
            self.FileNameList.Clear();
        }
    }

    private void FileNameListCollectionChangedHandler(object? sender, NotifyCollectionChangedEventArgs e) {
        if (FileNameList.Count > 0) {
            FirstFileName = FileNameList[0];
        }
    }

    /// <summary>
    /// Drop 事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PreviewDropHandler(object sender, DragEventArgs e) {
        FileData = e.Data.GetData(DataFormats.FileDrop);
        if (FileData != null && FileData is ICollection<string> list && list.Count > 0) {
            // 筛选文件
            if (!IsSupportDirectory) {
                list = list.Where(File.Exists).ToList();
            }
            // 重复则不添加
            list.ForEach(f => {
                if (!FileNameSet.Contains(f)) {
                    FileNameSet.Add(f);
                    FileNameList.Add(f);
                }
            });
            // 更新
            HasFile = FileNameList.Count > 0;
            if (HasFile) {
                DragDropEvent?.Invoke(sender, list.ToList());
            }
        }
    }

    /// <summary>
    /// 清空输入
    /// </summary>
    public void Clear() {
        HasFile = false;
        FirstFileName = InputText = string.Empty;
        FileData = null;
        FileNameSet.Clear();
        FileNameList.Clear();
    }

    /// <summary>
    /// 打开文件点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenFileClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender.GetElementDataContext<string>() is string path) {
            path.OpenFileWithAsync();
        }
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender.GetElementDataContext<string>() is string path) {
            path.OpenFileInExplorerAsync();
        }
    }

    /// <summary>
    /// 移除文件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DeleteClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (FileNameListBox.SelectedItems.Count > 0) {
            var files = FileNameListBox.SelectedItems.Cast<string>().ToList();
            for (int i = 0; i < files.Count; i++) {
                var f = files[i];
                FileNameList.Remove(f);
                FileNameSet.Remove(f);
            }
            // 更新
            HasFile = FileNameList.Count > 0;
        }
    }

    /// <summary>
    /// 鼠标移开
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewMouseLeaveHandler(object sender, MouseEventArgs e) {
        e.Handled = true;
        FileListInfoBorderFadeOutStoryboard.Begin();
    }

    /// <summary>
    /// 鼠标移入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewMouseEnterHandler(object sender, MouseEventArgs e) {
        e.Handled = true;
        FileListInfoBorderFadeOutStoryboard.Stop();
        FileListInfoBorder.Opacity = 1;
    }

    private void RootLoadedHandler(object sender, RoutedEventArgs e) {
        DragDropHelper.SetBackgroundProperty(this, BackgroundProperty);
        DragDropHelper.SetIsEnabled(this, true);
        DragDropHelper.SetBackgroundProperty(InputTextBox, BackgroundProperty);
        DragDropHelper.SetIsEnabled(InputTextBox, true);
    }

    private void RootUnloadedHandler(object sender, RoutedEventArgs e) {
        DragDropHelper.Dispose(this);
        DragDropHelper.Dispose(InputTextBox);
    }
}
