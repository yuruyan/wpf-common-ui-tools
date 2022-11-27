using CommonUITools.Model;
using System.Collections.Concurrent;

namespace CommonUITools.Utils;

public static class TaskUtils {
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
