namespace CommonTools.Utils;

public static class DebounceUtils {
    private static readonly IDictionary<Delegate, State> DebounceStateDict = new Dictionary<Delegate, State>();
    private static readonly IDictionary<object, State2> DebounceState2Dict = new Dictionary<object, State2>();
    /// <summary>
    /// 超过 interval * <see cref="NoResponseTimes"/> 时间没有请求，则暂停定时器
    /// </summary>
    private const int NoResponseTimes = 16;

    /// <summary>
    /// 防抖
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="callback"></param>
    /// <param name="callRegular">是否每隔 interval 时间调用一次</param>
    /// <param name="interval"></param>
    public static void Debounce(object identifier, Action callback, bool callRegular = false, int interval = 500) {
        if (DebounceState2Dict.TryGetValue(identifier, out var _state)) {
            _state.Timer.Start();
            _state.Callback = callback;
            _state.IsAccessed = true;
            _state.LastAccessTime = DateTime.Now;
            return;
        }
        // 初始化
        var state = new State2();
        state.Timer.Interval = interval;
        state.Timer.Elapsed += (o, e) => {
            // 没有访问
            if (!state.IsAccessed) {
                // 暂停计时，节流
                if ((DateTime.Now - state.LastAccessTime).TotalMilliseconds >= state.Timer.Interval * NoResponseTimes) {
                    state.Timer.Stop();
                }
                return;
            }
            if (callRegular) {
                var state = DebounceState2Dict[identifier];
                state.IsAccessed = false;
                state.Callback();
                return;
            }
            // 超过了 interval
            if (state.LastAccessTime.AddMilliseconds(interval) <= DateTime.Now) {
                var state = DebounceState2Dict[identifier];
                state.IsAccessed = false;
                state.LastAccessTime = DateTime.Now;
                state.Callback();
            }
        };
        state.Timer.Start();
        DebounceState2Dict[identifier] = state;
    }

    /// <summary>
    /// 防抖函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callback"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    public static Action<T> Debounce<T>(Action<T> callback, int interval) {
        Action<T> wrappedCallback = arg => callback(arg);
        Action<T> debounceFunc = arg => {
            // 更新状态
            if (DebounceStateDict.ContainsKey(wrappedCallback)) {
                State state = DebounceStateDict[wrappedCallback];
                state.LastInvokeTime = DateTimeUtils.CuruentMilliseconds;
                state.Arg = arg!;
                state.IsUpdated = true;
                return;
            }
            // 初始化
            System.Timers.Timer timer = new(interval);
            DebounceStateDict[wrappedCallback] = new(wrappedCallback, timer, arg!) {
                Interval = interval,
                IsUpdated = true,
            };
            timer.Elapsed += (o, e) => {
                State state = DebounceStateDict[wrappedCallback];
                // 更新了并且大于 interval
                if (state.IsUpdated && DateTimeUtils.CuruentMilliseconds - state.LastInvokeTime > state.Interval) {
                    // 重置
                    state.IsUpdated = false;
                    wrappedCallback((T)state.Arg);
                }
            };
            timer.Start();
        };
        return debounceFunc;
    }

    private record State2 {
        public System.Timers.Timer Timer { get; set; } = new();
        public DateTime LastAccessTime { get; set; } = DateTime.Now;
        public bool IsAccessed { get; set; } = false;
        public Action Callback { get; set; } = () => { };
    }

    private record State {
        public State(Delegate callback, System.Timers.Timer timer, object arg) {
            Callback = callback;
            Timer = timer;
            Arg = arg;
        }

        public int Interval { get; set; } = 500;
        public long LastInvokeTime { get; set; } = DateTimeUtils.CuruentMilliseconds;
        public bool IsUpdated { get; set; }
        public Delegate Callback { get; set; }
        public System.Timers.Timer Timer { get; set; }
        // 保存参数
        public object Arg { get; set; }
    }
}
