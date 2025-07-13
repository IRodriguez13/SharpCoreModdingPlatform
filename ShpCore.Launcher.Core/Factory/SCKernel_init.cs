
namespace SharpCore.Kernel.Init;

public static class SharpCoreKernel
{
    public static void Run(string payloadPath, string protocol = "namedpipe", string adapter = "forge")
    {
        var protocolInstance = ProtocolFactory.Get(protocol); // Usa el string para elegir el protocolo
        var bridge = protocolInstance.CreateBridge(adapter);  // Si tu factory lo soporta
        bridge.Start();

        var json = File.ReadAllText(payloadPath);
        bridge.Send(json);
    }
}
