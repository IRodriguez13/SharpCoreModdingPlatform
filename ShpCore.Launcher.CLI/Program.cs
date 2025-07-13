using ShpCore.Logging;
using MSharp.Launcher.Core;
using SharpCore.CLI.Env;
using System.Threading.Tasks;

namespace ShpCore.Launcher.CLI;

public class Program
{
    public static void Main(string[] args)
    {
        KernelLog.Info("[Launcher] Starting ShpCore Launcher CLI...");
        LauncherRunner.EjecutarMinecraft();
        
    }
    public static async Task MainCLI(string[] args)
    {
        await SharpCoreCLI.Run(args);
        KernelLog.Info("[Launcher] ShpCore Launcher CLI finished.");
    }
}
