using System.Diagnostics;

namespace MinecraftLauncher;

public class MinecraftLauncher
{
    
    // -- Acá cargo y lanzo el mineacraft con los argumentos necesarios --

    public void LaunchVanilla(string javaPath, string gameJar, string mainClass, string args, string workingDirectory)
    {
        ProcessStartInfo psi = new()
        {
            FileName = javaPath, // Ruta a java.exe
            Arguments = $"-cp \"{gameJar}\" {mainClass} {args}",
            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        Process prcss = new()
        {
            StartInfo = psi
        };

        prcss.OutputDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[MINECRAFT STDOUT] {e.Data}");
        };

        prcss.ErrorDataReceived += (s, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data)) Console.WriteLine($"[MINECRAFT STDERR] {e.Data}");
        };

        prcss.Start();
        prcss.BeginOutputReadLine();
        prcss.BeginErrorReadLine();
    }
}
