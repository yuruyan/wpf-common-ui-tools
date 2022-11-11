using CommonUITools.View;
using NLog;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CommonUITools.Utils;

public class Loading {
    private static ProgressBar? ProgressBar;

    public Loading(ProgressBar progressBar) {
        ProgressBar ??= progressBar;
    }

    public static void StartLoading() {
        if (ProgressBar != null) {
            ProgressBar.IsIndeterminate = true;
        }
    }

    public static void StopLoading() {
        if (ProgressBar != null) {
            ProgressBar.IsIndeterminate = false;
        }
    }
}

public class UIUtils {
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
    /// System.Drawing.Color 转 System.Windows.Media.Color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color DrawingColorToColor(System.Drawing.Color color)
        => Color.FromArgb(color.A, color.R, color.G, color.B);

    /// <summary>
    /// 获取 MergedDictionaries.ResourceDictionary
    /// </summary>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static ResourceDictionary? GetMergedResourceDictionary(string resourceName) {
        foreach (var res in App.Current.Resources.MergedDictionaries) {
            if (res.Source is not null) {
                if (res.Source.ToString().Contains(resourceName)) {
                    return res;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 在文件资源管理器中异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileInDirectoryAsync(string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("explorer.exe", "/select," + path.Replace('/', '\\'));
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
    /// 选择以什么方式异步打开文件
    /// </summary>
    /// <param name="path">文件绝对路径</param>
    /// <param name="failedCallback">发生异常回调</param>
    public static void OpenFileWithAsync(string path, Action<Exception>? failedCallback = null) {
        try {
            Process.Start("rundll32.exe", "shell32.dll, OpenAs_RunDLL " + path.Replace('/', '\\'));
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
    public static void NotificationOpenFileInDirectoryAsync(string filepath, string title = "成功", string message = "点击打开") {
        Widget.NotificationBox.Success(
            "成功",
            "点击打开",
            () => OpenFileInDirectoryAsync(filepath)
        );
    }

    /// <summary>
    /// 在 UI 线程中运行
    /// </summary>
    /// <param name="task"></param>
    public static void RunOnUIThread(Action task) {
        App.Current.Dispatcher.Invoke(() => task());
    }

    /// <summary>
    /// 在 UI 线程中运行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static T RunOnUIThread<T>(Func<T> task) {
        return App.Current.Dispatcher.Invoke(() => task());
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
    /// 拷贝 ImageSource
    /// </summary>
    /// <param name="filepath">图像路径</param>
    /// <returns></returns>
    public static BitmapImage CopyImageSource(string filepath) {
        var bi = new BitmapImage();
        using var bitmap = new System.Drawing.Bitmap(filepath);
        var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        bi.BeginInit();
        bi.StreamSource = memoryStream;
        bi.EndInit();
        return bi;
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
                    () => OpenFileInDirectoryAsync(outputPath)
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
    ) {
        // 文件检查是否存在
        if (!File.Exists(filePath)) {
            Widget.MessageBox.Error($"文件 '{Path.GetFileName(filePath)}' 不存在");
            return false;
        }
        // 二进制文件警告
        if (hasFile && CommonUtils.IsLikelyBinaryFile(filePath)) {
            var dialog = WarningDialog.Shared;
            dialog.DetailText = warningDetail;
            if (await dialog.ShowAsync() != ModernWpf.Controls.ContentDialogResult.Primary) {
                return false;
            }
        }
        // 输入文本检查
        if (!hasFile && string.IsNullOrEmpty(inputText)) {
            Widget.MessageBox.Info("请输入文本");
            return false;
        }
        return true;
    }
}
