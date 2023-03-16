using ModernWpf.Media.Animation;
using System.Reflection;

namespace CommonUITools.Route;

public enum NavigationTransitionEffect {
    Entrance,
    DrillIn,
    Suppress,
    Slide,
}

public class RouterNode {
    /// <summary>
    /// 路由名称，会清除两边的 <see cref="Router.RouterSeparator"/>
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 页面 Type
    /// </summary>
    public Type PageType { get; }
    /// <summary>
    /// Frame 名称，即 Name 属性
    /// </summary>
    public string? FrameName { get; }
    /// <summary>
    /// 子节点
    /// </summary>
    public IReadOnlyCollection<RouterNode> Children { get; }

    public RouterNode(
        string name,
        Type pageType,
        string? frameName = null,
        IReadOnlyCollection<RouterNode>? children = null
    ) {
        Name = name;
        FrameName = frameName;
        PageType = pageType;
        Children = children ?? Array.Empty<RouterNode>();
    }
}

/// <summary>
/// 路由，一个 Window 最多一个路由
/// </summary>
public class Router {
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly IDictionary<Window, Router> WindowRouterDict = new Dictionary<Window, Router>();

    private class RouterInternalNode : RouterNode {
        public Frame? Frame { get; private set; } = default;
        /// <summary>
        /// 对象实例
        /// </summary>
        private object Instance = default!;
        /// <summary>
        /// 是否有子节点
        /// </summary>
        public bool HasChild { get; }
        /// <summary>
        /// 是否已创建
        /// </summary>
        public bool IsCreated { get; private set; }
        /// <summary>
        /// 是否正在创建
        /// </summary>
        public bool IsCreating { get; private set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public new IReadOnlyCollection<RouterInternalNode> Children { get; }
        /// <summary>
        /// (Name, RouterInternalNode)
        /// </summary>
        public IReadOnlyDictionary<string, RouterInternalNode> ChildrenDict { get; }

        public RouterInternalNode(
            string name,
            Type pageType,
            string? frameName = null,
            IReadOnlyCollection<RouterNode>? children = null
        ) : base(name.Trim(Router.RouterSeparator), pageType, frameName, children) {
            var childList = new List<RouterInternalNode>();
            var childrenDict = new Dictionary<string, RouterInternalNode>();
            Children = childList;
            ChildrenDict = childrenDict;
            // 没有子节点
            if (base.Children.Count == 0) {
                return;
            }
            // 递归创建子节点
            foreach (var child in base.Children) {
                var node = CreateFromRouterNode(child);
                childList.Add(node);
                childrenDict[child.Name] = node;
            }
            HasChild = true;
        }

        /// <summary>
        /// 从 RouterNode 创建
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static RouterInternalNode CreateFromRouterNode(RouterNode node)
            => new(node.Name, node.PageType, node.FrameName, node.Children);

        /// <summary>
        /// 获取对象，如未创建则立即创建
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException">对象创建失败</exception>
        public object GetInstance() {
            if (IsCreated) {
                return Instance;
            }
            IsCreating = true;
            Instance = Activator.CreateInstance(PageType)
                ?? throw new ArgumentException($"Object of type {PageType} created failed");
            IsCreating = false;
            IsCreated = true;
            if (!HasChild) {
                return Instance;
            }
            // 初始化 Frame
            if (string.IsNullOrEmpty(FrameName)) {
                throw new ArgumentException($"FrameName in {PageType} cannot be null or empty");
            }
            if (Instance is FrameworkElement element) {
                if (element.FindName(FrameName) is not Frame frame) {
                    throw new ArgumentException($"Cannot find frame named {FrameName} in {PageType}");
                }
                Frame = frame;
            }
            return Instance;
        }

        /// <summary>
        /// 从已创建结点初始化
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="instance"></param>
        public void InitFromExisting(Frame frame, object instance) {
            Frame = frame;
            Instance = instance;
            IsCreated = true;
        }
    }

    /// <summary>
    /// 路由分隔符
    /// </summary>
    public const char RouterSeparator = '/';
    /// <summary>
    /// 根节点
    /// </summary>
    private RouterInternalNode RootNode { get; }
    /// <summary>
    /// 当前路由
    /// </summary>
    public string CurrentRoute { get; private set; } = string.Empty;
    /// <summary>
    /// 导航路径
    /// </summary>
    private readonly Stack<string> RouteStack = new();
    public event EventHandler<object>? Navigated;

    public static Router GetRouter(DependencyObject instance) => WindowRouterDict[Window.GetWindow(instance)];

    public Router(RouterNode rootRouterNode, Frame rootFrame, DependencyObject rootInstance) {
        RootNode = RouterInternalNode.CreateFromRouterNode(rootRouterNode);
        RootNode.InitFromExisting(rootFrame, rootInstance);
        CurrentRoute = RootNode.Name;
        WindowRouterDict[Window.GetWindow(rootInstance)] = this;
    }

    /// <summary>
    /// 获取路由
    /// </summary>
    /// <param name="routePath">全路径</param>
    /// <returns>not empty routes</returns>
    /// <exception cref="ArgumentException">
    /// routePath isNullOrEmpty or routes is empty
    /// </exception>
    private static IList<string> GetRoutes(string routePath) {
        if (string.IsNullOrEmpty(routePath)) {
            throw new ArgumentException($"RoutePath cannot be null or empty");
        }
        var paths = routePath.Split(RouterSeparator, StringSplitOptions.RemoveEmptyEntries);
        if (paths.Length == 0) {
            throw new ArgumentException($"RoutePath cannot be empty");
        }
        return paths;
    }

    /// <summary>
    /// 获取路由结点，不包括根节点
    /// </summary>
    /// <param name="routePath">全路径</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">路由不匹配</exception>
    private static IList<RouterInternalNode> GetRouteNodesWithoutRoot(RouterInternalNode rootNode, string routePath) {
        var currentNode = rootNode;
        var pathEnumerator = GetRoutes(routePath).GetEnumerator();
        _ = pathEnumerator.MoveNext();
        // 根路由不匹配
        if (pathEnumerator.Current != rootNode.Name) {
            throw new ArgumentException($"Cannot find routePath {routePath}");
        }
        var nodeList = new List<RouterInternalNode>();
        // 从下一结点开始
        while (pathEnumerator.MoveNext()) {
            string path = pathEnumerator.Current;
            // 没有子节点或不包含 path
            if (!currentNode.HasChild || !currentNode.ChildrenDict.ContainsKey(path)) {
                throw new ArgumentException($"Cannot find routePath {routePath}");
            }
            currentNode = currentNode.ChildrenDict[path];
            nodeList.Add(currentNode);
        }
        return nodeList;
    }

    /// <summary>
    /// 确保路径结点都已经初始化
    /// </summary>
    /// <param name="routeNodes"></param>
    /// <returns>全部初始化完成返回 true，正在初始化返回 false</returns>
    private static bool EnsureNodeAreCreated(IList<RouterInternalNode> routeNodes) {
        // 全部初始化完成
        if (routeNodes.All(r => r.IsCreated)) {
            return true;
        }
        // 从子节点开始初始化
        for (int i = routeNodes.Count - 1; i >= 0; i--) {
            var tempNode = routeNodes[i];
            // 正在初始化
            if (tempNode.IsCreating) {
                return false;
            }
            _ = tempNode.GetInstance();
        }
        return true;
    }

    /// <summary>
    /// 导航
    /// </summary>
    /// <param name="routePath">路由以 <see cref="RouterSeparator"/> 分隔，包含根路由</param>
    /// <param name="args">额外参数</param>
    public void Navigate(string routePath, object? args = null) {
        var nodeList = GetRouteNodesWithoutRoot(RootNode, routePath);
        // 正在创建节点
        if (!EnsureNodeAreCreated(nodeList)) {
            return;
        }
        var currentNode = RootNode;
        // 开始导航
        foreach (var item in nodeList) {
            currentNode.Frame!.Navigate(item.GetInstance());
            currentNode = item;
        }
        // invoke target method
        _ = ((Navigated?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
            .FirstOrDefault(d => d.Target == currentNode.GetInstance())
            ?.DynamicInvoke(args));
        CurrentRoute = $"{RootNode.Name}{RouterSeparator}{string.Join(RouterSeparator, nodeList.Select(node => node.Name))}";
        RouteStack.Push(CurrentRoute);
    }

    /// <summary>
    /// 获取路由对应实例对象
    /// </summary>
    /// <param name="routePath"></param>
    /// <returns></returns>
    public object GetInstance(string routePath) {
        var nodeList = GetRouteNodesWithoutRoot(RootNode, routePath);
        // 不应该返回 false
        if (!EnsureNodeAreCreated(nodeList)) {
            Logger.Error("Node created failed. This shouldn't happen");
            throw new ArgumentException("Node created failed.");
        }
        return nodeList[^1].GetInstance();
    }

    /// <summary>
    /// 回到上一级
    /// </summary>
    public void GoBack() {
        if (!CanGoBack()) {
            return;
        }
        RouteStack.Pop();
        Navigate(RouteStack.Pop());
    }

    /// <summary>
    /// 是否能回退
    /// </summary>
    /// <returns></returns>
    public bool CanGoBack() => RouteStack.Count > 1;
}

public class RouterService {
    private class RouterInfo {
        public RouterInfo(Type classType) {
            ClassType = classType;
        }

        public Type ClassType { get; set; }
        public object? Instance { get; set; }
    }

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
    /// <param name="view"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">View 不存在</exception>
    /// <exception cref="NullReferenceException">对象创建失败</exception>
    public object GetInstance(Type view) {
        if (!Routers.ContainsKey(view)) {
            throw new KeyNotFoundException("View 不存在");
        }
        Routers[view].Instance = (
            Routers[view].Instance ?? Activator.CreateInstance(Routers[view].ClassType)
        ) ?? throw new NullReferenceException($"对象 {view.GetType()} 创建失败");
        return CommonUtils.NullCheck(Routers[view].Instance);
    }

    public T GetInstance<T>() {
        return (T)GetInstance(typeof(T));
    }

    /// <summary>
    /// 移除页面实例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void RemovePage<T>() {
        Routers.Remove(typeof(T));
    }

    /// <summary>
    /// 移除页面实例
    /// </summary>
    /// <param name="pageType"></param>
    public void RemovePage(Type pageType) {
        Routers.Remove(pageType);
    }

    /// <summary>
    /// 获取子路由类型信息
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Type> GetRouteTypes() => Routers.Keys.ToList();

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
