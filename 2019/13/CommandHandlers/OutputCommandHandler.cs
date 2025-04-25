class OutputCommandHandler : ACommandHandler
{
	private IIntCodeIO ioChannel;

	public OutputCommandHandler(IIntCodeIO pIOChannel)
	{
		ioChannel = pIOChannel;
	}

	public override void Execute(Context pContext)
	{
		ioChannel.Write(GetValue(pContext, 1));
		pContext.instructionPointer += 2;
	}
}
