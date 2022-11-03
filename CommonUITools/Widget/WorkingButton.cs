using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace CommonUITools.Widget;

/// <summary>
/// 状态 Button
/// 注意不要直接设置 Content、ClickEventHandler
/// </summary>
public class WorkingButton : Button {
    public static readonly DependencyProperty IsWorkingProperty = DependencyProperty.Register("IsWorking", typeof(bool), typeof(WorkingButton), new PropertyMetadata(false));
    public static readonly DependencyProperty NormalContentProperty = DependencyProperty.Register("NormalContent", typeof(object), typeof(WorkingButton), new PropertyMetadata());
    public static readonly DependencyProperty WorkingContentProperty = DependencyProperty.Register("WorkingContent", typeof(object), typeof(WorkingButton), new PropertyMetadata());

    public event RoutedEventHandler? NormalClick;
    public event RoutedEventHandler? WorkingClick;

    /// <summary>
    /// 是否正在工作，默认 false
    /// </summary>
    public bool IsWorking {
        get { return (bool)GetValue(IsWorkingProperty); }
        set { SetValue(IsWorkingProperty, value); }
    }
    /// <summary>
    /// 非 working 时内容
    /// </summary>
    public object NormalContent {
        get { return (object)GetValue(NormalContentProperty); }
        set { SetValue(NormalContentProperty, value); }
    }
    /// <summary>
    /// Working 时内容
    /// </summary>
    public object WorkingContent {
        get { return (object)GetValue(WorkingContentProperty); }
        set { SetValue(WorkingContentProperty, value); }
    }

    public WorkingButton() {
        Click += ClickHandler;
        Initialized += (s, e) => Content = NormalContent;
        DependencyPropertyDescriptor.FromProperty(IsWorkingProperty, this.GetType())
            .AddValueChanged(this, IsWorkingPropertyChangedHandler);
    }

    /// <summary>
    /// Click 事件
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ClickHandler(object sender, RoutedEventArgs e) {
        if (IsWorking) {
            WorkingClick?.Invoke(sender, e);
        } else {
            NormalClick?.Invoke(sender, e);
        }
    }

    /// <summary>
    /// IsWorking 切换
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void IsWorkingPropertyChangedHandler(object? sender, EventArgs e) {
        Content = IsWorking ? WorkingContent : NormalContent;
    }
}
