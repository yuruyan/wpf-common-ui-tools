using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace CommonUITools.Extensions;

/// <summary>
/// 将 ImagePath 加载为 BitmapImage
/// </summary>
public class ImageStaticExtension : MarkupExtension {
    public string Path { get; set; }
    public UriKind UriKind { get; } = UriKind.Relative;

    public ImageStaticExtension(string path) {
        Path = path;
    }

    public ImageStaticExtension(string path, UriKind uriKind) : this(path) {
        UriKind = uriKind;
    }

    public override object ProvideValue(IServiceProvider serviceProvider) {
        return new BitmapImage(new Uri(Path, UriKind)) {
            CacheOption = BitmapCacheOption.OnDemand,
        };
    }
}
