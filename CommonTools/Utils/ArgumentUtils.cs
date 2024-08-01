using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CommonTools.Utils;

public static class ArgumentUtils {
    /// <summary>
    /// 检查参数是否有参数，没有则输出帮助信息并返回 false
    /// </summary>
    /// <param name="args"></param>
    /// <param name="helpMessage">参数为 0 时的提示消息</param>
    /// <returns></returns>
    public static bool CheckArgs(string[] args, string helpMessage) {
        if (args.Length == 0) {
            Console.WriteLine(helpMessage);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取 Configuration
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IConfiguration GetConfiguration(this string[] args) => new ConfigurationBuilder().AddCommandLine(args).Build();

    /// <summary>
    /// 是否包含 wait 参数
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static bool ContainsWaitArgument(this string[] args) => ContainsArgument(args, "wait");

    /// <summary>
    /// 是否包含 help 参数
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static bool ContainsHelpArgument(this string[] args) => ContainsArgument(args, "help");

    /// <summary>
    /// 是否包含指定参数
    /// </summary>
    /// <param name="args"></param>
    /// <param name="argument"></param>
    /// <returns></returns>
    public static bool ContainsArgument(this string[] args, string argument) {
        return args.Contains($"--{argument}", StringComparer.InvariantCultureIgnoreCase)
            || args.Contains($"/{argument}", StringComparer.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// 检查文件是否存在
    /// </summary>
    /// <param name="filePath">文件路径</param>
    /// <param name="argumentName">命令行参数名称</param>
    /// <returns>存在返回 true</returns>
    public static bool ValidateFileArgument(string? filePath, string argumentName) {
        if (string.IsNullOrEmpty(filePath)) {
            SharedLogging.Logger.LogError("Argument '{argumentName}' cannot be empty", argumentName);
            return false;
        }
        if (!File.Exists(filePath)) {
            SharedLogging.Logger.LogError("File \"{path}\" doesn't exist", filePath);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 检查文件夹是否存在
    /// </summary>
    /// <param name="directory">文件夹路径</param>
    /// <param name="argumentName">命令行参数名称</param>
    /// <returns></returns>
    public static bool ValidateDirectoryArgument(string? directory, string argumentName) {
        if (string.IsNullOrEmpty(directory)) {
            SharedLogging.Logger.LogError("Argument '{argumentName}' cannot be empty", argumentName);
            return false;
        }
        if (!Directory.Exists(directory)) {
            SharedLogging.Logger.LogError("Directory \"{path}\" doesn't exist", directory);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 等待用户按键退出
    /// </summary>
    public static void WaitUserInputToExit() {
        Process.Start(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe"),
            "/c pause"
        ).WaitForExit();
    }
}