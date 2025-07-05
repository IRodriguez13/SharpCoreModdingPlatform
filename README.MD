# MSharpCore ModLoader: Modding C# para Minecraft
---
## ¿Qué es?

- SharpCore es un microkernel de modding con una arquitectura pensada para escribir mods desde cualquier lenguaje de programación hacia cualquier plataforma que soporte named pipes como protocolo. 
---
## Características principales

- **Mods en C#**: Escribe mods usando C# y Visual Studio, sin depender de Java.
- **Comunicación en tiempo real**: Sincronización bidireccional entre Minecraft (Java) y mods en C# usando Named Pipes.
- **Carga dinámica de mods**: Los mods se cargan sin compilar, simplemente hay que enviar un json que mapee exactamente lo que necesita el juego.
- **Arquitectura extensible**: Pensado para desarrollar mods desde cualquier lenguaje de programación hasta cualquier plataforma que implemente la interfaz de IInstructionAdapter.
- **Assets en Unity**: Pensado para integrar herramientas visuales como Unity desde el lado c#.
---

## Flujo

```
Interface (IMsharpMod) --> BridgeCore (Named pipes, Staging System) --> Adapter Response(OK/FAIL) --> if OK Adapter Game implementation else Rollback
```

- **Java Side:** Mod Forge (`MSharpBridgeMod`) que actúa como cliente del pipe.
- **C# Side:** Launcher que carga mods, gestiona la pipe y despacha eventos.
- **Comunicación:** Named Pipes que transfieren el payload que se procesa en el núcleo y se implementa en el adaptador hacia el juego.

---


## Ejemplo de mod en C#

```csharp
public class TestCsharpMod : IMsharpMod
{
    public void OnStart() => Console.WriteLine("Test Mod iniciado.");
    public void OnEvent(string type, object? payload = {Correct Forge Values})
    {
        Console.WriteLine($"Evento recibido desde Java: {type} | Payload: {payload}");
    }
    public void OnTick() { }
}
```

## Licencia

Este proyecto se distribuye bajo la licencia LGPL v2.1 o posterior (ver archivos LICENSE).

