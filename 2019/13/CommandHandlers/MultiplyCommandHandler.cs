class MultiplyCommandHandler : ACommandHandler
{
	public override void Execute(Context pContext)
	{
		pContext.program[GetIndex(pContext, 3)] = GetValue(pContext, 1) * GetValue(pContext, 2);
		pContext.instructionPointer += 4;
	}
}
