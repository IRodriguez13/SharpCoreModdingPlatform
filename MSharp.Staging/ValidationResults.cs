namespace MSharp.Validation.Models;

// -- Esta clase define el resultado de la validación de una instrucción JSON --

public class InstructionValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage
    {
        get;
        set
        {
            if (!IsValid && string.IsNullOrEmpty(value)) throw new ArgumentException("ErrorMessage cannot be null or empty if IsValid is false.");

            value = value;
        }
    }

    private InstructionValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static InstructionValidationResult Success() => new(true, null);
    public static InstructionValidationResult Failure(string message) => new(false, message);
}
