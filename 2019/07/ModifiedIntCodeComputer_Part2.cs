// Similar to version 1, but now we can create instances of it
// and we can "pause" execution, while we are waiting for more input...

class ModifiedIntCodeComputer_Part2
{
	private int[] program;
	private int instructionPointer = 0;
	private Queue<int> inputQueue = new();
	private List<int> outputs = new();

	public enum ProgramState { WAITING_FOR_INPUT, ENDED, FAILED };

	public ModifiedIntCodeComputer_Part2(int[] pProgramToRun, int pConfiguration)
	{
		program = (int[])pProgramToRun.Clone();
		instructionPointer = 0;
		inputQueue.Enqueue(pConfiguration);
	}

	public void QueueInputs (List<int> pInputs)
	{
		foreach (int i in pInputs) inputQueue.Enqueue(i);
	}

	public List<int> GetOutputs() { return outputs; }

	public ProgramState Run()
	{
		outputs.Clear();

		while (true)
		{
			int valueRead = program[instructionPointer];
			int opCode = valueRead % 100;
			int param1Mode = (valueRead / 100) % 10;
			int param2Mode = (valueRead / 1000) % 10;

			if (opCode == 1)
			{
				program[program[instructionPointer + 3]] =
					IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode) +
					IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);

				instructionPointer += 4;
			}
			else if (opCode == 2)
			{
				program[program[instructionPointer + 3]] =
					IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode) * 
					IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);

				instructionPointer += 4;
			}
			else if (opCode == 3)
			{
				if (inputQueue.Count > 0)
				{
					program[program[instructionPointer + 1]] = inputQueue.Dequeue();
					instructionPointer += 2;
				}
				else
				{
					return ProgramState.WAITING_FOR_INPUT;
				}
			}
			else if (opCode == 4)
			{
				outputs.Add(IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode));

				instructionPointer += 2;
			}
			else if (opCode == 5)
			{
				int firstParam = IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode);

				if (firstParam != 0)
				{
					instructionPointer = IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);
				}
				else
				{
					instructionPointer += 3;
				}
			}
			else if (opCode == 6)
			{
				int firstParam = IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode);

				if (firstParam == 0)
				{
					instructionPointer = IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);
				}
				else
				{
					instructionPointer += 3;
				}
			}
			else if (opCode == 7)
			{
				int firstParam = IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode);
				int secondParam = IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);

				program[program[instructionPointer + 3]] = (firstParam < secondParam) ? 1 : 0;

				instructionPointer += 4;
			}
			else if (opCode == 8)
			{
				int firstParam = IntCodeUtil.GetValue(program, instructionPointer + 1, param1Mode);
				int secondParam = IntCodeUtil.GetValue(program, instructionPointer + 2, param2Mode);

				program[program[instructionPointer + 3]] = (firstParam == secondParam) ? 1 : 0;

				instructionPointer += 4;
			}
			else if (opCode == 99)
			{
				return ProgramState.ENDED;
			}
			else
			{
				Console.WriteLine("Invalid opcode:" + opCode);
				return ProgramState.FAILED;
			}
		}
	}
}

