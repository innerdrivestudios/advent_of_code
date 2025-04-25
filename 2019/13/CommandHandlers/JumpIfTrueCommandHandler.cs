class JumpIfTrueCommandHandler : ACommandHandler
{
	public override void Execute(Context pContext)
	{
		long firstParam = GetValue(pContext, 1);

		if (firstParam != 0)
		{
			pContext.instructionPointer = GetValue(pContext, 2);
		}
		else
		{
			pContext.instructionPointer += 3;
		}
	}
}
