﻿using CommonUITools.Controls;
using ModernWpf;
using ModernWpf.Controls;
using System.Timers;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media.Effects;
using Timer = System.Timers.Timer;

namespace CommonUITools.Utils;

/// <summary>
/// 显示时开始动画
/// </summary>
public static class ScaleAnimationHelper {
    private record State {
        public ScaleAnimationOption ScaleOption { get; set; } = ScaleAnimationOption.TopLeft;
        public Storyboard Storyboard { get; set; } = default!;
    }

    /// <summary>
    /// ScaleTransformFactory
    /// </summary>
    private static class ScaleTransformFactory {
        public static ScaleTransform CreateScaleTransform(FrameworkElement element, ScaleAnimationOption scaleOption) {
            return scaleOption switch {
                ScaleAnimationOption.Center => CreateCenterScaleTransform(element),
                ScaleAnimationOption.TopLeft => CreateTopLeftScaleTransform(element),
                _ => throw new NotImplementedException(),
            };
        }

        private static ScaleTransform CreateCenterScaleTransform(FrameworkElement element) {
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            return new();
        }

        private static ScaleTransform CreateTopLeftScaleTransform(FrameworkElement element) {
            return new ScaleTransform();
        }
    }

    /// <summary>
    /// Storyboard 缓存
    /// </summary>
    private static readonly IDictionary<FrameworkElement, State> StoryboardDict = new Dictionary<FrameworkElement, State>();
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ScaleAnimationHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyProperty ScaleOptionProperty = DependencyProperty.RegisterAttached("ScaleOption", typeof(ScaleAnimationOption), typeof(ScaleAnimationHelper), new PropertyMetadata(ScaleAnimationOption.TopLeft, ScaleOptionPropertyChangedHandler));

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }
    public static ScaleAnimationOption GetScaleOption(DependencyObject obj) {
        return (ScaleAnimationOption)obj.GetValue(ScaleOptionProperty);
    }
    /// <summary>
    /// 缩放选项
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetScaleOption(DependencyObject obj, ScaleAnimationOption value) {
        obj.SetValue(ScaleOptionProperty, value);
    }

    /// <summary>
    /// ScaleAnimationOptionPropertyChangedHandler
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void ScaleOptionPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            var option = (ScaleAnimationOption)e.NewValue;
            CheckAndSetStoryboardDict(element);
            StoryboardDict[element].ScaleOption = option;
            element.RenderTransform = CreateScaleTransform(element, option);
        }
    }

    /// <summary>
    /// 检查并设置 StoryboardDict
    /// </summary>
    /// <param name="element"></param>
    private static void CheckAndSetStoryboardDict(FrameworkElement element) {
        if (!StoryboardDict.ContainsKey(element)) {
            StoryboardDict[element] = new State();
        }
    }

    /// <summary>
    /// IsEnabledPropertyChangedHandler
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            if ((bool)e.NewValue) {
                if (element.IsVisible) {
                    element.BeginStoryboard(GetStoryboard(element));
                } else {
                    element.IsVisibleChanged += IsVisibleChangedHandler;
                }
            } else {
                element.IsVisibleChanged -= IsVisibleChangedHandler;
            }
        }
    }

    /// <summary>
    /// IsVisible 变化
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
        if ((bool)e.NewValue) {
            if (sender is FrameworkElement element) {
                element.BeginStoryboard(GetStoryboard(element));
            }
        }
    }

    /// <summary>
    /// 获取 Storyboard
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static Storyboard GetStoryboard(FrameworkElement element) {
        CheckAndSetStoryboardDict(element);
        if (StoryboardDict[element].Storyboard is not null) {
            return StoryboardDict[element].Storyboard;
        }
        // 必须要有 RenderTransform
        element.RenderTransform = CreateScaleTransform(element, StoryboardDict[element].ScaleOption);
        DoubleAnimation scaleYAnimation = new(0.5, 1, new Duration(TimeSpan.FromMilliseconds(300))) {
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 3 }
        };
        DoubleAnimation scaleXAnimation = new(0.5, 1, new Duration(TimeSpan.FromMilliseconds(300))) {
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 3 }
        };
        Storyboard.SetTarget(scaleYAnimation, element);
        Storyboard.SetTarget(scaleXAnimation, element);
        Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleX)"));
        Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("RenderTransform.(ScaleTransform.ScaleY)"));
        Storyboard storyboard = new() {
            Children = { scaleXAnimation, scaleYAnimation }
        };
        StoryboardDict[element].Storyboard = storyboard;
        return storyboard;
    }

    /// <summary>
    /// 创建 ScaleTransform
    /// </summary>
    /// <param name="element"></param>
    /// <param name="scaleOption"></param>
    /// <returns></returns>
    private static ScaleTransform CreateScaleTransform(FrameworkElement element, ScaleAnimationOption scaleOption) {
        return ScaleTransformFactory.CreateScaleTransform(element, scaleOption);
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    /// <param name="element"></param>
    public static void Dispose(FrameworkElement element) {
        if (StoryboardDict.TryGetValue(element, out var storyboard)) {
            if (storyboard.Storyboard?.CanFreeze == true) {
                storyboard.Storyboard.Freeze();
            }
        }
        StoryboardDict.Remove(element);
        element.IsVisibleChanged -= IsVisibleChangedHandler;
        element.ClearValue(IsEnabledProperty);
        element.ClearValue(ScaleOptionProperty);
        element.ClearValue(UIElement.RenderTransformProperty);
        element.ClearValue(UIElement.RenderTransformOriginProperty);
    }
}

/// <summary>
/// 点击自身开始缩放动画
/// </summary>
public static class CenterScaleAnimationHelper {
    private enum Direction {
        X, Y
    }

    private static readonly int AnimationDuration = 200;
    private static readonly IDictionary<FrameworkElement, IList<EventTrigger>> TriggerCollectionDict = new Dictionary<FrameworkElement, IList<EventTrigger>>();
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(CenterScaleAnimationHelper), new PropertyMetadata(false, IsEnabledChangedHandler));

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        // 添加 trigger
        if ((bool)e.NewValue) {
            foreach (var item in GetTriggerCollection(element)) {
                element.Triggers.Add(item);
            }
        }
        // Remove trigger
        else {
            foreach (var item in GetTriggerCollection(element)) {
                element.Triggers.Remove(item);
            }
        }
    }

    /// <summary>
    /// 获取 TriggerCollection
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static IList<EventTrigger> GetTriggerCollection(FrameworkElement element) {
        // Initialized
        if (!TriggerCollectionDict.TryGetValue(element, out var triggers)) {
            TriggerCollectionDict[element] = triggers = CreateTriggerCollection(element);
        }
        return triggers;
    }

    /// <summary>
    /// 创建 TriggerCollection
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static IList<EventTrigger> CreateTriggerCollection(FrameworkElement element) {
        CheckAndSetRenderTransform(element);
        return new List<EventTrigger>() {
            CreateTrigger(element, FrameworkElement.MouseDownEvent, 0.96),
            CreateTrigger(element, FrameworkElement.MouseUpEvent, 1),
            CreateTrigger(element, FrameworkElement.MouseLeaveEvent, 1),
        };
    }

    /// <summary>
    /// 创建 Trigger
    /// </summary>
    /// <param name="element"></param>
    /// <param name="routedEvent"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private static EventTrigger CreateTrigger(FrameworkElement element, RoutedEvent routedEvent, double to) {
        EventTrigger eventTrigger = new(routedEvent);
        BeginStoryboard beginStoryboard = new();
        int beginTime = to == 1 ? AnimationDuration : 0;
        beginStoryboard.Storyboard = new Storyboard() {
            Children = {
                CreateDoubleAnimation(element, to, Direction.X, beginTime),
                CreateDoubleAnimation(element, to, Direction.Y, beginTime)
            }
        };
        eventTrigger.Actions.Add(beginStoryboard);
        return eventTrigger;
    }

    /// <summary>
    /// 创建 DoubleAnimation
    /// </summary>
    /// <param name="element"></param>
    /// <param name="to"></param>
    /// <param name="direction"></param>
    /// <param name="beginTime"></param>
    /// <returns></returns>
    private static DoubleAnimation CreateDoubleAnimation(FrameworkElement element, double to, Direction direction, int beginTime = 0) {
        DoubleAnimation animation = new(to, new Duration(TimeSpan.FromMilliseconds(AnimationDuration))) {
            BeginTime = TimeSpan.FromMilliseconds(beginTime),
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 3 }
        };
        Storyboard.SetTarget(animation, element);
        Storyboard.SetTargetProperty(animation, new PropertyPath($"RenderTransform.(ScaleTransform.Scale{direction})"));
        return animation;
    }

    /// <summary>
    /// 检查并设置 RenderTransform
    /// </summary>
    /// <param name="element"></param>
    private static void CheckAndSetRenderTransform(FrameworkElement element) {
        element.RenderTransform = CreateScaleTransform(element);
    }

    /// <summary>
    /// 创建 ScaleTransform
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static ScaleTransform CreateScaleTransform(FrameworkElement element) {
        element.RenderTransformOrigin = new Point(0.5, 0.5);
        return new();
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    /// <param name="element"></param>
    public static void Dispose(FrameworkElement element) {
        element.ClearValue(IsEnabledProperty);
        element.ClearValue(UIElement.RenderTransformProperty);
        element.ClearValue(UIElement.RenderTransformOriginProperty);
        // Remove trigger
        if (TriggerCollectionDict.TryGetValue(element, out var triggers)) {
            element.Triggers.RemoveList(triggers);
            triggers.ForEach(t => t.Actions.Clear());
            triggers.Clear();
        }
        TriggerCollectionDict.Remove(element);
    }
}

/// <summary>
/// 淡入动画
/// </summary>
public static class FadeInAnimationHelper {
    /// <summary>
    /// 默认动画持续时间
    /// </summary>
    public const int DefaultDuration = 300;
    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached("Duration", typeof(int), typeof(FadeInAnimationHelper), new PropertyMetadata(DefaultDuration, DurationPropertyChangedHandler));
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(FadeInAnimationHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    private static readonly IDictionary<FrameworkElement, Storyboard> StoryBoardDict = new Dictionary<FrameworkElement, Storyboard>();

    public static bool GetEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(EnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetEnabled(DependencyObject obj, bool value) {
        obj.SetValue(EnabledProperty, value);
    }
    public static int GetDuration(DependencyObject obj) {
        return (int)obj.GetValue(DurationProperty);
    }
    /// <summary>
    /// 持续时间 ms
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetDuration(DependencyObject obj, int value) {
        obj.SetValue(DurationProperty, value);
    }

    /// <summary>
    /// IsEnabled 改变
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        var enabled = (bool)e.NewValue;
        if (d is FrameworkElement element) {
            if (enabled) {
                element.IsVisibleChanged += IsVisibleChangedHandler;
            } else {
                element.IsVisibleChanged -= IsVisibleChangedHandler;
            }
        }
    }

    /// <summary>
    /// Duration 改变
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void DurationPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            // 重新生成
            StoryBoardDict.Remove(element);
            _ = GetStoryboard(element);
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void IsVisibleChangedHandler(object sender, DependencyPropertyChangedEventArgs e) {
        if (sender is FrameworkElement element && e.NewValue is true) {
            element.BeginStoryboard(GetStoryboard(element));
        }
    }

    /// <summary>
    /// 获取 Storyboard，未获取到则自动设置
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static Storyboard GetStoryboard(FrameworkElement element) {
        if (!StoryBoardDict.TryGetValue(element, out var storyboard)) {
            DoubleAnimation doubleAnimation = new(0, 1, new(
                TimeSpan.FromMilliseconds(GetDuration(element))
            ));
            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));
            storyboard = new Storyboard() {
                Children = new(new Timeline[] { doubleAnimation })
            };
            StoryBoardDict[element] = storyboard;
        }
        return storyboard;
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    /// <param name="element"></param>
    public static void Dispose(FrameworkElement element) {
        element.IsVisibleChanged -= IsVisibleChangedHandler;
        element.ClearValue(EnabledProperty);
        element.ClearValue(DurationProperty);
        StoryBoardDict.Remove(element);
    }
}

/// <summary>
/// GridViewColumnHelper
/// </summary>
public static class GridViewColumnHelper {
    public static readonly DependencyProperty HeaderMinWidthProperty = DependencyProperty.RegisterAttached("HeaderMinWidth", typeof(double), typeof(GridViewColumnHelper), new PropertyMetadata(0.0, HeaderMinWidthPropertyChangedHandler));

    public static double GetHeaderMinWidth(DependencyObject obj) {
        return (double)obj.GetValue(HeaderMinWidthProperty);
    }
    /// <summary>
    /// HeaderColumn 最小宽度
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetHeaderMinWidth(DependencyObject obj, double value) {
        obj.SetValue(HeaderMinWidthProperty, value);
    }

    private static void HeaderMinWidthPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is GridViewColumnHeader header) {
            // 设置 SizeChanged 事件
            header.SizeChanged -= GridViewColumnHeaderSizeChangedHandler;
            header.SizeChanged += GridViewColumnHeaderSizeChangedHandler;
        }
    }

    private static void GridViewColumnHeaderSizeChangedHandler(object sender, SizeChangedEventArgs args) {
        if (sender is GridViewColumnHeader header) {
            if (args.NewSize.Width <= GetHeaderMinWidth(header)) {
                // 可能为 null
                if (header.Column is null) {
                    return;
                }
                header.Column.Width = GetHeaderMinWidth(header);
                args.Handled = true;
            }
        }
    }
}

/// <summary>
/// DragDropHelper，DragOver 时背景会发生变化
/// 默认背景为 DragDropOverBackgroundBrush
/// </summary>
public static class DragDropHelper {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyProperty OverBackgroundProperty = DependencyProperty.RegisterAttached("OverBackground", typeof(Brush), typeof(DragDropHelper), new PropertyMetadata());
    public static readonly DependencyProperty BackgroundPropertyProperty = DependencyProperty.RegisterAttached("BackgroundProperty", typeof(DependencyProperty), typeof(DragDropHelper), new PropertyMetadata());
    private static readonly Brush TransparentBackgroundBrush = new SolidColorBrush(Colors.Transparent);

    public static DependencyProperty GetBackgroundProperty(DependencyObject obj) {
        return (DependencyProperty)obj.GetValue(BackgroundPropertyProperty);
    }
    /// <summary>
    /// BackgroundPropertyProperty
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetBackgroundProperty(DependencyObject obj, DependencyProperty value) {
        obj.SetValue(BackgroundPropertyProperty, value);
    }
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }
    public static Brush GetOverBackground(DependencyObject obj) {
        return (Brush)obj.GetValue(OverBackgroundProperty);
    }
    /// <summary>
    /// 拖拽进入区域时背景颜色
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetOverBackground(DependencyObject obj, Brush value) {
        obj.SetValue(OverBackgroundProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement && d is not FrameworkContentElement) {
            return;
        }

        var isEnabled = (bool)e.NewValue;
        // 设置事件
        // 文本框特殊处理
        if (d is TextBoxBase textBox) {
            if (isEnabled) {
                textBox.PreviewDragOver += TextBoxPreviewDragOverHandler;
            } else {
                textBox.PreviewDragOver -= TextBoxPreviewDragOverHandler;
            }
        }
        // FrameworkElement
        if (d is FrameworkElement element) {
            if (isEnabled) {
                element.AllowDrop = true;
                element.PreviewDragEnter += PreviewDragEnterHandler;
                element.PreviewDragLeave += PreviewDragLeaveHandler;
                element.PreviewDrop += PreviewDropHandler;
            } else {
                element.AllowDrop = false;
                element.PreviewDragEnter -= PreviewDragEnterHandler;
                element.PreviewDragLeave -= PreviewDragLeaveHandler;
                element.PreviewDrop -= PreviewDropHandler;
            }
        }
        // FrameworkContentElement
        else if (d is FrameworkContentElement contentElement) {
            if (isEnabled) {
                contentElement.AllowDrop = true;
                contentElement.PreviewDragEnter += PreviewDragEnterHandler;
                contentElement.PreviewDragLeave += PreviewDragLeaveHandler;
                contentElement.PreviewDrop += PreviewDropHandler;
            } else {
                contentElement.AllowDrop = false;
                contentElement.PreviewDragEnter -= PreviewDragEnterHandler;
                contentElement.PreviewDragLeave -= PreviewDragLeaveHandler;
                contentElement.PreviewDrop -= PreviewDropHandler;
            }
        }
    }

    /// <summary>
    /// 文本框 PreviewDragOver
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void TextBoxPreviewDragOverHandler(object sender, DragEventArgs e) {
        e.Handled = true;
        e.Effects = DragDropEffects.Copy;
    }

    private static void PreviewDropHandler(object sender, DragEventArgs e) => ResetBackground(sender);

    private static void PreviewDragLeaveHandler(object sender, DragEventArgs e) => ResetBackground(sender);

    /// <summary>
    /// 重置 Background
    /// </summary>
    /// <param name="sender"></param>
    private static void ResetBackground(object sender) {
        if (sender is DependencyObject element && GetBackgroundProperty(element) is { } backgroundProperty) {
            element.SetValue(backgroundProperty, TransparentBackgroundBrush);
        }
    }

    private static void PreviewDragEnterHandler(object sender, DragEventArgs e) {
        if (sender is FrameworkElement element && GetBackgroundProperty(element) is { } backgroundProperty) {
            var newBrush = GetOverBackground(element);
            newBrush ??= element.TryFindResource("DragDropOverBackgroundBrush") as SolidColorBrush;
            newBrush ??= TransparentBackgroundBrush;
            element.SetValue(backgroundProperty, newBrush);
        }
    }

    /// <summary>
    /// 释放引用
    /// </summary>
    /// <param name="dp"></param>
    public static void Dispose(DependencyObject dp) {
        dp.ClearValue(IsEnabledProperty);
        dp.ClearValue(OverBackgroundProperty);
        if (GetBackgroundProperty(dp) is DependencyProperty backgroundProperty) {
            dp.ClearValue(backgroundProperty);
        }
        dp.ClearValue(BackgroundPropertyProperty);

        #region Remove event handlers
        if (dp is TextBoxBase textBox) {
            textBox.PreviewDragOver -= TextBoxPreviewDragOverHandler;
        }
        if (dp is FrameworkElement element) {
            element.PreviewDragEnter -= PreviewDragEnterHandler;
            element.PreviewDragLeave -= PreviewDragLeaveHandler;
            element.PreviewDrop -= PreviewDropHandler;
        } else if (dp is FrameworkContentElement contentElement) {
            contentElement.PreviewDragEnter -= PreviewDragEnterHandler;
            contentElement.PreviewDragLeave -= PreviewDragLeaveHandler;
            contentElement.PreviewDrop -= PreviewDropHandler;
        }
        #endregion
    }
}

/// <summary>
/// 加载状态，无法作用于 <see cref="Window"/>
/// </summary>
public static class LoadingBoxHelper {
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(LoadingBoxHelper), new PropertyMetadata(false, IsLoadingPropertyChangedHandler));
    public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached("Size", typeof(double), typeof(LoadingBoxHelper), new PropertyMetadata(LoadingBox.DefaultSize, SizePropertyChangedHandler));
    private static readonly IDictionary<FrameworkElement, LoadingBox> LoadingBoxDict = new Dictionary<FrameworkElement, LoadingBox>();

    public static bool GetIsLoading(DependencyObject obj) {
        return (bool)obj.GetValue(IsLoadingProperty);
    }
    /// <summary>
    /// 是否显示加载
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetIsLoading(DependencyObject obj, bool value) {
        obj.SetValue(IsLoadingProperty, value);
    }
    public static double GetSize(DependencyObject obj) {
        return (double)obj.GetValue(SizeProperty);
    }
    /// <summary>
    /// loading size
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetSize(DependencyObject obj, double value) {
        obj.SetValue(SizeProperty, value);
    }

    private static void IsLoadingPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        // Has Initialized
        if (LoadingBoxDict.TryGetValue(element, out var loading)) {
            if (e.NewValue is true) {
                LoadingBoxDict[element].Show();
            } else {
                LoadingBoxDict[element].Close();
            }
        }
        // Initialized
        if (element.IsLoaded) {
            EnsureLoadingBoxIsInitialized(element);
            if (e.NewValue is true) {
                LoadingBoxDict[element].Show();
            } else {
                LoadingBoxDict[element].Close();
            }
            return;
        }
        // 等待加载
        element.SetLoadedOnceEventHandler((sender, _) => {
            if (sender is FrameworkElement element) {
                EnsureLoadingBoxIsInitialized(element);
                if (GetIsLoading(element)) {
                    LoadingBoxDict[element].Show();
                } else {
                    LoadingBoxDict[element].Close();
                }
            }
        });
    }

    private static void SizePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        double size = (double)e.NewValue;
        // Is Initialized
        if (LoadingBoxDict.TryGetValue(element, out var loadingBox)) {
            loadingBox.Size = size;
            return;
        }
        if (element.IsLoaded) {
            EnsureLoadingBoxIsInitialized(element);
            LoadingBoxDict[element].Size = size;
            return;
        }
        // Waiting for Loaded
        element.SetLoadedOnceEventHandler((sender, _) => {
            if (sender is FrameworkElement element) {
                EnsureLoadingBoxIsInitialized(element);
                LoadingBoxDict[element].Size = GetSize(element);
            }
        });
    }

    /// <summary>
    /// 确保已创建 LoadingBox
    /// </summary>
    /// <param name="element"></param>
    private static void EnsureLoadingBoxIsInitialized(FrameworkElement element) {
        // 已经加载过
        if (LoadingBoxDict.ContainsKey(element)) {
            return;
        }
        if (AdornerLayer.GetAdornerLayer(element) is not AdornerLayer adornerLayer) {
            return;
        }
        var loadingBox = new LoadingBox();
        LoadingBoxDict[element] = loadingBox;
        adornerLayer.Add(new CommonAdorner(
            element,
            new CommonAdorner.ElementInfo[] { new(loadingBox) }
        ));
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    /// <param name="element"></param>
    public static void Dispose(FrameworkElement element) {
        element.ClearValue(SizeProperty);
        element.ClearValue(IsLoadingProperty);
        LoadingBoxDict.Remove(element);
    }
}

/// <summary>
/// Name
/// </summary>
public static class NameHelper {
    public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(NameHelper), new PropertyMetadata());

    public static string GetName(DependencyObject obj) {
        return (string)obj.GetValue(NameProperty);
    }
    /// <summary>
    /// Name
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetName(DependencyObject obj, string value) {
        obj.SetValue(NameProperty, value);
    }
}

/// <summary>
/// 长按事件
/// </summary>
/// <remarks>
/// 注意在其他自定义事件中根据 <see cref="GetIsLongPressEventHappend"/> 来做进一步处理
/// </remarks>
public static class LongPressHelper {
    private class LongPressEventInfo {
        public System.Timers.Timer Timer { get; } = new(100);
        public DateTime MouseDownTime { get; set; } = DateTime.Now;
        public bool Invoked { get; set; }
    }

    private const int LongPressTimeThreshold = 700;
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(LongPressHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyPropertyKey IsLongPressEventHappendPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsLongPressEventHappend", typeof(bool), typeof(LongPressHelper), new PropertyMetadata(false));
    public static readonly DependencyProperty IsLongPressEventHappendProperty = IsLongPressEventHappendPropertyKey.DependencyProperty;
    public static readonly DependencyProperty LongPressProperty = DependencyProperty.RegisterAttached("LongPress", typeof(EventHandler), typeof(LongPressHelper), new PropertyMetadata());
    private static readonly IDictionary<FrameworkElement, LongPressEventInfo> ElementLongPressEventInfoDict = new Dictionary<FrameworkElement, LongPressEventInfo>();

    /// <summary>
    /// 是否触发了长按事件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsLongPressEventHappend(DependencyObject obj) {
        return (bool)obj.GetValue(IsLongPressEventHappendProperty);
    }
    public static EventHandler GetLongPress(DependencyObject obj) {
        return (EventHandler)obj.GetValue(LongPressProperty);
    }
    /// <summary>
    /// 长按事件
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetLongPress(DependencyObject obj, EventHandler value) {
        obj.SetValue(LongPressProperty, value);
    }
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        var isEnabled = (bool)e.NewValue;
        // 初始化
        if (!ElementLongPressEventInfoDict.ContainsKey(element)) {
            Init(element);
        }

        if (isEnabled) {
            element.PreviewMouseLeftButtonDown += PreviewLeftButtonDown;
            element.PreviewMouseLeftButtonUp += PreviewLeftButtonUp;
        } else {
            ElementLongPressEventInfoDict[element].Timer.Stop();
            element.PreviewMouseLeftButtonDown -= PreviewLeftButtonDown;
            element.PreviewMouseLeftButtonUp -= PreviewLeftButtonUp;
        }
    }

    private static void Init(FrameworkElement element) {
        var info = new LongPressEventInfo();
        ElementLongPressEventInfoDict[element] = info;
        info.Timer.Elapsed += (_, _) => {
            // 触发长按事件
            if ((DateTime.Now - info.MouseDownTime).TotalMilliseconds >= LongPressTimeThreshold) {
                info.Timer.Stop();
                info.Invoked = true;
                Application.Current.Dispatcher.Invoke(() => {
                    element.SetValue(IsLongPressEventHappendPropertyKey, true);
                    GetLongPress(element)?.Invoke(element, EventArgs.Empty);
                });
            }
        };
    }

    private static void PreviewLeftButtonDown(object sender, MouseButtonEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }

        var info = ElementLongPressEventInfoDict[element];
        // 重置
        info.Invoked = false;
        element.SetValue(IsLongPressEventHappendPropertyKey, false);
        info.MouseDownTime = DateTime.Now;
        info.Timer.Start();
    }

    private static void PreviewLeftButtonUp(object sender, MouseButtonEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }

        ElementLongPressEventInfoDict[element].Timer.Stop();
    }

    /// <summary>
    /// 清除引用
    /// </summary>
    /// <param name="element"></param>
    public static void Dispose(FrameworkElement element) {
        if (ElementLongPressEventInfoDict.TryGetValue(element, out var info)) {
            ElementLongPressEventInfoDict.Remove(element);
            info.Timer.Stop();
        }
        element.ClearValue(LongPressProperty);
        element.ClearValue(IsEnabledProperty);
        element.ClearValue(IsLongPressEventHappendProperty);
        element.PreviewMouseLeftButtonDown -= PreviewLeftButtonDown;
        element.PreviewMouseLeftButtonUp -= PreviewLeftButtonUp;
    }
}

/// <summary>
/// 显示 / 隐藏动画
/// </summary>
/// <remarks>不应直接设置 Visibility, 而是使用 <see cref="SetVisible(FrameworkElement, bool)"/></remarks>
public static class VisibilityAnimationHelper {
    //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty VisibleStoryboardProperty = DependencyProperty.RegisterAttached("VisibleStoryboard", typeof(Storyboard), typeof(VisibilityAnimationHelper), new PropertyMetadata(VisibleStoryboardPropertyChangedHandler));
    public static readonly DependencyProperty InVisibleStoryboardProperty = DependencyProperty.RegisterAttached("InVisibleStoryboard", typeof(Storyboard), typeof(VisibilityAnimationHelper), new PropertyMetadata(InVisibleStoryboardPropertyChangedHandler));

    public static Storyboard GetVisibleStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(VisibleStoryboardProperty);
    }
    /// <summary>
    /// 显示动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetVisibleStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(VisibleStoryboardProperty, value);
    }
    public static Storyboard GetInVisibleStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(InVisibleStoryboardProperty);
    }
    /// <summary>
    /// 隐藏动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetInVisibleStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(InVisibleStoryboardProperty, value);
    }

    public static void SetVisible(this FrameworkElement element, bool isVisible) {
        if ((isVisible ? GetVisibleStoryboard(element) : GetInVisibleStoryboard(element)) is Storyboard storyboard) {
            element.Visibility = Visibility.Visible;
            storyboard.Begin();
        }
        // No storyboard
        else {
            element.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }

    private static void VisibleStoryboardPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (e.NewValue is Storyboard storyboard) {
            storyboard.Completed -= VisibleStoryboardCompletedHandler;
            storyboard.Completed += VisibleStoryboardCompletedHandler;
        }
    }

    private static void InVisibleStoryboardPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (e.NewValue is Storyboard storyboard) {
            storyboard.Completed -= InVisibleStoryboardCompletedHandler;
            storyboard.Completed += InVisibleStoryboardCompletedHandler;
        }
    }

    private static void VisibleStoryboardCompletedHandler(object? sender, EventArgs e) {
        if (sender is Clock clock && clock.Timeline is Storyboard storyboard) {
            if (storyboard.Children.FirstOrDefault() is Timeline timeline) {
                if (Storyboard.GetTarget(timeline) is FrameworkElement element) {
                    element.Visibility = Visibility.Visible;
                }
            }
        }
    }

    private static void InVisibleStoryboardCompletedHandler(object? sender, EventArgs e) {
        if (sender is Clock clock && clock.Timeline is Storyboard storyboard) {
            if (storyboard.Children.FirstOrDefault() is Timeline timeline) {
                if (Storyboard.GetTarget(timeline) is FrameworkElement element) {
                    element.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}

/// <summary>
/// 添加阴影
/// </summary>
public static class DropShadowEffectHelper {
    public static readonly DependencyProperty WeightProperty = DependencyProperty.RegisterAttached("Weight", typeof(DropShadowEffectWeight), typeof(DropShadowEffectHelper), new PropertyMetadata(DropShadowEffectWeight.None, EffectWeightChangedHandler));
    private static readonly Color DropShadowColor = "#dfdfdf".ToColor();

    public static DropShadowEffectWeight GetWeight(DependencyObject obj) {
        return (DropShadowEffectWeight)obj.GetValue(WeightProperty);
    }
    /// <summary>
    /// 阴影程度
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetWeight(DependencyObject obj, DropShadowEffectWeight value) {
        obj.SetValue(WeightProperty, value);
    }

    private static void EffectWeightChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        if (e.NewValue is DropShadowEffectWeight weight) {
            element.Effect = weight switch {
                DropShadowEffectWeight.None => null,
                DropShadowEffectWeight.Lighter => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 8 },
                DropShadowEffectWeight.Normal => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 16 },
                DropShadowEffectWeight.Darker => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 32 },
                _ => null
            };
        }
    }
}

/// <summary>
/// 移除 ListBox DragSelection 行为
/// </summary>
public static class RemoveListBoxDefaultSelectionBehavior {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RemoveListBoxDefaultSelectionBehavior), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        if (e.NewValue is true) {
            element.PreviewMouseMove += PreviewMouseMoveHandler;
        } else {
            element.PreviewMouseMove -= PreviewMouseMoveHandler;
        }
    }

    private static void PreviewMouseMoveHandler(object sender, MouseEventArgs e) {
        if (sender is UIElement element) {
            element.ReleaseMouseCapture();
        }
    }

    public static void Dispose(FrameworkElement element) {
        element.PreviewMouseMove -= PreviewMouseMoveHandler;
        element.ClearValue(IsEnabledProperty);
    }
}

/// <summary>
/// 移除 ListBoxItem MouseRightClickSelection 行为
/// </summary>
public static class RemoveListBoxItemDefaultSelectionBehavior {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RemoveListBoxItemDefaultSelectionBehavior), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        if (e.NewValue is true) {
            element.MouseRightButtonDown += MouseRightButtonDownHandler;
        } else {
            element.MouseRightButtonDown -= MouseRightButtonDownHandler;
        }
    }

    private static void MouseRightButtonDownHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
    }

    public static void Dispose(FrameworkElement element) {
        element.MouseRightButtonDown -= MouseRightButtonDownHandler;
        element.ClearValue(IsEnabledProperty);
    }
}

/// <summary>
/// 双击鼠标事件，DoubleMouseUp
/// </summary>
public static class DoubleMouseClickHelper {
    public static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached("Handler", typeof(MouseButtonEventHandler), typeof(DoubleMouseClickHelper), new PropertyMetadata(MouseButtonEventHandlerChangedHandler));
    private static readonly ushort DoubleClickTime = PInvokeUtils.GetDoubleClickTime();
    private static readonly IDictionary<UIElement, long> ElementClickTime = new Dictionary<UIElement, long>();
    private static readonly IDictionary<UIElement, long> ElementEventRaisedTime = new Dictionary<UIElement, long>();

    public static MouseButtonEventHandler GetHandler(DependencyObject obj) {
        return (MouseButtonEventHandler)obj.GetValue(HandlerProperty);
    }
    /// <summary>
    /// MouseButtonEventHandler
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static void SetHandler(DependencyObject obj, MouseButtonEventHandler value) {
        obj.SetValue(HandlerProperty, value);
    }

    private static void MouseButtonEventHandlerChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        if (e.NewValue is not null) {
            element.MouseLeftButtonUp -= ElementMouseLeftButtonUpHandler;
            element.MouseLeftButtonUp += ElementMouseLeftButtonUpHandler;
        } else {
            element.MouseLeftButtonUp -= ElementMouseLeftButtonUpHandler;
        }
    }

    private static void ElementMouseLeftButtonUpHandler(object sender, MouseButtonEventArgs e) {
        var nowTime = DateTimeUtils.CuruentMilliseconds;
        var elem = (UIElement)sender;
        if (ElementClickTime.TryGetValue(elem, out var lastClickTime)) {
            // Raise event
            if (nowTime - lastClickTime <= DoubleClickTime) {
                // Restrict raise times
                if (!ElementEventRaisedTime.TryGetValue(elem, out var lastRaisedTime) || nowTime - lastRaisedTime >= DoubleClickTime) {
                    ElementEventRaisedTime[elem] = nowTime;
                    GetHandler(elem).Invoke(sender, e);
                }
            }
        }
        ElementClickTime[elem] = DateTimeUtils.CuruentMilliseconds;
    }

    public static void Dispose(FrameworkElement element) {
        element.MouseLeftButtonUp -= ElementMouseLeftButtonUpHandler;
        ElementClickTime.Remove(element);
        ElementEventRaisedTime.Remove(element);
        element.ClearValue(HandlerProperty);
    }
}

/// <summary>
/// MouseClickHelper
/// </summary>
public static class MouseClickHelper {
    /// <summary>
    /// MouseLeftClick, set null to clear
    /// </summary>
    public static readonly DependencyProperty MouseLeftClickProperty = DependencyProperty.RegisterAttached("MouseLeftClick", typeof(MouseButtonEventHandler), typeof(MouseClickHelper), new PropertyMetadata(MouseLeftClickPropertyChangedHandler));
    private static readonly DependencyProperty IsMouseLeftButtonDownProperty = DependencyProperty.RegisterAttached("IsMouseLeftButtonDown", typeof(bool), typeof(MouseClickHelper), new PropertyMetadata(false));

    public static MouseButtonEventHandler GetMouseLeftClick(DependencyObject obj) {
        return (MouseButtonEventHandler)obj.GetValue(MouseLeftClickProperty);
    }
    public static void SetMouseLeftClick(DependencyObject obj, MouseButtonEventHandler value) {
        obj.SetValue(MouseLeftClickProperty, value);
    }
    private static bool GetIsMouseLeftButtonDown(DependencyObject obj) {
        return (bool)obj.GetValue(IsMouseLeftButtonDownProperty);
    }
    private static void SetIsMouseLeftButtonDown(DependencyObject obj, bool value) {
        obj.SetValue(IsMouseLeftButtonDownProperty, value);
    }

    private static void MouseLeftClickPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            element.MouseLeftButtonDown -= ElementMouseLeftButtonDownHandler;
            element.MouseLeftButtonUp -= ElementMouseLeftButtonUpHandler;
            if (e.NewValue is not null) {
                element.MouseLeftButtonDown += ElementMouseLeftButtonDownHandler;
                element.MouseLeftButtonUp += ElementMouseLeftButtonUpHandler;
            }
        } else if (d is FrameworkContentElement contentElement) {
            contentElement.MouseLeftButtonDown -= ElementMouseLeftButtonDownHandler;
            contentElement.MouseLeftButtonUp -= ElementMouseLeftButtonUpHandler;
            if (e.NewValue is not null) {
                contentElement.MouseLeftButtonDown += ElementMouseLeftButtonDownHandler;
                contentElement.MouseLeftButtonUp += ElementMouseLeftButtonUpHandler;
            }
        }
    }

    private static void ElementMouseLeftButtonDownHandler(object sender, MouseButtonEventArgs e) {
        if (sender is DependencyObject dp) {
            SetIsMouseLeftButtonDown(dp, true);
        }
    }

    private static void ElementMouseLeftButtonUpHandler(object sender, MouseButtonEventArgs e) {
        if (sender is not DependencyObject dp) {
            return;
        }
        if (GetIsMouseLeftButtonDown(dp) && GetMouseLeftClick(dp) is { } handler) {
            handler(sender, e);
        }
        // Clear
        SetIsMouseLeftButtonDown(dp, false);
    }
}

/// <summary>
/// 设置 ListViewGroup Style
/// </summary>
public static class ListViewGroupHelper {
    public static readonly DependencyProperty EnableDefaultGroupStyleProperty = DependencyProperty.RegisterAttached("EnableDefaultGroupStyle", typeof(bool), typeof(ListViewGroupHelper), new PropertyMetadata(false, EnableDefaultGroupStylePropertyChangedHandler));
    private static readonly IDictionary<ItemsControl, GroupStyle> GroupStyleDict = new Dictionary<ItemsControl, GroupStyle>();

    public static bool GetEnableDefaultGroupStyle(DependencyObject obj) {
        return (bool)obj.GetValue(EnableDefaultGroupStyleProperty);
    }
    /// <summary>
    /// 设置默认 GroupStyle
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetEnableDefaultGroupStyle(DependencyObject obj, bool value) {
        obj.SetValue(EnableDefaultGroupStyleProperty, value);
    }

    private static void EnableDefaultGroupStylePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not ItemsControl control) {
            throw new InvalidOperationException("Element is not of type ItemsControl");
        }
        // 初始化 GroupStyleDict
        if (!GroupStyleDict.ContainsKey(control)) {
            GroupStyleDict[control] = CreateGroupStyle(control);
        }
        if ((bool)e.NewValue) {
            control.GroupStyle.Add(GroupStyleDict[control]);
        } else {
            control.GroupStyle.Remove(GroupStyleDict[control]);
        }
    }

    /// <summary>
    /// 创建 GroupStyle
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
    private static GroupStyle CreateGroupStyle(ItemsControl control) {
        var template = new DataTemplate();
        var elementFactory = new FrameworkElementFactory(typeof(TextBlock));
        elementFactory.SetValue(TextBlock.StyleProperty, control.FindResource("ItemsControlGroupTextBlockStyle"));
        elementFactory.SetBinding(TextBlock.TextProperty, new Binding("Name"));
        template.VisualTree = elementFactory;
        return new GroupStyle() {
            HeaderTemplate = template
        };
    }
}

/// <summary>
/// ContextMenuHelper
/// </summary>
public static class ContextMenuHelper {
    public static readonly DependencyProperty OpenOnMouseLeftClickProperty = DependencyProperty.RegisterAttached("OpenOnMouseLeftClick", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, OpenOnMouseLeftClickPropertyChangedHandler));
    public static readonly DependencyProperty EnableOpeningAnimationProperty = DependencyProperty.RegisterAttached("EnableOpeningAnimation", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, EnableOpeningAnimationPropertyChangedHandler));
    public static readonly DependencyProperty CenterHorizontalProperty = DependencyProperty.RegisterAttached("CenterHorizontal", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, CenterHorizontalPropertyChangedHandler));
    private static readonly DependencyProperty OpeningBottomStoryboardProperty = DependencyProperty.RegisterAttached("OpeningBottomStoryboard", typeof(Storyboard), typeof(ContextMenuHelper), new PropertyMetadata());
    private static readonly DependencyProperty OpeningTopStoryboardProperty = DependencyProperty.RegisterAttached("OpeningTopStoryboard", typeof(Storyboard), typeof(ContextMenuHelper), new PropertyMetadata());

    public static bool GetOpenOnMouseLeftClick(DependencyObject obj) {
        return (bool)obj.GetValue(OpenOnMouseLeftClickProperty);
    }
    /// <summary>
    /// 鼠标左键单击时是否显示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>设置在宿主上</remarks>
    public static void SetOpenOnMouseLeftClick(DependencyObject obj, bool value) {
        obj.SetValue(OpenOnMouseLeftClickProperty, value);
    }
    public static bool GetEnableOpeningAnimation(DependencyObject obj) {
        return (bool)obj.GetValue(EnableOpeningAnimationProperty);
    }
    /// <summary>
    /// 设置显示动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>设置在 ContextMenu 上</remarks>
    public static void SetEnableOpeningAnimation(DependencyObject obj, bool value) {
        obj.SetValue(EnableOpeningAnimationProperty, value);
    }
    public static bool GetCenterHorizontal(DependencyObject obj) {
        return (bool)obj.GetValue(CenterHorizontalProperty);
    }
    /// <summary>
    /// 是否水平居中
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetCenterHorizontal(DependencyObject obj, bool value) {
        obj.SetValue(CenterHorizontalProperty, value);
    }
    private static Storyboard GetOpeningBottomStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(OpeningBottomStoryboardProperty);
    }
    /// <summary>
    /// Open Storyboard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    private static void SetOpeningBottomStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(OpeningBottomStoryboardProperty, value);
    }
    private static Storyboard GetOpeningTopStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(OpeningTopStoryboardProperty);
    }
    /// <summary>
    /// OpeningTopStoryboard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    private static void SetOpeningTopStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(OpeningTopStoryboardProperty, value);
    }

    private static void EnableOpeningAnimationPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not ContextMenu menu) {
            return;
        }
        if (e.NewValue is true) {
            menu.Opened += ContextMenuOpenedHandler;
        } else {
            menu.Opened -= ContextMenuOpenedHandler;
        }
    }

    private static void ContextMenuOpenedHandler(object sender, RoutedEventArgs e) {
        if (sender is not ContextMenu menu) {
            return;
        }
        GetOpeningStoryboard(menu).Begin();
    }

    private static Storyboard GetOpeningStoryboard(ContextMenu menu) {
        var bottomHeight = Math.Abs(menu.PlacementTarget.PointFromScreen(new()).Y)
            + Mouse.GetPosition(menu.PlacementTarget).Y
            + menu.ActualHeight;
        if (bottomHeight >= SystemParameters.MaximizedPrimaryScreenHeight) {
            if (GetOpeningTopStoryboard(menu) is not Storyboard storyboard) {
                storyboard = menu.CreateOpeningStoryboard(false);
                SetOpeningTopStoryboard(menu, storyboard);
            }
            return storyboard;
        } else {
            if (GetOpeningBottomStoryboard(menu) is not Storyboard storyboard) {
                storyboard = menu.CreateOpeningStoryboard(true);
                SetOpeningBottomStoryboard(menu, storyboard);
            }
            return storyboard;
        }
    }

    private static void OpenOnMouseLeftClickPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        if (e.NewValue is true) {
            element.PreviewMouseLeftButtonDown += ShowContextMenuLeftButtonDownHandler;
            element.PreviewMouseLeftButtonUp += ShowContextMenuLeftButtonUpHandler;
        } else {
            element.PreviewMouseLeftButtonDown -= ShowContextMenuLeftButtonDownHandler;
            element.PreviewMouseLeftButtonUp -= ShowContextMenuLeftButtonUpHandler;
        }
    }

    /// <summary>
    /// Capture mouse
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ShowContextMenuLeftButtonDownHandler(object sender, MouseButtonEventArgs e) {
        if (sender is IInputElement inputElement) {
            _ = inputElement.CaptureMouse();
        }
    }

    /// <summary>
    /// 显示 ContextMenu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ShowContextMenuLeftButtonUpHandler(object sender, MouseButtonEventArgs e) {
        if (sender is IInputElement inputElement) {
            inputElement.ReleaseMouseCapture();
        }
        if (sender is FrameworkElement element && element.IsMouseOver && element.ContextMenu is not null) {
            element.ContextMenu.PlacementTarget = element;
            element.ContextMenu.IsOpen = true;
            element.UpdateDefaultStyle();
        }
    }

    private static void CenterHorizontalPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not ContextMenu menu) {
            return;
        }
        if (e.NewValue is true) {
            menu.Opened -= CenterHorizontalContextMenuOpenedHandler;
            menu.Opened += CenterHorizontalContextMenuOpenedHandler;
        } else {
            menu.Opened -= CenterHorizontalContextMenuOpenedHandler;
        }
    }

    private static void CenterHorizontalContextMenuOpenedHandler(object sender, RoutedEventArgs e) {
        if (sender is not ContextMenu menu) {
            return;
        }
        if (menu.PlacementTarget is not FrameworkElement target) {
            return;
        }
        menu.HorizontalOffset = 0;
        var point = menu.TranslatePoint(new(), target);
        menu.HorizontalOffset = -point.X;
        menu.HorizontalOffset -= (menu.ActualWidth - target.ActualWidth) / 2;
    }

    public static void Dispose(FrameworkElement element) {
        if (element.ContextMenu is ContextMenu menu) {
            menu.Opened -= ContextMenuOpenedHandler;
            menu.Opened -= CenterHorizontalContextMenuOpenedHandler;
            menu.ClearValue(OpeningBottomStoryboardProperty);
            menu.ClearValue(OpeningTopStoryboardProperty);
            menu.ClearValue(EnableOpeningAnimationProperty);
        }
        element.PreviewMouseLeftButtonDown -= ShowContextMenuLeftButtonDownHandler;
        element.PreviewMouseLeftButtonUp -= ShowContextMenuLeftButtonUpHandler;
        element.ClearValue(OpenOnMouseLeftClickProperty);
        element.ClearValue(CenterHorizontalProperty);
    }
}

/// <summary>
/// 控件自动显示 / 隐藏。点击外部隐藏，点击内部不会
/// </summary>
public static class AutoHideHelper {
    private class State {
        public State(UIElement element) {
            Element = element;
        }

        public UIElement Element { get; set; }
        public Window ElementWindow { get; set; } = Application.Current.MainWindow;
        public bool IsToggled { get; set; }
        public bool PreventOpen { get; set; }
    }

    /// <summary>
    /// One to one
    /// </summary>
    private static readonly IDictionary<string, State> TargetElementStateDict = new Dictionary<string, State>();
    /// <summary>
    /// Many to one
    /// </summary>
    private static readonly IDictionary<DependencyObject, string> SourceElementTargetIdDict = new Dictionary<DependencyObject, string>();

    /// <summary>
    /// 启用当点击 <paramref name="element"/> 外部时自动隐藏
    /// </summary>
    /// <param name="element">要隐藏 / 显示的控件</param>
    /// <param name="targetId">标识 <paramref name="element"/> id</param>
    public static void EnableAutoHideOnClickOuter(FrameworkElement element, string targetId) {
        TargetElementStateDict[targetId] = new(element);
        if (Window.GetWindow(element) is Window window) {
            TargetElementStateDict[targetId].ElementWindow = window;
            window.PreviewMouseUp -= WindowPreviewMouseUpHandler;
            window.PreviewMouseUp += WindowPreviewMouseUpHandler;
        } else {
            element.SetLoadedOnceEventHandler((_, _) => {
                var window = Window.GetWindow(element);
                TargetElementStateDict[targetId].ElementWindow = window;
                window.PreviewMouseUp -= WindowPreviewMouseUpHandler;
                window.PreviewMouseUp += WindowPreviewMouseUpHandler;
            });
        }
    }

    /// <summary>
    /// 设置点击 <paramref name="dp"/> 时显示 <paramref name="targetId"/> 对应的控件
    /// </summary>
    /// <param name="dp"></param>
    /// <param name="targetId"></param>
    public static void SetOpenOnClick(DependencyObject dp, string targetId) {
        SourceElementTargetIdDict[dp] = targetId;
        if (dp is UIElement element) {
            element.PreviewMouseDown -= PreviewMouseDownHandler;
            element.PreviewMouseDown += PreviewMouseDownHandler;
            // 按钮
            if (element is ButtonBase button) {
                button.Click -= ButtonClickHandler;
                button.Click += ButtonClickHandler;
            } else {
                element.MouseUp -= MouseUpHandler;
                element.MouseUp += MouseUpHandler;
            }
        } else if (dp is ContentElement contentElement) {
            contentElement.PreviewMouseDown -= PreviewMouseDownHandler;
            contentElement.PreviewMouseDown += PreviewMouseDownHandler;
            contentElement.MouseUp -= MouseUpHandler;
            contentElement.MouseUp += MouseUpHandler;
        }
    }

    private static void WindowPreviewMouseUpHandler(object sender, MouseButtonEventArgs e) {
        foreach (var state in TargetElementStateDict.Values) {
            // Invisible
            if (!state.Element.IsVisible) {
                state.IsToggled = false;
                continue;
            }
            // Toggle state
            if (state.IsToggled) {
                state.IsToggled = false;
                state.Element.Visibility = Visibility.Collapsed;
                state.PreventOpen = true;
                continue;
            }
            // Check if mouse clicks element
            if (e.OriginalSource is Visual element && (
                element.GetType() == state.Element.GetType() ||
                (element.FindAscendant(state.Element.GetType()) is not null)
            )) {
                continue;
            }
            // Click outer
            state.Element.Visibility = Visibility.Collapsed;
        }
    }

    private static void PreviewMouseDownHandler(object sender, MouseButtonEventArgs e) {
        if (sender is DependencyObject dp) {
            // Toggle state
            if (TargetElementStateDict.TryGetValue(SourceElementTargetIdDict[dp], out var state)) {
                state.IsToggled = true;
            }
        }
    }

    /// <summary>
    /// Toggle open state
    /// </summary>
    /// <param name="sender"></param>
    /// <remarks>Happens after WindowPreviewMouseUp</remarks>
    private static void HandleMouseEvent(object sender) {
        if (sender is not UIElement element) {
            return;
        }
        if (TargetElementStateDict.TryGetValue(SourceElementTargetIdDict[element], out var state)) {
            if (state.PreventOpen) {
                state.PreventOpen = false;
                return;
            }
            state.Element.Visibility = Visibility.Visible;
        }
    }

    private static void MouseUpHandler(object sender, MouseButtonEventArgs e) {
        HandleMouseEvent(sender);
    }

    private static void ButtonClickHandler(object sender, RoutedEventArgs e) {
        HandleMouseEvent(sender);
    }

    public static void DisposeTargetElement(FrameworkElement targetElement) {
        var kv = TargetElementStateDict.FirstOrDefault(kv => kv.Value.Element == targetElement);
        if (!kv.Equals(default(KeyValuePair<string, State>))) {
            TargetElementStateDict.Remove(kv.Key);
        }
    }

    public static void DisposeSourceElement(DependencyObject sourceElement) {
        SourceElementTargetIdDict.Remove(sourceElement);
        if (sourceElement is UIElement element) {
            element.PreviewMouseDown -= PreviewMouseDownHandler;
            // 按钮
            if (element is ButtonBase button) {
                button.Click -= ButtonClickHandler;
            } else {
                element.MouseUp -= MouseUpHandler;
            }
        } else if (sourceElement is ContentElement contentElement) {
            contentElement.PreviewMouseDown -= PreviewMouseDownHandler;
            contentElement.MouseUp -= MouseUpHandler;
        }
    }
}

/// <summary>
/// 鼠标移入显示，移除隐藏 TargetElement
/// </summary>
public static class HoverVisibleHelper {
    public static readonly DependencyProperty TargetElementProperty = DependencyProperty.RegisterAttached("TargetElement", typeof(FrameworkElement), typeof(HoverVisibleHelper), new PropertyMetadata(TargetElementPropertyChangedHandler));

    public static FrameworkElement GetTargetElement(DependencyObject obj) {
        return (FrameworkElement)obj.GetValue(TargetElementProperty);
    }
    /// <summary>
    /// 目标元素
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetTargetElement(DependencyObject obj, FrameworkElement value) {
        obj.SetValue(TargetElementProperty, value);
    }

    private static void TargetElementPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        if (e.NewValue is not null) {
            element.MouseEnter -= ElementMouseEnterHandler;
            element.MouseLeave -= ElementMouseLeaveHandler;
            element.MouseEnter += ElementMouseEnterHandler;
            element.MouseLeave += ElementMouseLeaveHandler;
        } else {
            element.MouseEnter -= ElementMouseEnterHandler;
            element.MouseLeave -= ElementMouseLeaveHandler;
        }
    }

    private static void ElementMouseLeaveHandler(object sender, MouseEventArgs e) {
        if (sender is UIElement element) {
            GetTargetElement(element).Visibility = Visibility.Collapsed;
        }
    }

    private static void ElementMouseEnterHandler(object sender, MouseEventArgs e) {
        if (sender is UIElement element) {
            GetTargetElement(element).Visibility = Visibility.Visible;
        }
    }

    public static void Dispose(FrameworkElement element) {
        element.MouseEnter -= ElementMouseEnterHandler;
        element.MouseLeave -= ElementMouseLeaveHandler;
        element.ClearValue(TargetElementProperty);
    }
}

/// <summary>
/// RevealBackgroundHelper
/// </summary>
public static class RevealBackgroundHelper {
    public const double DefaultRadius = 50;
    public static readonly Color DefaultBrushColor = (Color)ColorConverter.ConvertFromString("#8f8f8f");
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RevealBackgroundHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyProperty BackgroundPropertyProperty = DependencyProperty.RegisterAttached("BackgroundProperty", typeof(DependencyProperty), typeof(RevealBackgroundHelper), new PropertyMetadata(BackgroundPropertyPropertyChangedHandler));
    public static readonly DependencyProperty RadiusProperty = DependencyProperty.RegisterAttached("Radius", typeof(double), typeof(RevealBackgroundHelper), new PropertyMetadata(DefaultRadius, RadiusPropertyChangedHandler));
    public static readonly DependencyProperty BrushColorProperty = DependencyProperty.RegisterAttached("BrushColor", typeof(Color), typeof(RevealBackgroundHelper), new PropertyMetadata(DefaultBrushColor, BrushColorPropertyChangedHandler));
    private static readonly IDictionary<Window, ICollection<(FrameworkElement, RadialGradientBrush)>> WindowElementDict = new Dictionary<Window, ICollection<(FrameworkElement, RadialGradientBrush)>>();

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }
    public static DependencyProperty GetBackgroundProperty(DependencyObject obj) {
        return (DependencyProperty)obj.GetValue(BackgroundPropertyProperty);
    }
    /// <summary>
    /// BackgroundProperty
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetBackgroundProperty(DependencyObject obj, DependencyProperty value) {
        obj.SetValue(BackgroundPropertyProperty, value);
    }
    public static double GetRadius(DependencyObject obj) {
        return (double)obj.GetValue(RadiusProperty);
    }
    /// <summary>
    /// 径向渐变 Radius
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetRadius(DependencyObject obj, double value) {
        obj.SetValue(RadiusProperty, value);
    }
    public static Color GetBrushColor(DependencyObject obj) {
        return (Color)obj.GetValue(BrushColorProperty);
    }
    /// <summary>
    /// 背景颜色
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetBrushColor(DependencyObject obj, Color value) {
        obj.SetValue(BrushColorProperty, value);
    }

    private static void BrushColorPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        if (element.IsLoaded) {
            if (!GetIsEnabled(element)) {
                return;
            }
            SetBackground(element);
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }

    private static void RadiusPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        if (element.IsLoaded) {
            if (!GetIsEnabled(element)) {
                return;
            }
            SetBackground(element);
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }

    private static void BackgroundPropertyPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        if (element.IsLoaded) {
            if (!GetIsEnabled(element)) {
                return;
            }
            SetBackground(element);
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        if (element.IsLoaded) {
            // 直接调用，减少代码重复
            ElementLoadedHandler(element, new(FrameworkElement.LoadedEvent, element));
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }

    private static void ElementLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        // Remove Background
        if (GetIsEnabled(element) is false) {
            if (GetBackgroundProperty(element) is DependencyProperty backgroundProperty) {
                element.ClearValue(backgroundProperty);
            }
            RemoveFromWindowElementDict(element);
        }
        // Set Background
        else {
            SetBackground(element);
        }
    }

    private static void SetBackground(FrameworkElement element) {
        if (GetBackgroundProperty(element) is DependencyProperty backgroundProperty) {
            var window = Window.GetWindow(element);
            window.MouseMove -= WindowMouseMoveHandler;
            window.MouseMove += WindowMouseMoveHandler;
            var brush = CreateBrush(element);
            element.SetValue(backgroundProperty, brush);
            AddToWindowElementDict(window, (element, brush));
        }
    }

    private static void AddToWindowElementDict(Window window, (FrameworkElement, RadialGradientBrush) value) {
        if (!WindowElementDict.TryGetValue(window, out var elements)) {
            WindowElementDict[window] = elements = new List<(FrameworkElement, RadialGradientBrush)>();
        }
        elements.Remove(item => item.Item1 == value.Item1);
        elements.Add(value);
    }

    private static void WindowMouseMoveHandler(object sender, MouseEventArgs e) {
        if (sender is not Window window) {
            return;
        }
        foreach (var (element, brush) in WindowElementDict[window]) {
            if (brush is RadialGradientBrush radialGradientBrush) {
                var position = e.GetPosition(element);
                radialGradientBrush.GradientOrigin = position;
                radialGradientBrush.Center = position;
            }
        }
    }

    private static RadialGradientBrush CreateBrush(FrameworkElement element) {
        var brushColor = GetBrushColor(element);
        brushColor = brushColor == default ? DefaultBrushColor : brushColor;
        return new RadialGradientBrush(brushColor, Colors.Transparent) {
            MappingMode = BrushMappingMode.Absolute,
            RadiusX = GetRadius(element),
            RadiusY = GetRadius(element),
            Opacity = 1,
            // 设置初次加载时背景
            GradientOrigin = new(int.MaxValue, int.MaxValue),
            Transform = Transform.Identity,
            RelativeTransform = Transform.Identity,
        };
    }

    private static void RemoveFromWindowElementDict(FrameworkElement element) {
        foreach (var (_, elements) in WindowElementDict) {
            var target = elements.FirstOrDefault(item => item.Item1 == element);
            if (target != default((FrameworkElement, RadialGradientBrush))) {
                elements.Remove(target);
                return;
            }
        }
    }

    public static void Dispose(FrameworkElement element) {
        element.ClearValue(RadiusProperty);
        element.ClearValue(IsEnabledProperty);
        element.ClearValue(BrushColorProperty);
        if (GetBackgroundProperty(element) is DependencyProperty backgroundProperty) {
            element.ClearValue(backgroundProperty);
        }
        element.ClearValue(BackgroundPropertyProperty);
        RemoveFromWindowElementDict(element);
    }
}

/// <summary>
/// 自动设置 Width, Height
/// </summary>
public static class AutoSizeHelper {
    private class GroupInfo {
        public bool MatchMax { get; set; } = true;
        public readonly IDictionary<DependencyObject, DependencyProperty> ElementInfoGetterList = new Dictionary<DependencyObject, DependencyProperty>();
        public readonly IDictionary<DependencyObject, DependencyProperty> ElementInfoSetterList = new Dictionary<DependencyObject, DependencyProperty>();

        public GroupInfo() { }

        public GroupInfo(bool matchMax, IDictionary<DependencyObject, DependencyProperty> elementInfoList, IDictionary<DependencyObject, DependencyProperty> elementInfoSetterList) {
            MatchMax = matchMax;
            ElementInfoGetterList = elementInfoList;
            ElementInfoSetterList = elementInfoSetterList;
        }
    }

    private static readonly IDictionary<string, GroupInfo> WidthGroupInfoDict = new Dictionary<string, GroupInfo>();
    private static readonly IDictionary<string, GroupInfo> HeightGroupInfoDict = new Dictionary<string, GroupInfo>();
    private static readonly IDictionary<DependencyObject, string> ElementWidthGroupIdDict = new Dictionary<DependencyObject, string>();
    private static readonly IDictionary<DependencyObject, string> ElementHeightGroupIdDict = new Dictionary<DependencyObject, string>();

    private static void EnsureInitialized(string groupId) {
        if (!WidthGroupInfoDict.ContainsKey(groupId)) {
            WidthGroupInfoDict[groupId] = new();
        }
        if (!HeightGroupInfoDict.ContainsKey(groupId)) {
            HeightGroupInfoDict[groupId] = new();
        }
    }

    /// <summary>
    /// Add to GroupInfoDictionary
    /// </summary>
    /// <param name="groupInfo"></param>
    /// <param name="matchMax"></param>
    /// <param name="elements">(element, getter, setter)</param>
    private static void AddToGroupInfo(GroupInfo groupInfo, bool matchMax, IEnumerable<(FrameworkElement, DependencyProperty, DependencyProperty)> elements) {
        groupInfo.MatchMax = matchMax;
        // Add elementInfos
        foreach (var elementItem in elements) {
            groupInfo.ElementInfoGetterList[elementItem.Item1] = elementItem.Item2;
            groupInfo.ElementInfoSetterList[elementItem.Item1] = elementItem.Item3;
        }
    }

    private static void HandleElementLoadedEvent(object sender, IDictionary<DependencyObject, string> elementGruopIdDict, IDictionary<string, GroupInfo> groupInfoDict) {
        if (sender is not FrameworkElement element) {
            return;
        }
        if (!elementGruopIdDict.TryGetValue(element, out var groupId)) {
            return;
        }
        if (!groupInfoDict.TryGetValue(groupId, out var groupInfo)) {
            return;
        }
        if (!groupInfo.ElementInfoGetterList.TryGetValue(element, out var getterDp)) {
            return;
        }
        if (!groupInfo.ElementInfoSetterList.TryGetValue(element, out var setterDp)) {
            return;
        }

        element.SetValue(setterDp, groupInfo.ElementInfoGetterList.Max(
            item => (double)item.Key.GetValue(item.Value))
        );
    }

    private static void ElementLoadedWidthHandler(object sender, RoutedEventArgs e) {
        HandleElementLoadedEvent(sender, ElementWidthGroupIdDict, WidthGroupInfoDict);
    }

    private static void ElementLoadedHeightHandler(object sender, RoutedEventArgs e) {
        HandleElementLoadedEvent(sender, ElementHeightGroupIdDict, HeightGroupInfoDict);
    }

    /// <summary>
    /// 显式更新 Width
    /// </summary>
    /// <param name="groupId"></param>
    public static void UpdateWidth(string groupId) {
        if (WidthGroupInfoDict.TryGetValue(groupId, out var groupInfo)) {
            foreach (var (element, _) in groupInfo.ElementInfoGetterList) {
                ElementLoadedWidthHandler(element, new RoutedEventArgs(FrameworkElement.LoadedEvent, element));
            }
        }
    }

    /// <summary>
    /// 显式更新 Height
    /// </summary>
    /// <param name="groupId"></param>
    public static void UpdateHeight(string groupId) {
        if (HeightGroupInfoDict.TryGetValue(groupId, out var groupInfo)) {
            foreach (var (element, _) in groupInfo.ElementInfoGetterList) {
                ElementLoadedHeightHandler(element, new RoutedEventArgs(FrameworkElement.LoadedEvent, element));
            }
        }
    }

    /// <summary>
    /// 设置自动宽度
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="elements">默认 <see cref="FrameworkElement.WidthProperty"/> (set), <see cref="FrameworkElement.ActualWidthProperty"/> (get)</param>
    /// <param name="matchMax">设置为最大宽度</param>
    public static void SetAutoWidth(
        string groupId,
        IEnumerable<FrameworkElement> elements,
        bool matchMax = true
    ) => SetAutoWidth(groupId, elements, FrameworkElement.ActualWidthProperty, FrameworkElement.WidthProperty, matchMax);

    /// <summary>
    /// 设置自动宽度
    /// </summary>
    /// <param name="groupId">分组 Id</param>
    /// <param name="elements"></param>
    /// <param name="widthGetterProperty"></param>
    /// <param name="widthSetterProperty"></param>
    /// <param name="matchMax">设置为最大宽度</param>
    public static void SetAutoWidth(
        string groupId,
        IEnumerable<FrameworkElement> elements,
        DependencyProperty widthGetterProperty,
        DependencyProperty widthSetterProperty,
        bool matchMax = true
    ) {
        SetAutoWidth(
            groupId,
            elements.Select(item => (item, widthGetterProperty, widthSetterProperty)),
            matchMax
        );
    }

    /// <summary>
    /// 设置自动 Width
    /// </summary>
    /// <param name="groupId">分组 Id</param>
    /// <param name="elementInfos">(element, getter, setter)</param>
    /// <param name="matchMax">设置为最大 Width</param>
    public static void SetAutoWidth(
        string groupId,
        IEnumerable<(FrameworkElement, DependencyProperty, DependencyProperty)> elementInfos,
        bool matchMax = true
    ) {
        EnsureInitialized(groupId);
        AddToGroupInfo(WidthGroupInfoDict[groupId], matchMax, elementInfos);
        elementInfos.ForEach(item => {
            var element = item.Item1;
            ElementWidthGroupIdDict[element] = groupId;
            element.Loaded -= ElementLoadedWidthHandler;
            element.Loaded += ElementLoadedWidthHandler;
            // Set explicitly
            if (element.IsLoaded) {
                ElementLoadedWidthHandler(element, new RoutedEventArgs(FrameworkElement.LoadedEvent, element));
            }
        });
    }

    /// <summary>
    /// 设置自动 Height
    /// </summary>
    /// <param name="groupId">分组 Id</param>
    /// <param name="elements">默认 <see cref="FrameworkElement.HeightProperty"/> (set), <see cref="FrameworkElement.ActualHeightProperty"/> (get)</param>
    /// <param name="matchMax">设置为最大 Height</param>
    public static void SetAutoHeight(
        string groupId,
        IEnumerable<FrameworkElement> elements,
        bool matchMax = true
    ) => SetAutoHeight(groupId, elements, FrameworkElement.ActualHeightProperty, FrameworkElement.HeightProperty, matchMax);

    /// <summary>
    /// 设置自动 Height
    /// </summary>
    /// <param name="groupId">分组 Id</param>
    /// <param name="elements"></param>
    /// <param name="heightGetterProperty"></param>
    /// <param name="heightSetterProperty"></param>
    /// <param name="matchMax">设置为最大 Height</param>
    public static void SetAutoHeight(
        string groupId,
        IEnumerable<FrameworkElement> elements,
        DependencyProperty heightGetterProperty,
        DependencyProperty heightSetterProperty,
        bool matchMax = true
    ) {
        SetAutoHeight(
            groupId,
            elements.Select(item => (item, heightGetterProperty, heightSetterProperty)),
            matchMax
        );
    }

    /// <summary>
    /// 设置自动 Height
    /// </summary>
    /// <param name="groupId"></param>
    /// <param name="elementInfos">(element, getter, setter)</param>
    /// <param name="matchMax">设置为最大 Height</param>
    public static void SetAutoHeight(
        string groupId,
        IEnumerable<(FrameworkElement, DependencyProperty, DependencyProperty)> elementInfos,
        bool matchMax = true
    ) {
        EnsureInitialized(groupId);
        AddToGroupInfo(HeightGroupInfoDict[groupId], matchMax, elementInfos);
        elementInfos.ForEach(item => {
            var element = item.Item1;
            ElementHeightGroupIdDict[element] = groupId;
            element.Loaded -= ElementLoadedHeightHandler;
            element.Loaded += ElementLoadedHeightHandler;
            // Set explicitly
            if (element.IsLoaded) {
                ElementLoadedHeightHandler(element, new RoutedEventArgs(FrameworkElement.LoadedEvent, element));
            }
        });
    }

    /// <summary>
    /// Disable auto adjust Width
    /// </summary>
    /// <param name="groupId"></param>
    public static void DisableAutoWidth(string groupId) {
        if (WidthGroupInfoDict.TryGetValue(groupId, out var elementInfos)) {
            foreach (var obj in elementInfos.ElementInfoGetterList.Keys) {
                ElementWidthGroupIdDict.Remove(obj);
                if (obj is FrameworkElement element) {
                    element.Loaded -= ElementLoadedWidthHandler;
                }
            }
            elementInfos.ElementInfoSetterList.ForEach(item => item.Key.ClearValue(item.Value));
            elementInfos.ElementInfoGetterList.Clear();
            elementInfos.ElementInfoSetterList.Clear();
        }
    }

    /// <summary>
    /// Disable auto adjust Height
    /// </summary>
    /// <param name="groupId"></param>
    public static void DisableAutoHeight(string groupId) {
        if (HeightGroupInfoDict.TryGetValue(groupId, out var elementInfos)) {
            foreach (var obj in elementInfos.ElementInfoGetterList.Keys) {
                ElementHeightGroupIdDict.Remove(obj);
                if (obj is FrameworkElement element) {
                    element.Loaded -= ElementLoadedHeightHandler;
                }
            }
            elementInfos.ElementInfoSetterList.ForEach(item => item.Key.ClearValue(item.Value));
            elementInfos.ElementInfoGetterList.Clear();
            elementInfos.ElementInfoSetterList.Clear();
        }
    }
}

/// <summary>
/// 添加 Password 附加属性，OneWayToSource
/// </summary>
public static class PasswordBoxHelper {
    public static readonly DependencyProperty TargetProperty = DependencyProperty.RegisterAttached("Target", typeof(DependencyObject), typeof(PasswordBoxHelper), new PropertyMetadata(TargetPropertyChangedHandler));
    public static readonly DependencyProperty PasswordPropertyProperty = DependencyProperty.RegisterAttached("PasswordProperty", typeof(DependencyProperty), typeof(PasswordBoxHelper), new PropertyMetadata());

    public static DependencyProperty GetPasswordProperty(DependencyObject obj) {
        return (DependencyProperty)obj.GetValue(PasswordPropertyProperty);
    }
    /// <summary>
    /// Password Property
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetPasswordProperty(DependencyObject obj, DependencyProperty value) {
        obj.SetValue(PasswordPropertyProperty, value);
    }
    public static DependencyObject GetTarget(DependencyObject obj) {
        return (DependencyObject)obj.GetValue(TargetProperty);
    }
    /// <summary>
    /// Password 所属 Element
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetTarget(DependencyObject obj, DependencyObject value) {
        obj.SetValue(TargetProperty, value);
    }

    private static void TargetPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not PasswordBox box) {
            return;
        }
        box.PasswordChanged -= PasswordChangedHandler;
        box.PasswordChanged += PasswordChangedHandler;
    }

    private static void PasswordChangedHandler(object sender, RoutedEventArgs e) {
        if (sender is not PasswordBox box) {
            return;
        }
        if (GetTarget(box) is not DependencyObject target) {
            return;
        }
        if (GetPasswordProperty(box) is not DependencyProperty property) {
            return;
        }
        target.SetValue(property, box.Password);
    }

    public static void Dispose(PasswordBox passwordBox) {
        passwordBox.PasswordChanged -= PasswordChangedHandler;
        passwordBox.ClearValue(TargetProperty);
        passwordBox.ClearValue(PasswordPropertyProperty);
    }
}

/// <summary>
/// NumberBoxStyleHelper
/// </summary>
public static class NumberBoxStyleHelper {
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(NumberBoxStyleHelper), new PropertyMetadata(false, EnabledPropertyChangedHandler));
    private static readonly IList<NumberBox> NumberBoxes = new List<NumberBox>();

    public static bool GetEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(EnabledProperty);
    }
    public static void SetEnabled(DependencyObject obj, bool value) {
        obj.SetValue(EnabledProperty, value);
    }

    static NumberBoxStyleHelper() {
        // Update theme
        CommonUITools.Themes.ThemeManager.ThemeChanged += (_, theme) => {
            foreach (var box in NumberBoxes) {
                UpdateStyle(box);
            }
        };
    }

    private static void EnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not NumberBox box) {
            return;
        }
        if (e.NewValue is false) {
            // Clear style
            if (GetTextBox(box) is TextBox textbox) {
                textbox.ClearValue(FrameworkElement.StyleProperty);
            }
            box.Loaded -= NumberBoxLoadedHandler;
            box.Unloaded -= NumberBoxUnloadedHandler;
            return;
        }
        if (box.IsLoaded) {
            NumberBoxLoadedHandler(box, null!);
            return;
        }
        box.Loaded -= NumberBoxLoadedHandler;
        box.Loaded += NumberBoxLoadedHandler;
        box.Unloaded -= NumberBoxUnloadedHandler;
        box.Unloaded += NumberBoxUnloadedHandler;
    }

    private static Style GetTextBoxStyle(NumberBox box) {
        var style = (Style)box.FindResource("GlobalTextBoxStyle");
        var newStyle = new Style(typeof(TextBox), style);
        newStyle.Resources["TextControlBackgroundPointerOver"] = new SolidColorBrush(Colors.Transparent);
        newStyle.Resources["TextControlBackgroundFocused"] = new SolidColorBrush(Colors.Transparent);
        newStyle.Resources["TextControlBackground"] = new SolidColorBrush(Colors.Transparent);
        newStyle.Resources["TextControlBorderBrushPointerOver"] = box.FindResource("TextControlBorderBrushPointerOver");
        newStyle.Resources["TextControlBorderBrush"] = box.FindResource("TextControlBorderBrush");
        newStyle.Resources["TextControlCaretBrush"] = box.FindResource("TextControlCaretBrush");
        newStyle.Resources["MenuFlyoutPresenterBackground"] = box.FindResource("MenuFlyoutPresenterBackground");
        return newStyle;
    }

    private static TextBox? GetTextBox(NumberBox box) {
        return box.Template.FindName("InputBox", box) as TextBox;
    }

    private static void UpdatePopUpStyle(NumberBox box) {
        if (box.Template.FindName("UpDownPopup", box) is Popup popup) {
            popup.HorizontalOffset = 0;
            if (popup.FindName("PopupContentRoot") is Border border) {
                border.Background = (Brush)border.FindResource("ApplicationBackgroundBrush");
            }
        }
    }

    private static void UpdateTextBoxStyle(NumberBox box) {
        if (GetTextBox(box) is TextBox textbox) {
            textbox.Style = GetTextBoxStyle(box);
        }
    }

    private static void UpdateStyle(NumberBox box) {
        UpdateTextBoxStyle(box);
        // Update border style
        UpdatePopUpStyle(box);
    }

    private static void NumberBoxUnloadedHandler(object sender, RoutedEventArgs e) {
        var box = (NumberBox)sender;
        NumberBoxes.Remove(box);
    }

    private static void NumberBoxLoadedHandler(object sender, RoutedEventArgs e) {
        var box = (NumberBox)sender;
        NumberBoxes.Add(box);
        UpdateStyle(box);
    }
}

/// <summary>
/// ControlHelper
/// </summary>
public static class PopupHelper {
    private static readonly DependencyProperty PopupBottomStoryboardProperty = DependencyProperty.RegisterAttached("PopupBottomStoryboard", typeof(Storyboard), typeof(PopupHelper), new PropertyMetadata());
    private static readonly DependencyProperty PopupTopStoryboardProperty = DependencyProperty.RegisterAttached("PopupTopStoryboard", typeof(Storyboard), typeof(PopupHelper), new PropertyMetadata());
    private static readonly DependencyProperty PopupParentProperty = DependencyProperty.RegisterAttached("PopupParent", typeof(Control), typeof(PopupHelper), new PropertyMetadata());
    public static readonly DependencyProperty EnableStylePopupProperty = DependencyProperty.RegisterAttached("EnableStylePopup", typeof(bool), typeof(PopupHelper), new PropertyMetadata(false, EnableStylePopupPropertyChangedHandler));
    public static readonly DependencyProperty PlacementTargetProperty = DependencyProperty.RegisterAttached("PlacementTarget", typeof(UIElement), typeof(PopupHelper), new PropertyMetadata());

    public static bool GetEnableStylePopup(DependencyObject obj) {
        return (bool)obj.GetValue(EnableStylePopupProperty);
    }
    /// <summary>
    /// 设置 Popup 样式
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetEnableStylePopup(DependencyObject obj, bool value) {
        obj.SetValue(EnableStylePopupProperty, value);
    }
    public static UIElement GetPlacementTarget(DependencyObject obj) {
        return (UIElement)obj.GetValue(PlacementTargetProperty);
    }
    /// <summary>
    /// When cannot get PlacementTarget for this popup, this works
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetPlacementTarget(DependencyObject obj, UIElement value) {
        obj.SetValue(PlacementTargetProperty, value);
    }
    private static Storyboard GetPopupBottomStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(PopupBottomStoryboardProperty);
    }
    /// <summary>
    /// Popup bottom Storyboard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    private static void SetPopupBottomStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(PopupBottomStoryboardProperty, value);
    }
    private static Storyboard GetPopupTopStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(PopupTopStoryboardProperty);
    }
    /// <summary>
    /// Popup Top Storyboard
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    private static void SetPopupTopStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(PopupTopStoryboardProperty, value);
    }
    private static Control GetPopupParent(DependencyObject obj) {
        return (Control)obj.GetValue(PopupParentProperty);
    }
    /// <summary>
    /// The parent of popup
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    private static void SetPopupParent(DependencyObject obj, Control value) {
        obj.SetValue(PopupParentProperty, value);
    }

    private static void EnableStylePopupPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not Control box) {
            return;
        }

        if (e.NewValue is true) {
            if (box.IsLoaded) {
                ControlLoadedOnceHandler(box, null!);
                return;
            }
            box.Loaded -= ControlLoadedOnceHandler;
            box.Loaded += ControlLoadedOnceHandler;
        } else {
            box.Loaded -= ControlLoadedOnceHandler;
            if (box.Template.FindName("PART_Popup", box) is Popup popup) {
                popup.Opened -= PopupOpenedHandler;
            }
        }
    }

    private static void ControlLoadedOnceHandler(object sender, RoutedEventArgs e) {
        if (sender is not Control control) {
            return;
        }
        control.Loaded -= ControlLoadedOnceHandler;
        if (control.FindTemplateChild<Popup>("PART_Popup") is Popup popup) {
            SetPopupParent(popup, control);
            popup.Opened -= PopupOpenedHandler;
            popup.Opened += PopupOpenedHandler;
        }
    }

    /// <summary>
    /// Begin animation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void PopupOpenedHandler(object? sender, EventArgs e) {
        if (sender is not Popup popup) {
            return;
        }
        if (popup.PlacementTarget is null) {
            if (GetPlacementTarget(GetPopupParent(popup)) is not UIElement target) {
                return;
            }
            popup.PlacementTarget = target;
        }
        popup.AlignCenterParent();
        double bottomHeight = Math.Abs(popup.PlacementTarget.PointFromScreen(new()).Y)
            + popup.PlacementTarget.RenderSize.Height
            + popup.Child.RenderSize.Height;
        // Check final placement
        if (bottomHeight >= SystemParameters.MaximizedPrimaryScreenHeight) {
            popup.Placement = PlacementMode.Top;
            popup.VerticalOffset = -4;
            if (GetPopupTopStoryboard(popup) == null) {
                SetPopupTopStoryboard(popup, popup.Child.CreateOpeningStoryboard(false));
            }
            GetPopupTopStoryboard(popup).Begin();
        } else {
            popup.Placement = PlacementMode.Bottom;
            popup.VerticalOffset = 4;
            if (GetPopupBottomStoryboard(popup) == null) {
                SetPopupBottomStoryboard(popup, popup.Child.CreateOpeningStoryboard(true));
            }
            GetPopupBottomStoryboard(popup).Begin();
        }
    }
}

internal static class HelperUtils {
    /// <summary>
    /// OpeningStoryboard
    /// </summary>
    /// <param name="element"></param>
    /// <param name="topToBottom"></param>
    /// <returns></returns>
    public static Storyboard CreateOpeningStoryboard(this UIElement element, bool topToBottom) {
        element.RenderTransform = new TranslateTransform();
        var window = Window.GetWindow(element) ?? Application.Current.MainWindow;
        var from = topToBottom ? -16.0 : 16.0;
        var doubleAnimation = new DoubleAnimation(from, 0.0, (Duration)window.FindResource("AnimationDuration")) {
            EasingFunction = (IEasingFunction)window.FindResource("AnimationEaseFunction")
        };
        Storyboard.SetTarget(doubleAnimation, element);
        Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        return new Storyboard {
            Children = { doubleAnimation }
        };
    }

    /// <summary>
    /// Center popup to parent
    /// </summary>
    /// <param name="popup"></param>
    public static void AlignCenterParent(this Popup popup) {
        popup.HorizontalOffset = -(popup.Child.RenderSize.Width - popup.PlacementTarget.RenderSize.Width) / 2;
    }

    /// <summary>
    /// Find Template child
    /// </summary>
    /// <param name="control"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T? FindTemplateChild<T>(this Control control, string name) where T : DependencyObject {
        if (control.Template.FindName(name, control) is not T element) {
            // Ensure ApplyTemplate
            if (!control.ApplyTemplate() || control.Template.FindName(name, control) is not T element1) {
                return null;
            }
            element = element1;
        }
        return element;
    }
}

/// <summary>
/// WindowHelper
/// </summary>
public static class WindowHelper {
    public const int MinimumBuildVersion = 22523;

    public static readonly DependencyProperty BackDropStyleProperty = DependencyProperty.RegisterAttached("BackDropStyle", typeof(BackdropStyle), typeof(WindowHelper), new PropertyMetadata(BackdropStyle.None, BackDropStylePropertyChangedHandler));

    public static BackdropStyle GetBackDropStyle(DependencyObject obj) {
        return (BackdropStyle)obj.GetValue(BackDropStyleProperty);
    }
    /// <summary>
    /// BackDropStyle
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    public static void SetBackDropStyle(DependencyObject obj, BackdropStyle value) {
        obj.SetValue(BackDropStyleProperty, value);
    }

    static WindowHelper() {
        CommonUITools.Themes.ThemeManager.ThemeChanged += ThemeChangedHandler;
    }

    /// <summary>
    /// Is system support feature
    /// </summary>
    public static bool IsSystemSupport => Environment.OSVersion.Version.Build >= MinimumBuildVersion;

    /// <summary>
    /// Apply the optimal theme
    /// </summary>
    /// <param name="window"></param>
    public static void ApplyOptimalThemeStyle(Window window) {
        if (IsSystemSupport) {
            window.Background = new SolidColorBrush(Colors.Transparent);
            SetBackDropStyle(window, BackdropStyle.Mica);
        } else {
            window.SetResourceReference(Control.BackgroundProperty, "ApplicationBackgroundBrush");
        }
    }

    private static void BackDropStylePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not Window window) {
            return;
        }
        if (!IsSystemSupport) {
            return;
        }

        if (window.IsLoaded) {
            WindowLoadedHandler(window, null!);
            return;
        }
        window.Loaded -= WindowLoadedHandler;
        window.Loaded += WindowLoadedHandler;
    }

    private static void WindowLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is not Window window) {
            return;
        }
        UpdateFrame(window);
        UpdateBackdrop(window, GetBackDropStyle(window));
        UpdateTheme(window);
    }

    private static void ThemeChangedHandler(object? sender, ThemeMode e) {
        Application.Current.Windows.OfType<Window>().ForEach(window => {
            if (GetBackDropStyle(window) != BackdropStyle.None) {
                UpdateTheme(window);
            }
        });
    }

    private static void UpdateFrame(Window window) {
        IntPtr windowPtr = new WindowInteropHelper(window).Handle;
        HwndSource windowSource = HwndSource.FromHwnd(windowPtr);
        windowSource.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);
        _ = PInvokeUtils.ExtendFrame(windowSource.Handle, new(-1));
        PInvokeUtils.HideAllWindowButtons(windowPtr);
    }

    private static void UpdateBackdrop(Window window, BackdropStyle style) {
        _ = PInvokeUtils.SetWindowAttribute(
            new WindowInteropHelper(window).Handle,
            PInvokeUtils.DwmWindowAttribute.SystembackdropType,
            (int)style
        );
    }

    private static void UpdateTheme(Window window) {
        var isDark = CommonUITools.Themes.ThemeManager.CurrentTheme == ThemeMode.Dark;
        int flag = isDark ? 1 : 0;
        _ = PInvokeUtils.SetWindowAttribute(
            new WindowInteropHelper(window).Handle,
            PInvokeUtils.DwmWindowAttribute.UseImmersiveDarkMode,
            flag
        );
    }
}

/// <summary>
/// GridSplitterHelper，鼠标悬浮一段时间会出现背景
/// </summary>
public static class GridSplitterHelper {
    public static readonly DependencyProperty EnableHoverFillVisibleProperty = DependencyProperty.RegisterAttached("EnableHoverFillVisible", typeof(bool), typeof(GridSplitterHelper), new PropertyMetadata(false, EnableHoverFillVisiblePropertyChangedHandler));
    public static readonly DependencyProperty HoverBrushProperty = DependencyProperty.RegisterAttached("HoverBrush", typeof(Brush), typeof(GridSplitterHelper), new PropertyMetadata());

    private static readonly DependencyProperty TimerProperty = DependencyProperty.RegisterAttached("Timer", typeof(Timer), typeof(GridSplitterHelper), new PropertyMetadata());
    private static readonly DependencyProperty AnimationProperty = DependencyProperty.RegisterAttached("Animation", typeof(AnimationTimeline), typeof(GridSplitterHelper), new PropertyMetadata());
    private static readonly IDictionary<Timer, GridSplitter> TimerGridSplitterDict = new Dictionary<Timer, GridSplitter>();
    private const int HoverDuration = 500;
    private const string GridSplitterOverBrushKey = "GridSplitterOverBrush";

    public static bool GetEnableHoverFillVisible(DependencyObject obj) {
        return (bool)obj.GetValue(EnableHoverFillVisibleProperty);
    }
    public static void SetEnableHoverFillVisible(DependencyObject obj, bool value) {
        obj.SetValue(EnableHoverFillVisibleProperty, value);
    }
    public static Brush GetHoverBrush(DependencyObject obj) {
        return (Brush)obj.GetValue(HoverBrushProperty);
    }
    public static void SetHoverBrush(DependencyObject obj, Brush value) {
        obj.SetValue(HoverBrushProperty, value);
    }
    private static Timer GetTimer(DependencyObject obj) {
        return (Timer)obj.GetValue(TimerProperty);
    }
    private static void SetTimer(DependencyObject obj, Timer value) {
        obj.SetValue(TimerProperty, value);
    }
    private static AnimationTimeline GetAnimation(DependencyObject obj) {
        return (AnimationTimeline)obj.GetValue(AnimationProperty);
    }
    private static void SetAnimation(DependencyObject obj, AnimationTimeline value) {
        obj.SetValue(AnimationProperty, value);
    }
    private static void EnableHoverFillVisiblePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not GridSplitter splitter) {
            return;
        }
        if (e.NewValue is false) {
            Dispose(splitter);
            return;
        }

        EnableHoverFillVisible(splitter);
    }

    private static void EnableHoverFillVisible(GridSplitter splitter) {
        var timer = new Timer(HoverDuration);
        timer.Elapsed += TimerElapsedHandler;
        SetTimer(splitter, timer);
        TimerGridSplitterDict[timer] = splitter;
        splitter.MouseEnter += GridSplitterMouseEnterHandler;
        splitter.MouseLeave += GridSplitterMouseLeaveHandler;
    }

    private static void GridSplitterMouseLeaveHandler(object sender, MouseEventArgs e) {
        if (sender is GridSplitter splitter && GetTimer(splitter) is Timer timer) {
            timer.Stop();
            splitter.ClearValue(Control.BackgroundProperty);
        }
    }

    private static void GridSplitterMouseEnterHandler(object sender, MouseEventArgs e) {
        if (sender is GridSplitter splitter && GetTimer(splitter) is Timer timer) {
            timer.Start();
        }
    }

    /// <summary>
    /// When this is invoked, timer stops
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void TimerElapsedHandler(object? sender, ElapsedEventArgs e) {
        if (sender is not Timer timer) {
            return;
        }
        timer.Stop();
        UIUtils.RunOnUIThread(timer => {
            if (TimerGridSplitterDict.TryGetValue(timer, out var splitter)) {
                splitter.Background = GetHoverBrush(splitter) ?? (Brush)splitter.FindResource(GridSplitterOverBrushKey);
                splitter.BeginAnimation(UIElement.OpacityProperty, GetOrInitAnimation(splitter));
            }
        }, timer);
    }

    /// <summary>
    /// Get or init AnimationTimeline
    /// </summary>
    /// <param name="splitter"></param>
    /// <returns></returns>
    private static AnimationTimeline GetOrInitAnimation(GridSplitter splitter) {
        if (GetAnimation(splitter) is not AnimationTimeline animation) {
            animation = new DoubleAnimation(0, 1, (Duration)splitter.FindResource("AnimationDuration"));
            SetAnimation(splitter, animation);
            Storyboard.SetTarget(splitter, animation);
            Storyboard.SetTargetProperty(splitter, new("Opacity"));
        }
        return animation;
    }

    public static void Dispose(GridSplitter gridSplitter) {
        gridSplitter.ClearValue(EnableHoverFillVisibleProperty);
        gridSplitter.ClearValue(AnimationProperty);
        if (GetTimer(gridSplitter) is Timer timer) {
            timer.Dispose();
            TimerGridSplitterDict.Remove(timer);
        }
        gridSplitter.ClearValue(TimerProperty);
        gridSplitter.MouseEnter -= GridSplitterMouseEnterHandler;
        gridSplitter.MouseLeave -= GridSplitterMouseLeaveHandler;
    }
}