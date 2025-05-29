/// <summary>
/// Handles the jump-if-true instruction (opcode 5).
/// If the first parameter is non-zero, sets the instruction pointer to the value of the second parameter.
/// Otherwise, advances the pointer by 3.
/// </summary>
class JumpIfTrueCommandHandler : ACommandHandler
{
    /// <inheritdoc />
    public override void Execute(Context pContext)
    {
        long firstParam = GetValue(pContext, 1);

        pContext.instructionPointer = (firstParam != 0)
            ? GetValue(pContext, 2)
            : pContext.instructionPointer + 3;
    }
}
