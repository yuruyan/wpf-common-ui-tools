﻿using System.Windows.Documents;

namespace CommonUITools;

internal static class App {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly object UniqueObject = new();

    /// <summary>
    /// 注册 MessageBox 和 NotificationBox 界面
    /// </summary>
    /// <remarks>多次调用无效果</remarks>
    public static void RegisterWidgetPage(Window window) {
        TaskUtils.EnsureCalledOnce((window, UniqueObject), () => {
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
        var messageBoxPanel = CreateMessageBoxPanel();
        var notificationPanel = CreateNotificationPanel();
        #region 设置 ContentPanel
        Widget.MessageBox.RegisterMessagePanel(window, messageBoxPanel);
        Widget.NotificationBox.RegisterNotificationPanel(window, notificationPanel);
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