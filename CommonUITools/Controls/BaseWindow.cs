using CommonUITools.Themes;

namespace CommonUITools.Controls;

public class BaseWindow : Window {
    public BaseWindow() {
        // Initializ explicitly
        ThemeManager.SwitchToAutoTheme();
        App.RegisterWidgetPage(this);
    }
}
