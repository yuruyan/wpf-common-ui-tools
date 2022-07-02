using CommonUITools.Utils;
using ModernWpf.Media.Animation;
using System.Reflection;
using System.Windows.Controls;

namespace CommonUITools.Route;

public enum NavigationTransitionEffect {
    Entrance,
    DrillIn,
    Suppress,
    Slide,
}

public class RouterService {
    private readonly Frame Frame;
    private readonly IDictionary<Type, RouterInfo> Routers = new Dictionary<Type, RouterInfo>();
    private readonly IDictionary<NavigationTransitionEffect, NavigationTransitionInfo> NavigationTransitionEffectDict = new Dictionary<NavigationTransitionEffect, NavigationTransitionInfo>() {
        {NavigationTransitionEffect.DrillIn, new DrillInNavigationTransitionInfo()},
        {NavigationTransitionEffect.Entrance, new EntranceNavigationTransitionInfo()},
        {NavigationTransitionEffect.Suppress, new SuppressNavigationTransitionInfo()},
        {NavigationTransitionEffect.Slide, new SlideNavigationTransitionInfo()},
    };
    /// <summary>
    /// 默认 NavigationTransitionInfo
    /// </summary>
    public NavigationTransitionEffect DefalutNavigationTransitionEffect = NavigationTransitionEffect.Entrance;
    private readonly FieldInfo? TransitionFieldInfo; // 通过反射设置过渡动画

    private record RouterInfo {
        public RouterInfo(Type classType) {
            ClassType = classType;
        }
        public Type ClassType { get; set; }
        public object? Instance { get; set; }
    }

    /// <summary>
    /// 获取实例对象，对象未创建则会动态创建
    /// </summary>
    /// <param name="view"></param>
    /// <returns></returns>
    public object GetInstance(Type view) {
        if (!Routers.ContainsKey(view)) {
            throw new KeyNotFoundException("View 不存在");
        }
        Routers[view].Instance = (
            Routers[view].Instance ?? Activator.CreateInstance(Routers[view].ClassType)
        ) ?? throw new NullReferenceException($"对象 {view.GetType()} 创建失败");
        return CommonUtils.NullCheck(Routers[view].Instance);
    }

    public RouterService(ModernWpf.Controls.Frame frame, IEnumerable<Type> routers, NavigationTransitionEffect defaultNavigationTransitionEffect = NavigationTransitionEffect.Entrance) {
        Frame = frame;
        // 添加路由信息
        foreach (var item in routers) {
            Routers.Add(item, new(item));
        }
        TransitionFieldInfo = frame
                              .GetType()
                              .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .FirstOrDefault(f => f.Name == "_transitionInfoOverride");
        SetTransitionEffect(defaultNavigationTransitionEffect);
    }

    public void Navigate(Type view) {
        Navigate(view, DefalutNavigationTransitionEffect);
    }

    /// <summary>
    /// 导航至页面
    /// </summary>
    /// <param name="view">路由</param>
    /// <param name="effect">过渡动画</param>
    public void Navigate(Type view, NavigationTransitionEffect effect) {
        if (!Routers.ContainsKey(view)) {
            throw new KeyNotFoundException("View 不存在");
        }
        _ = GetInstance(view);
        // 设置过渡动画
        SetTransitionEffect(effect);
        Frame.Navigate(Routers[view].Instance);
    }

    private void SetTransitionEffect(NavigationTransitionEffect effect) {
        TransitionFieldInfo?.SetValue(Frame, NavigationTransitionEffectDict[effect]);
    }

    /// <summary>
    /// 返回
    /// </summary>
    public void Back() {
        if (Frame.CanGoBack) {
            Frame.GoBack();
        }
    }
}
