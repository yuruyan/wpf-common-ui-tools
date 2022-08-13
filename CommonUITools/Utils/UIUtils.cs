using NLog;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CommonUITools.Utils {
    public class Loading {
        private static ProgressBar? ProgressBar;

        public Loading(ProgressBar progressBar) {
            if (ProgressBar == null) {
                ProgressBar = progressBar;
            }
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
        /// <returns></returns>
        public static bool CheckInputNullOrEmpty(string? input, bool showMessage = true, string message = "输入不能为空") {
            if (string.IsNullOrEmpty(input)) {
                if (showMessage) {
                    Widget.MessageBox.Error(message);
                }
                return false;
            }
            return true;
        }
    }
}
