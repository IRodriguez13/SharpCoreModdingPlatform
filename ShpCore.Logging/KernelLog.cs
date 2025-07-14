using Serilog;
using Serilog.Events;

namespace ShpCore.Logging;

public static class KernelLog
{
  static KernelLog()
  {
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File("kernel.log", rollingInterval: RollingInterval.Day)
        .CreateLogger();
  }

  // Fatal Errors
  public static void Panic(string msg, Exception? ex = null)
  {
    var fullMessage = $"[PANIC] {msg}";

    if (ex is null)
    {
      Log.Fatal(fullMessage);
      return;
    }
    
    Log.Fatal(ex, fullMessage);
    
  }

  // Necesary Info for events in runtime
  public static void Info(string msg) => Log.Information($"[INFO] {msg}");


  // Util warns in development 
  public static void Warn(string msg) => Log.Warning($"[WARN] {msg}");


  // Runtime Data (most used)
  public static void Debug(string msg) => Log.Debug($"[DEBUG] {msg}");

}
