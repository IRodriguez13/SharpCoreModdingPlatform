using System.IO.Pipes;
using MSharp.ModLoader.StagingSystem;
using MSharp.Launcher.Core.Models;
using MSharp.Staging.Instruction_adapters;
using System.Text;
using System.Text.Json;

namespace MSharp.Launcher.Core.Bridge
{
    public class NamedPipeBridgeConnection : IBridgeConnection
    {
        private readonly string pipeName;
        private NamedPipeServerStream? server;
        private Thread? listenThread;

        private readonly FinishAdapterLayer _adapter; // Adapter para procesar instrucciones
        private readonly StagingManager<MSharpInstruction> _stageManager; // Manejador de staging para aplicar y revertir instrucciones

        public event Action<string>? OnMessage; // Esto queda por compatibilidad, pero  no es el punto de entrada principal

        public NamedPipeBridgeConnection(string pipeName = "msharp_bridge", FinishAdapterLayer _adapter = null!)
        {
            this.pipeName = pipeName;
            this._adapter = new FinishAdapterLayer("msharp_bridge") ?? throw new ArgumentNullException(nameof(_adapter), "El adapter no puede ser nulo.");

            // Inicializamos el StagingManager con callbacks para aplicar y revertir instrucciones.
            // Estos callbacks pueden ser adaptados para interactuar con tu lógica de modding.
            _stageManager = new StagingManager<MSharpInstruction>(
                applyCallback: instruction => Console.WriteLine($"[Staging] Aplicando instrucción: {instruction.Tipo}"),
                rollbackCallback: instruction => Console.WriteLine($"[Staging] Revirtiendo instrucción: {instruction.Tipo}")
            );
        }

        public void Start()
        {
            listenThread = new Thread(() =>
            {
                for (;;)
                {
                    try
                    {
                        server = new NamedPipeServerStream(
                            pipeName,
                            PipeDirection.InOut,
                            1,
                        #if WINDOWS
                            PipeTransmissionMode.Message,
                        #else
                            PipeTransmissionMode.Byte,
                        #endif
                            PipeOptions.Asynchronous
                        );

                        Console.WriteLine("[Bridge] Pipe levantado. Esperando conexión Java...");
                        server.WaitForConnection();
                        Console.WriteLine("[Bridge] ¡Conexión Java ↔ C# establecida!");

                        byte[] buffer = new byte[2048];
                        while (server.IsConnected)
                        {
                            int bytesRead = server.Read(buffer, 0, buffer.Length);

                            if (bytesRead == 0) break;

                            string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            HandleIncomingInstruction(json);
                        }
                    }
                    catch (IOException ioEx)
                    {
                        Console.WriteLine($"[Bridge] I/O Pipe error: {ioEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Bridge] Error general: {ex.Message}");
                    }
                    finally
                    {
                        server?.Dispose();
                        server = null;
                        Console.WriteLine("[Bridge] Pipe cerrado. Esperando nueva conexión...");
                        Thread.Sleep(1000);
                    }
                }
            })
            {
                IsBackground = true,
                Name = "MSharpNamedPipeListener"
            };

            listenThread.Start();
        }

        public void Send(string message)
        {
            try
            {
                if (server is { IsConnected: false })
                {
                    Console.WriteLine("[Bridge] No se puede enviar mensaje. No hay conexión.");
                    return;
                }

                byte[] buffer = Encoding.UTF8.GetBytes(message);

                if (server is null) throw new InvalidOperationException("[Bridge] El servidor de pipe no está inicializado.");

                server.Write(buffer, 0, buffer.Length);
                server.Flush();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Bridge] Error al enviar por pipe: {ex.Message}");
            }
        }

        private void HandleIncomingInstruction(string json)
        {
            try
            {
                var payload = JsonSerializer.Deserialize<MSharpInstruction>(json);
                if (payload == null)
                {
                    Console.WriteLine("[Bridge] Dev, Instrucción vacía o inválida.");
                    return;
                }

                _stageManager.MSadd(payload); // lo añado al stage si es distinto de null

                // Ejecutamos el adapter Java o cualquier otro
                bool success = ExecuteInstruction(payload);

                if (!success)
                {
                    _stageManager.MSrevert();
                    Console.WriteLine("[staging] Instrucción fallida. Se hizo rollback.");
                    return;
                }

                _stageManager.MScommit();
                Console.WriteLine("[staging] Instrucción aplicada y comiteada.");
                Console.WriteLine($"[staging] payload Comitteado: {payload?.Tipo} - {payload?.Entidad}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"[staging] Error procesando instrucción: {ex.Message}");
            }
        }

        // first point of contact for the adapter
        private bool ExecuteInstruction(MSharpInstruction payload)
        {
            if (_adapter == null)
            {
                Console.WriteLine("[adapter] No hay adapter configurado.");
                return false;
            }

            try
            {
                var validationResult = _adapter.Validate(payload) ?? throw new InvalidOperationException("[adapter] El adapter devolvió un resultado de validación nulo.");


                if (!validationResult.IsValid)
                {
                    Console.WriteLine($"[validator] Instrucción inválida: {validationResult.ErrorMessage}");
                    return false;
                }

                var result = _adapter.Apply(payload); // Acá se habla con Java mediante la implementacion de la interfaz IInstructionAdapter

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[staging] Error en ExecuteInstruction: {ex.Message}");
                return false;
            }
        }

    }
}
