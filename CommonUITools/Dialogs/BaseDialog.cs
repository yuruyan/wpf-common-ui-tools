using ModernWpf.Controls;

namespace CommonUITools.Controls;

public class BaseDialog : ContentDialog {
    public static readonly DependencyProperty DetailTextProperty = DependencyProperty.Register("DetailText", typeof(string), typeof(BaseDialog), new PropertyMetadata(""));
    /// <summary>
    /// DetailText
    /// </summary>
    public string DetailText {
        get { return (string)GetValue(DetailTextProperty); }
        set { SetValue(DetailTextProperty, value); }
    }
    /// <summary>
    /// 初次加载动画是否启用
    /// </summary>
    public bool IsFirstLoadingAnimationDisabled { get; }

    public BaseDialog() : this(false) { }

    public BaseDialog(bool disableInitializationAnimation) {
        IsFirstLoadingAnimationDisabled = disableInitializationAnimation;
        PrimaryButtonText = "确定";
        CloseButtonText = "取消";
        PrimaryButtonStyle = (Style)FindResource("GlobalAccentButtonStyle");
        if (TryFindResource("BaseDialogDataTemplate") is DataTemplate titleDataTemplate) {
            TitleTemplate = titleDataTemplate;
        }
        if (IsFirstLoadingAnimationDisabled) {
            Unloaded += ViewUnloadedOnceHandler;
        } else {
            Opened += DialogOpenedOnceHandler;
        }
    }

    private void DialogOpenedOnceHandler(ContentDialog sender, ContentDialogOpenedEventArgs args) {
        Opened -= DialogOpenedOnceHandler;
        EnableLoadingAnimation();
    }

    private void ViewUnloadedOnceHandler(object sender, RoutedEventArgs e) {
        Unloaded -= ViewUnloadedOnceHandler;
        EnableLoadingAnimation();
    }

    private void EnableLoadingAnimation() {
        if (Content is DependencyObject content) {
            ScaleAnimationHelper.SetIsEnabled(content, true);
            ScaleAnimationHelper.SetScaleOption(content, ScaleAnimationOption.Center);
        }
    }

    /// <inheritdoc cref="ContentDialog.ShowAsync()"/>
    public new Task<ContentDialogResult> ShowAsync() => this.ShowSafeAsync();

    /// <inheritdoc cref="ContentDialog.ShowAsync(ContentDialogPlacement)"/>
    public new Task<ContentDialogResult> ShowAsync(ContentDialogPlacement placement) => this.ShowSafeAsync(placement);
}
