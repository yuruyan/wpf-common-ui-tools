namespace CommonTools.Utils;

/// <summary>
/// 消息工具
/// </summary>
public static class MessageUtils {
    /// <summary>
    /// 消息处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="message"></param>
    public delegate void MessageEventHandler(object sender, object? message);

    private static readonly IDictionary<string, IList<MessageEventHandler>> MessageEventHandlerDict = new Dictionary<string, IList<MessageEventHandler>>();
    /// <summary>
    /// 最近一次消息数据
    /// </summary>
    private static readonly IDictionary<string, object?> LastMessageDict = new Dictionary<string, object?>();

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="groupName">组名称，区分大小写</param>
    /// <param name="sender"></param>
    /// <param name="message">消息</param>
    public static void SendMessage(string groupName, object sender, object? message) {
        LastMessageDict[groupName] = message;
        if (MessageEventHandlerDict.TryGetValue(groupName, out var handlers)) {
            foreach (var handler in handlers) {
                handler.Invoke(sender, message);
            }
        }
    }

    /// <summary>
    /// 获取最近一次消息
    /// </summary>
    /// <param name="groupName">分组名称</param>
    /// <param name="message">返回的消息</param>
    /// <returns>存在消息返回 true，否则返回 false</returns>
    public static bool GetLastMessage(string groupName, out object? message) {
        if (LastMessageDict.TryGetValue(groupName, out message)) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 订阅消息
    /// </summary>
    /// <param name="groupName">组名称，区分大小写</param>
    /// <param name="handler"></param>
    public static void SubscribeMessage(string groupName, MessageEventHandler handler) {
        // Initialize
        if (!MessageEventHandlerDict.TryGetValue(groupName, out var handlers)) {
            handlers = new List<MessageEventHandler>();
            MessageEventHandlerDict[groupName] = handlers;
        }
        handlers.Add(handler);
    }

    /// <summary>
    /// 取消订阅消息
    /// </summary>
    /// <param name="groupName">组名称，区分大小写</param>
    /// <param name="handler"></param>
    public static void UnSubscribeMessage(string groupName, MessageEventHandler handler) {
        // Initialize
        if (!MessageEventHandlerDict.TryGetValue(groupName, out var handlers)) {
            handlers = new List<MessageEventHandler>();
            MessageEventHandlerDict[groupName] = handlers;
        }
        handlers.Remove(handler);
    }
}
