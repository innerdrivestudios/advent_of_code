/// <summary>
/// Handles the multiply instruction (opcode 2) in the Intcode computer.
/// Multiplies two parameter values and stores the result at the resolved memory location.
/// </summary>
class MultiplyCommandHandler : ACommandHandler
{
    /// <inheritdoc />
    public override void Execute(Context pContext)
    {
        pContext.program[GetIndex(pContext, 3)] = GetValue(pContext, 1) * GetValue(pContext, 2);
        pContext.instructionPointer += 4;
    }
}
