namespace CommonUITools.Themes;

public static class ThemeManager {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly ResourceDictionary GenericResourceDictionary;

    private const string GenericSource1 = "/CommonUITools;component/Themes/Generic.xaml";
    private const string GenericSource2 = "pack://application:,,,/CommonUITools;component/Themes/Generic.xaml";
    private const string LightThemeSource = "/CommonUITools;component/Themes/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUITools;component/Themes/DarkThemeResources.xaml";

    public static ThemeMode CurrentTheme { get; private set; } = ThemeMode.Light;

    static ThemeManager() {
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

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public static void SwitchToLightTheme() {
        if (CurrentTheme == ThemeMode.Light) {
            return;
        }
        GenericResourceDictionary.MergedDictionaries.ReplaceResourceDictionary(
            DarkThemeSource,
            LightThemeSource
        );
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
        CurrentTheme = ThemeMode.Light;
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    public static void SwitchToDarkTheme() {
        if (CurrentTheme == ThemeMode.Dark) {
            return;
        }
        GenericResourceDictionary.MergedDictionaries.ReplaceResourceDictionary(
            LightThemeSource,
            DarkThemeSource
        );
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
        CurrentTheme = ThemeMode.Dark;
    }
}
