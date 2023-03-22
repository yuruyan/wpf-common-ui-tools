using ModernWpf.Controls;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using SysDraw = System.Drawing;
using SysDrawBitmap = System.Drawing.Bitmap;
using SysDrawImg = System.Drawing.Imaging;

namespace CommonUITools.Utils;

/// <summary>
/// Extention for UIUtils
/// </summary>
public static partial class UIUtilsExtension {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 左键是否单击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsLeftButtonDown(this MouseButtonEventArgs e) {
        return e.LeftButton == MouseButtonState.Pressed;
    }

    /// <summary>
    /// 右键是否单击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsRightButtonDown(this MouseButtonEventArgs e) {
        return e.RightButton == MouseButtonState.Pressed;
    }

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
    /// <see cref="string"/> 转 <see cref="SolidColorBrush"/>
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SolidColorBrush ToBrush(this string color) {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }

    /// <summary>
    /// <see cref="string"/> 转 <see cref="Color"/>
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ToColor(this string color) {
        return color.ToBrush().Color;
    }

    /// <summary>
    /// <see cref="System.Drawing.Color"/> 转 <see cref="System.Windows.Media.Color"/>
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ToColor(this SysDraw.Color color) {
        return Color.FromArgb(color.A, color.R, color.G, color.B);
    }

    /// <summary>
    /// 在文件资源管理器中异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileInExplorerAsync(this string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("explorer.exe", "/select," + path.ReplaceSlashWithBackSlash());
        } catch (Exception error) {
            if (failedCallback != null) {
                failedCallback(error);
                return;
            }
            MessageBoxUtils.Error("打开失败," + error.Message);
            Logger.Info(error);
        }
    }

    /// <summary>
    /// 在文件资源管理器中异步打开文件，如果存在路径相同的窗口，则不会新建窗口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="failedCallback"></param>
    public static void OpenFileInExistingExplorerAsync(this string path, Action<Exception>? failedCallback = null) {
        try {
            PInvokeUtils.ShellExecuteW(
                IntPtr.Zero,
                "open",
                path.ReplaceSlashWithBackSlash(),
                string.Empty,
                string.Empty,
                ShowCommands.SW_SHOWNORMAL
            );
        } catch (Exception error) {
            if (failedCallback != null) {
                failedCallback(error);
                return;
            }
            MessageBoxUtils.Error("打开失败," + error.Message);
            Logger.Info(error);
        }
    }

    /// <summary>
    /// 选择以什么方式异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileWithAsync(this string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("rundll32.exe", $"shell32.dll, OpenAs_RunDLL {path.ReplaceSlashWithBackSlash()}");
        } catch (Exception error) {
            if (failedCallback != null) {
                failedCallback(error);
                return;
            }
            MessageBoxUtils.Error("打开失败," + error.Message);
            Logger.Info(error);
        }
    }

    /// <summary>
    /// 通知打开文件
    /// </summary>
    /// <param name="filepath"></param>
    /// <param name="title"></param>
    /// <param name="message"></param>
    public static void NotificationOpenFileInExplorerAsync(string filepath, string title = "成功", string message = "点击打开") {
        MessageBoxUtils.NotifySuccess(
            title,
            message,
            callback: () => filepath.OpenFileInExplorerAsync()
        );
    }

    /// <summary>
    /// 在浏览器中打开
    /// </summary>
    /// <param name="url"></param>
    public static void OpenInBrowser(this string url) {
        Process.Start(new ProcessStartInfo() {
            UseShellExecute = true,
            FileName = url,
        });
    }

    /// <summary>
    /// 反转 Panel Children 顺序
    /// </summary>
    /// <param name="panel"></param>
    public static void ReverseChildrenOrder(this Panel panel) {
        var children = panel.Children.Cast(o => o as UIElement).Reverse();
        panel.Children.Clear();
        children.ForEach(item => panel.Children.Add(item));
    }

    /// <summary>
    /// 获取 <see cref="ImageSource"/>
    /// </summary>
    /// <param name="filepath">图像路径</param>
    /// <returns></returns>
    public static BitmapImage GetImageSource(this string filepath) {
        var bi = new BitmapImage();
        using var bitmap = new SysDrawBitmap(filepath);
        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, SysDrawImg.ImageFormat.Png);
        memoryStream.Position = 0;
        bi.BeginInit();
        bi.StreamSource = memoryStream;
        bi.EndInit();
        return bi;
    }

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

    /// <summary>
    /// <see cref="BitmapSource"/> 转为 <see cref="SysDrawBitmap"/>
    /// </summary>
    /// <param name="bitmapSource"></param>
    /// <returns></returns>
    public static SysDrawBitmap ToBitmap(this BitmapSource bitmapSource) {
        SysDrawBitmap bitmap = new(
            bitmapSource.PixelWidth,
            bitmapSource.PixelHeight,
            SysDrawImg.PixelFormat.Format32bppArgb
        );
        SysDrawImg.BitmapData data = bitmap.LockBits(
            new SysDraw.Rectangle(SysDraw.Point.Empty, bitmap.Size),
            SysDrawImg.ImageLockMode.WriteOnly,
            SysDrawImg.PixelFormat.Format32bppArgb
        );
        bitmapSource.CopyPixels(
            Int32Rect.Empty,
            data.Scan0,
            data.Height * data.Stride,
            data.Stride
        );
        bitmap.UnlockBits(data);
        return bitmap;
    }

    /// <summary>
    /// <see cref="SysDrawBitmap"/> 转为 <see cref="ImageSource"/>
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static ImageSource ToImageSource(this SysDrawBitmap bitmap) {
        IntPtr intPtr = bitmap.GetHbitmap();
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            intPtr,
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()
        );
    }

    /// <summary>
    /// <see cref="SysDrawBitmap"/> 转 <see cref="Stream"/>
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static Stream ToStream(this SysDrawBitmap bitmap) {
        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, SysDrawImg.ImageFormat.Png);
        memoryStream.Position = 0;
        return memoryStream;
    }

    /// <summary>
    /// 在 MergedDictionaries 中查找
    /// </summary>
    /// <param name="mergedDictionaries"></param>
    /// <param name="sourcePath"></param>
    /// <returns></returns>
    public static ResourceDictionary? FindResource(this Collection<ResourceDictionary> mergedDictionaries, string sourcePath) {
        return mergedDictionaries.FirstOrDefault(
            r => r.Source != null && r.Source.OriginalString == sourcePath
        );
    }

    /// <summary>
    /// 替换 <see cref="ResourceDictionary"/>
    /// </summary>
    /// <param name="mergedDictionaries"></param>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    /// <returns></returns>
    public static bool ReplaceResourceDictionary(this Collection<ResourceDictionary> mergedDictionaries, string oldSource, string newSource) {
        var oldResource = mergedDictionaries.FindResource(oldSource);
        if (oldResource is null) {
            Logger.Error($"Cannot find resource {oldSource}");
            return false;
        }
        oldResource.BeginInit();
        oldResource.Source = new(newSource, UriKind.Relative);
        oldResource.EndInit();
        return true;
    }

    /// <summary>
    /// 获取 <paramref name="sender"/> 的 <see cref="FrameworkElement.DataContext"/>
    /// </summary>
    /// <typeparam name="T">DataContext type</typeparam>
    /// <param name="sender">继承自 FrameworkContentElement 或 FrameworkElement</param>
    /// <returns>获取失败返回 default(<typeparamref name="T"/>)</returns>
    public static T? GetElementDataContext<T>(this object sender) {
        if (sender is FrameworkElement element && element.DataContext is T data) {
            return data;
        }
        if (sender is FrameworkContentElement contentElement && contentElement.DataContext is T contentData) {
            return contentData;
        }
        return default;
    }

    private static readonly IDictionary<DependencyObject, ICollection<RoutedEventHandler>> LoadedOnceEventHandlersDict = new Dictionary<DependencyObject, ICollection<RoutedEventHandler>>();

    /// <summary>
    /// 设置只执行一次的 Loaded EventHandler
    /// </summary>
    /// <param name="element"></param>
    /// <param name="handler"></param>
    public static void SetLoadedOnceEventHandler(this FrameworkElement element, RoutedEventHandler handler) {
        SetLoadedOnceEventHandler(element as DependencyObject, handler);
    }

    /// <inheritdoc cref="SetLoadedOnceEventHandler(FrameworkElement, RoutedEventHandler)"/>
    public static void SetLoadedOnceEventHandler(this FrameworkContentElement element, RoutedEventHandler handler) {
        SetLoadedOnceEventHandler(element as DependencyObject, handler);
    }

    private static void SetLoadedOnceEventHandler(DependencyObject dp, RoutedEventHandler handler) {
        // Initialize
        if (!LoadedOnceEventHandlersDict.TryGetValue(dp, out var handlers)) {
            LoadedOnceEventHandlersDict[dp] = handlers = new List<RoutedEventHandler>();
        }
        if (dp is FrameworkElement element) {
            element.Loaded -= LoadedOnceEventHandlerInternal;
            element.Loaded += LoadedOnceEventHandlerInternal;
        } else if (dp is FrameworkContentElement contentElement) {
            contentElement.Loaded -= LoadedOnceEventHandlerInternal;
            contentElement.Loaded += LoadedOnceEventHandlerInternal;
        }
        handlers.Add(handler);
    }

    private static void LoadedOnceEventHandlerInternal(object sender, RoutedEventArgs e) {
        if (sender is not DependencyObject dp) {
            return;
        }
        var handlers = LoadedOnceEventHandlersDict[dp];
        // 清除
        LoadedOnceEventHandlersDict.Remove(dp);
        // Remove event handler
        if (sender is FrameworkElement element) {
            element.Loaded -= LoadedOnceEventHandlerInternal;
        } else if (sender is FrameworkContentElement contentElement) {
            contentElement.Loaded -= LoadedOnceEventHandlerInternal;
        }
        // 逐一调用
        foreach (var handler in handlers) {
            handler(sender, e);
        }
        handlers.Clear();
    }
}

/// <summary>
/// For ContentDialogAutoResize
/// </summary>
public static partial class UIUtilsExtension {
    /// <summary>
    /// ContentDialogResizeRatio
    /// </summary>
    public const double ContentDialogResizeRatio = 2;

    private readonly struct ContentDialogAutoResizeArgs {
        public ContentDialogAutoResizeArgs(WeakReference<ContentDialog> dialog, double minWidth, double maxWidth, double resizeRatio) {
            Dialog = dialog;
            MinWidth = minWidth;
            MaxWidth = maxWidth;
            ResizeRatio = resizeRatio;
        }

        public WeakReference<ContentDialog> Dialog { get; }
        public double MinWidth { get; }
        public double MaxWidth { get; }
        public double ResizeRatio { get; }
    }

    private static readonly IDictionary<Window, ICollection<ContentDialogAutoResizeArgs>> ContentDialogAutoResizeDict = new Dictionary<Window, ICollection<ContentDialogAutoResizeArgs>>();
    /// <summary>
    /// 用于获取 <see cref="ContentDialogAutoResizeArgs"/>
    /// </summary>
    private static readonly IDictionary<ContentDialog, ContentDialogAutoResizeArgs> ContentDialogDict = new Dictionary<ContentDialog, ContentDialogAutoResizeArgs>();

    /// <summary>
    /// 启用 ContentDialog 自动调整大小
    /// </summary>
    /// <param name="dialog"></param>
    /// <param name="minWidth"></param>
    /// <param name="maxWidth"></param>
    /// <param name="resizeRatio">NewWidth = Window.ActualWidth / <paramref name="resizeRatio"/></param>
    public static void EnableAutoResize(
        this ContentDialog dialog,
        double minWidth,
        double maxWidth = double.MaxValue,
        double resizeRatio = ContentDialogResizeRatio
    ) {
        ContentDialogDict[dialog] = new(new(dialog), minWidth, maxWidth, resizeRatio);
        if (dialog.IsLoaded) {
            // Manual invoke method
            DialogLoadedHandler(dialog, new(FrameworkElement.LoadedEvent, dialog));
            return;
        }
        dialog.Loaded -= DialogLoadedHandler;
        dialog.Loaded += DialogLoadedHandler;
    }

    /// <summary>
    /// Update DialogContent Width
    /// </summary>
    /// <param name="args"></param>
    /// <param name="newWidth"></param>
    private static void UpdateAutoResizeDialogWidth(ContentDialogAutoResizeArgs args, double newWidth) {
        double minWidth = args.MinWidth, maxWidth = args.MaxWidth;
        if (newWidth < minWidth) {
            newWidth = minWidth;
        } else if (newWidth > maxWidth) {
            newWidth = maxWidth;
        }
        if (args.Dialog.TryGetTarget(out var dialog)) {
            if (dialog.Content is FrameworkElement element) {
                element.Width = newWidth;
            }
        }
    }

    private static void DialogLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is not ContentDialog dialog) {
            return;
        }
        dialog.Loaded -= DialogLoadedHandler;
        var window = Window.GetWindow(dialog);
        var args = ContentDialogDict[dialog];
        AddToContentDialogAutoResizeDict(window, args);
        // Remove reference
        ContentDialogDict.Remove(dialog);
        // Update now
        UpdateAutoResizeDialogWidth(args, window.ActualWidth / args.ResizeRatio);
        window.SizeChanged -= AutoResizeDialogWindowSizeChangedHandler;
        window.SizeChanged += AutoResizeDialogWindowSizeChangedHandler;
    }

    private static void AddToContentDialogAutoResizeDict(Window window, ContentDialogAutoResizeArgs args) {
        if (!ContentDialogAutoResizeDict.TryGetValue(window, out var dialogArgs)) {
            ContentDialogAutoResizeDict[window] = dialogArgs = new List<ContentDialogAutoResizeArgs>();
        }
        // Remove duplicated
        dialogArgs.Remove(item => {
            if (item.Dialog.TryGetTarget(out var dialog)) {
                if (args.Dialog.TryGetTarget(out var currentDialog) && dialog == currentDialog) {
                    return true;
                }
            }
            return false;
        });
        dialogArgs.Add(args);
    }

    /// <summary>
    /// Update ContentDialog width
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void AutoResizeDialogWindowSizeChangedHandler(object sender, SizeChangedEventArgs e) {
        if (sender is not Window window) {
            return;
        }
        if (ContentDialogAutoResizeDict.TryGetValue(window, out var dialogArgs)) {
            bool cleanFlag = false;
            foreach (var args in dialogArgs) {
                if (args.Dialog.TryGetTarget(out var _)) {
                    UpdateAutoResizeDialogWidth(args, window.ActualWidth / args.ResizeRatio);
                } else {
                    cleanFlag = true;
                }
            }
            // 清除
            if (cleanFlag) {
                CleanContentDialogAutoResizeDict(window);
            }
        }
    }

    /// <summary>
    /// Clean dict
    /// </summary>
    /// <param name="window"></param>
    private static void CleanContentDialogAutoResizeDict(Window window) {
        if (ContentDialogAutoResizeDict.TryGetValue(window, out var dialogArgList)) {
            dialogArgList.RemoveAll(args => !args.Dialog.TryGetTarget(out _));
        }
    }
}