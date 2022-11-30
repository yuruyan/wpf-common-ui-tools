using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CommonUITools.Widget;

public partial class NotificationBox : UserControl {
    private static readonly DependencyProperty BoxForegroundProperty = DependencyProperty.Register("BoxForeground", typeof(string), typeof(NotificationBox), new PropertyMetadata("White"));
    public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(NotificationBox), new PropertyMetadata(""));
    public static readonly DependencyProperty ClickCallbackProperty = DependencyProperty.Register("ClickCallback", typeof(Action), typeof(NotificationBox), new PropertyMetadata());

    /// <summary>
    /// 用于添加 NotificationBox
    /// </summary>
    private static UIElementCollection? PanelChildren;
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
    public string Message {
        get { return (string)GetValue(MessageProperty); }
        set { SetValue(MessageProperty, value); }
    }
    /// <summary>
    /// 标题
    /// </summary>
    public string Title {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    /// <summary>
    /// 显示时间 (ms)
    /// </summary>
    public int ShowingDuration { get; set; } = 4500;
    /// <summary>
    /// 关闭定时器
    /// </summary>
    private readonly System.Timers.Timer UnloadTimer;
    /// <summary>
    /// 点击回调
    /// </summary>
    public Action? ClickCallback {
        get { return (Action?)GetValue(ClickCallbackProperty); }
        set { SetValue(ClickCallbackProperty, value); }
    }

    /// <summary>
    /// 设置内容 Panel
    /// </summary>
    /// <param name="contentPanel"></param>
    public static void SetContentPanel(Panel contentPanel) => PanelChildren = contentPanel.Children;

    public static void ShowNotification(string title, string message, MessageType messageType = MessageType.Info, Action? callback = null) {
        // 检查权限
        if (Application.Current.Dispatcher.CheckAccess()) {
            PanelChildren?.Add(new NotificationBox(title, message, messageType, callback));
        } else {
            Application.Current.Dispatcher.Invoke(() => {
                PanelChildren?.Add(new NotificationBox(title, message, messageType, callback));
            });
        }
    }

    public static void Info(string title, string message, Action? callback = null)
       => ShowNotification(title, message, MessageType.Info, callback);

    public static void Warning(string title, string message, Action? callback = null)
       => ShowNotification(title, message, MessageType.Warning, callback);

    public static void Success(string title, string message, Action? callback = null)
       => ShowNotification(title, message, MessageType.Success, callback);

    public static void Error(string title, string message, Action? callback = null)
       => ShowNotification(title, message, MessageType.Error, callback);

    public NotificationBox(string title, string message, MessageType messageType = MessageType.Info, Action? callback = null) {
        Message = message;
        Title = title;
        ClickCallback = callback;
        Icon = WidgetGlobal.MessageInfoDict[messageType].Icon;
        BoxForeground = WidgetGlobal.MessageInfoDict[messageType].Foreground;
        InitializeComponent();
        UnloadTimer = new(ShowingDuration) { AutoReset = false };
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
        if (Resources["UnLoadStoryboard"] is not Storyboard unLoadStoryboard) {
            return;
        }

        var timeline = unLoadStoryboard.Children.FirstOrDefault(t => t.Name == "UnLoadHeightAnimation");
        if (timeline is DoubleAnimation heightAnimation) {
            heightAnimation.From = ActualHeight;
        }
        unLoadStoryboard.Completed += (s, e) => {
            Visibility = Visibility.Collapsed;
            PanelChildren?.Remove(this);
        };
        unLoadStoryboard.Begin();
        UnloadTimer.Dispose();
    }

    /// <summary>
    /// 显示时触发
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootLoaded(object sender, RoutedEventArgs e) {
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
    /// 关闭通知
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseMouseUp(object sender, MouseButtonEventArgs e) => UnLoadNotificationBox();

    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ActionTextBlockMouseUp(object sender, MouseButtonEventArgs e) => ClickCallback?.Invoke();
}
