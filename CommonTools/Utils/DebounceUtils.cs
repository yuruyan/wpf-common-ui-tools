using System.Runtime.CompilerServices;
using Timer = System.Timers.Timer;

namespace CommonTools.Utils;

public static class DebounceUtils {
    private static readonly IDictionary<Delegate, State> DebounceStateDict = new Dictionary<Delegate, State>();
    private static readonly IDictionary<object, State2> DebounceState2Dict = new Dictionary<object, State2>();
    private static readonly IDictionary<Timer, State2> TimerDict = new Dictionary<Timer, State2>();

    /// <summary>
    /// 超过 interval * <see cref="NoResponseTimes"/> 时间没有请求，则暂停定时器
    /// </summary>
    private const int NoResponseTimes = 8;
    private const int DefaultInterval = 500;

    /// <inheritdoc cref="Debounce(object, Delegate, bool, object?, bool, int)"/>
    public static void Debounce(object identifier, Action callback, bool callRegular = false, int interval = DefaultInterval) {
        Debounce(identifier, callback, false, null, callRegular, interval);
    }

    /// <inheritdoc cref="Debounce(object, Delegate, bool, object?, bool, int)"/>
    public static void Debounce(object identifier, Func<object?> callback, object? args, bool callRegular = false, int interval = DefaultInterval) {
        Debounce(identifier, callback, true, args, callRegular, interval);
    }

    /// <summary>
    /// 防抖
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="callback"></param>
    /// <param name="hasArguments">是否有参数</param>
    /// <param name="args">参数</param>
    /// <param name="callRegular">是否每隔 interval 时间调用一次</param>
    /// <param name="interval"></param>
    private static void Debounce(
        object identifier,
        Delegate callback,
        bool hasArguments,
        object? args = null,
        bool callRegular = false,
        int interval = DefaultInterval
    ) {
        if (DebounceState2Dict.TryGetValue(identifier, out var _state)) {
            _state.IsAccessed = true;
            _state.Callback = callback;
            _state.Argument = args;
            // 超过时间则立即调用
            if (CanInvoke(_state)) {
                _state.IsAccessed = false;
                Call(_state);
            }
            _state.Timer.Start();
            _state.LastAccessTime = DateTime.Now;
            return;
        }
        // 初始化
        var state = new State2(callback) {
            CallRegular = callRegular,
            HasArgument = hasArguments,
            Argument = args,
        };
        DebounceState2Dict[identifier] = TimerDict[state.Timer] = state;
        state.Timer.Interval = interval;
        state.Timer.Elapsed += TimerElapsedHandler;
        state.Timer.Start();
        // 首次立即执行
        Call(state);
    }

    /// <summary>
    /// 调用函数
    /// </summary>
    /// <param name="state"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Call(State2 state) {
        if (state.HasArgument) {
            state.Callback.DynamicInvoke(new object?[] { state.Argument });
        } else {
            state.Callback.DynamicInvoke();
        }
    }

    private static void TimerElapsedHandler(object? sender, System.Timers.ElapsedEventArgs e) {
        if (sender is not Timer timer) {
            return;
        }
        var state = TimerDict[timer];
        // 没有访问
        if (!state.IsAccessed) {
            // 暂停计时，节流
            if (CanStop(state)) {
                state.Timer.Stop();
            }
            return;
        }
        // 定期执行
        if (state.CallRegular) {
            state.IsAccessed = false;
            Call(state);
            return;
        }
        // 是否可以调用
        if (CanInvoke(state)) {
            state.IsAccessed = false;
            state.LastAccessTime = DateTime.Now;
            Call(state);
        }
    }

    /// <summary>
    /// 是否可以停在计时以节省资源
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanStop(State2 state) {
        return (DateTime.Now - state.LastAccessTime).TotalMilliseconds >= state.Timer.Interval * NoResponseTimes;
    }

    /// <summary>
    /// 是否可以调用
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool CanInvoke(State2 state) {
        return state.LastAccessTime.AddMilliseconds(state.Timer.Interval) <= DateTime.Now;
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

    private class State2 {
        public Timer Timer { get; set; } = new();
        public DateTime LastAccessTime { get; set; } = DateTime.Now;
        public bool IsAccessed { get; set; }
        public bool CallRegular { get; set; }
        public bool HasArgument { get; set; }
        public object? Argument { get; set; }
        public Delegate Callback { get; set; }

        public State2(Delegate callback) {
            Callback = callback;
        }
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
