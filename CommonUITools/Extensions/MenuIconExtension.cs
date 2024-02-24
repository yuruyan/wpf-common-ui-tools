using System.Windows.Markup;

namespace CommonUITools.Extensions;

/// <summary>
/// 将 Icon 加载为 TextBlock Icon
/// </summary>
public class MenuIconExtension : MarkupExtension {
    private const string DefaultStyle = "IconFontStyle";

    public string Icon { get; set; }
    public Brush? Foreground { get; set; }
    /// <summary>
    /// Default style is <see cref="DefaultStyle"/>
    /// </summary>
    public Style? Style { get; set; }

    public MenuIconExtension(string icon) : this(icon, null!) { }

    public MenuIconExtension(string icon, Brush foreground) {
        Icon = icon;
        Foreground = foreground;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget target) {
            return null!;
        }
        Style ??= target.TargetObject switch {
            FrameworkElement element => (Style)element.FindResource(DefaultStyle),
            FrameworkContentElement contentElement => (Style)contentElement.FindResource(DefaultStyle),
            _ => null
        };
        // Element is neither FrameworkElement nor FrameworkContentElement
        if (Style is null) {
            return null!;
        }

        var iconElement = new TextBlock() {
            Text = Icon,
            Style = Style
        };
        // Set Foreground
        if (Foreground != null) {
            iconElement.Foreground = Foreground;
        }
        return iconElement;
    }
}
