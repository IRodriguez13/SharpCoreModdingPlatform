using MSharp.Validation.Models;
namespace MSharp.Validation;

// -- Define el criterio de validación de instrucciones JSON --
public interface IInstructionValidator
{
	InstructionValidationResult Validate(string json);
}