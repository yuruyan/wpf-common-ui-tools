using CommonTools.Model;
using Windows.UI.ViewManagement;
using Color = Windows.UI.Color;

namespace CommonUITools.Utils;

public static class SystemColorsHelper {
    private static readonly Color LightThemeColor = Color.FromArgb(255, 255, 255, 255);
    private static readonly Color DarkThemeColor = Color.FromArgb(255, 0, 0, 0);
    private static readonly ObservableProperty<Color> CurrentThemeColor = new(LightThemeColor);
    public static event EventHandler<ThemeMode>? SystemThemeChanged;
    private static readonly UISettings UISettings = new();

    static SystemColorsHelper() {
        CurrentThemeColor.ValueChanged += ThemeColorChanged;
        CurrentThemeColor.Value = UISettings.GetColorValue(UIColorType.Background);
        UISettings.ColorValuesChanged += SystemColorValuesChanged;
    }

    private static void ThemeColorChanged(Color oldVal, Color newVal) {
        var themeMode = newVal == LightThemeColor ? ThemeMode.Light : ThemeMode.Dark;
        SystemThemeChanged?.Invoke(null, themeMode);
    }

    private static void SystemColorValuesChanged(UISettings sender, object args) {
        CurrentThemeColor.Value = sender.GetColorValue(UIColorType.Background);
    }
}
