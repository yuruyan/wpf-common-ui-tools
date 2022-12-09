using CommonUITools.View;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using SysDraw = System.Drawing;
using SysDrawBitmap = System.Drawing.Bitmap;
using SysDrawImg = System.Drawing.Imaging;

namespace CommonUITools.Utils;

public class CommonAdorner : Adorner {
    public readonly struct ElementInfo {
        public readonly FrameworkElement Element;
        /// <summary>
        /// 绑定对象，为 null 则绑定到 adornedElement
        /// </summary>
        public readonly FrameworkElement? TargetBindingElement;
        /// <summary>
        /// 是否绑定 Width
        /// </summary>
        public readonly bool BindWidth;
        /// <summary>
        /// 是否绑定 Height
        /// </summary>
        public readonly bool BindHeight;

        public ElementInfo(
            FrameworkElement element,
            bool bindWidth = true,
            bool bindHeight = true,
            FrameworkElement? targetBindingElement = null
        ) {
            Element = element;
            BindWidth = bindWidth;
            BindHeight = bindHeight;
            TargetBindingElement = targetBindingElement;
        }
    }

    private readonly VisualCollection VisualChildren;
    /// <summary>
    /// UIElement 集合
    /// </summary>
    private readonly IEnumerable<UIElement> Elements;

    protected override int VisualChildrenCount => VisualChildren.Count;

    public CommonAdorner(FrameworkElement adornedElement, IEnumerable<ElementInfo> elementInfos) : base(adornedElement) {
        VisualChildren = new(this);
        Elements = elementInfos.Select(e => e.Element);
        // 绑定 size
        foreach (var item in elementInfos) {
            UIUtils.BindSize(
                item.Element,
                item.TargetBindingElement ?? adornedElement,
                item.BindWidth,
                item.BindHeight
            );
            VisualChildren.Add(item.Element);
        }
    }

    protected override Visual GetVisualChild(int index) => VisualChildren[index];

    protected override Size ArrangeOverride(Size finalSize) {
        foreach (var item in Elements) {
            item.Arrange(new(finalSize));
        }
        return base.ArrangeOverride(finalSize);
    }
}

public static class UIUtils {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 左键是否单击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsLeftButtonDown(MouseButtonEventArgs e) {
        return e.LeftButton == MouseButtonState.Pressed;
    }

    /// <summary>
    /// 右键是否单击
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public static bool IsRightButtonDown(MouseButtonEventArgs e) {
        return e.RightButton == MouseButtonState.Pressed;
    }

    /// <summary>
    /// 对齐 ContextMenu 位置、调整宽度
    /// </summary>
    /// <param name="menuOf">ContextMenu 所属控件</param>
    /// <param name="e"></param>
    public static void AlignContextMenu(FrameworkElement menuOf, MouseButtonEventArgs e) {
        ContextMenu contextMenu = menuOf.ContextMenu;
        Point point = e.GetPosition(menuOf);
        contextMenu.HorizontalOffset = -point.X;
        contextMenu.VerticalOffset = -point.Y;
        contextMenu.VerticalOffset += menuOf.ActualHeight;
        contextMenu.Width = menuOf.ActualWidth;
    }

    /// <summary>
    /// string 转 Brush
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static SolidColorBrush StringToBrush(string color) {
        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }

    /// <summary>
    /// string 转 Color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color StringToColor(string color) {
        return StringToBrush(color).Color;
    }

    /// <summary>
    /// SystemDrawing.Color 转 System.Windows.Media.Color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color DrawingColorToColor(SysDraw.Color color)
        => Color.FromArgb(color.A, color.R, color.G, color.B);

    /// <summary>
    /// 在文件资源管理器中异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileInExplorerAsync(string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("explorer.exe", "/select," + path.ReplaceSlashWithBackSlash());
        } catch (Exception error) {
            if (failedCallback != null) {
                failedCallback(error);
                return;
            }
            CommonUITools.Widget.MessageBox.Error("打开失败," + error.Message);
            Logger.Info(error);
        }
    }

    /// <summary>
    /// 在文件资源管理器中异步打开文件，如果存在路径相同的窗口，则不会新建窗口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="failedCallback"></param>
    public static void OpenFileInExistingExplorerAsync(string path, Action<Exception>? failedCallback = null) {
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
            Widget.MessageBox.Error("打开失败," + error.Message);
            Logger.Info(error);
        }
    }

    /// <summary>
    /// 选择以什么方式异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileWithAsync(string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("rundll32.exe", $"shell32.dll, OpenAs_RunDLL {path.ReplaceSlashWithBackSlash()}");
        } catch (Exception error) {
            if (failedCallback != null) {
                failedCallback(error);
                return;
            }
            CommonUITools.Widget.MessageBox.Error("打开失败," + error.Message);
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
        Widget.NotificationBox.Success(
            title,
            message,
            () => OpenFileInExplorerAsync(filepath)
        );
    }

    /// <summary>
    /// 在 UI 线程中运行
    /// </summary>
    /// <param name="task"></param>
    public static void RunOnUIThread(Action task) {
        Application.Current.Dispatcher.Invoke(() => task());
    }

    /// <summary>
    /// 在 UI 线程中运行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static T RunOnUIThread<T>(Func<T> task) {
        return Application.Current.Dispatcher.Invoke(() => task());
    }

    /// <summary>
    /// 检查输入是否为 null 或空
    /// </summary>
    /// <param name="input"></param>
    /// <param name="showMessage">如果不合法则显示消息提示</param>
    /// <param name="message">显示的消息</param>
    /// <returns>合法返回 true</returns>
    public static bool CheckInputNullOrEmpty(
        string? input,
        bool showMessage = true,
        string message = "输入不能为空"
    ) {
        return CheckInputNullOrEmpty(
            new KeyValuePair<string?, string>[] {
                    new (input, message),
            },
            showMessage,
            message
        );
    }

    /// <summary>
    /// 检查输入是否都不为空或 null
    /// </summary>
    /// <param name="inputWithMessageList">(inputValue, message)</param>
    /// <param name="showMessage">如果不合法则显示消息提示</param>
    /// <param name="message">显示的消息</param>
    /// <returns></returns>
    public static bool CheckInputNullOrEmpty(
        IEnumerable<KeyValuePair<string?, string>> inputWithMessageList,
        bool showMessage = true,
        string message = "输入不能为空"
    ) {
        message ??= "输入不能为空";
        foreach (var item in inputWithMessageList) {
            if (string.IsNullOrEmpty(item.Key)) {
                // 显示提示消息
                if (showMessage) {
                    Widget.MessageBox.Error(
                        string.IsNullOrEmpty(item.Value) ? message : item.Value
                    );
                }
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 在浏览器中打开
    /// </summary>
    /// <param name="url"></param>
    public static void OpenInBrowser(string url) {
        Process.Start(new ProcessStartInfo() {
            UseShellExecute = true,
            FileName = url,
        });
    }

    /// <summary>
    /// 创建通用文件处理任务
    /// </summary>
    /// <param name="func">主调用函数</param>
    /// <param name="outputPath">文件保存路径</param>
    /// <param name="notify">是否通知用户</param>
    /// <param name="notificationTitle">通知 Title</param>
    /// <param name="notificationMessage">通知详情</param>
    /// <param name="successCallback">成功回调</param>
    /// <param name="errorCallback">失败回调</param>
    /// <param name="finallyCallback">回调函数</param>
    /// <param name="showErrorInfo">显示错误信息</param>
    /// <param name="reThrowError">是否抛出异常</param>
    /// <param name="args">主调用函数参数</param>
    /// <returns></returns>
    public static Task CreateFileProcessTask(
        Delegate func,
        string outputPath,
        bool notify = true,
        string notificationTitle = "成功",
        string notificationMessage = "点击打开",
        Action? successCallback = null,
        Action? errorCallback = null,
        Action? finallyCallback = null,
        bool showErrorInfo = true,
        bool reThrowError = false,
        params object?[]? args
    ) => Task.Run(() => {
        bool success = false;
        try {
            _ = func.DynamicInvoke(args);
            success = true;
            successCallback?.Invoke();
            // 通知用户
            if (notify) {
                Widget.NotificationBox.Success(
                    notificationTitle,
                    notificationMessage,
                    () => OpenFileInExplorerAsync(outputPath)
                );
            }
        } catch (IOException) {
            if (showErrorInfo) {
                Widget.MessageBox.Error("文件读取或写入失败");
            }
            if (reThrowError) {
                throw;
            }
        } catch {
            if (showErrorInfo) {
                Widget.MessageBox.Error("失败");
            }
            if (reThrowError) {
                throw;
            }
        } finally {
            // 失败
            if (!success) {
                errorCallback?.Invoke();
            }
            finallyCallback?.Invoke();
        }
    });

    /// <summary>
    /// 检查文本和文件输入
    /// </summary>
    /// <param name="inputText"></param>
    /// <param name="hasFile"></param>
    /// <param name="filePath"></param>
    /// <param name="warningDetail"></param>
    /// <returns>通过返回 true</returns>
    public static async Task<bool> CheckTextAndFileInputAsync(
        string? inputText,
        bool hasFile,
        string? filePath,
        string warningDetail = "文件可能是二进制文件，是否继续？"
    ) => await CheckTextAndFileInputAsync(
            inputText,
            hasFile,
            new string[] { filePath! },
            warningDetail
        );

    /// <summary>
    /// 检查文本和文件输入
    /// </summary>
    /// <param name="inputText"></param>
    /// <param name="hasFile"></param>
    /// <param name="filepaths"></param>
    /// <param name="warningDetail"></param>
    /// <returns>通过返回 true</returns>
    /// <remarks>
    /// 如果有多个文件则不检查
    /// </remarks>
    public static async Task<bool> CheckTextAndFileInputAsync(
        string? inputText,
        bool hasFile,
        ICollection<string> filepaths,
        string warningDetail = "文件可能是二进制文件，是否继续？"
    ) {
        // 检查文件
        if (hasFile && filepaths.Count == 1) {
            var filePath = filepaths.First();
            // 文件检查是否存在
            if (!File.Exists(filePath)) {
                Widget.MessageBox.Error($"文件 '{Path.GetFileName(filePath)}' 不存在");
                return false;
            }
            // 二进制文件警告
            if (CommonUtils.IsLikelyBinaryFile(filePath)) {
                var dialog = WarningDialog.Shared;
                dialog.DetailText = warningDetail;
                if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                    return false;
                }
            }
        }
        // 输入文本检查
        if (!hasFile && string.IsNullOrEmpty(inputText)) {
            Widget.MessageBox.Info("请输入文本");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 将 <paramref name="thisElement"/> width、height 绑定到 <paramref name="targetElement"/>
    /// </summary>
    /// <param name="thisElement"></param>
    /// <param name="targetElement"></param>
    /// <param name="bindWidth">是否绑定 width</param>
    /// <param name="bindHeight">是否绑定 height</param>
    public static void BindSize(
        FrameworkElement thisElement,
        FrameworkElement targetElement,
        bool bindWidth = true,
        bool bindHeight = true
    ) {
        if (bindWidth) {
            var widthBinding = new Binding("ActualWidth") { Source = targetElement, };
            thisElement.SetBinding(FrameworkElement.WidthProperty, widthBinding);
        }
        if (bindHeight) {
            var heightBinding = new Binding("ActualHeight") { Source = targetElement, };
            thisElement.SetBinding(FrameworkElement.HeightProperty, heightBinding);
        }
    }

    private static readonly IDictionary<FrameworkElement, ICollection<RoutedEventHandler>> LoadedOnceEventHandlersDict = new Dictionary<FrameworkElement, ICollection<RoutedEventHandler>>();

    /// <summary>
    /// 设置只执行一次的 Loaded EventHandler
    /// </summary>
    /// <param name="element"></param>
    /// <param name="handler"></param>
    public static void SetLoadedOnceEventHandler(FrameworkElement element, RoutedEventHandler handler) {
        // 初始化
        if (!LoadedOnceEventHandlersDict.ContainsKey(element)) {
            LoadedOnceEventHandlersDict[element] = new List<RoutedEventHandler>();
            element.Loaded += LoadedOnceEventHandlerInternal;
        }
        LoadedOnceEventHandlersDict[element].Add(handler);
    }

    private static void LoadedOnceEventHandlerInternal(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            // 逐一调用
            foreach (var handler in LoadedOnceEventHandlersDict[element]) {
                handler(sender, e);
            }
            // 清除
            LoadedOnceEventHandlersDict[element].Clear();
        }
    }

    /// <summary>
    /// 反转 Panel Children 顺序
    /// </summary>
    /// <param name="panel"></param>
    public static void ReversePanelChildrenOrder(Panel panel) {
        var children = panel.Children.Cast(o => o as UIElement).Reverse();
        panel.Children.Clear();
        children.ForEach(item => panel.Children.Add(item));
    }

    /// <summary>
    /// 拷贝 ImageSource
    /// </summary>
    /// <param name="filepath">图像路径</param>
    /// <returns></returns>
    public static BitmapImage CopyImageSource(string filepath) {
        var bi = new BitmapImage();
        using var bitmap = new SysDrawBitmap(filepath);
        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, SysDrawImg.ImageFormat.Png);
        bi.BeginInit();
        bi.StreamSource = memoryStream;
        bi.EndInit();
        return bi;
    }

    /// <summary>
    /// BitmapSource 转为Bitmap
    /// </summary>
    /// <param name="bitmapSource"></param>
    /// <returns></returns>
    public static SysDrawBitmap BitmapSourceToBitmap(BitmapSource bitmapSource) {
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
    /// Bitmap 转为位图
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static ImageSource BitmapToImageSource(SysDrawBitmap bitmap) {
        IntPtr intPtr = bitmap.GetHbitmap();
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            intPtr,
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions()
        );
    }

    /// <summary>
    /// Bitmap 转 Stream
    /// </summary>
    /// <param name="bitmap"></param>
    /// <returns></returns>
    public static Stream BitmapToStream(SysDrawBitmap bitmap) {
        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, SysDrawImg.ImageFormat.Png);
        return memoryStream;
    }

    /// <summary>
    /// 在 MergedDictionaries 中查找
    /// </summary>
    /// <param name="mergedDictionaries"></param>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static ResourceDictionary? FindResourceInMergedDictionaries(Collection<ResourceDictionary> mergedDictionaries, string resourceName) {
        return mergedDictionaries.FirstOrDefault(
                r => r.Source != null && r.Source.OriginalString == resourceName
            );
    }

    /// <summary>
    /// 替换 ResourceDictionary
    /// </summary>
    /// <param name="mergedDictionaries"></param>
    /// <param name="oldSource"></param>
    /// <param name="newSource"></param>
    /// <returns></returns>
    public static bool ReplaceResourceDictionary(Collection<ResourceDictionary> mergedDictionaries, string oldSource, string newSource) {
        var oldResource = FindResourceInMergedDictionaries(mergedDictionaries, oldSource);
        if (oldResource is null) {
            Logger.Error($"Cannot find resource {oldSource}");
            return false;
        }
        oldResource.BeginInit();
        oldResource.Source = new(newSource, UriKind.Relative);
        oldResource.EndInit();
        return true;
    }
}
