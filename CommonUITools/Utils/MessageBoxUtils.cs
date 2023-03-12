using CommonTools.Model;
using CommonUITools.Widget;
using MessageBox = CommonUITools.Widget.MessageBox;

namespace CommonUITools.Utils;

[CanBeCalledInAnyThread]
public static class MessageBoxUtils {
    /// <summary>
    /// 提示消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Info(string message, DependencyObject? @this = null, uint dispalyDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, dispalyDuration, MessageType.Info);

    /// <summary>
    /// 警告消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Waring(string message, DependencyObject? @this = null, uint dispalyDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, dispalyDuration, MessageType.Warning);

    /// <summary>
    /// 成功消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Success(string message, DependencyObject? @this = null, uint dispalyDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, dispalyDuration, MessageType.Success);

    /// <summary>
    /// 失败消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Error(string message, DependencyObject? @this = null, uint dispalyDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, dispalyDuration, MessageType.Error);
}
