namespace CommonUITools.Widget;

public class WidgetGlobal {
    public static readonly IReadOnlyDictionary<MessageType, MessageTypeInfo> MessageInfoDict = new Dictionary<MessageType, MessageTypeInfo>() {
        {
            MessageType.Info,
            new("#F4F4F5", "#9D9399", "#e5e5e6", "\ue650")
        },
        {
            MessageType.Success,
            new("#f0f9eb", "#67C28A", "#dbe4d7", "\ue63c")
        },
        {
            MessageType.Warning,
            new("#fdf6ec", "#E6A23C", "#e8e1d8", "\ue6d2")
        },
        {
            MessageType.Error,
            new("#fef0f0", "#F66C6C", "#eee1e1", "\ue6c6")
        }
    };
}

public readonly struct MessageTypeInfo {
    public MessageTypeInfo(string background, string foreground, string borderColor, string icon) {
        Background = background;
        Foreground = foreground;
        BorderColor = borderColor;
        Icon = icon;
    }

    public readonly string Background;
    public readonly string Foreground;
    public readonly string BorderColor;
    public readonly string Icon;
}

public enum MessageType {
    Info, Success, Warning, Error
}
