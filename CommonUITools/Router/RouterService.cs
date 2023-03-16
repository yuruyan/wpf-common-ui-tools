using System.Reflection;
using System.Windows.Navigation;

namespace CommonUITools.Route;

/// <summary>
/// 如果需要接受参数，需要实现 <see cref="INavigationService"/> 接口
/// </summary>
public class RouterService {
    private readonly IReadOnlyList<Type> Routers;
    /// <summary>
    /// 页面实例
    /// </summary>
    private readonly IDictionary<Type, object> RouterInstanceDict = new Dictionary<Type, object>();
    /// <summary>
    /// 额外参数
    /// </summary>
    private readonly IDictionary<Type, object?> PageArgsDict = new Dictionary<Type, object?>();
    /// <summary>
    /// 默认 NavigationTransitionInfo
    /// </summary>
    public readonly NavigationTransitionEffect DefaultTransitionEffect;
    private readonly FieldInfo? TransitionFieldInfo; // 通过反射设置过渡动画
    /// <summary>
    /// 当前 Service Frame
    /// </summary>
    public Frame Frame { get; }
    /// <summary>
    /// 当前页面 Type
    /// </summary>
    public Type? CurrentPageType => Frame.Content?.GetType();
    /// <summary>
    /// 当前页面实例
    /// </summary>
    public object? CurrentPage => Frame.Content;

    /// <summary>
    /// 获取实例对象，对象未创建则会动态创建
    /// </summary>
    /// <param name="pageType"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">View 不存在</exception>
    /// <exception cref="NullReferenceException">对象创建失败</exception>
    public object GetInstance(Type pageType) {
        if (!Routers.Contains(pageType)) {
            throw new KeyNotFoundException($"{pageType} dosen't exist in Routers");
        }
        // Initialize
        if (!RouterInstanceDict.TryGetValue(pageType, out var instance)) {
            RouterInstanceDict[pageType] = instance = Activator.CreateInstance(pageType)!;
        }
        return instance;
    }

    /// <inheritdoc cref="GetInstance(Type)"/>
    public T GetInstance<T>() {
        return (T)GetInstance(typeof(T));
    }

    /// <summary>
    /// 移除页面实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemovePage<T>() {
        RouterInstanceDict.Remove(typeof(T));
        PageArgsDict.Remove(typeof(T));
    }

    /// <summary>
    /// 移除页面实例
    /// </summary>
    /// <param name="pageType"></param>
    public void RemovePage(Type pageType) {
        RouterInstanceDict.Remove(pageType);
        PageArgsDict.Remove(pageType);
    }

    /// <summary>
    /// 获取子路由类型信息
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Type> GetRouteTypes() => Routers;

    public RouterService(ModernWpf.Controls.Frame frame, IEnumerable<Type> routers, NavigationTransitionEffect defaultNavigationTransitionEffect = NavigationTransitionEffect.Entrance) {
        Frame = frame;
        Routers = routers.ToArray();
        DefaultTransitionEffect = defaultNavigationTransitionEffect;
        TransitionFieldInfo = frame
            .GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(f => f.Name == "_transitionInfoOverride");
        SetTransitionEffect(defaultNavigationTransitionEffect);

        frame.Navigated += FrameNavigatedHandler;
    }

    /// <summary>
    /// 传递参数
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FrameNavigatedHandler(object sender, NavigationEventArgs e) {
        // Reset
        SetTransitionEffect(DefaultTransitionEffect);
        if (e.Content is not INavigationService service) {
            return;
        }
        var pageType = e.Content.GetType();
        if (PageArgsDict.TryGetValue(pageType, out var args)) {
            service.Navigated(args);
            // 清除
            PageArgsDict.Remove(pageType);
        }
    }

    /// <inheritdoc cref="Navigate(Type, object, NavigationTransitionEffect)"/>
    public void Navigate(Type view) => Navigate(view, null, DefaultTransitionEffect);

    /// <inheritdoc cref="Navigate(Type, object?, NavigationTransitionEffect)"/>
    public void Navigate(Type view, NavigationTransitionEffect effect) => Navigate(view, null, effect);

    /// <inheritdoc cref="Navigate(Type, object, NavigationTransitionEffect)"/>
    public void Navigate(Type view, object? args) => Navigate(view, args, DefaultTransitionEffect);

    /// <summary>
    /// 导航
    /// </summary>
    /// <param name="view">PageType</param>
    /// <param name="args">Extra data</param>
    /// <param name="effect">Navigation animation</param>
    public void Navigate(Type view, object? args, NavigationTransitionEffect effect) {
        if (!Routers.Contains(view)) {
            throw new KeyNotFoundException($"{view} dosen't exist in Routers");
        }
        // 设置过渡动画
        if (effect != DefaultTransitionEffect) {
            SetTransitionEffect(effect);
        }
        PageArgsDict[view] = args;
        Frame.Navigate(GetInstance(view));
    }

    private void SetTransitionEffect(NavigationTransitionEffect effect) {
        TransitionFieldInfo?.SetValue(
            Frame,
            ConstantCollections.NavigationTransitionEffectDict[effect]
        );
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
