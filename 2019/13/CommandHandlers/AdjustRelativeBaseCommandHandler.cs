class AdjustRelativeBaseCommandHandler : ACommandHandler
{
	public override void Execute(Context pContext)
	{
		pContext.relativeBase += GetValue(pContext, 1);
		pContext.instructionPointer += 2;
	}
}
