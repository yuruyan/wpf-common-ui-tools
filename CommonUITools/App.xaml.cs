using CommonUITools.Utils;
using NLog;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace CommonUITools;

public partial class App : Application {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 注册 MessageBox 和 NotificationBox 界面
    /// 多次调用无效果
    /// </summary>
    public static void RegisterWidgetPage(Window window)
        => window.Loaded += (_, _) => RegisterWidgetPageInternal(window);

    private static void RegisterWidgetPageInternal(Window window) {
        CommonUtils.EnsureCalledOnce(window, () => {
            // 不是 UIElement
            if (window.Content is not UIElement mainContent) {
                Logger.Error("Window Content is not of type UIElement");
                return;
            }
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(mainContent);
            if (adornerLayer != null) {
                adornerLayer.Add(new WindowAdorner(mainContent, window));
            } else {
                Logger.Error("Window Content has no adornerLayer");
            }
        });
    }
}

internal class WindowAdorner : Adorner {
    private readonly Panel MessageBoxPanel;
    private readonly Panel NotificationPanel;
    private readonly VisualCollection Children;

    protected override int VisualChildrenCount => Children.Count;

    /// <summary>
    /// 绑定 width、height
    /// </summary>
    /// <param name="window"></param>
    /// <param name="panel"></param>
    /// <param name="bindWidth"></param>
    /// <param name="bindHeight"></param>
    private void BindWindowSize(Window window, Panel panel, bool bindWidth = true, bool bindHeight = true) {
        if (bindWidth) {
            var widthBinding = new Binding("ActualWidth") { Source = window, };
            panel.SetBinding(StackPanel.WidthProperty, widthBinding);
        }
        if (bindHeight) {
            var heightBinding = new Binding("ActualHeight") { Source = window, };
            panel.SetBinding(StackPanel.HeightProperty, heightBinding);
        }
    }

    /// <summary>
    /// 创建 MessageBoxPanel
    /// </summary>
    /// <returns></returns>
    private Panel CreateMessageBoxPanel(Window window) {
        var panel = new StackPanel() { Margin = new() { Top = 10 }, };
        BindWindowSize(window, panel);
        return panel;
    }

    /// <summary>
    /// 创建 NotificationPanel
    /// </summary>
    /// <param name="window"></param>
    /// <returns></returns>
    private Panel CreateNotificationPanel(Window window) {
        var panel = new StackPanel() {
            Margin = new() { Top = 30, Right = 10 },
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
        };
        return panel;
    }

    public WindowAdorner(UIElement adornedElement, Window window) : base(adornedElement) {
        MessageBoxPanel = CreateMessageBoxPanel(window);
        NotificationPanel = CreateNotificationPanel(window);
        // 初始化面板
        Children = new(this) {
            MessageBoxPanel,
            NotificationPanel
        };
        Widget.MessageBox.SetContentPanel(MessageBoxPanel);
        Widget.NotificationBox.SetContentPanel(NotificationPanel);
    }

    protected override Visual GetVisualChild(int index) => Children[index];

    protected override Size ArrangeOverride(Size finalSize) {
        MessageBoxPanel.Arrange(new(finalSize));
        NotificationPanel.Arrange(new(finalSize));
        return base.ArrangeOverride(finalSize);
    }
}