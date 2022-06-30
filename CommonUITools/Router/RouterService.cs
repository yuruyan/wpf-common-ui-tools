using ModernWpf.Media.Animation;
using System.Reflection;
using System.Windows.Controls;

namespace CommonUITools.Route;

public class RouterService {
    private readonly Frame Frame;
    private readonly Dictionary<Type, RouterInfo> Routers = new();
    private readonly FieldInfo? TransitionFieldInfo; // 通过反射设置过渡动画
    public readonly NavigationTransitionInfo DrillInTransitionInfo = new DrillInNavigationTransitionInfo();

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
    /// <param name="path"></param>
    /// <returns></returns>
    public object GetInstance(Type path) {
        if (!Routers.ContainsKey(path)) {
            throw new KeyNotFoundException("View 不存在");
        }
        Routers[path].Instance = (
            Routers[path].Instance ?? Activator.CreateInstance(Routers[path].ClassType)
        ) ?? throw new NullReferenceException($"对象 {path.GetType()} 创建失败");
#pragma warning disable CS8603 // Possible null reference return.
        return Routers[path].Instance;
#pragma warning restore CS8603 // Possible null reference return.
    }

    public RouterService(Frame frame, IEnumerable<Type> routers) {
        Frame = frame;
        // 添加路由信息
        foreach (var item in routers) {
            Routers.Add(item, new(item));
        }
        TransitionFieldInfo = frame
                              .GetType()
                              .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .FirstOrDefault(f => f.Name == "_transitionInfoOverride");
        //TransitionFieldInfo?.SetValue(Frame, DrillInTransitionInfo);
    }

    /// <summary>
    /// 导航至页面
    /// </summary>
    /// <param name="path">路由</param>
    /// <param name="transitionInfo">过渡动画</param>
    public void Navigate(Type path, NavigationTransitionInfo? transitionInfo = null) {
        if (!Routers.ContainsKey(path)) {
            throw new KeyNotFoundException("View 不存在");
        }
        Routers[path].Instance = (
            Routers[path].Instance ?? Activator.CreateInstance(Routers[path].ClassType)
        ) ?? throw new NullReferenceException($"对象 {path.GetType()} 创建失败");
        if (transitionInfo == null) {
            Frame.Navigate(Routers[path].Instance);
            return;
        }
        // 设置过渡动画
        TransitionFieldInfo?.SetValue(Frame, transitionInfo);
        Frame.Navigate(Routers[path].Instance);
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
