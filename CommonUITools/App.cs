using System.Windows.Documents;

namespace CommonUITools;

internal static class App {
    /// <summary>
    /// 注册 MessageBox 和 NotificationBox 界面
    /// </summary>
    /// <remarks>多次调用无效果</remarks>
    public static void RegisterWidgetPage(Window window) {
        TaskUtils.EnsureCalledOnce($"{typeof(App).GetHashCode()}{RegisterWidgetPage}{window.GetHashCode()}", () => {
            window.SetLoadedOnceEventHandler(static (obj, _) => {
                if (obj is not Window window) {
                    return;
                }
                // 不是 FrameworkElement
                if (window.Content is not FrameworkElement mainContent) {
                    return;
                }
                if (AdornerLayer.GetAdornerLayer(mainContent) is not AdornerLayer adornerLayer) {
                    return;
                }

                adornerLayer.Add(WindowAdorner.CreateWindowAdorner(mainContent, window));
            });
        });
    }
}

internal static class WindowAdorner {
    public static Adorner CreateWindowAdorner(FrameworkElement adornedElement, Window window) {
        var messageBoxPanel = CreateMessageBoxPanel();
        var notificationPanel = CreateNotificationPanel();
        #region 设置 ContentPanel
        Controls.MessageBox.RegisterMessagePanel(window, messageBoxPanel);
        Controls.NotificationBox.RegisterNotificationPanel(window, notificationPanel);
        #endregion
        return new CommonAdorner(adornedElement, new CommonAdorner.ElementInfo[] {
            new (messageBoxPanel , targetBindingElement: window),
            new (notificationPanel ,false, false, targetBindingElement: window)
        });
    }

    /// <summary>
    /// 创建 MessageBoxPanel
    /// </summary>
    /// <returns></returns>
    private static Panel CreateMessageBoxPanel() => new StackPanel() { Margin = new() { Top = 10 }, };

    /// <summary>
    /// 创建 NotificationPanel
    /// </summary>
    /// <returns></returns>
    private static Panel CreateNotificationPanel() {
        return new StackPanel() {
            Margin = new() { Top = 30, Right = 10 },
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
    }
}