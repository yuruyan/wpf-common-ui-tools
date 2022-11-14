using CommonUITools.Utils;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUITools.Resource.ResourceDictionary;

public partial class StyleResources {
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
            UIUtils.OpenFileInExplorerAsync(file ?? string.Empty);
        }
    }
}
