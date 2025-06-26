using ShpCore.Logging;
using MSharp.Launcher.Core;

namespace ShpCore.Launcher.CLI;

class Program
{
    static void Main()
    {
        KernelLog.Info("[Launcher] Starting ShpCore Launcher CLI...");
        LauncherRunner.EjecutarMinecraft();
    }
}
