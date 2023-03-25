using ModernWpf.Controls;

namespace CommonUITools.Resources.ResourceDictionary;

public partial class StyleResources {
    /// <summary>
    /// 转换浮点数为整数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IntegerNumberBoxLostFocusHandler(object sender, RoutedEventArgs e) {
        e.Handled = true;
        // 浮点数转整数
        if (sender is NumberBox numberBox) {
            TaskUtils.Try(() => numberBox.Value = (long)numberBox.Value);
        }
    }
}
