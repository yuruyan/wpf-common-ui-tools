using CommonUITools.Utils;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CommonUITools;

public partial class App {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 注册 MessageBox 和 NotificationBox 界面
    /// 多次调用无效果
    /// </summary>
    public static void RegisterWidgetPage(Window window) {
        TaskUtils.EnsureCalledOnce((window, typeof(App)), () => {
            UIUtils.SetLoadedOnceEventHandler(window, static (obj, _) => {
                if (obj is not Window window) {
                    return;
                }
                // 不是 FrameworkElement
                if (window.Content is not FrameworkElement mainContent) {
                    Logger.Error("Window Content is not of type FrameworkElement");
                    return;
                }
                if (AdornerLayer.GetAdornerLayer(mainContent) is not AdornerLayer adornerLayer) {
                    Logger.Error("Window Content has no adornerLayer");
                    return;
                }

                adornerLayer.Add(WindowAdorner.CreateWindowAdorner(mainContent, window));
            });
        });
    }
}

internal static class WindowAdorner {
    public static Adorner CreateWindowAdorner(FrameworkElement adornedElement, Window window) {
        var messageBoxPanel = CreateMessageBoxPanel(window);
        var notificationPanel = CreateNotificationPanel();
        #region 设置 ContentPanel
        Widget.MessageBox.SetContentPanel(messageBoxPanel);
        Widget.NotificationBox.SetContentPanel(notificationPanel);
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
    private static Panel CreateMessageBoxPanel(Window window) => new StackPanel() { Margin = new() { Top = 10 }, };

    /// <summary>
    /// 创建 NotificationPanel
    /// </summary>
    /// <returns></returns>
    private static Panel CreateNotificationPanel()
        => new StackPanel() {
            Margin = new() { Top = 30, Right = 10 },
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
}