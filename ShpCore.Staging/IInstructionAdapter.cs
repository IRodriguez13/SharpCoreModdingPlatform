using MSharp.Validation.Models;
using MSharp.Launcher.Core.Models;

namespace MSharp.Staging.Instruction_adapters;

// -- Interface to Validate instrutions -- 

public interface IInstructionAdapter
{

    InstructionValidationResult Validate(MSharpInstruction instruction);
    bool Apply(MSharpInstruction instruction);

}

