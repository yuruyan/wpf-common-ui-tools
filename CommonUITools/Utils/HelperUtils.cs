using CommonTools.Utils;
using CommonUITools.Widget;
using ModernWpf;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Effects;

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
/// 加载状态
/// </summary>
public static class LoadingBoxHelper {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            Logger.Error($"The AdornerLayer of {element.GetType()} is null");
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
public static class VisibilityAnimationHelper {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty VisibleStoryboardProperty = DependencyProperty.RegisterAttached("VisibleStoryboard", typeof(Storyboard), typeof(VisibilityAnimationHelper), new PropertyMetadata(VisibleStoryboardPropertyChangedHandler));

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

    private static void VisibleStoryboardPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            Logger.Error($"{d} is not of type FrameworkElement");
            return;
        }
        element.IsVisibleChanged -= ElementIsVisibleChanged;
        element.IsVisibleChanged += ElementIsVisibleChanged;
    }

    private static void ElementIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }
        if (e.NewValue is true) {
            GetVisibleStoryboard(element)?.Begin();
        }
    }

    public static void Dispose(FrameworkElement element) {
        element.IsVisibleChanged -= ElementIsVisibleChanged;
        element.ClearValue(VisibleStoryboardProperty);
    }
}

/// <summary>
/// 添加阴影
/// </summary>
public static class DropShadowEffectHelper {
    public static readonly DependencyProperty WeightProperty = DependencyProperty.RegisterAttached("Weight", typeof(DropShadowEffectWeight), typeof(DropShadowEffectHelper), new PropertyMetadata(DropShadowEffectWeight.None, EffectWeightChangedHandler));
    private static readonly Color DropShadowColor = UIUtils.StringToColor("#dfdfdf");

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
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty OpenOnMouseLeftClickProperty = DependencyProperty.RegisterAttached("OpenOnMouseLeftClick", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, OpenOnMouseLeftClickPropertyChangedHandler));
    public static readonly DependencyProperty EnableOpeningAnimationProperty = DependencyProperty.RegisterAttached("EnableOpeningAnimation", typeof(bool), typeof(ContextMenuHelper), new PropertyMetadata(false, EnableOpeningAnimationPropertyChangedHandler));
    private static readonly IDictionary<ContextMenu, Storyboard> OpeningStoroboardDict = new Dictionary<ContextMenu, Storyboard>();

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

    private static void EnableOpeningAnimationPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not ContextMenu menu) {
            Logger.Warn($"Element is not of type ContextMenu");
            return;
        }
        if ((bool)e.NewValue) {
            menu.Opened += ContextMenuOpenedHandler;
        } else {
            menu.Opened -= ContextMenuOpenedHandler;
        }
    }

    private static void ContextMenuOpenedHandler(object sender, RoutedEventArgs e) {
        if (sender is ContextMenu menu) {
            GetOpeningStoryboard(menu).Begin();
        }
    }

    private static Storyboard GetOpeningStoryboard(ContextMenu menu) {
        if (!OpeningStoroboardDict.TryGetValue(menu, out var storyboard)) {
            storyboard = CreateOpeningStoryboard(menu);
            OpeningStoroboardDict[menu] = storyboard;
        }
        return storyboard;
    }

    private static Storyboard CreateOpeningStoryboard(FrameworkElement element) {
        // Override RenderTransform
        element.RenderTransform = new TranslateTransform();
        var translateYAnimation = new DoubleAnimation(-16, 0, (Duration)element.FindResource("AnimationDuration")) {
            EasingFunction = (IEasingFunction)element.FindResource("AnimationEaseFunction")
        };
        Storyboard.SetTarget(translateYAnimation, element);
        Storyboard.SetTargetProperty(translateYAnimation, new("(UIElement.RenderTransform).(TranslateTransform.Y)"));
        return new Storyboard() {
            Children = { translateYAnimation }
        };
    }

    private static void OpenOnMouseLeftClickPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement && d is not FrameworkContentElement) {
            Logger.Warn($"Element is not of type FrameworkElement and FrameworkContentElement");
            return;
        }
        var element = (UIElement)d;
        if ((bool)e.NewValue) {
            element.PreviewMouseLeftButtonUp += ShowContextMenuHandler;
        } else {
            element.PreviewMouseLeftButtonUp -= ShowContextMenuHandler;
        }
    }

    /// <summary>
    /// 显示 ContextMenu
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ShowContextMenuHandler(object sender, MouseButtonEventArgs e) {
        if (sender is FrameworkElement element && element.ContextMenu is not null) {
            element.ContextMenu.IsOpen = true;
        } else if (sender is FrameworkContentElement contentElement && contentElement.ContextMenu is not null) {
            contentElement.ContextMenu.IsOpen = true;
        }
    }
}

/// <summary>
/// 控件自动显示 / 隐藏。点击外部隐藏，点击内部不会
/// </summary>
public static class ElementVisibilityHelper {
    private class State {
        public State(UIElement element) {
            Element = element;
        }

        public UIElement Element { get; set; }
        public Window ElementWindow { get; set; } = Application.Current.MainWindow;
        public bool IsToggled { get; set; }
        public bool PreventOpen { get; set; }
    }

    private static readonly IDictionary<string, State> TargetElementStateDict = new Dictionary<string, State>();
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
            UIUtils.SetLoadedOnceEventHandler(element, (_, _) => {
                var window = Window.GetWindow(element);
                TargetElementStateDict[targetId].ElementWindow = window;
                window.PreviewMouseUp -= WindowPreviewMouseUpHandler;
                window.PreviewMouseUp += WindowPreviewMouseUpHandler;
            });
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

    /// <summary>
    /// 设置点击 <paramref name="element"/> 时显示 <paramref name="targetId"/> 对应的控件
    /// </summary>
    /// <param name="element"></param>
    /// <param name="targetId"></param>
    public static void SetOpenOnClick(UIElement element, string targetId) {
        SourceElementTargetIdDict[element] = targetId;
        element.PreviewMouseDown -= PreviewMouseDownHandler;
        element.PreviewMouseDown += PreviewMouseDownHandler;
        if (element is ButtonBase button) {
            button.Click -= ButtonClickHandler;
            button.Click += ButtonClickHandler;
        } else {
            element.MouseUp -= MouseUpHandler;
            element.MouseUp += MouseUpHandler;
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
        var state = TargetElementStateDict[SourceElementTargetIdDict[element]];
        if (state.PreventOpen) {
            state.PreventOpen = false;
            return;
        }
        state.Element.Visibility = Visibility.Visible;
    }

    private static void MouseUpHandler(object sender, MouseButtonEventArgs e) {
        HandleMouseEvent(sender);
    }

    private static void ButtonClickHandler(object sender, RoutedEventArgs e) {
        HandleMouseEvent(sender);
    }
}

/// <summary>
/// 鼠标移入显示，移除隐藏 TargetElement
/// </summary>
public static class HoverVisibleHelper {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
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
            Logger.Warn("Element is not UIElement");
            return;
        }
        DependencyPropertyDescriptor
            .FromProperty(UIElement.IsMouseOverProperty, element.GetType())
            .AddValueChanged(element, (sender, _) => {
                if (sender is UIElement element) {
                    GetTargetElement(element).Visibility = element.IsMouseOver ? Visibility.Visible : Visibility.Collapsed;
                }
            });
    }
}