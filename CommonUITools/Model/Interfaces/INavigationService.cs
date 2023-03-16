using CommonUITools.Route;

namespace CommonUITools.Model;

/// <summary>
/// 该接口方法由 <see cref="RouterService"/> 调用
/// </summary>
public interface INavigationService {
    public void Navigated(object? data);
}
