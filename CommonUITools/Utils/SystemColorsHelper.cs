using CommonTools.Model;
using Windows.UI.ViewManagement;
using Color = Windows.UI.Color;

namespace CommonUITools.Utils;

public static class SystemColorsHelper {
    private static readonly ObservableProperty<Color> CurrentThemeColorProperty = new();
    private static readonly ObservableProperty<Color> CurrentAccentColorProperty = new();
    private static readonly UISettings UISettings = new();

    /// <summary>
    /// Running on UI thread
    /// </summary>
    public static event EventHandler<ThemeMode>? SystemThemeChanged;
    /// <summary>
    /// Running on UI thread
    /// </summary>
    public static event EventHandler<Color>? SystemAccentColorChanged;
    public static ThemeMode CurrentSystemTheme => IsLightThemeColor(CurrentThemeColorProperty.Value) ? ThemeMode.Light : ThemeMode.Dark;
    public static Color CurrentSystemAccentColor => CurrentAccentColorProperty.Value;

    static SystemColorsHelper() {
        CurrentThemeColorProperty.Value = UISettings.GetColorValue(UIColorType.Background);
        CurrentAccentColorProperty.Value = UISettings.GetColorValue(UIColorType.Accent);
        CurrentThemeColorProperty.ValueChanged += ThemeColorChanged;
        CurrentAccentColorProperty.ValueChanged += AccentColorChanged;
        UISettings.ColorValuesChanged += SystemColorValuesChanged;
    }

    private static bool IsLightThemeColor(Color color) => ((5 * color.G) + (2 * color.R) + color.B) > (8 * 128);

    private static void ThemeColorChanged(Color oldVal, Color newVal) {
        UIUtils.RunOnUIThreadAsync(() => SystemThemeChanged?.Invoke(null, CurrentSystemTheme));
    }

    private static void AccentColorChanged(Color oldVal, Color newVal) {
        UIUtils.RunOnUIThreadAsync(() => SystemAccentColorChanged?.Invoke(null, newVal));
    }

    private static void SystemColorValuesChanged(UISettings sender, object args) {
        CurrentThemeColorProperty.Value = sender.GetColorValue(UIColorType.Background);
        CurrentAccentColorProperty.Value = UISettings.GetColorValue(UIColorType.Accent);
    }
}
