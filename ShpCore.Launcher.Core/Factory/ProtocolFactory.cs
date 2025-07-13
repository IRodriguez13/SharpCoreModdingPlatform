using 

namespace ShpCore.Launcher.Core.Factory;

public static class ProtocolFactory
{
    public static IProtocol Get(string protocol)
    {
        return protocol.ToLowerInvariant() switch
        {
            "namedpipe" => new NamedPipeProtocol(),
            "grpc" => new GrpcProtocol(),
            "file" => new FileProtocol(),
            _ => throw new ArgumentException($"Unknown protocol: {protocol}")
        };
    }
}