﻿using CommonUITools.Converters;

namespace CommonUITools.Controls;

/// <summary>
/// 文件名、图标 widget
/// </summary>
public partial class FileNameIconBox : UserControl {
    public static readonly DependencyProperty FileNameProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FileNameIconBox), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(FileNameIconBox), new PropertyMetadata(string.Empty));
    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register("IconSize", typeof(double), typeof(FileNameIconBox), new PropertyMetadata(20.0));
    public static readonly DependencyProperty AutoIconProperty = DependencyProperty.Register("AutoIcon", typeof(bool), typeof(FileNameIconBox), new PropertyMetadata(true, AutoIconPropertyChangedHandler));

    public string FileName {
        get { return (string)GetValue(FileNameProperty); }
        set { SetValue(FileNameProperty, value); }
    }
    /// <summary>
    /// 图标，用于显示
    /// </summary>
    public string IconPath {
        get { return (string)GetValue(IconPathProperty); }
        set { SetValue(IconPathProperty, value); }
    }
    /// <summary>
    /// 图标大小，默认 20
    /// </summary>
    public double IconSize {
        get { return (double)GetValue(IconSizeProperty); }
        set { SetValue(IconSizeProperty, value); }
    }
    /// <summary>
    /// 是否自动获取 Icon，默认 true
    /// </summary>
    public bool AutoIcon {
        get { return (bool)GetValue(AutoIconProperty); }
        set { SetValue(AutoIconProperty, value); }
    }
    private readonly IValueConverter FileIconConverter = new FileIconConverter();

    public FileNameIconBox() {
        InitializeComponent();
    }

    private static void AutoIconPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FileNameIconBox self) {
            return;
        }

        // 自动获取
        if (e.NewValue is false) {
            self.FileIconImage.SetBinding(Image.SourceProperty, new Binding(nameof(IconPath)));
            return;
        }
        self.FileIconImage.SetBinding(Image.SourceProperty, new Binding(nameof(FileName)) {
            Converter = self.FileIconConverter
        });
    }
}
