using System.Diagnostics;
using System;
using System.IO;
using MSharp.Launcher.Core.Bridge;
using MSharp.Launcher.Core.ModRunner;
using ShpCore.Logging;

namespace MSharp.Launcher.Core;

public class LauncherRunner
{
    public static void EjecutarMinecraft()
    {
        // Configuración de ejecución de Minecraft -- Boot minecraft config 
        string javaPath = @"D:\Users\pc\Desktop\Devtools\jdk8u442-b06\bin\java.exe";
        string mainClass = "net.minecraft.launchwrapper.Launch";

        // JVM Args to increse performance and memory usage
        string vmArgs = "-Xmx2G -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC " +
                     "-XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 " +
                     "-XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M";

        // Directorio de natives de Minecraft
        string nativesDir = @"C:\Users\pc\AppData\Roaming\.minecraft\natives";
        string javaArgs = $"-Djava.library.path=\"{nativesDir}\"";

        // Classpath to load Minecraft and Forge libraries 
        string classpath = File.ReadAllText(@"D:\Users\pc\Desktop\RoadToM#\MSharp.Launcher.CLI\bin\Debug\net9.0\classpath.txt");

        // Forge Args to run Minecraft with Forge 1.8.9

        string forgeArgs = "--username DevTest --version 1.8.9-forge " +
                        "--gameDir \"C:\\Users\\pc\\AppData\\Roaming\\.minecraft\" " +
                        "--assetsDir \"C:\\Users\\pc\\AppData\\Roaming\\.minecraft\\assets\" " +
                        "--assetIndex 1.8 --uuid 1234 --accessToken 1234 " +
                        "--userProperties {} --userType mojang " +
                        "--tweakClass net.minecraftforge.fml.common.launcher.FMLTweaker";

        KernelLog.Debug("[Launcher] Iniciando Minecraft con los siguientes parámetros:");

        // Here we build the full command line to launch Minecraft with the specified settings 
        string launchArgs = $"{vmArgs} {javaArgs} -cp \"{classpath}\" {mainClass} {forgeArgs}";

        ProcessStartInfo psi = new()
        {
            FileName = javaPath,
            Arguments = launchArgs,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false
        };

        Process proceso = new() { StartInfo = psi };

        proceso.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[OUT] {e.Data}");
        };

        proceso.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[ERR] {e.Data}");
        };

        // mod loader
        string modPath = @"D:\Users\pc\Desktop\RoadToM#\Mods#\Mods";
        KernelLog.Debug("[ModLoader] Cargando mods C# desde carpeta:");
        KernelLog.Debug(modPath);

        var mods = ModLoader.CargarMods(modPath);

        foreach (var mod in mods)
        {
            Console.WriteLine("[ModLoader] Ejecutando OnStart() de mod...");
            mod.OnStart();
        }

        // Conexión al mod de Java vía Named Pipe
        NamedPipeBridgeConnection bridge = new();
        bridge.OnMessage += msg =>
        {
            KernelLog.Debug($"[ModLoader] Mensaje del mod puente: {msg}");

            foreach (var mod in mods)

                mod.OnEvent("BRIDGE_MSG", msg);

        };

        KernelLog.Debug("[Bridge] Iniciando servidor de Named Pipe...");
        bridge.Start();

        // Run Minecraft with the configured settings

        KernelLog.Debug("[Bridge] Iniciando Minecraft desde C#...");
        proceso.Start();
        proceso.BeginOutputReadLine();
        proceso.BeginErrorReadLine();

        proceso.WaitForExit();
    }

}
