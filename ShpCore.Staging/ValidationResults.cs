namespace MSharp.Validation.Models;

// -- Esta clase define el resultado de la validación de una instrucción JSON que viene del adapter --

public class InstructionValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage
    { get; }

    private InstructionValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static InstructionValidationResult Success() => new(true, null);
    public static InstructionValidationResult Failure(string message) => new(false, message);
}
