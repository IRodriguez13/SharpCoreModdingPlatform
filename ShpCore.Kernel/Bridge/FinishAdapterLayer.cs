using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using MSharp.Staging.Instruction_adapters;
using MSharp.Launcher.Core.Models;
using MSharp.Validation.Models;
using ShpCore.Logging;
namespace MSharp.Validation.Payloads;

public class FinishAdapterLayer : IInstructionAdapter
{
    private readonly string _pipeName;

    public FinishAdapterLayer(string pipeName = "msharp_bridge") => _pipeName = pipeName;


    // Este adaptador se encarga de enviar instrucciones a través de un Named Pipe
    public InstructionValidationResult Validate(MSharpInstruction instruction)
    {
        var response = SendPayloadOverPipe(instruction, isValidation: true);

        return response?.ToLowerInvariant() switch
        {
            "ok" => InstructionValidationResult.Success(),
            var msg => InstructionValidationResult.Failure(msg ?? "[bridge] Null response from adapter.")
        };
    }

    // para aplicar los cambios en caso de que recibamos OK en el adapter
    public bool Apply(MSharpInstruction instruction)
    {
        string response = SendPayloadOverPipe(instruction, isValidation: false) ?? throw new InvalidOperationException("Couldnt send payl to adapter.");
        return response?.ToLowerInvariant() == "ok";
    }

    // Si el adapter me lo permite (si me devolvió OK), abro pipes y envío el payload final que se debería implementar en el juego
    // Si no, devuelvo un mensaje de error
    // El payload es un JSON con el tipo de instrucción y los datos necesarios para aplicar

    private string? SendPayloadOverPipe(MSharpInstruction instruction, bool isValidation)
    {
        try
        {
            KernelLog.Debug($"[FinishAdapterLayer] Sending payload to adapter: {instruction}");

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


            // Read Response from adapter		

            using MemoryStream ms = new();
            byte[] responseBuffer = new byte[256];
            int bytesRead;

            while ((bytesRead = client.Read(responseBuffer, 0, responseBuffer.Length)) > 0)
            {
                ms.Write(responseBuffer, 0, bytesRead);


                if (!client.CanRead)
                {
                    KernelLog.Panic($"[FinishAdapterLayer] Dev, Adapter cannot read buffer");
                    break;
                }


                // If the message is complete, break
                if (client.IsMessageComplete)
                    break;
            }

            KernelLog.Debug($"[FinishAdapterLayer] Adapter Response: {Encoding.UTF8.GetString(responseBuffer, 0, bytesRead)}");

            // Response mapping 
            AdapterResponse? AdResp = JsonSerializer.Deserialize<AdapterResponse>(responseBuffer);

            if (AdResp == null)
            {
                KernelLog.Panic("[Adapter] Adapter response couldnt be deserialized.");
                return null;
            }

            if (AdResp.Status.ToLowerInvariant() != "ok")
            {
               KernelLog.Warn($"[Adapter] Error Received: {AdResp.Info} | UUID: {AdResp.Uuid}");
            }

       
            return AdResp?.Status;
        }

        catch (Exception ex)
        {
            KernelLog.Panic($"[FinishAdapterLayer] Fatal err with send or reading response: {ex.Message}");
            return null;
        }
    }
}
