using MSharp.Validation.Models;
using MSharp.Launcher.Core.Models;

namespace MSharp.Staging.Instruction_adapters;

// -- En esta interfaz defino los métodos que deben implementar los adaters -- 

public interface IInstructionAdapter
{
	InstructionValidationResult Validate(MSharpInstruction instruction);
	bool Apply(MSharpInstruction instruction);
}
