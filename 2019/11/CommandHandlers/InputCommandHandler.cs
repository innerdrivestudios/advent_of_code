class InputCommandHandler : ACommandHandler
{
	private IIntCodeIO ioChannel;

	public InputCommandHandler(IIntCodeIO pIOChannel)
	{
		ioChannel = pIOChannel;
	}

	public override void Execute(Context pContext)
	{
		long singleIntegerAsInput = ioChannel.Read();

		pContext.program[GetIndex(pContext, 1)] = singleIntegerAsInput;
		pContext.instructionPointer += 2;
	}
}
