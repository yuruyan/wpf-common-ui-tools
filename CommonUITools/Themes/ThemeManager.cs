using CommonUITools.Utils;
using NLog;
using System.Windows;

namespace CommonUITools.Themes;

public static class ThemeManager {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const string GenericSource1 = "/CommonUITools;component/Themes/Generic.xaml";
    private const string GenericSource2 = "pack://application:,,,/CommonUITools;component/Themes/Generic.xaml";
    private const string LightThemeSource = "/CommonUITools;component/Resource/ResourceDictionary/LightThemeResources.xaml";
    private const string DarkThemeSource = "/CommonUITools;component/Resource/ResourceDictionary/DarkThemeResources.xaml";

    private static readonly ResourceDictionary GenericResourceDictionary;

    static ThemeManager() {
        // 查找 GenericResourceDictionary
        var res = UIUtils.FindResourceInMergedDictionaries(
            App.Current.Resources.MergedDictionaries,
            GenericSource1
        );
        res ??= UIUtils.FindResourceInMergedDictionaries(
            App.Current.Resources.MergedDictionaries,
            GenericSource2
        );
        if (res is null) {
            throw new KeyNotFoundException("Cannot find CommonUITools Generic Resource");
        }
        GenericResourceDictionary = res;
    }

    /// <summary>
    /// 切换为 LightTheme
    /// </summary>
    public static void SwitchToLightTheme() {
        UIUtils.ReplaceResourceDictionary(
            GenericResourceDictionary.MergedDictionaries,
            DarkThemeSource,
            LightThemeSource
        );
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Light;
    }

    /// <summary>
    /// 切换为 DarkTheme
    /// </summary>
    public static void SwitchToDarkTheme() {
        UIUtils.ReplaceResourceDictionary(
            GenericResourceDictionary.MergedDictionaries,
            LightThemeSource,
            DarkThemeSource
        );
        ModernWpf.ThemeManager.Current.ApplicationTheme = ModernWpf.ApplicationTheme.Dark;
    }
}
