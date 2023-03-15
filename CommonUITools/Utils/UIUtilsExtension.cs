using ModernWpf.Controls;

namespace CommonUITools.Utils;

/// <summary>
/// Extention for UIUtils
/// </summary>
public static class UIUtilsExtension {
    /// <summary>
    /// ContentDialogResizeRatio
    /// </summary>
    public const double ContentDialogResizeRatio = 2;

    /// <inheritdoc cref="UIUtils.IsLeftButtonDown(MouseButtonEventArgs)"/>
    public static bool IsLeftButtonDown(this MouseButtonEventArgs e) => UIUtils.IsLeftButtonDown(e);

    /// <inheritdoc cref="UIUtils.IsRightButtonDown(MouseButtonEventArgs)"/>
    public static bool IsRightButtonDown(MouseButtonEventArgs e) => UIUtils.IsRightButtonDown(e);

    /// <summary>
    /// 鼠标左键是否按下
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsLeftButtonDown(this MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed;

    /// <summary>
    /// 鼠标右键是否按下
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsRightButtonDown(this MouseEventArgs e) => e.RightButton == MouseButtonState.Pressed;

    /// <summary>
    /// 启用 ContentDialog 自动调整大小
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="minWidth"></param>
    /// <param name="maxWidth"></param>
    /// <param name="resizeRatio">NewWidth = Window.ActualWidth / <paramref name="resizeRatio"/></param>
    public static void EnableContentDialogAutoResize(
        this ContentDialog dialog,
        double minWidth,
        double maxWidth = double.MaxValue,
        double resizeRatio = ContentDialogResizeRatio
    ) {
        UIUtils.SetLoadedOnceEventHandler(dialog, (_, _) => {
            var window = Window.GetWindow(dialog);
            var dialogDebounceId = new object();
            // Update now
            UpdateDialogWidth(dialog, window.ActualWidth / resizeRatio, minWidth, maxWidth);
            window.SizeChanged += (_, arg) => DebounceUtils.Debounce(dialogDebounceId, () => {
                // Reduce update frequency
                dialog.Dispatcher.Invoke(() => {
                    UpdateDialogWidth(dialog, arg.NewSize.Width / resizeRatio, minWidth, maxWidth);
                });
            }, true, 100);
        });
        // Update DialogContent Width
        static void UpdateDialogWidth(
            ContentDialog dialog,
            double newWidth,
            double minWidth,
            double maxWidth
        ) {
            if (newWidth < minWidth) {
                newWidth = minWidth;
            } else if (newWidth > maxWidth) {
                newWidth = maxWidth;
            }
            if (dialog.Content is FrameworkElement element) {
                element.Width = newWidth;
            }
        }
    }

    /// <inheritdoc cref="UIUtils.SetLoadedOnceEventHandler(FrameworkContentElement, RoutedEventHandler)"/>
    public static void SetLoadedOnceEventHandler(this FrameworkContentElement element, RoutedEventHandler handler) {
        UIUtils.SetLoadedOnceEventHandler(element, handler);
    }

    /// <inheritdoc cref="UIUtils.SetLoadedOnceEventHandler(FrameworkElement, RoutedEventHandler)"/>
    public static void SetLoadedOnceEventHandler(this FrameworkElement element, RoutedEventHandler handler) {
        UIUtils.SetLoadedOnceEventHandler(element, handler);
    }
}
