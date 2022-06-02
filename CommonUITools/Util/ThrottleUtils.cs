namespace CommonUITools.Utils;

public class ThrottleUtils {
    private record State {
        public long LastInvokeTime { get; set; } = CommonUtils.CuruentMilliseconds;
        public bool IsFinished { get; set; }
    }

    private static readonly IDictionary<Delegate, State> ThrottleDict = new Dictionary<Delegate, State>();
    /// <summary>
    /// 默认调用间隔时间
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
    /// <param name="method"></param>
    /// <param name="interval">调用时间间隔</param>
    /// <returns>通过则返回 true，否则 false</returns>
    public static bool CheckStateAndSet(Delegate method, int interval) {
        // 首次调用
        if (!ThrottleDict.ContainsKey(method)) {
            ThrottleDict[method] = new();
            return true;
        }
        State state = ThrottleDict[method];
        // 未完成
        if (!state.IsFinished) {
            return false;
        }
        long invokeTime = CommonUtils.CuruentMilliseconds;
        // 验证通过
        if (invokeTime - validateInterval(interval) >= state.LastInvokeTime) {
            state.IsFinished = false;
            state.LastInvokeTime = invokeTime;
            return true;
        }
        return false;
    }

    /// <summary>
    /// 检查状态并设置
    /// </summary>
    /// <param name="method"></param>
    /// <returns>通过则返回 true，否则 false</returns>
    public static bool CheckStateAndSet(Delegate method) {
        return CheckStateAndSet(method, Interval);
    }

    /// <summary>
    /// 设置为已完成
    /// </summary>
    /// <param name="method"></param>
    public static void SetFinished(Delegate method) {
        if (ThrottleDict.ContainsKey(method)) {
            ThrottleDict[method].IsFinished = true;
        }
    }

    /// <summary>
    /// 检查是否完成，不存在则返回 true
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static bool IsFinished(Delegate method) {
        if (!ThrottleDict.ContainsKey(method)) {
            return true;
        }
        return ThrottleDict[method].IsFinished;
    }

    /// <summary>
    /// 同步节流
    /// </summary>
    /// <param name="method"></param>
    /// <param name="callback"></param>
    public static void Throttle(Delegate method, Action callback) {
        if (!CheckStateAndSet(method)) {
            return;
        }
        callback();
        SetFinished(method);
    }

    /// <summary>
    /// 异步节流
    /// </summary>
    /// <param name="method"></param>
    /// <param name="callback"></param>
    public static async void ThrottleAsync(Delegate method, Func<Task> callback) {
        if (!CheckStateAndSet(method)) {
            return;
        }
        await callback();
        SetFinished(method);
    }
}
