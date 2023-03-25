namespace CommonUITools.Resources.ResourceDictionary;

public partial class ButtonResources {
    private void OpenFileTextBlockMouseUpHandler(object sender, MouseButtonEventArgs e) {
        if (sender is TextBlock textBlock) {
            // 首选 text
            string file = textBlock.Text ?? string.Empty;
            // 查询 tag
            if (!File.Exists(file) || !Directory.Exists(file)) {
                if (textBlock.Tag is string tag) {
                    file = tag;
                }
            }
            (file ?? string.Empty).OpenFileInExplorerAsync();
        }
    }
}
