namespace CommonTools.Utils;

/// <summary>
/// This class is used to print messages to the console.
/// </summary>
public static class ConsoleUtils {
    /// <summary>
    /// 打印错误信息
    /// </summary>
    /// <param name="error"></param>
    public static void WriteError(string error) {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(error);
        Console.ForegroundColor = oldColor;
    }

    /// <summary>
    /// 打印错误信息
    /// </summary>
    /// <param name="error"></param>
    public static void WriteError(Exception error) {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(error);
        Console.ForegroundColor = oldColor;
    }

    /// <summary>
    /// 打印成功信息
    /// </summary>
    /// <param name="message"></param>
    public static void WriteSuccess(string message) {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = oldColor;
    }

    /// <summary>
    /// 打印警告信息
    /// </summary>
    /// <param name="message"></param>
    public static void WriteWarning(string message) {
        var oldColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = oldColor;
    }
}
