namespace CommonTools.Utils;

public static class FileLockUtils {
    /// <summary>
    /// 尝试锁定文件
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static FileStream? TryLockFile(string filepath) {
        return TaskUtils.Try(() => File.Open(
            filepath,
            FileMode.OpenOrCreate,
            FileAccess.Read,
            FileShare.None
        ), null);
    }

    /// <summary>
    /// 文件是否锁定
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static bool IsFileLocked(string filepath) {
        return TaskUtils.Try<bool?>(() => {
            using var file = File.Open(
                filepath,
                FileMode.OpenOrCreate,
                FileAccess.Read,
                FileShare.None
            );
            return false;
        }, null) ?? true;
    }
}
