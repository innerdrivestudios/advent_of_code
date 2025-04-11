class IntCodeComputer
{
	// For this version, we don't use the actual int program array,
	// since the puzzle mentions:
	// "The computer's available memory should be much larger than the initial program.
	// Memory beyond the initial program starts with the value 0 and can be read or written
	// like any other memory.
	// (It is invalid to try to access memory at a negative address, though.)"
	//
	// The easiest way to simulate this is to use a Dictionary of long to longs
	// (from instruction pointer, to memory contents)

	Dictionary<long, long> memory = new Dictionary<long, long>();

	// Command handlers for each int opcode:
	Dictionary<int, ACommandHandler> commandHandler = new();

	public IntCodeComputer(string pProgramToRun, IIntCodeIO pIOChannel)
	{
		CompileProgramIntoRunnableCode(pProgramToRun);
		SetupIntCodeCommandHandlers(pIOChannel);
	}

	void CompileProgramIntoRunnableCode(string pProgramToRun)
	{
		// Due to the big number requirement we interpret everything as longs
		long[] program = pProgramToRun.Split(",").Select(long.Parse).ToArray();

		// Copy the original program into the alternative "big" memory
		for (uint i = 0; i < program.Length; i++) memory[i] = program[i];
	}

	void SetupIntCodeCommandHandlers(IIntCodeIO pIOChannel)
	{
		commandHandler[1] = new AddCommandHandler();
		commandHandler[2] = new MultiplyCommandHandler();
		commandHandler[3] = new InputCommandHandler(pIOChannel);
		commandHandler[4] = new OutputCommandHandler(pIOChannel);
		commandHandler[5] = new JumpIfTrueCommandHandler();
		commandHandler[6] = new JumpIfFalseCommandHandler();
		commandHandler[7] = new LessThanCommandHandler();
		commandHandler[8] = new EqualsCommandHandler();
		commandHandler[9] = new AdjustRelativeBaseCommandHandler();
	}

	public long Run()
	{
		//Clone the program in case we want to run it again
		var program = new Dictionary<long, long>(memory);

		Context context = new Context();
		context.program = program;
		context.instructionPointer = 0;
		context.relativeBase = 0;

		while (true)
		{
			long valueRead = context.program[context.instructionPointer];                
			int opCode = (int)valueRead % 100;

			if (opCode == 99)
			{
				break;
			}
			else if (commandHandler.ContainsKey(opCode))
			{
				commandHandler[opCode].Execute(context);
			}
			else
			{
				Console.WriteLine("Invalid opcode:" + opCode);
			}
		}

		return memory[0];
	}

}
