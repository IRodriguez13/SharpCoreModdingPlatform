using ShpCore.Logging;
using SharpCore.Kernel.Init;
public class Program
{
    public static void Main(string[] args)
    {
        KernelLog.Info("[Core] Launcher: Starting ShpCore...");

        if (args.Length < 3)
        {
            KernelLog.Panic("Usage: <payloadPath> <protocol> <adapterPath>");
            return;
        }

        var payloadPath = args[0];
        var protocol = args[1];
        var adapter = args[2];

        SharpCoreKernel.Run(payloadPath, protocol, adapter);
    }

}