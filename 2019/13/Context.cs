class Context
{
	public Dictionary<long, long> program;

	public long instructionPointer;
	public long relativeBase;

	public int GetParamMode(int pParam) {
		return (int)((program[instructionPointer] / Math.Pow(10, pParam+1) % 10));
	}

}

