/// <summary>
/// Handles the addition instruction (opcode 1) in the Intcode computer.
/// Adds two parameter values and stores the result in the target address.
/// </summary>
class AddCommandHandler : ACommandHandler
{
    /// <summary>
    /// Executes the addition operation.
    /// Resolves two operand values and stores their sum at the resolved third parameter index.
    /// Advances the instruction pointer by 4.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context for the Intcode program.</param>
    public override void Execute(Context pContext)
    {
        pContext.program[GetIndex(pContext, 3)] = GetValue(pContext, 1) + GetValue(pContext, 2);
        pContext.instructionPointer += 4;
    }
}
