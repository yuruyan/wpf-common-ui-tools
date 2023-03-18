using System.Timers;

namespace CommonUITools.Widget;

public partial class MessageBox : UserControl {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly DependencyProperty BoxBackgroundProperty = DependencyProperty.Register("BoxBackground", typeof(SolidColorBrush), typeof(MessageBox), new PropertyMetadata());
    private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(SolidColorBrush), typeof(MessageBox), new PropertyMetadata());
    private static readonly DependencyProperty BorderColorProperty = DependencyProperty.Register("BorderColor", typeof(SolidColorBrush), typeof(MessageBox), new PropertyMetadata());
    private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    private static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MessageBox), new PropertyMetadata(""));
    private static readonly DependencyProperty MessageTypeProperty = DependencyProperty.Register("MessageType", typeof(MessageType), typeof(MessageBox), new PropertyMetadata(MessageType.Info));

    /// <summary>
    /// 默认显示时长
    /// </summary>
    public const uint DefaultDisplayDuration = 3000;
    /// <summary>
    /// 窗口对应消息面板
    /// </summary>
    private static readonly IDictionary<Window, UIElementCollection> WindowPanelDict = new Dictionary<Window, UIElementCollection>();
    /// <summary>
    /// 关闭定时器
    /// </summary>
    private readonly System.Timers.Timer UnloadTimer;
    /// <summary>
    /// 显示时间 (ms)
    /// </summary>
    private readonly uint DisplayDuration = 3000;
    /// <summary>
    /// 当只有一个窗口时，默认的消息面板
    /// </summary>
    private static UIElementCollection? DefaultWindowPanel;

    public string Text {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    /// <summary>
    /// Background
    /// </summary>
    private SolidColorBrush BoxBackground {
        get { return (SolidColorBrush)GetValue(BoxBackgroundProperty); }
        set { SetValue(BoxBackgroundProperty, value); }
    }
    /// <summary>
    /// Foreground
    /// </summary>
    private SolidColorBrush BoxForeground {
        get { return (SolidColorBrush)GetValue(BoxForegroundProperty); }
        set { SetValue(BoxForegroundProperty, value); }
    }
    /// <summary>
    /// BorderColor
    /// </summary>
    private SolidColorBrush BorderColor {
        get { return (SolidColorBrush)GetValue(BorderColorProperty); }
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
    private MessageType MessageType {
        get { return (MessageType)GetValue(MessageTypeProperty); }
        set { SetValue(MessageTypeProperty, value); }
    }

    /// <summary>
    /// 注册消息 Panel
    /// </summary>
    /// <param name="window"></param>
    /// <param name="panel"></param>
    public static void RegisterMessagePanel(Window window, Panel panel) => WindowPanelDict[window] = panel.Children;

    /// <summary>
    /// ShowMessage
    /// </summary>
    /// <param name="message">消息</param>
    /// <param name="this">多窗口情况下使用，将调用方作为此参数传入</param>
    /// <param name="displayDuration">显示时长</param>
    /// <param name="type">消息类型</param>
    internal static void ShowMessage(string message, DependencyObject? @this = null, uint displayDuration = DefaultDisplayDuration, MessageType type = MessageType.Info) {
        if (WindowPanelDict.Count == 0) {
            Logger.Error("No window has registered MessagePanel");
            return;
        }
        // 单窗口
        if (WindowPanelDict.Count == 1) {
            DefaultWindowPanel ??= WindowPanelDict.First().Value;
            UIUtils.RunOnUIThread(() => {
                DefaultWindowPanel.Add(new MessageBox(message, displayDuration, type));
            });
            return;
        }
        // 多窗口
        if (Window.GetWindow(@this) is Window window) {
            if (!WindowPanelDict.TryGetValue(window, out var panel)) {
                Logger.Error("No MessagePanel has registered with this window");
                return;
            }
            UIUtils.RunOnUIThread(() => {
                panel.Add(new MessageBox(message, displayDuration, type));
            });
        }
    }

    private MessageBox(string message, uint displayDuration = DefaultDisplayDuration, MessageType messageType = MessageType.Info) {
        Text = message;
        MessageType = messageType;
        Icon = ConstantCollections.MessageInfoDict[MessageType].Icon;
        InitializeComponent();
        DisplayDuration = displayDuration;
        UnloadTimer = new(DisplayDuration) { AutoReset = false };
        UnloadTimer.Elapsed += RootUnLoad;
        UnloadTimer.Start();
    }

    /// <summary>
    /// 卸载 MessageBox
    /// </summary>
    private void UnloadMessageBox() {
        UnloadTimer.Dispose();
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
            WindowPanelDict[Window.GetWindow(this)].Remove(this);
        };
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
    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        if (Parent is not FrameworkElement) {
            return;
        }
        if (Resources["LoadStoryboard"] is not Storyboard loadStoryboard) {
            return;
        }
        UpdateBrush();
        loadStoryboard.Begin();
    }

    /// <summary>
    /// 更新 Color
    /// </summary>
    private void UpdateBrush() {
        if (TryFindResource($"MessageBox{MessageType}BackgroundBrush") is SolidColorBrush bgb) {
            BoxBackground = bgb;
        }
        if (TryFindResource($"MessageBox{MessageType}ForegroundBrush") is SolidColorBrush fgb) {
            BoxForeground = fgb;
        }
        if (TryFindResource($"MessageBox{MessageType}BorderBrush") is SolidColorBrush borderBrush) {
            BorderColor = borderBrush;
        }
    }
}
