/// <summary>
/// Handles the equals instruction (opcode 8) in the Intcode computer.
/// Compares two values and stores 1 if they are equal, or 0 otherwise, at the target address.
/// </summary>
class EqualsCommandHandler : ACommandHandler
{
    /// <summary>
    /// Executes the equals operation.
    /// Resolves two operand values and stores 1 at the target address if they are equal, otherwise stores 0.
    /// Advances the instruction pointer by 4.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context for the Intcode program.</param>
    public override void Execute(Context pContext)
    {
        long firstParam = GetValue(pContext, 1);
        long secondParam = GetValue(pContext, 2);

        pContext.program[GetIndex(pContext, 3)] = (firstParam == secondParam) ? 1 : 0;

        pContext.instructionPointer += 4;
    }
}
