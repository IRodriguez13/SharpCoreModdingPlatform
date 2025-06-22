using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using MSharp.Staging.Instruction_adapters;
using MSharp.Launcher.Core.Models;
using MSharp.Validation.Models;

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
            var msg => InstructionValidationResult.Failure(msg ?? "Respuesta nula del adaptador.")
        };
    }

    // para aplicar los cambios en caso de que recibamos OK en el adapter
    public bool Apply(MSharpInstruction instruction)
    {
        string response = SendPayloadOverPipe(instruction, isValidation: false);
        return response?.ToLowerInvariant() == "ok";
    }

    // Si el adapter me lo permite, abro pipes y envío el payload
    private string? SendPayloadOverPipe(MSharpInstruction instruction, bool isValidation)
    {
        try
        {
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
            return Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error en SendPayloadOverPipe: {ex.Message}");

            return null; 
        }
    }
}
