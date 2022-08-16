using CommonUITools.Utils;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonUITools.Resource.ResourceDictionary;

public partial class StyleResources {
    private void OpenFileTextBlockMouseUpHandler(object sender, MouseButtonEventArgs e) {
        if (sender is TextBlock textBlock) {
            UIUtils.OpenFileInDirectoryAsync(textBlock.Text ?? string.Empty);
        }
    }
}
