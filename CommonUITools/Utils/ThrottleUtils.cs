namespace CommonUITools.Utils;

public class ThrottleUtils {
    private record State {
        public DateTime LastInvokeTime { get; set; } = DateTime.Now;
        public bool IsFinished { get; set; }
    }

    private static readonly IDictionary<object, State> ThrottleDict = new Dictionary<object, State>();
    /// <summary>
    /// 默认调用间隔时间 ms
    /// </summary>
    private static readonly int Interval = 300;

    /// <summary>
    /// 合法化 interval
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    private static int validateInterval(int interval) {
        return Math.Abs(interval);
    }

    /// <summary>
    /// 检查状态并设置，完成了并且调用时间间隔大于 interval 才返回 true
    /// </summary>
    /// <param name="identifier">标识</param>
    /// <param name="interval">调用时间间隔</param>
    /// <returns>通过则返回 true，否则 false</returns>
    public static bool CheckStateAndSet(object identifier, int interval) {
        // 首次调用
        if (!ThrottleDict.ContainsKey(identifier)) {
            ThrottleDict[identifier] = new();
            return true;
        }
        State state = ThrottleDict[identifier];
        // 未完成
        if (!state.IsFinished) {
            return false;
        }
        var invokeTime = DateTime.Now;
        // 验证通过
        //if (invokeTime - validateInterval(interval) >= state.LastInvokeTime) {
        if (invokeTime.AddMilliseconds(validateInterval(interval)) >= state.LastInvokeTime) {
            state.IsFinished = false;
            state.LastInvokeTime = invokeTime;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检查状态并设置
    /// </summary>
    /// <param name="identifier">标识</param>
    /// <returns>通过则返回 true，否则 false</returns>
    public static bool CheckStateAndSet(object identifier) {
        return CheckStateAndSet(identifier, Interval);
    }

    /// <summary>
    /// 设置为已完成
    /// </summary>
    /// <param name="identifier">标识</param>
    public static void SetFinished(object identifier) {
        if (ThrottleDict.ContainsKey(identifier)) {
            ThrottleDict[identifier].IsFinished = true;
        }
    }

    /// <summary>
    /// 检查是否完成，不存在则返回 true
    /// </summary>
    /// <param name="identifier">标识</param>
    /// <returns></returns>
    public static bool IsFinished(object identifier) {
        if (!ThrottleDict.ContainsKey(identifier)) {
            return true;
        }
        return ThrottleDict[identifier].IsFinished;
    }

    /// <summary>
    /// 同步节流
    /// </summary>
    /// <param name="identifier">标识</param>
    /// <param name="callback"></param>
    public static void Throttle(object identifier, Action callback) {
        if (!CheckStateAndSet(identifier)) {
            return;
        }
        callback();
        SetFinished(identifier);
    }

    /// <summary>
    /// 异步节流
    /// </summary>
    /// <param name="identifier">标识</param>
    /// <param name="callback"></param>
    public static async void ThrottleAsync(object identifier, Func<Task> callback) {
        if (!CheckStateAndSet(identifier)) {
            return;
        }
        await callback();
        SetFinished(identifier);
    }
}
