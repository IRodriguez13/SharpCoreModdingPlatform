# SharpCore ModLoader: Modding C# for Minecraft Forge
---
## What is SharpCore?

- SharpCore is a lightweight modding microkernel with an architecture designed to allow mod development from any programming language targeting any platform that supports named pipes as a protocol. 
---
## Key Features

- **Mods in C#**: Write mods using C# and Visual Studio, with no dependency on Java.

- **Real-time communication**: Bidirectional synchronization between Minecraft (Java) and C# mods via Named Pipes.

- **Dynamic mod loading**: Mods are loaded without compilation; you just need to send a JSON mapping exactly what the game requires.

- **Extensible architecture**: Designed to support modding from any language to any platform implementing the IInstructionAdapter interface.

- **Unity Assets support**: Built with visual tools like Unity in mind, integrated from the C# side.
---

## Flow

```
Interface (IMsharpMod) --> BridgeCore (Named pipes, Staging System) --> Adapter Response(OK/FAIL) --> if OK Adapter Game implementation else Rollback
```

- **Java Side**: A Forge mod (MSharpBridgeMod) acting as the named pipe client.

- **C# Side**: A launcher that loads mods, manages the pipe, and dispatches events.

- **Communication**: Named Pipes transfer the payload to the core, which is then handled by the adapter implementation for the game.

---


## Ejemplo de mod en C#

```csharp
public class TestCsharpMod : IMsharpMod
{
    public void OnStart() => Console.WriteLine("Test Mod running.");
    public void OnEvent(string type, object? payload = {Correct Forge Values})
    {
        Console.WriteLine($"Event cached from Forge side: {type} | Payload: {payload}");
    }
    public void OnTick() { }
}
```

## Licencia

*This project is licensed under the LGPL v2.1 or later (see LICENSE files).*

### Contact

- <ivanrwcm25@gmail.com>

