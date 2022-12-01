using ModernWpf.Controls;

namespace CommonUITools.Model;

public record DialogResult<T> {
    public ContentDialogResult Result { get; set; }
    public T Data { get; set; } = default!;
}
