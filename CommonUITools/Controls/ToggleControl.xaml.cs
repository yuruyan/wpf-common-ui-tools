namespace CommonUITools.Controls;

public class ToggleControl : Control {
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register("State", typeof(bool), typeof(ToggleControl), new PropertyMetadata(true));
    public static readonly DependencyProperty FirstControlProperty = DependencyProperty.Register("FirstControl", typeof(object), typeof(ToggleControl), new PropertyMetadata());
    public static readonly DependencyProperty SecondControlProperty = DependencyProperty.Register("SecondControl", typeof(object), typeof(ToggleControl), new PropertyMetadata());

    /// <summary>
    /// 状态
    /// </summary>
    public bool State {
        get { return (bool)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }
    /// <summary>
    /// 当 <see cref="State"/> 为 true 时显示
    /// </summary>
    public object FirstControl {
        get { return (object)GetValue(FirstControlProperty); }
        set { SetValue(FirstControlProperty, value); }
    }
    /// <summary>
    /// 当 <see cref="State"/> 为 false 时显示
    /// </summary>
    public object SecondControl {
        get { return (object)GetValue(SecondControlProperty); }
        set { SetValue(SecondControlProperty, value); }
    }

    static ToggleControl() {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleControl), new FrameworkPropertyMetadata(typeof(ToggleControl)));
    }

    public ToggleControl() {
        this.SetLoadedOnceEventHandler((_, _) => {
            if (Template.FindName("FirstControl", this) is ContentPresenter firstControl) {
                firstControl.SetBinding(VisibilityProperty, new Binding(nameof(State)) {
                    RelativeSource = new(RelativeSourceMode.TemplatedParent),
                    Converter = (IValueConverter)FindResource("HideIfFalseConverter")
                });
            }
            if (Template.FindName("SecondControl", this) is ContentPresenter secondControl) {
                secondControl.SetBinding(VisibilityProperty, new Binding(nameof(State)) {
                    RelativeSource = new(RelativeSourceMode.TemplatedParent),
                    Converter = (IValueConverter)FindResource("HideIfTrueConverter")
                });
            }
        });
    }
}
