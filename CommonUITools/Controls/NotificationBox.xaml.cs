using System.Timers;

namespace CommonUITools.Controls;

public partial class NotificationBox : UserControl {
    private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(string), typeof(NotificationBox), new PropertyMetadata("White"));
    private static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    private static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    private static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    private static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register("ClickCallback", typeof(Action), typeof(NotificationBox), new PropertyMetadata());

    /// <summary>
    /// 默认显示时长
    /// </summary>
    public const uint DefaultDisplayDuration = 3000;
    /// <summary>
    /// 当只有一个窗口时，默认的消息面板
    /// </summary>
    private static UIElementCollection? DefaultWindowPanel;
    /// <summary>
    /// 显示时间 (ms)
    /// </summary>
    private readonly uint DisplayDuration = 4500;
    /// <summary>
    /// 关闭定时器
    /// </summary>
    private readonly System.Timers.Timer UnloadTimer;
    /// <summary>
    /// 窗口对应消息面板
    /// </summary>
    private static readonly IDictionary<Window, UIElementCollection> WindowPanelDict = new Dictionary<Window, UIElementCollection>();

    /// <summary>
    /// Background
    /// </summary>
    private string BoxForeground {
        get { return (string)GetValue(BoxForegroundProperty); }
        set { SetValue(BoxForegroundProperty, value); }
    }
    /// <summary>
    /// 消息
    /// </summary>
    private string Message {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }
    /// <summary>
    /// 标题
    /// </summary>
    private string Title {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    /// <summary>
    /// 图标
    /// </summary>
    private string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    /// <summary>
    /// 点击回调
    /// </summary>
    private Action? ClickCallback {
        get { return (Action?)GetValue(ClickCallbackProperty); }
        set { SetValue(ClickCallbackProperty, value); }
    }

    /// <summary>
    /// 注册消息 Panel
    /// </summary>
    /// <param name="window"></param>
    /// <param name="panel"></param>
    public static void RegisterNotificationPanel(Window window, Panel panel) => WindowPanelDict[window] = panel.Children;

    /// <summary>
    /// 显示通知消息
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">信息</param>
    /// <param name="this">多窗口情况下使用，将调用方作为此参数传入</param>
    /// <param name="displayDuration">显示时长</param>
    /// <param name="messageType">消息类型</param>
    /// <param name="callback">回调</param>
    internal static void ShowNotification(string title, string message, DependencyObject? @this = null, uint displayDuration = DefaultDisplayDuration, MessageType messageType = MessageType.Info, Action? callback = null) {
        if (WindowPanelDict.Count == 0) {
            return;
        }
        // 单窗口
        if (WindowPanelDict.Count == 1) {
            DefaultWindowPanel ??= WindowPanelDict.First().Value;
            UIUtils.RunOnUIThread(() => {
                DefaultWindowPanel.Add(new NotificationBox(title, message, displayDuration, messageType, callback));
            });
            return;
        }
        // 多窗口
        if (Window.GetWindow(@this) is Window window) {
            if (!WindowPanelDict.TryGetValue(window, out var panel)) {
                return;
            }
            UIUtils.RunOnUIThread(() => {
                panel.Add(new NotificationBox(title, message, displayDuration, messageType, callback));
            });
        }
    }

    private NotificationBox(string title, string message, uint displayDuration = DefaultDisplayDuration, MessageType messageType = MessageType.Info, Action? callback = null) {
        Message = message;
        Title = title;
        ClickCallback = callback;
        Icon = ConstantCollections.MessageInfoDict[messageType].Icon;
        BoxForeground = ConstantCollections.MessageInfoDict[messageType].Foreground;
        DisplayDuration = displayDuration;
        InitializeComponent();
        UnloadTimer = new(DisplayDuration) { AutoReset = false };
        UnloadTimer.Elapsed += RootUnLoad;
        UnloadTimer.Start();
    }

    /// <summary>
    /// 消失时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootUnLoad(object? sender, ElapsedEventArgs e) => Dispatcher.Invoke(UnLoadNotificationBox);

    /// <summary>
    /// 关闭通知
    /// </summary>
    private void UnLoadNotificationBox() {
        UnloadTimer.Dispose();
        if (Resources["UnLoadStoryboard"] is not Storyboard unLoadStoryboard) {
            return;
        }

        var timeline = unLoadStoryboard.Children.FirstOrDefault(t => t.Name == "UnLoadHeightAnimation");
        if (timeline is DoubleAnimation heightAnimation) {
            heightAnimation.From = ActualHeight;
        }
        unLoadStoryboard.Completed += (s, e) => {
            Visibility = Visibility.Collapsed;
            var window = Window.GetWindow(this);
            // Cannot get window
            if (window is null) {
                return;
            }
            if (WindowPanelDict.TryGetValue(window, out var panel)) {
                panel.Remove(this);
            }
        };
        unLoadStoryboard.Begin();
    }

    /// <summary>
    /// 显示时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ViewLoadedHandler(object sender, RoutedEventArgs e) {
        if (Parent is not FrameworkElement parent) {
            return;
        }
        if (Resources["LoadStoryboard"] is not Storyboard loadStoryboard) {
            return;
        }

        var timeline = loadStoryboard.Children.FirstOrDefault(t => t.Name == "LoadHeightAnimation");
        if (timeline is DoubleAnimation heightAnimation) {
            heightAnimation.To = ActualHeight;
        }
        loadStoryboard.Begin();
    }

    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ActionTextBlockMouseUp(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
        ClickCallback?.Invoke();
        UnLoadNotificationBox();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseClickHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        UnLoadNotificationBox();
    }
}
