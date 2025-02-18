static class ModifiedIntCodeComputer_Part1
{
	// Runs the modified int code from day 5
	public static int Run(int[] pProgram, Queue<int> pInput)
	{
		pProgram = (int[])pProgram.Clone();

		int i = 0;

		while (true)
		{
			int valueRead = pProgram[i];
			int opCode = valueRead % 100;
			int param1Mode = (valueRead / 100) % 10;
			int param2Mode = (valueRead / 1000) % 10;

			if (opCode == 1)
			{
				pProgram[pProgram[i + 3]] =
					IntCodeUtil.GetValue(pProgram, i + 1, param1Mode) +
					IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);

				i += 4;
			}
			else if (opCode == 2)
			{
				pProgram[pProgram[i + 3]] =
					IntCodeUtil.GetValue(pProgram, i + 1, param1Mode) * 
					IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);

				i += 4;
			}
			else if (opCode == 3)
			{
				pProgram[pProgram[i + 1]] = pInput.Dequeue();

				i += 2;
			}
			else if (opCode == 4)
			{
				// Immediately return the output...
				return IntCodeUtil.GetValue(pProgram, i + 1, param1Mode);

				i += 2;
			}
			else if (opCode == 5)
			{
				int firstParam = IntCodeUtil.GetValue(pProgram, i + 1, param1Mode);

				if (firstParam != 0)
				{
					i = IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);
				}
				else
				{
					i += 3;
				}
			}
			else if (opCode == 6)
			{
				int firstParam = IntCodeUtil.GetValue(pProgram, i + 1, param1Mode);

				if (firstParam == 0)
				{
					i = IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);
				}
				else
				{
					i += 3;
				}
			}
			else if (opCode == 7)
			{
				int firstParam = IntCodeUtil.GetValue(pProgram, i + 1, param1Mode);
				int secondParam = IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);

				pProgram[pProgram[i + 3]] = (firstParam < secondParam) ? 1 : 0;

				i += 4;
			}
			else if (opCode == 8)
			{
				int firstParam = IntCodeUtil.GetValue(pProgram, i + 1, param1Mode);
				int secondParam = IntCodeUtil.GetValue(pProgram, i + 2, param2Mode);

				pProgram[pProgram[i + 3]] = (firstParam == secondParam) ? 1 : 0;

				i += 4;
			}
			else if (opCode == 99)
			{
				break;
			}
			else
			{
				Console.WriteLine("Invalid opcode:" + opCode);
			}
		}

		return -1;
	}

}

