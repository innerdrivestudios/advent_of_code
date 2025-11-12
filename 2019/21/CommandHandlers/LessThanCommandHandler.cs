/// <summary>
/// Handles the less-than instruction (opcode 7).
/// Compares two values and stores 1 if the first is less than the second, else stores 0.
/// </summary>
class LessThanCommandHandler : ACommandHandler
{
    /// <inheritdoc />
    public override void Execute(Context pContext)
    {
        long firstParam = GetValue(pContext, 1);
        long secondParam = GetValue(pContext, 2);

        pContext.program[GetIndex(pContext, 3)] = (firstParam < secondParam) ? 1 : 0;

        pContext.instructionPointer += 4;
    }
}
