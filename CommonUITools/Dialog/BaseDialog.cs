using ModernWpf.Controls;

namespace CommonUITools.View;

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
        if (TryFindResource("GlobalDefaultButtonStyle") is Style closeButtonStyle) {
            CloseButtonStyle = closeButtonStyle;
        }
        if (TryFindResource("BaseDialogDataTemplate") is DataTemplate titleDataTemplate) {
            TitleTemplate = titleDataTemplate;
        }
        // 设置 ScaleAnimation
        Opened += (dialog, _) => TaskUtils.EnsureCalledOnce((dialog, Application.Current), () => {
            Utils.ScaleAnimationHelper.SetIsEnabled((DependencyObject)dialog.Content, true);
            Utils.ScaleAnimationHelper.SetScaleOption((DependencyObject)dialog.Content, Utils.ScaleAnimationHelper.ScaleOption.Center);
        });
    }
}
