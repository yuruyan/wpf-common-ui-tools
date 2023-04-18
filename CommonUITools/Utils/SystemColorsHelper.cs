using CommonTools.Model;
using Windows.UI.ViewManagement;
using Color = Windows.UI.Color;

namespace CommonUITools.Utils;

public static class SystemColorsHelper {
    private static readonly Color LightThemeColor = Color.FromArgb(255, 255, 255, 255);
    private static readonly Color DarkThemeColor = Color.FromArgb(255, 0, 0, 0);
    private static readonly ObservableProperty<Color> CurrentThemeColor = new(LightThemeColor);
    private static readonly UISettings UISettings = new();
    public static event EventHandler<ThemeMode>? SystemThemeChanged;
    public static ThemeMode CurrentSystemTheme => CurrentThemeColor.Value == LightThemeColor ? ThemeMode.Light : ThemeMode.Dark;

    static SystemColorsHelper() {
        CurrentThemeColor.ValueChanged += ThemeColorChanged;
        CurrentThemeColor.Value = UISettings.GetColorValue(UIColorType.Background);
        UISettings.ColorValuesChanged += SystemColorValuesChanged;
    }

    private static void ThemeColorChanged(Color oldVal, Color newVal) {
        UIUtils.RunOnUIThreadAsync(() => {
            SystemThemeChanged?.Invoke(null, CurrentSystemTheme);
        });
    }

    private static void SystemColorValuesChanged(UISettings sender, object args) {
        CurrentThemeColor.Value = sender.GetColorValue(UIColorType.Background);
    }
}
