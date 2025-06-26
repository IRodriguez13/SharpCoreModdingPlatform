using Serilog;

namespace ShpCore.Logging;


public static class KernelLog
{
    static KernelLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("kernel.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public static void Info(string msg) => Log.Information(msg);
    public static void Warn(string msg) => Log.Warning(msg);
    public static void Error(string msg, Exception? ex = null) => Log.Error(ex, msg);
    public static void Debug(string msg) => Log.Debug(msg);
}