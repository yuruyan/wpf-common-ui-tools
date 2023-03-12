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
    public static void Info(string message, DependencyObject? @this = null, uint displayDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, displayDuration, MessageType.Info);

    /// <summary>
    /// 警告消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Waring(string message, DependencyObject? @this = null, uint displayDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, displayDuration, MessageType.Warning);

    /// <summary>
    /// 成功消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Success(string message, DependencyObject? @this = null, uint displayDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, displayDuration, MessageType.Success);

    /// <summary>
    /// 失败消息
    /// </summary>
    /// <inheritdoc cref="MessageBox.ShowMessage(string, DependencyObject?, uint, MessageType)"/>
    public static void Error(string message, DependencyObject? @this = null, uint displayDuration = MessageBox.DefaultDisplayDuration) => MessageBox.ShowMessage(message, @this, displayDuration, MessageType.Error);

    /// <summary>
    /// 提示消息
    /// </summary>
    /// <inheritdoc cref="NotificationBox.ShowNotification(string, string, DependencyObject?, uint, MessageType, Action?)"/>
    public static void NotifyInfo(string title, string message, DependencyObject? @this = null, uint displayDuration = NotificationBox.DefaultDisplayDuration, Action? callback = null)
       => NotificationBox.ShowNotification(title, message, @this, displayDuration, MessageType.Info, callback);

    /// <summary>
    /// 警告消息
    /// </summary>
    /// <inheritdoc cref="NotificationBox.ShowNotification(string, string, DependencyObject?, uint, MessageType, Action?)"/>
    public static void NotifyWarning(string title, string message, DependencyObject? @this = null, uint displayDuration = NotificationBox.DefaultDisplayDuration, Action? callback = null)
       => NotificationBox.ShowNotification(title, message, @this, displayDuration, MessageType.Warning, callback);

    /// <summary>
    /// 成功消息
    /// </summary>
    /// <inheritdoc cref="NotificationBox.ShowNotification(string, string, DependencyObject?, uint, MessageType, Action?)"/>
    public static void NotifySuccess(string title, string message, DependencyObject? @this = null, uint displayDuration = NotificationBox.DefaultDisplayDuration, Action? callback = null)
       => NotificationBox.ShowNotification(title, message, @this, displayDuration, MessageType.Success, callback);

    /// <summary>
    /// 失败消息
    /// </summary>
    /// <inheritdoc cref="NotificationBox.ShowNotification(string, string, DependencyObject?, uint, MessageType, Action?)"/>
    public static void NotifyError(string title, string message, DependencyObject? @this = null, uint displayDuration = NotificationBox.DefaultDisplayDuration, Action? callback = null)
       => NotificationBox.ShowNotification(title, message, @this, displayDuration, MessageType.Error, callback);
}
