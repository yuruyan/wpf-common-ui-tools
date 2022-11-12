﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace CommonUITools.Widget;

public partial class LoadingBox : UserControl {

    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(LoadingBox), new PropertyMetadata(20.0));
    /// <summary>
    /// ProcessRing 大小
    /// </summary>
    public double Size {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }
    private readonly Storyboard LoadStoryboard;
    private readonly Storyboard UnLoadStoryboard;

    public LoadingBox() {
        Visibility = Visibility.Collapsed;
        InitializeComponent();
        LoadStoryboard = Resources["LoadStoryboard"] is Storyboard loadStoryboard ? loadStoryboard : new();
        UnLoadStoryboard = Resources["UnLoadStoryboard"] is Storyboard unloadStoryboard ? unloadStoryboard : new();
        UnLoadStoryboard.Completed += (_, _) => {
            Visibility = Visibility.Collapsed;
            Opacity = 0;
        };
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show() {
        Visibility = Visibility.Visible;
        Opacity = 0;
        LoadStoryboard.Begin();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close() {
        Visibility = Visibility.Visible;
        Opacity = 1;
        UnLoadStoryboard.Begin();
    }
}