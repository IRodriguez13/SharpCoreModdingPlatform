using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using MSharp.Staging.Instruction_adapters;
using MSharp.Launcher.Core.Models;
using MSharp.Validation.Models;
using ShpCore.Logging;

public class FinishAdapterLayer : IInstructionAdapter
{
    private readonly string _pipeName;

    public FinishAdapterLayer(string pipeName = "msharp_bridge") =>  _pipeName = pipeName;
    

    // Este adaptador se encarga de enviar instrucciones a través de un Named Pipe
    public InstructionValidationResult Validate(MSharpInstruction instruction)
    {
        var response = SendPayloadOverPipe(instruction, isValidation: true);

        return response?.ToLowerInvariant() switch
        {
            "ok" => InstructionValidationResult.Success(),
            var msg => InstructionValidationResult.Failure(msg ?? "[bridge] Respuesta nula del adaptador.")
        };
    }

    // para aplicar los cambios en caso de que recibamos OK en el adapter
    public bool Apply(MSharpInstruction instruction)
    {
        string response = SendPayloadOverPipe(instruction, isValidation: false) ?? throw new InvalidOperationException("No se pudo enviar el payload al adaptador.");
        return response?.ToLowerInvariant() == "ok";
    }

    // Si el adapter me lo permite (si me devolvió OK), abro pipes y envío el payload final que se debería implementar en el juego
    // Si no, devuelvo un mensaje de error
    // El payload es un JSON con el tipo de instrucción y los datos necesarios para aplicar
    private string? SendPayloadOverPipe(MSharpInstruction instruction, bool isValidation)
    {
        try
        {
            KernelLog.Debug($"[FinishAdapterLayer] Enviando payload al adaptador: {instruction}");

            using NamedPipeClientStream client = new(".", _pipeName, PipeDirection.InOut, PipeOptions.None);
            client.Connect(2000); // Timeout de 2s para esperar la conexión antes del serialize del payl

            var payl = JsonSerializer.Serialize(
                new
                {
                    type = isValidation ? "validate" : "apply",
                    data = instruction
                });

            byte[] buffer = Encoding.UTF8.GetBytes(payl);
            client.Write(buffer, 0, buffer.Length);
            client.Flush();

            // Leer respuesta
            byte[] responseBuffer = new byte[256];
            int bytesRead = client.Read(responseBuffer, 0, responseBuffer.Length);

            KernelLog.Debug($"[FinishAdapterLayer] Respuesta del adaptador: {Encoding.UTF8.GetString(responseBuffer, 0, bytesRead)}");

            return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
        }
        catch (Exception ex)
        {
            KernelLog.Panic($"[FinishAdapterLayer] Error al enviar el payload al adaptador: {ex.Message}");
            return null;
        }
    }
}
