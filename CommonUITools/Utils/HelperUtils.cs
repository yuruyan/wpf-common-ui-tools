using CommonUITools.Converter;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CommonUITools.Utils;

/// <summary>
/// 显示时开始动画
/// </summary>
public class ScaleAnimationHelper {

    public enum ScaleOption {
        TopLeft,
        Center,
    }

    private record State {
        public ScaleOption ScaleOption { get; set; } = ScaleOption.TopLeft;
        public Storyboard Storyboard { get; set; }
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
            return StoryboardDict[element].Storyboard;
        }
        // 必须要有 RenderTransform
        element.RenderTransform = CreateScaleTransform(element, StoryboardDict[element].ScaleOption);
        DoubleAnimation scaleYAnimation = new(0.3, 1, new Duration(TimeSpan.FromMilliseconds(300))) {
            EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 3 }
        };
        DoubleAnimation scaleXAnimation = new(0.3, 1, new Duration(TimeSpan.FromMilliseconds(300))) {
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
            ScaleTransform transform = new();
            IValueConverter divideTwoConverter = new DivideTwoConverter();
            Binding widthBinding = new("ActualWidth") {
                Source = element,
                Converter = divideTwoConverter,
            };
            Binding heightBinding = new("ActualHeight") {
                Source = element,
                Converter = divideTwoConverter,
            };
            BindingOperations.SetBinding(transform, ScaleTransform.CenterXProperty, widthBinding);
            BindingOperations.SetBinding(transform, ScaleTransform.CenterYProperty, heightBinding);
            return transform;
        }

        private static ScaleTransform CreateTopLeftScaleTransform(FrameworkElement element) {
            return new ScaleTransform();
        }

    }
}

/// <summary>
/// 点击自身开始缩放动画
/// </summary>
public class MouseEventScaleAnimationHelper {

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
        ScaleTransform transform = new();
        IValueConverter divideTwoConverter = new DivideTwoConverter();
        Binding widthBinding = new("ActualWidth") {
            Source = element,
            Converter = divideTwoConverter,
        };
        Binding heightBinding = new("ActualHeight") {
            Source = element,
            Converter = divideTwoConverter,
        };
        BindingOperations.SetBinding(transform, ScaleTransform.CenterXProperty, widthBinding);
        BindingOperations.SetBinding(transform, ScaleTransform.CenterYProperty, heightBinding);
        return transform;
    }
}

/// <summary>
/// 淡入动画
/// </summary>
public class FadeInAnimationHelper {
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
