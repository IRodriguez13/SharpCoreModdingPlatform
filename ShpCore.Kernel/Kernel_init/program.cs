using ShpCore.Logging;
using SharpCore.Kernel.Init;

namespace SharpCore.Kernel.Start;
public class Program
{
    public static void Main(string[] start_arguments)
    {
        KernelLog.Info("[Start] Launcher: Starting ShpCore...");

        if (start_arguments.Length < 3)
        {
            KernelLog.Panic("[Start] Dev, dont forget Usage: <payloadPath> <protocol> <adapterPath>");
            return;
        }

        var payloadPath = start_arguments[0];
        var protocol = start_arguments[1];
        var adapter = start_arguments[2];

        SharpCoreKernel.Run(payloadPath, protocol, adapter);
    }

}