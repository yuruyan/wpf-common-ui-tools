using CommonUITools.Widget;
using System.Reflection;
using System.Windows.Documents;
using System.Windows.Media.Effects;

namespace CommonUITools.Utils;

/// <summary>
/// 显示时开始动画
/// </summary>
public static class ScaleAnimationHelper {

    public enum ScaleOption {
        TopLeft,
        Center,
    }

    private record State {
        public ScaleOption ScaleOption { get; set; } = ScaleOption.TopLeft;
        public Storyboard? Storyboard { get; set; }
    }

    /// <summary>
    /// Storyboard 缓存
    /// </summary>
    private static readonly IDictionary<FrameworkElement, State> StoryboardDict = new Dictionary<FrameworkElement, State>();

    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ScaleAnimationHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyProperty ScaleOptionProperty = DependencyProperty.RegisterAttached("ScaleOption", typeof(ScaleOption), typeof(ScaleAnimationHelper), new PropertyMetadata(ScaleOption.TopLeft, ScaleOptionPropertyChangedHandler));

    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }
    /// <summary>
    /// 缩放选项
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static ScaleOption GetScaleOption(DependencyObject obj) {
        return (ScaleOption)obj.GetValue(ScaleOptionProperty);
    }
    public static void SetScaleOption(DependencyObject obj, ScaleOption value) {
        obj.SetValue(ScaleOptionProperty, value);
    }

    /// <summary>
    /// ScaleOptionPropertyChangedHandler
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void ScaleOptionPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            var option = (ScaleOption)e.NewValue;
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
            return StoryboardDict[element].Storyboard!;
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
    private static ScaleTransform CreateScaleTransform(FrameworkElement element, ScaleOption scaleOption) {
        return ScaleTransformFactory.CreateScaleTransform(element, scaleOption);
    }

    /// <summary>
    /// ScaleTransformFactory
    /// </summary>
    private static class ScaleTransformFactory {
        public static ScaleTransform CreateScaleTransform(FrameworkElement element, ScaleOption scaleOption) {
            return scaleOption switch {
                ScaleOption.Center => CreateCenterScaleTransform(element),
                ScaleOption.TopLeft => CreateTopLeftScaleTransform(element),
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
}

/// <summary>
/// 点击自身开始缩放动画
/// </summary>
public static class MouseEventScaleAnimationHelper {

    private enum Direction {
        X, Y
    }

    private static readonly int AnimationDuration = 200;
    private static readonly IDictionary<FrameworkElement, IEnumerable<TriggerBase>> TriggerCollectionDict = new Dictionary<FrameworkElement, IEnumerable<TriggerBase>>();
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(MouseEventScaleAnimationHelper), new PropertyMetadata(false, IsEnabledChangedHandler));

    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            // 添加 trigger
            if ((bool)e.NewValue) {
                foreach (var item in GetTriggerCollection(element)) {
                    element.Triggers.Add(item);
                }
            } else {
                foreach (var item in GetTriggerCollection(element)) {
                    element.Triggers.Remove(item);
                }
            }
        }
    }

    /// <summary>
    /// 获取 TriggerCollection
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static IEnumerable<TriggerBase> GetTriggerCollection(FrameworkElement element) {
        if (TriggerCollectionDict.ContainsKey(element)) {
            return TriggerCollectionDict[element];
        }
        var triggers = CreateTriggerCollection(element);
        // 缓存
        TriggerCollectionDict[element] = triggers;
        return triggers;
    }

    /// <summary>
    /// 创建 TriggerCollection
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static IEnumerable<TriggerBase> CreateTriggerCollection(FrameworkElement element) {
        CheckAndSetRenderTransform(element);
        return new List<TriggerBase>() {
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
    private static TriggerBase CreateTrigger(FrameworkElement element, RoutedEvent routedEvent, double to) {
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
    /// <param name="direction"></param>
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
        //Transform renderTransform = element.RenderTransform;
        //if (renderTransform is TransformGroup group) {
        //    // 不包含 ScaleTransform
        //    if (!group.Children.Select(t => t.GetType()).Contains(typeof(ScaleTransform))) {
        //        group.Children.Add(CreateScaleTransform(element));
        //    }
        //} else if (renderTransform is not ScaleTransform) {
        //    // 将其他 Transform 添加到 TransformGroup
        //    var newGroup = new TransformGroup();
        //    newGroup.Children.Add(element.RenderTransform);
        //    newGroup.Children.Add(CreateScaleTransform(element));
        //    element.RenderTransform = newGroup;
        //}
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
}

/// <summary>
/// 淡入动画
/// </summary>
public static class FadeInAnimationHelper {
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(EnabledProperty);
    }
    public static void SetEnabled(DependencyObject obj, bool value) {
        obj.SetValue(EnabledProperty, value);
    }
    /// <summary>
    /// 持续时间 ms
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static int GetDuration(DependencyObject obj) {
        return (int)obj.GetValue(DurationProperty);
    }
    public static void SetDuration(DependencyObject obj, int value) {
        obj.SetValue(DurationProperty, value);
    }

    /// <summary>
    /// 默认动画持续时间
    /// </summary>
    public const int DefaultDuration = 250;
    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached("Duration", typeof(int), typeof(FadeInAnimationHelper), new PropertyMetadata(DefaultDuration, DurationPropertyChangedHandler));
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(FadeInAnimationHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    private static readonly Dictionary<FrameworkElement, Storyboard> StoryBoardDict = new();

    /// <summary>
    /// IsEnabled 改变
    /// </summary>
    /// <param name="d"></param>
    /// <param name="e"></param>
    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is FrameworkElement element) {
            if ((bool)e.NewValue) {
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
        if (sender is FrameworkElement element && Convert.ToBoolean(e.NewValue)) {
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
            DoubleAnimation doubleAnimation = new(0, 1, new(TimeSpan.FromMilliseconds(GetDuration(element))));
            Storyboard.SetTarget(doubleAnimation, element);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Opacity"));
            storyboard = new Storyboard() {
                Children = new(new Timeline[] { doubleAnimation })
            };
            StoryBoardDict[element] = storyboard;
        }
        return storyboard;
    }
}

/// <summary>
/// GridViewColumnHelper
/// </summary>
public static class GridViewColumnHelper {

    public static double GetHeaderMinWidth(DependencyObject obj) {
        return (double)obj.GetValue(HeaderMinWidthProperty);
    }
    public static void SetHeaderMinWidth(DependencyObject obj, double value) {
        obj.SetValue(HeaderMinWidthProperty, value);
    }
    /// <summary>
    /// Header 最小宽度
    /// </summary>
    public static readonly DependencyProperty HeaderMinWidthProperty = DependencyProperty.RegisterAttached("HeaderMinWidth", typeof(double), typeof(GridViewColumnHelper), new PropertyMetadata(0.0, HeaderMinWidthPropertyChangedHandler));

    private static void HeaderMinWidthPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is GridViewColumnHeader header) {
            // 设置 SizeChanged 事件
            TaskUtils.EnsureCalledOnce(header, () => {
                header.SizeChanged += (sender, args) => {
                    if (args.NewSize.Width <= GetHeaderMinWidth(header)) {
                        // 可能为 null
                        if (header.Column is null) {
                            return;
                        }
                        header.Column.Width = GetHeaderMinWidth(header);
                        args.Handled = true;
                    }
                };
            });
        }
    }
}

/// <summary>
/// DragDropHelper，DragOver 时背景会发生变化
/// 默认背景为 ApplicationBackgroundBrushLight1
/// </summary>
public static class DragDropHelper {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DragDropHelper), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    public static readonly DependencyProperty OverBackgroundProperty = DependencyProperty.RegisterAttached("OverBackground", typeof(Brush), typeof(DragDropHelper), new PropertyMetadata());

    /// <summary>
    /// 是否启用
    /// </summary>
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }
    /// <summary>
    /// 拖拽进入区域时背景颜色
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Brush GetOverBackground(DependencyObject obj) {
        return (Brush)obj.GetValue(OverBackgroundProperty);
    }
    public static void SetOverBackground(DependencyObject obj, Brush value) {
        obj.SetValue(OverBackgroundProperty, value);
    }

    /// <summary>
    /// 元素初始状态设置的 Background
    /// </summary>
    private static readonly IDictionary<DependencyObject, Brush> ElementBackgroundDict = new Dictionary<DependencyObject, Brush>();
    private static readonly IDictionary<DependencyObject, BindingExpression?> ElementBackgroundBindingDict = new Dictionary<DependencyObject, BindingExpression?>();
    /// <summary>
    /// 元素 Background PropertyInfo
    /// </summary>
    private static readonly IDictionary<DependencyObject, PropertyInfo> ElementPropertyDict = new Dictionary<DependencyObject, PropertyInfo>();
    private static readonly Brush FallbackOverBackgroundBrush = new SolidColorBrush(Colors.Transparent);

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }

        var isEnabled = (bool)e.NewValue;
        var backgroundInfo = d.GetType()
            .GetProperty("Background", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public);
        // 没有 BackgroundProperty 属性或类型不正确
        if (backgroundInfo == null || backgroundInfo.PropertyType != typeof(Brush)) {
            return;
        }

        // 设置初始值
        if (!ElementBackgroundDict.ContainsKey(element)) {
            ElementPropertyDict[element] = backgroundInfo;
            // 还没有初始化 background 时设置为 transparent
            ElementBackgroundDict[element] = backgroundInfo.GetValue(d) as Brush ?? new SolidColorBrush(Colors.Transparent);
        }
        // 设置事件
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
        // 文本框特殊处理
        if (element is TextBox textBox) {
            if (isEnabled) {
                textBox.PreviewDragOver += TextBoxPreviewDragOverHandler;
            } else {
                textBox.PreviewDragOver -= TextBoxPreviewDragOverHandler;
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

    private static void PreviewDropHandler(object sender, DragEventArgs e) {
        if (sender is DependencyObject element) {
            ElementPropertyDict[element].SetValue(element, FallbackOverBackgroundBrush);
        }
    }

    private static void PreviewDragLeaveHandler(object sender, DragEventArgs e) {
        if (sender is DependencyObject element) {
            ElementPropertyDict[element].SetValue(element, FallbackOverBackgroundBrush);
        }
    }

    private static void PreviewDragEnterHandler(object sender, DragEventArgs e) {
        if (sender is FrameworkElement element) {
            var newBrush = GetOverBackground(element);
            newBrush ??= element.TryFindResource("DragDropOverBackgroundBrush") as SolidColorBrush;
            ElementPropertyDict[element].SetValue(
                element,
                newBrush ?? FallbackOverBackgroundBrush
            );
        }
    }
}

/// <summary>
/// 加载状态
/// </summary>
public static class LoadingBoxHelper {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.RegisterAttached("IsLoading", typeof(bool), typeof(LoadingBoxHelper), new PropertyMetadata(false, IsLoadingPropertyChangedHandler));
    public static readonly DependencyProperty SizeProperty = DependencyProperty.RegisterAttached("Size", typeof(double), typeof(LoadingBoxHelper), new PropertyMetadata(LoadingBox.DefaultSize, SizePropertyChangedHandler));

    /// <summary>
    /// 是否显示加载
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsLoading(DependencyObject obj) {
        return (bool)obj.GetValue(IsLoadingProperty);
    }
    public static void SetIsLoading(DependencyObject obj, bool value) {
        obj.SetValue(IsLoadingProperty, value);
    }
    /// <summary>
    /// loading size
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static double GetSize(DependencyObject obj) {
        return (double)obj.GetValue(SizeProperty);
    }
    public static void SetSize(DependencyObject obj, double value) {
        obj.SetValue(SizeProperty, value);
    }

    private static readonly IDictionary<FrameworkElement, LoadingBox> LoadingBoxDict = new Dictionary<FrameworkElement, LoadingBox>();
    /// <summary>
    /// 未完成的任务
    /// </summary>
    private static readonly Queue<Action> UndoneTasks = new();

    private static void IsLoadingPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        bool isLoading = (bool)e.NewValue;
        // 初始化过
        if (LoadingBoxDict.ContainsKey(element)) {
            Proceed(element, isLoading);
            return;
        }

        // 进行初始化工作
        if (element.IsLoaded) {
            AddLoadingAdorner(element);
            Proceed(element, isLoading);
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }

    private static void SizePropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not FrameworkElement element) {
            return;
        }
        double size = (double)e.NewValue;
        // 已初始化
        if (LoadingBoxDict.TryGetValue(element, out var loadingBox)) {
            loadingBox.Size = size;
            return;
        }
        UndoneTasks.Enqueue(() => LoadingBoxDict[element].Size = size);
    }

    private static void ElementLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is FrameworkElement element) {
            AddLoadingAdorner(element);
            Proceed(element, (bool)element.GetValue(IsLoadingProperty));
        }
    }

    /// <summary>
    /// 继续
    /// </summary>
    /// <param name="element"></param>
    /// <param name="isLoading"></param>
    private static void Proceed(FrameworkElement element, bool isLoading) {
        if (LoadingBoxDict.TryGetValue(element, out var loadingBox)) {
            // 执行未完成的任务
            while (UndoneTasks.Count > 0) {
                UndoneTasks.Dequeue()();
            }
            if (isLoading) {
                loadingBox.Show();
            } else {
                loadingBox.Close();
            }
        }
    }

    /// <summary>
    /// 添加 LoadingAdorner
    /// </summary>
    /// <param name="element"></param>
    private static void AddLoadingAdorner(FrameworkElement element) {
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
}

/// <summary>
/// Name
/// </summary>
public static class NameHelper {
    public static readonly DependencyProperty NameProperty = DependencyProperty.RegisterAttached("Name", typeof(string), typeof(NameHelper), new PropertyMetadata());
    /// <summary>
    /// Name
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetName(DependencyObject obj) {
        return (string)obj.GetValue(NameProperty);
    }
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
    public static readonly DependencyProperty IsLongPressEventHappendProperty = DependencyProperty.RegisterAttached("IsLongPressEventHappend", typeof(bool), typeof(LongPressHelper), new PropertyMetadata(false));
    public static readonly DependencyProperty LongPressProperty = DependencyProperty.RegisterAttached("LongPress", typeof(EventHandler), typeof(LongPressHelper), new PropertyMetadata());
    private static readonly IDictionary<FrameworkElement, LongPressEventInfo> ElementLongPressEventInfoDict = new Dictionary<FrameworkElement, LongPressEventInfo>();

    /// <summary>
    /// 长按事件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static EventHandler GetLongPress(DependencyObject obj) {
        return (EventHandler)obj.GetValue(LongPressProperty);
    }
    public static void SetLongPress(DependencyObject obj, EventHandler value) {
        obj.SetValue(LongPressProperty, value);
    }
    /// <summary>
    /// 是否触发了长按事件
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsLongPressEventHappend(DependencyObject obj) {
        return (bool)obj.GetValue(IsLongPressEventHappendProperty);
    }
    private static void SetIsLongPressEventHappend(DependencyObject obj, bool value) {
        obj.SetValue(IsLongPressEventHappendProperty, value);
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
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
                    SetIsLongPressEventHappend(element, true);
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
        SetIsLongPressEventHappend(element, false);
        info.MouseDownTime = DateTime.Now;
        info.Timer.Start();
    }

    private static void PreviewLeftButtonUp(object sender, MouseButtonEventArgs e) {
        if (sender is not FrameworkElement element) {
            return;
        }

        ElementLongPressEventInfoDict[element].Timer.Stop();
    }
}

/// <summary>
/// 加载动画
/// </summary>
public static class LoadedStoryboardHelper {
    public static readonly DependencyProperty StoryboardProperty = DependencyProperty.RegisterAttached("Storyboard", typeof(Storyboard), typeof(LoadedStoryboardHelper), new PropertyMetadata());
    public static readonly DependencyProperty TimesProperty = DependencyProperty.RegisterAttached("Times", typeof(int), typeof(LoadedStoryboardHelper), new PropertyMetadata(-1));

    /// <summary>
    /// 动画
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Storyboard GetStoryboard(DependencyObject obj) {
        return (Storyboard)obj.GetValue(StoryboardProperty);
    }
    public static void SetStoryboard(DependencyObject obj, Storyboard value) {
        obj.SetValue(StoryboardProperty, value);
        if (obj is not FrameworkElement element) {
            return;
        }
        element.Loaded -= ElementLoadedHandler;
        element.Loaded += ElementLoadedHandler;
    }
    /// <summary>
    /// 执行时间，小于 0 表示无限执行，默认 -1
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    /// <remarks>
    /// 只能设置一次，多次设置无效
    /// </remarks>
    public static int GetTimes(DependencyObject obj) {
        return (int)obj.GetValue(TimesProperty);
    }
    public static void SetTimes(DependencyObject obj, int value) {
        if (GetTimes(obj) == -1) {
            obj.SetValue(TimesProperty, value);
        }
    }
    /// <summary>
    /// 执行次数
    /// </summary>
    private static readonly IDictionary<DependencyObject, int> ElementInvokeTimesDict = new Dictionary<DependencyObject, int>();

    /// <summary>
    /// 元素加载
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void ElementLoadedHandler(object sender, RoutedEventArgs e) {
        if (sender is DependencyObject obj && GetStoryboard(obj) is Storyboard storyboard) {
            // 初始化
            if (!ElementInvokeTimesDict.ContainsKey(obj)) {
                ElementInvokeTimesDict[obj] = 0;
            }
            int times = GetTimes(obj);
            // 无限次数或未达到
            if (times < 0 || ElementInvokeTimesDict[obj] < times) {
                ElementInvokeTimesDict[obj]++;
                storyboard.Begin();
            }
        }
    }
}

/// <summary>
/// 添加阴影
/// </summary>
public static class DropShadowEffectHelper {
    public enum EffectWeight {
        None,
        Normal,
        Lighter,
        Darker,
    }

    /// <summary>
    /// 阴影程度
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static EffectWeight GetWeight(DependencyObject obj) {
        return (EffectWeight)obj.GetValue(WeightProperty);
    }
    public static void SetWeight(DependencyObject obj, EffectWeight value) {
        obj.SetValue(WeightProperty, value);
    }
    public static readonly DependencyProperty WeightProperty = DependencyProperty.RegisterAttached("Weight", typeof(EffectWeight), typeof(DropShadowEffectHelper), new PropertyMetadata(EffectWeight.None, EffectWeightChangedHandler));
    private static readonly Color DropShadowColor = UIUtils.StringToColor("#dfdfdf");

    private static void EffectWeightChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is UIElement element) {
            if (e.NewValue is EffectWeight weight) {
                element.Effect = weight switch {
                    EffectWeight.None => null,
                    EffectWeight.Lighter => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 8 },
                    EffectWeight.Normal => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 16 },
                    EffectWeight.Darker => new DropShadowEffect() { Color = DropShadowColor, ShadowDepth = 0, BlurRadius = 32 },
                    _ => throw new ArgumentException("Invalid value")
                };
            }
        }
    }
}

/// <summary>
/// 移除 ListBox DragSelection 行为
/// </summary>
public static class RemoveListBoxDefaultSelectionBehavior {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RemoveListBoxDefaultSelectionBehavior), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));
    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        var isEnabled = (bool)e.NewValue;
        if (isEnabled) {
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
}

/// <summary>
/// 移除 ListBoxItem MouseRightClickSelection 行为
/// </summary>
public static class RemoveListBoxItemDefaultSelectionBehavior {
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(RemoveListBoxItemDefaultSelectionBehavior), new PropertyMetadata(false, IsEnabledPropertyChangedHandler));

    /// <summary>
    /// 是否启用
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool GetIsEnabled(DependencyObject obj) {
        return (bool)obj.GetValue(IsEnabledProperty);
    }
    public static void SetIsEnabled(DependencyObject obj, bool value) {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void IsEnabledPropertyChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        var isEnabled = (bool)e.NewValue;
        if (isEnabled) {
            element.MouseRightButtonDown += MouseRightButtonDownHandler;
        } else {
            element.MouseRightButtonDown -= MouseRightButtonDownHandler;
        }
    }

    private static void MouseRightButtonDownHandler(object sender, MouseButtonEventArgs e) {
        e.Handled = true;
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

    /// <summary>
    /// MouseButtonEventHandler
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static MouseButtonEventHandler GetHandler(DependencyObject obj) {
        return (MouseButtonEventHandler)obj.GetValue(HandlerProperty);
    }
    public static void SetHandler(DependencyObject obj, MouseButtonEventHandler value) {
        obj.SetValue(HandlerProperty, value);
    }

    private static void MouseButtonEventHandlerChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e) {
        if (d is not UIElement element) {
            return;
        }
        TaskUtils.EnsureCalledOnce(d, () => {
            element.MouseLeftButtonUp += (sender, args) => {
                var notTime = DateTimeUtils.CuruentMilliseconds;
                var elem = (UIElement)sender;
                if (ElementClickTime.TryGetValue(elem, out var lastClickTime)) {
                    // Raise event
                    if (notTime - lastClickTime <= DoubleClickTime) {
                        // Restrict raise times
                        if (!ElementEventRaisedTime.TryGetValue(elem, out var lastRaisedTime) || notTime - lastRaisedTime >= DoubleClickTime) {
                            ElementEventRaisedTime[elem] = notTime;
                            GetHandler(elem).Invoke(sender, args);
                        }
                    }
                }
                ElementClickTime[elem] = DateTimeUtils.CuruentMilliseconds;
            };
        });
    }

}