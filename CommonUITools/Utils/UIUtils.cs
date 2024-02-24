using CommonUITools.Controls;

namespace CommonUITools.Utils;

/// <summary>
/// UI 工具类
/// </summary>
public static class UIUtils {
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
    /// 在 UI 线程中运行
    /// </summary>
    /// <param name="task"></param>
    public static void RunOnUIThread(Action task) {
        Application.Current.Dispatcher.Invoke(() => task());
    }

    /// <summary>
    /// 在 UI 线程中异步运行
    /// </summary>
    /// <param name="task"></param>
    public static void RunOnUIThreadAsync(Action task) {
        Application.Current.Dispatcher.InvokeAsync(() => task());
    }

    /// <summary>
    /// 在 UI 线程中运行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="state"></param>
    public static void RunOnUIThread<T>(Action<T> task, T state) {
        _ = Application.Current.Dispatcher.Invoke(delegate (T o) {
            task(o);
        }, state);
    }

    /// <summary>
    /// 在 UI 线程中异步运行
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="state"></param>
    public static void RunOnUIThreadAsync<T>(Action<T> task, T state) {
        _ = Application.Current.Dispatcher.BeginInvoke(delegate (T o) {
            task(o);
        }, state);
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
    /// 在 UI 线程中运行
    /// </summary>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task"></param>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static TResult RunOnUIThread<TArgument, TResult>(Func<TArgument, TResult> task, TArgument arg) {
        return (TResult)Application.Current.Dispatcher.Invoke(delegate (TArgument arg) {
            return task(arg);
        }, arg);
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
                    MessageBoxUtils.Error(
                        string.IsNullOrEmpty(item.Value) ? message : item.Value
                    );
                }
                return false;
            }
        }
        return true;
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
                MessageBoxUtils.NotifySuccess(
                    notificationTitle,
                    notificationMessage,
                    callback: () => outputPath.OpenFileInExplorerAsync()
                );
            }
        } catch (IOException) {
            if (showErrorInfo) {
                MessageBoxUtils.Error("文件读取或写入失败");
            }
            if (reThrowError) {
                throw;
            }
        } catch {
            if (showErrorInfo) {
                MessageBoxUtils.Error("失败");
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
                MessageBoxUtils.Error($"文件 '{Path.GetFileName(filePath)}' 不存在");
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
            MessageBoxUtils.Info("请输入文本");
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
}
