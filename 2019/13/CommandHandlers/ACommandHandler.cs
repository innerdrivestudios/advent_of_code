abstract class ACommandHandler
{
	//executes the command, return a modified context
	public abstract void Execute(Context pContext);

	// GetValue looks up a value in memory using the GetIndex method
	protected long GetValue(Context pContext, int pOffset)
	{
		return pContext.program.GetValueOrDefault(GetIndex(pContext, pOffset));
	}

	protected long GetIndex(Context pContext, int pOffset)
	{
		int paramMode = pContext.GetParamMode(pOffset);
		long index = pContext.instructionPointer + pOffset;
		var program = pContext.program;

		// Parameter mode 0 - Position mode, value is interpreted as a memory address
		if (paramMode == 0) return program.GetValueOrDefault(index);
		// Parameter mode 1 - Immediate mode, value is interpreted as a value
		else if (paramMode == 1) return index;
		// Parameter mode 2 - Relative mode, value is interpreted as memory address relative to the relativeBase
		else if (paramMode  == 2) return pContext.relativeBase + program.GetValueOrDefault(index);

		throw new InvalidDataException("Parameter mode " + paramMode + " not supported.");
	}


}
