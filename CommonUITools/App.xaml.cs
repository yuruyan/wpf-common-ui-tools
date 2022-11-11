using CommonUITools.Converter;
using CommonUITools.Utils;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CommonUITools;

public partial class App : Application {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 注册 MessageBox 和 NotificationBox 界面
    /// 需在 MainWindow 中调用
    /// </summary>
    public static void RegisterWidgetPage(Window window) {
        CommonUtils.EnsureCalledOnce(RegisterWidgetPage, () => {
            var grid = new Grid();
            var messageBoxPanel = CreateMessageBoxPanel();
            var notificationPanel = CreateNotificationPanel(window);
            #region 添加组件
            if (window.Content is UIElement mainContent) {
                // 需要先清除
                window.ClearValue(Window.ContentProperty);
                grid.Children.Add(mainContent);
            } else {
                Logger.Error("Window Content is not of type UIElement");
            }
            grid.Children.Add(messageBoxPanel);
            grid.Children.Add(notificationPanel);
            #endregion
            window.Content = grid;
            // 初始化面板
            Widget.MessageBox.SetContentPanel(messageBoxPanel);
            Widget.NotificationBox.SetContentPanel(notificationPanel);
        });
    }

    /// <summary>
    /// 创建 MessageBoxPanel
    /// </summary>
    /// <returns></returns>
    private static Panel CreateMessageBoxPanel() {
        var panel = new StackPanel() {
            Margin = new(0, 10, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Top,
        };
        Panel.SetZIndex(panel, 10000);
        return panel;
    }

    /// <summary>
    /// 创建 NotificationPanel
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    private static Panel CreateNotificationPanel(Window window) {
        var panel = new StackPanel() {
            Margin = new(0, 30, 10, 0),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        var widthBinding = new Binding("ActualWidth") {
            Source = window,
            Converter = (window.TryFindResource("DivideTwoConverter") as IValueConverter) ?? new DivideTwoConverter()
        };
        panel.SetBinding(StackPanel.MaxWidthProperty, widthBinding);
        Panel.SetZIndex(panel, 10000);
        return panel;
    }
}