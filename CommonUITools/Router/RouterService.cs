using ModernWpf.Media.Animation;
using System.Reflection;
using System.Windows.Controls;

namespace CommonUITools.Route;

public class RouterService {
    private readonly Frame Frame;
    private readonly Dictionary<Type, RouterInfo> Routers = new();
    private readonly FieldInfo? TransitionFieldInfo; // 通过反射设置过渡动画
    private readonly NavigationTransitionInfo _TransitionInfo = new DrillInNavigationTransitionInfo();

    private class RouterInfo {
        public Type ClassType { get; set; } = typeof(object);
        public object Instance { get; set; }
    }

    /// <summary>
    /// 获取实例对象
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public object GetInstance(Type path) {
        Routers[path].Instance = Routers[path].Instance ?? Activator.CreateInstance(Routers[path].ClassType);
        return Routers[path].Instance;
    }

    public RouterService(Frame frame, IEnumerable<Type> routers) {
        Frame = frame;
        // 添加路由信息
        foreach (var item in routers) {
            Routers.Add(item, new() { ClassType = item });
        }
        TransitionFieldInfo = frame
                              .GetType()
                              .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                              .FirstOrDefault(f => f.Name == "_transitionInfoOverride");
        TransitionFieldInfo?.SetValue(Frame, _TransitionInfo);
    }

    /// <summary>
    /// 导航至页面
    /// </summary>
    /// <param name="path">路由</param>
    /// <param name="transitionInfo">过渡动画</param>
    public void Navigate(Type path, NavigationTransitionInfo? transitionInfo = null) {
        Routers[path].Instance = Routers[path].Instance ?? Activator.CreateInstance(Routers[path].ClassType);
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
