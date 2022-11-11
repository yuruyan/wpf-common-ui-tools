using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CommonUITools.Widget;

/// <summary>
/// 无论是否在 ui 线程，都可以调用静态方法
/// </summary>
public partial class MessageBox : UserControl {

    private static readonly DependencyProperty BoxBackgroundProperty = DependencyProperty.Register("BoxBackground", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    private static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    public static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register("MessageType", typeof(MessageType), typeof(MessageBox), new PropertyMetadata(MessageType.INFO));

    /// <summary>
    /// 用于添加 MessageBox
    /// </summary>
    private static UIElementCollection? PanelChildren;
    public string Text {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    /// <summary>
    /// Background
    /// </summary>
    private string BoxBackground {
        get { return (string)GetValue(BoxBackgroundProperty); }
        set { SetValue(BoxBackgroundProperty, value); }
    }
    /// <summary>
    /// Foreground
    /// </summary>
    private string BoxForeground {
        get { return (string)GetValue(BoxForegroundProperty); }
        set { SetValue(BoxForegroundProperty, value); }
    }
    /// <summary>
    /// BorderColor
    /// </summary>
    private string BorderColor {
        get { return (string)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }
    /// <summary>
    /// Icon
    /// </summary>
    private string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    /// <summary>
    /// MessageType
    /// </summary>
    public MessageType MessageType {
        get { return (MessageType)GetValue(MessageTypeProperty); }
        set { SetValue(MessageTypeProperty, value); }
    }
    /// <summary>
    /// 关闭定时器
    /// </summary>
    private readonly System.Timers.Timer UnloadTimer;
    /// <summary>
    /// 显示时间 (ms)
    /// </summary>
    public int ShowingDuration { get; set; } = 3000;

    /// <summary>
    /// 设置内容 Panel
    /// </summary>
    /// <param name="contentPanel"></param>
    public static void SetContentPanel(Panel contentPanel) => PanelChildren = contentPanel.Children;

    public static void Info(string message) => ShowMessage(message, MessageType.INFO);

    public static void Waring(string message) => ShowMessage(message, MessageType.WARNING);

    public static void Success(string message) => ShowMessage(message, MessageType.SUCCESS);

    public static void Error(string message) => ShowMessage(message, MessageType.ERROR);

    private static void ShowMessage(string message, MessageType type = MessageType.INFO) {
        // 检查权限
        if (App.Current.Dispatcher.CheckAccess()) {
            PanelChildren?.Add(new MessageBox(message, type));
        } else {
            App.Current.Dispatcher.Invoke(() => PanelChildren?.Add(new MessageBox(message, type)));
        }
    }

    public MessageBox(string message, MessageType messageType = MessageType.INFO) {
        Text = message;
        MessageType = messageType;
        BoxBackground = WidgetGlobal.MessageInfoDict[MessageType].Background;
        BoxForeground = WidgetGlobal.MessageInfoDict[MessageType].Foreground;
        BorderColor = WidgetGlobal.MessageInfoDict[MessageType].BorderColor;
        Icon = WidgetGlobal.MessageInfoDict[MessageType].Icon;
        InitializeComponent();
        UnloadTimer = new(ShowingDuration) { AutoReset = false };
        UnloadTimer.Elapsed += RootUnLoad;
        UnloadTimer.Start();
    }

    /// <summary>
    /// 卸载 MessageBox
    /// </summary>
    private void UnloadMessageBox() {
        if (Resources["UnLoadStoryboard"] is not Storyboard unLoadStoryboard) {
            return;
        }

        var timeline = unLoadStoryboard.Children.FirstOrDefault(t => t.Name == "HeightAnimation");
        if (timeline is DoubleAnimation heightAnimation) {
            heightAnimation.From = ActualHeight;
        }
        // 移除
        unLoadStoryboard.Completed += (s, e) => {
            Visibility = Visibility.Collapsed;
            PanelChildren?.Remove(this);
        };
        UnloadTimer.Dispose();
        unLoadStoryboard.Begin();
    }

    /// <summary>
    /// 消失时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootUnLoad(object? sender, ElapsedEventArgs e) => Dispatcher.Invoke(UnloadMessageBox);

    /// <summary>
    /// 显示时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootLoaded(object sender, RoutedEventArgs e) {
        if (Parent is not FrameworkElement) {
            return;
        }
        if (Resources["LoadStoryboard"] is not Storyboard loadStoryboard) {
            return;
        }

        loadStoryboard.Begin();
    }
}
