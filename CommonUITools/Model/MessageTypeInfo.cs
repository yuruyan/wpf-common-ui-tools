namespace CommonUITools.Model;

internal readonly struct MessageTypeInfo {
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