using CommonTools.Model;
using System.Collections.Concurrent;

namespace CommonTools.Utils;

public static class TaskUtils {
    /// <summary>
    /// 简化 try 代码块
    /// </summary>
    /// <param name="task"></param>
    [NoException]
    public static void Try(Action task) {
        try {
            task();
        } catch { }
    }

    /// <summary>
    /// 简化 try 代码块，以异步方式
    /// </summary>
    /// <param name="task"></param>
    [NoException]
    public static async void TryAsync(Func<Task> task) {
        try {
            await task();
        } catch { }
    }

    /// <summary>
    /// 简化 try 代码块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <param name="defaultValue">发生异常时的返回值</param>
    /// <returns></returns>
    [NoException]
    public static T? Try<T>(Func<T> task, T? defaultValue = default) {
        try {
            return task();
        } catch {
            return defaultValue ?? default;
        }
    }

    /// <summary>
    /// 简化 try 代码块，以异步方式
    /// </summary>
    /// <param name="task"></param>
    [NoException]
    public static async Task<T?> TryAsync<T>(Func<Task<T>> task, T? defaultValue = default) {
        try {
            return await task();
        } catch {
            return defaultValue ?? default;
        }
    }

    /// <summary>
    /// 延迟执行任务
    /// </summary>
    /// <param name="millisecond"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    public static async Task DelayTaskAsync(int millisecond, Action action) {
        await Task.Delay(millisecond);
        action();
    }

    /// <summary>
    /// 延迟执行任务
    /// </summary>
    /// <param name="millisecond"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    public static async Task<T> DelayTaskAsync<T>(int millisecond, Func<T> func) {
        await Task.Delay(millisecond);
        return func();
    }

    /// <summary>
    /// 等待 predicate 返回 true，然后执行 action
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="action"></param>
    /// <param name="interval">检查间隔时间(ms)</param>
    public static Task WaitFor(Func<bool> predicate, Action action, int interval = 50) {
        return Task.Run(() => {
            while (!predicate()) {
                Thread.Sleep(interval);
            }
            action();
        });
    }

    /// <summary>
    /// 调用过的方法 Set
    /// </summary>
    private static readonly ConcurrentBag<object> MethodCalledSet = new();

    /// <summary>
    /// 调用过的方法 Set
    /// </summary>
    private static readonly ConcurrentBag<(object, object)> MethodCalledTupleSet = new();
    /// <summary>
    /// Tuple lock
    /// </summary>
    private static readonly IDictionary<(object, object), object> MethodCalledLockDict = new ConcurrentDictionary<(object, object), object>();

    /// <summary>
    /// 确保方法只调用一次
    /// </summary>
    /// <param name="identifier">唯一标识</param>
    /// <param name="callback">回调方法</param>
    /// <returns>返回 true 则未调用过，否则返回 false</returns>
    public static bool EnsureCalledOnce(object identifier, Delegate callback) {
        return EnsureCalledOnce((identifier, identifier), callback);
    }

    /// <inheritdoc cref="EnsureCalledOnce"/>
    public static bool EnsureCalledOnce((object, object) identifier, Delegate callback) {
        ArgumentNullException.ThrowIfNull(identifier);
        if (MethodCalledSet.Contains(identifier)) {
            return false;
        }
        if (!MethodCalledLockDict.ContainsKey(identifier)) {
            MethodCalledLockDict[identifier] = new();
        }
        lock (MethodCalledLockDict[identifier]) {
            if (MethodCalledSet.Contains(identifier)) {
                return false;
            }
            callback.DynamicInvoke();
            // 调用成功后再 Add
            MethodCalledSet.Add(identifier);
            return true;
        }
    }

    /// <summary>
    /// (Identifier, 上一个任务)
    /// </summary>
    private static readonly IDictionary<object, Task> PreviousTaskDict = new ConcurrentDictionary<object, Task>();

    /// <summary>
    /// 添加到任务队列
    /// </summary>
    /// <param name="Identifier">标识</param>
    /// <param name="newTaskCallback">第一个参数为已经执行完的上一个任务，第二个参数为需要执行的任务</param>
    [ThreadSafe]
    public static void AddToTaskQueue(object Identifier, Action<Task> newTaskCallback) {
        // 初始化
        if (!PreviousTaskDict.ContainsKey(Identifier)) {
            PreviousTaskDict[Identifier] = Task.CompletedTask;
        }
        var previousTask = PreviousTaskDict[Identifier];
        // 已经完成，立即执行
        if (previousTask.IsCompleted) {
            PreviousTaskDict[Identifier] = Task.Run(() => newTaskCallback(previousTask));
        } else {
            PreviousTaskDict[Identifier] = Task.Run(async () => {
                await previousTask.ContinueWith(newTaskCallback);
            });
        }
    }
}
