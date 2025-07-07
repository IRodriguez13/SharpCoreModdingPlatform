using Serilog;

namespace ShpCore.Logging;


public static class KernelLog
{
    static KernelLog()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
             // .WriteTo.File("kernel.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

		// Necesary Info for events in runtime
    public static void Info(string msg) => Log.Information(msg);
   
   	// Util warns in development 
    public static void Warn(string msg) => Log.Warning(msg);
   
    // Fatal Errors
    public static void Panic(string msg, Exception? ex = null) => Log.Error(ex, msg);

    // Runtime Data (most used)
    public static void Debug(string msg) => Log.Debug(msg);
}
