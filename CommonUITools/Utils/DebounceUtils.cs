namespace CommonUITools.Utils;

public class DebounceUtils {
    private static readonly IDictionary<Delegate, State> DebounceStateDict = new Dictionary<Delegate, State>();

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
                state.LastInvokeTime = CommonUtils.CuruentMilliseconds;
                state.Arg = arg;
                state.IsUpdated = true;
                return;
            }
            // 初始化
            System.Timers.Timer timer = new(interval);
            DebounceStateDict[wrappedCallback] = new(wrappedCallback, timer, arg) {
                Interval = interval,
                IsUpdated = true,
            };
            timer.Elapsed += (o, e) => {
                State state = DebounceStateDict[wrappedCallback];
                // 更新了并且大于 interval
                if (state.IsUpdated && CommonUtils.CuruentMilliseconds - state.LastInvokeTime > state.Interval) {
                    // 重置
                    state.IsUpdated = false;
                    wrappedCallback((T)state.Arg);
                }
            };
            timer.Start();
        };
        return debounceFunc;
    }

    private record State {
        public State(Delegate callback, System.Timers.Timer timer, object arg) {
            Callback = callback;
            Timer = timer;
            Arg = arg;
        }

        public int Interval { get; set; } = 500;
        public long LastInvokeTime { get; set; } = CommonUtils.CuruentMilliseconds;
        public bool IsUpdated { get; set; }
        public Delegate Callback { get; set; }
        public System.Timers.Timer Timer { get; set; }
        // 保存参数
        public object Arg { get; set; }
    }
}
