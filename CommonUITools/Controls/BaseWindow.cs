namespace CommonUITools.Controls;

public class BaseWindow : Window {
    public BaseWindow() {
        App.RegisterWidgetPage(this);
    }
}
