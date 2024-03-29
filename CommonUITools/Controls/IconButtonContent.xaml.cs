﻿namespace CommonUITools.Controls;

public partial class IconButtonContent : UserControl {
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconButtonContent), new PropertyMetadata(""));
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(IconButtonContent), new PropertyMetadata(""));

    /// <summary>
    /// 内容
    /// </summary>
    public string Text {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
    /// <summary>
    /// 图标
    /// </summary>
    public string Icon {
        get { return (string)GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }

    public IconButtonContent() {
        InitializeComponent();
    }
}
