using CommonTools.Utils;
using CommonUITools.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CommonUITools.Widget;

public partial class MultiDragDropTextBox : UserControl {
    public static readonly DependencyProperty InputTextProperty = DependencyProperty.Register("InputText", typeof(string), typeof(MultiDragDropTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(MultiDragDropTextBox), new PropertyMetadata());
    public static readonly DependencyProperty HasFileProperty = DependencyProperty.Register("HasFile", typeof(bool), typeof(MultiDragDropTextBox), new PropertyMetadata(false));
    private static readonly DependencyProperty FileNameListProperty = DependencyProperty.Register("FileNameList", typeof(ObservableCollection<string>), typeof(MultiDragDropTextBox), new PropertyMetadata());
    private static readonly DependencyProperty FirstFileNameProperty = DependencyProperty.Register("FirstFileName", typeof(string), typeof(MultiDragDropTextBox), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IsSupportDirectoryProperty = DependencyProperty.Register("IsSupportDirectory", typeof(bool), typeof(MultiDragDropTextBox), new PropertyMetadata(false));

    /// <summary>
    /// DragDropEvent，参数为 本次拖拽的 FileNames
    /// </summary>
    public event EventHandler<IList<string>>? DragDropEvent;

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
    private ObservableCollection<string> FileNameList {
        get { return (ObservableCollection<string>)GetValue(FileNameListProperty); }
        set { SetValue(FileNameListProperty, value); }
    }
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
    private Storyboard? FileListInfoBorderFadeOutStoryboard;

    public MultiDragDropTextBox() {
        FileNameList = new();
        // 更新 FirstFileName
        FileNameList.CollectionChanged += (_, _) => {
            if (FileNameList.Count > 0) {
                FirstFileName = FileNameList[0];
            }
        };
        DependencyPropertyDescriptor
            .FromProperty(HasFileProperty, this.GetType())
            .AddValueChanged(this, (_, _) => {
                if (!HasFile) {
                    FileData = null;
                    FileNameSet.Clear();
                    FileNameList.Clear();
                }
            });
        // 设置 TextBox 默认 Style
        UIUtils.SetLoadedOnceEventHandler(this, (_, _) => {
            if (TryFindResource("MultilineTextBoxStyle") is Style style) {
                TextBoxStyle ??= style;
            }
            FileListInfoBorderFadeOutStoryboard = (Storyboard)Resources["FileListInfoBorderFadeOutStoryboard"];
        });
        InitializeComponent();
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
        InputText = string.Empty;
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
        if (sender is FrameworkElement element && element.DataContext is string path) {
            UIUtils.OpenFileWithAsync(path);
        }
    }

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenDirectoryClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        if (sender is FrameworkElement element && element.DataContext is string path) {
            UIUtils.OpenFileInExplorerAsync(path);
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
        FileListInfoBorderFadeOutStoryboard?.Begin();
    }

    /// <summary>
    /// 鼠标移入
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewMouseEnterHandler(object sender, MouseEventArgs e) {
        FileListInfoBorderFadeOutStoryboard?.Stop();
        FileListInfoBorder.Opacity = 1;
    }
}
