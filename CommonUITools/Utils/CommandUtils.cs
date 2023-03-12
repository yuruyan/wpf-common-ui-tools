namespace CommonUITools.Utils;

public static class CommandUtils {
    /// <summary>
    /// CanExecute = true
    /// </summary>
    public static readonly CanExecuteRoutedEventHandler CanExecuteHandler = CanExecute;

    public static void CanExecute(object sender, CanExecuteRoutedEventArgs e) {
        e.CanExecute = true;
        e.Handled = true;
    }
}
