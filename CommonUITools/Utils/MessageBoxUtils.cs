using CommonUITools.Widget;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUITools.Utils;

public static class MessageBoxUtils {
    /// <summary>
    /// 提示消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="this">多窗口情况下使用，将调用方作为此参数传入</param>
    public static void Info(string message, DependencyObject? @this = null) => MessageBox.ShowMessage(message, @this, MessageType.Info);

    /// <summary>
    /// 警告消息
    /// </summary>
    /// <inheritdoc cref="Info(string, DependencyObject?)"/>
    public static void Waring(string message, DependencyObject? @this = null) => MessageBox.ShowMessage(message, @this, MessageType.Warning);

    /// <summary>
    /// 成功消息
    /// </summary>
    /// <inheritdoc cref="Info(string, DependencyObject?)"/>
    public static void Success(string message, DependencyObject? @this = null) => MessageBox.ShowMessage(message, @this, MessageType.Success);

    /// <summary>
    /// 失败消息
    /// </summary>
    /// <inheritdoc cref="Info(string, DependencyObject?)"/>
    public static void Error(string message, DependencyObject? @this = null) => MessageBox.ShowMessage(message, @this, MessageType.Error);
}
