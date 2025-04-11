class EqualsCommandHandler : ACommandHandler
{
	public override void Execute(Context pContext)
	{
		long firstParam = GetValue(pContext, 1);
		long secondParam = GetValue(pContext, 2);

		pContext.program[GetIndex(pContext, 3)] = (firstParam == secondParam) ? 1 : 0;

		pContext.instructionPointer += 4;
	}
}
