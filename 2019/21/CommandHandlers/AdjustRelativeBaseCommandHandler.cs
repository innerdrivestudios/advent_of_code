/// <summary>
/// Handles the adjust-relative-base instruction (opcode 9) in the Intcode computer.
/// Modifies the relative base by the value of its single parameter.
/// </summary>
class AdjustRelativeBaseCommandHandler : ACommandHandler
{
    /// <summary>
    /// Executes the adjust-relative-base operation.
    /// Increments the relative base by the resolved parameter value.
    /// Advances the instruction pointer by 2.
    /// </summary>
    /// 
    /// <param name="pContext">The current execution context for the Intcode program.</param>
    public override void Execute(Context pContext)
    {
        pContext.relativeBase += GetValue(pContext, 1);
        pContext.instructionPointer += 2;
    }
}
