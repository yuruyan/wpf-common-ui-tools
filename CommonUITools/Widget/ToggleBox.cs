using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CommonUITools.Widget;

/// <summary>
/// 切换内容 Box
/// </summary>
public class ToggleBox : UserControl {
    public static readonly DependencyProperty IsOnProperty = DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleBox), new PropertyMetadata(false));
    public static readonly DependencyProperty OnContentProperty = DependencyProperty.Register("OnContent", typeof(object), typeof(ToggleBox), new PropertyMetadata());
    public static readonly DependencyProperty OffContentProperty = DependencyProperty.Register("OffContent", typeof(object), typeof(ToggleBox), new PropertyMetadata());

    /// <summary>
    /// 状态
    /// </summary>
    public bool IsOn {
        get { return (bool)GetValue(IsOnProperty); }
        set { SetValue(IsOnProperty, value); }
    }
    /// <summary>
    /// On State 内容
    /// </summary>
    public object OnContent {
        get { return (object)GetValue(OnContentProperty); }
        set { SetValue(OnContentProperty, value); }
    }
    /// <summary>
    /// Off State 内容
    /// </summary>
    public object OffContent {
        get { return (object)GetValue(OffContentProperty); }
        set { SetValue(OffContentProperty, value); }
    }

    public ToggleBox() {
        Loaded += (_, _) => Content = OnContent;
        DependencyPropertyDescriptor
            .FromProperty(IsOnProperty, this.GetType())
            .AddValueChanged(this, (_, _) => Content = IsOn ? OnContent : OffContent);
    }

}
