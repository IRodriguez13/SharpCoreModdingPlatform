using MSharp.Launcher.Core.Bridge;

namespace ShpCore.Launcher.Core.Factory;

public static class ProtocolFactory
{
    public static IProtocol Get(string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "namedpipe" => new NamedPipeBridgeConnection(),
            "grpc" => new GrpcProtocol(),
            "unix" => new FileProtocol(),
            _ => throw new ArgumentException($"Unknown protocol: {protocol}")

        };
    }
}