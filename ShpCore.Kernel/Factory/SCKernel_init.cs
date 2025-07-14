using ShpCore.Launcher.Core.Factory;
using ShpCore.Logging;

namespace SharpCore.Kernel.Init;

public static class SharpCoreKernel
{
    public static void Run(string payloadPath, string protocol, string adapter)
    {
        if (!File.Exists(payloadPath))
        {
            KernelLog.Panic($"[Runner] El payload no existe en la ruta: {payloadPath}");
            return;
        }

        if (!Directory.Exists(adapter))
        {
            KernelLog.Panic($"[Runner] La ruta del adaptador no existe: {adapter}");
            return;
        }

        try
        {
            var protocolInstance = ProtocolFactory.Get(protocol);
            var bridge = protocolInstance.CreateBridge(adapter);
            bridge.Start();

            var json = File.ReadAllText(payloadPath);
            bridge.Send(json);

            KernelLog.Info("[Runner] Payload enviado exitosamente.");
        }
        catch (Exception ex)
        {
            KernelLog.Panic("[Runner] Fallo al ejecutar el núcleo.", ex);
        }
        finally
        {
            KernelLog.Info("[Runner] Proceso de ejecución del núcleo finalizado.");
        }
    }
}
