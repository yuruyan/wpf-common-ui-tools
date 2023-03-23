using System.Windows.Media.Imaging;

namespace CommonUITools.Controls;

public partial class EmptyView : UserControl {
    private BitmapImage BitmapImage = default!;
    private readonly Uri ImageUri = new("/CommonUITools;component/Resource/Images/Empty.png", UriKind.Relative);

    public EmptyView() {
        InitializeComponent();
    }

    /// <summary>
    /// Load Image
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootLoadedHandler(object sender, RoutedEventArgs e) {
        BitmapImage = ImageUri.GetApplicationResourceBitmapImage();
        DescriptionImage.Source = BitmapImage;
    }

    /// <summary>
    /// Release Memory
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RootUnloadedHandler(object sender, RoutedEventArgs e) {
        DescriptionImage.ClearValue(Image.SourceProperty);
        BitmapImage.StreamSource.Dispose();
        if (BitmapImage.CanFreeze) {
            BitmapImage.Freeze();
        }
    }
}
