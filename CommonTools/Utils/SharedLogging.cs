using Karambolo.Extensions.Logging.File;
using Microsoft.Extensions.Logging;

namespace CommonTools;

public static class SharedLogging {
    public static readonly ILogger FileLogger;
    public static readonly ILogger Logger;
    public static readonly ILoggerFactory FileLoggerFactory;
    public static readonly ILoggerFactory LoggerFactory;

    static SharedLogging() {
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());
        FileLoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
            builder => builder.AddConsole().AddFile(context => {
                context.RootPath = Path.GetDirectoryName(Environment.ProcessPath);
                context.Files = [new LogFileOptions() {
                    Path = "app.log",
                    MinLevel = new Dictionary<string, LogLevel> {
                        {"Default", LogLevel.Warning}
                    }
                }];
            })
        );
        Logger = LoggerFactory.CreateLogger("Main");
        FileLogger = FileLoggerFactory.CreateLogger("Main");
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public static void Dispose() {
        LoggerFactory.Dispose();
        FileLoggerFactory.Dispose();
    }
}
