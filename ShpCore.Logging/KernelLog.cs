using Serilog;
using Serilog.Events;

namespace ShpCore.Logging;

public static class KernelLog
{
    private static bool _initialized = false;

    public static void Init(string logDirectory)
    {
        if (_initialized) return; // Evitás múltiples inicializaciones

        string logPath = Path.Combine(logDirectory, "kernel.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        _initialized = true;
    }

    // Fatal Errors
    public static void Panic(string msg, Exception? ex = null)
    {
        string fullMessage = $"[PANIC] {msg}";

        if (ex is null)
            Log.Fatal(fullMessage);
        else
            Log.Fatal(ex, fullMessage);

        // Además, informás por consola dónde fue logueado
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(fullMessage);
        Console.ResetColor();
    }

    // Necesary Info for events in runtime
    public static void Info(string msg)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"[INFO] {msg}");
        Console.ResetColor();
        Log.Information(msg);
    }

    // Util warns in development 
    public static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {msg}");
        Console.ResetColor();
        Log.Warning(msg);
    }

    // Runtime Data (most used)
    public static void Debug(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[DEBUG] {msg}");
        Console.ResetColor();
        Log.Debug(msg);
    }
}
