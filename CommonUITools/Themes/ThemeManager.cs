using CommonTools.Model;

namespace CommonUITools.Themes;

public static class ThemeManager {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly ResourceDictionary GenericResourceDictionary;
    private static readonly ObservableProperty<ThemeMode> CurrentThemeProperty = new(ThemeMode.Light);

    private const string GenericSource1 = "/CommonUITools;component/Themes/Generic.xaml";
    private const string GenericSource2 = "pack://application:,,,/CommonUITools;component/Themes/Generic.xaml";
    private const string LightThemeSource = "/CommonUITools;component/Themes/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUITools;component/Themes/DarkThemeResources.xaml";

    public static ThemeMode CurrentTheme => CurrentThemeProperty.Value;
    public static event EventHandler<ThemeMode>? ThemeChanged;

    static ThemeManager() {
        SystemColorsHelper.SystemAccentColorChanged += SystemAccentColorChangedHandler;
        SystemColorsHelper.SystemThemeChanged += SystemThemeChangedHandler;
        CurrentThemeProperty.ValueChanged += CurrentThemeChanged;
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
        var dictionary = Application.Current.Resources.MergedDictionaries;
        // 查找 GenericResourceDictionary
        var res = dictionary.FindResource(GenericSource1);
        res ??= dictionary.FindResource(GenericSource2);
        if (res is null) {
            throw new KeyNotFoundException("Cannot find CommonUITools Generic Resource");
        }
        GenericResourceDictionary = res;
    }

    private static void SystemThemeChangedHandler(object? sender, ThemeMode e) => CurrentThemeProperty.Value = e;

    private static void SystemAccentColorChangedHandler(object? sender, Windows.UI.Color e) => UpdateTheme();

    private static void CurrentThemeChanged(ThemeMode oldVal, ThemeMode newVal) {
        var oldSource = newVal is ThemeMode.Light ? DarkThemeSource : LightThemeSource;
        var newSource = newVal is ThemeMode.Light ? LightThemeSource : DarkThemeSource;
        var theme = newVal is ThemeMode.Light ? ModernWpf.ApplicationTheme.Light : ModernWpf.ApplicationTheme.Dark;
        ModernWpf.ThemeManager.Current.ApplicationTheme = theme;
        GenericResourceDictionary.MergedDictionaries.ReplaceResourceDictionary(oldSource, newSource);
        ThemeChanged?.Invoke(null, newVal);
    }

    /// <summary>
    /// Update Current Theme
    /// </summary>
    public static void UpdateTheme() {
        var source = CurrentTheme is ThemeMode.Light ? LightThemeSource : DarkThemeSource;
        GenericResourceDictionary.MergedDictionaries.ReplaceResourceDictionary(
            source,
            source
        );
    }

    /// <summary>
    /// Auto theme
    /// </summary>
    public static void SwitchToAutoTheme() {
        CurrentThemeProperty.Value = SystemColorsHelper.CurrentSystemTheme;
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        SystemColorsHelper.SystemThemeChanged += SystemThemeChangedHandler;
    }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public static void SwitchToLightTheme() {
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        CurrentThemeProperty.Value = ThemeMode.Light;
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    public static void SwitchToDarkTheme() {
        SystemColorsHelper.SystemThemeChanged -= SystemThemeChangedHandler;
        CurrentThemeProperty.Value = ThemeMode.Dark;
    }
}
