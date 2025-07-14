using MSharp.Launcher.Core.Bridge;

namespace ShpCore.Launcher.Core.Factory;

public interface IProtocol
{
    IBridgeConnection CreateBridge(string adapter);
}
