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

    public BaseDialog() {
        PrimaryButtonText = "确定";
        CloseButtonText = "取消";
        PrimaryButtonStyle = (Style)FindResource("GlobalAccentButtonStyle");
        if (TryFindResource("BaseDialogDataTemplate") is DataTemplate titleDataTemplate) {
            TitleTemplate = titleDataTemplate;
        }
        // 设置 ScaleAnimation
        Opened += (dialog, _) => TaskUtils.EnsureCalledOnce((dialog, Application.Current), () => {
            Utils.ScaleAnimationHelper.SetIsEnabled((DependencyObject)dialog.Content, true);
            Utils.ScaleAnimationHelper.SetScaleOption((DependencyObject)dialog.Content, ScaleAnimationOption.Center);
        });
    }
}
