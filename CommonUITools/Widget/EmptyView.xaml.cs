using System.Windows.Media.Imaging;

namespace CommonUITools.Widget;

public partial class EmptyView : UserControl, IDisposable {
    private readonly BitmapImage BitmapImage;
    private readonly Uri ImageUri = new("/CommonUITools;component/Resource/Images/Empty.png", UriKind.Relative);

    public EmptyView() {
        BitmapImage = ImageUri.GetApplicationResourceBitmapImage();
        InitializeComponent();
        DescriptionImage.Source = BitmapImage;
    }

    public void Dispose() {
        DataContext = null;
        DescriptionImage.Source = null;
        BitmapImage.StreamSource?.Dispose();
        if (BitmapImage.CanFreeze) {
            BitmapImage.Freeze();
        }
        GC.SuppressFinalize(this);
    }
}
