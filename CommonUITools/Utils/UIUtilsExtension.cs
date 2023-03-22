using ModernWpf.Controls;
using System.Windows.Media.Imaging;

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
    public static bool IsRightButtonDown(this MouseButtonEventArgs e) => UIUtils.IsRightButtonDown(e);

    /// <summary>
    /// 鼠标左键是否按下
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsLeftButtonDown(this MouseEventArgs e) => e.LeftButton == MouseButtonState.Pressed;

    /// <inheritdoc cref="UIUtils.StringToBrush(string)"/>
    public static SolidColorBrush ToBrush(this string color) => UIUtils.StringToBrush(color);

    /// <inheritdoc cref="UIUtils.StringToColor(string)"/>
    public static Color ToColor(this string color) => UIUtils.StringToColor(color);

    /// <inheritdoc cref="UIUtils.DrawingColorToColor(System.Drawing.Color)"/>
    public static Color ToColor(this System.Drawing.Color color) => UIUtils.DrawingColorToColor(color);

    /// <inheritdoc cref="UIUtils.OpenFileInExplorerAsync(string, Action{Exception}?)"/>
    public static void OpenFileInExplorerAsync(this string path, Action<Exception>? failedCallback = null) {
        UIUtils.OpenFileInExplorerAsync(path, failedCallback);
    }

    /// <inheritdoc cref="UIUtils.OpenFileInExistingExplorerAsync(string, Action{Exception}?)"/>
    public static void OpenFileInExistingExplorerAsync(this string path, Action<Exception>? failedCallback = null) {
        UIUtils.OpenFileInExistingExplorerAsync(path, failedCallback);
    }

    /// <inheritdoc cref="UIUtils.OpenFileWithAsync(string, Action{Exception}?)"/>
    public static void OpenFileWithAsync(this string path, Action<Exception>? failedCallback = null) {
        UIUtils.OpenFileWithAsync(path, failedCallback);
    }

    /// <inheritdoc cref="UIUtils.OpenInBrowser(string)"/>
    public static void OpenInBrowser(this string url) => UIUtils.OpenInBrowser(url);

    /// <inheritdoc cref="UIUtils.ReversePanelChildrenOrder(Panel)"/>
    public static void ReverseChildrenOrder(this Panel panel) => UIUtils.ReversePanelChildrenOrder(panel);

    /// <inheritdoc cref="UIUtils.CopyImageSource(string)"/>
    public static BitmapImage GetImageSource(this string filepath) => UIUtils.CopyImageSource(filepath);

    /// <summary>
    /// 获取资源文件 BitmapImage
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static BitmapImage GetApplicationResourceBitmapImage(this Uri uri) {
        var stream = Application.GetResourceStream(uri).Stream;
        stream.Position = 0;
        var bi = new BitmapImage();
        bi.BeginInit();
        bi.StreamSource = stream;
        bi.EndInit();
        return bi;
    }

    /// <inheritdoc cref="UIUtils.BitmapSourceToBitmap(BitmapSource)"/>
    public static System.Drawing.Bitmap ToBitmap(this BitmapSource bitmapSource) => UIUtils.BitmapSourceToBitmap(bitmapSource);

    /// <inheritdoc cref="UIUtils.BitmapToImageSource(System.Drawing.Bitmap)"/>
    public static ImageSource ToImageSource(this System.Drawing.Bitmap bitmap) => UIUtils.BitmapToImageSource(bitmap);

    /// <inheritdoc cref="UIUtils.BitmapToStream(System.Drawing.Bitmap)"/>
    public static Stream ToStream(this System.Drawing.Bitmap bitmap) => UIUtils.BitmapToStream(bitmap);

    /// <inheritdoc cref="UIUtils.FindResourceInMergedDictionaries(Collection{ResourceDictionary}, string)"/>
    public static ResourceDictionary? FindResource(this Collection<ResourceDictionary> mergedDictionaries, string sourcePath) {
        return UIUtils.FindResourceInMergedDictionaries(mergedDictionaries, sourcePath);
    }

    /// <inheritdoc cref="UIUtils.ReplaceResourceDictionary(Collection{ResourceDictionary}, string, string)"/>
    public static bool ReplaceResourceDictionary(this Collection<ResourceDictionary> mergedDictionaries, string oldSource, string newSource) {
        return UIUtils.ReplaceResourceDictionary(mergedDictionaries, oldSource, newSource);
    }

    /// <inheritdoc cref="UIUtils.GetElementDataContext{T}(object)"/>
    public static T? GetElementDataContext<T>(this object sender) => UIUtils.GetElementDataContext<T>(sender);

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
