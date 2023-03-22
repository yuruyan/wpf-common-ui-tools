using Timer = System.Timers.Timer;

namespace CommonTools.Utils;

public class Debounce : IDisposable {
    /// <summary>
    /// 超过 interval * <see cref="NoResponseTimes"/> 时间没有请求，则暂停定时器
    /// </summary>
    private const int NoResponseTimes = 8;
    private const int DefaultInterval = 500;

    private readonly Timer Timer;
    private readonly int Interval;
    private readonly bool CallRegular;
    private DateTime LastAccessTime = DateTime.MinValue;
    private bool IsAccessed;
    private bool IsDisposed;
    private Action Callback = default!;

    public Debounce(int interval = DefaultInterval, bool callRegular = false) {
        Interval = interval;
        CallRegular = callRegular;
        Timer = new(interval);
        Timer.Elapsed += TimerElapsedHandler;
    }

    public void Run(Action callback) {
        if (IsDisposed) {
            throw new ObjectDisposedException(GetType().FullName);
        }
        Callback = callback;
        IsAccessed = true;
        if (CanInvoke) {
            IsAccessed = false;
            Callback();
        }
        Timer.Start();
        LastAccessTime = DateTime.Now;
    }

    private void TimerElapsedHandler(object? sender, System.Timers.ElapsedEventArgs e) {
        // 没有访问
        if (!IsAccessed) {
            // 暂停计时，节流
            if (CanStop) {
                Timer.Stop();
            }
            return;
        }
        // 定期执行 or 可以调用
        if (CallRegular || CanInvoke) {
            IsAccessed = false;
            LastAccessTime = DateTime.Now;
            Callback();
        }
    }

    /// <summary>
    /// 是否可以调用
    /// </summary>
    /// <returns></returns>
    private bool CanInvoke => LastAccessTime.AddMilliseconds(Interval) <= DateTime.Now;

    /// <summary>
    /// 是否可以停止计时以节省资源
    /// </summary>
    /// <returns></returns>
    private bool CanStop => (DateTime.Now - LastAccessTime).TotalMilliseconds >= Interval * NoResponseTimes;

    public virtual void Dispose() {
        IsDisposed = true;
        Timer.Close();
        Timer.Dispose();
        Callback = null!;
        GC.SuppressFinalize(this);
    }
}