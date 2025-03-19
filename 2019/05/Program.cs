// Solution for https://adventofcode.com/2019/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

int[] program = myInput.Split(",").Select(int.Parse).ToArray();

// ** Both part 1 & 2 consist of running the given program on a modified intcode machine presented in day 2.
//    The hardest part for this assignment are the unclear ambiguous instructions, 
//    but other than that, it is a very simple "just follow the instructions" exercise

Console.WriteLine("Running program 1... Please provide some input (suggestion = 1 :))");
RunProgramPart1(program);
Console.WriteLine();

Console.WriteLine("Running program 2... Please provide some input (suggestion = 5 :))");
RunProgramPart2(program);
Console.WriteLine();

// ** Part 1 -
// If you "run" the program
// provide 1 to the only input instruction and pass all the tests,
// what diagnostic code does the program produce?
//
// For instructions on how to run the program see the instructions at:
// - https://adventofcode.com/2019/day/2 &
// - https://adventofcode.com/2019/day/5

long RunProgramPart1(int[] pProgram)
{
	pProgram = (int[])pProgram.Clone();

	int i = 0;

	while (true)
	{
		int valueRead = pProgram[i];
		int opCode = valueRead % 100;
		int param1Mode = (valueRead / 100) % 10;
		int param2Mode = (valueRead / 1000) % 10;
		int param3Mode = (valueRead / 10000) % 10;	//Not used anywhere??

		if (opCode == 1)
		{
			pProgram[pProgram[i + 3]] =
				GetValue(pProgram, i + 1, param1Mode) + GetValue(pProgram, i + 2, param2Mode);

			i += 4;
		}
		else if (opCode == 2)
		{
			pProgram[pProgram[i + 3]] = 
				GetValue(pProgram, i + 1, param1Mode) * GetValue(pProgram, i + 2, param2Mode);

            i += 4;
        }
		else if (opCode == 3)
		{
			int singleIntegerAsInput = int.Parse(Console.ReadLine());
            pProgram[pProgram[i+1]] = singleIntegerAsInput;

            i += 2;
        }
        else if (opCode == 4)
        {
			Console.WriteLine(GetValue(pProgram, i + 1, param1Mode));

            i += 2;
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

	return pProgram[0];
}

int GetValue(int[] pProgram, int pIndex, int pParameterMode)
{
	if (pParameterMode == 0)
	{
		return pProgram[pProgram[pIndex]];
	}
	else if (pParameterMode == 1)
	{
		return pProgram[pIndex];
	}

	throw new InvalidDataException("Invalid parameter mode: " + pParameterMode);
}

// ** Part 2 -
// Implemented the additional instructions...
// for clarity I did this in another method, however this interpreter still works for part 1 as well
// (Yes Eric Wastl is fucking brilliant :))

long RunProgramPart2(int[] pProgram)
{
    pProgram = (int[])pProgram.Clone();

    int i = 0;

    while (true)
    {
        int valueRead = pProgram[i];
        int opCode = valueRead % 100;
        int param1Mode = (valueRead / 100) % 10;
        int param2Mode = (valueRead / 1000) % 10;
        int param3Mode = (valueRead / 10000) % 10;  //Not used anywhere??

        if (opCode == 1)
        {
            pProgram[pProgram[i + 3]] =
                GetValue(pProgram, i + 1, param1Mode) + GetValue(pProgram, i + 2, param2Mode);

            i += 4;
        }
        else if (opCode == 2)
        {
            pProgram[pProgram[i + 3]] =
                GetValue(pProgram, i + 1, param1Mode) * GetValue(pProgram, i + 2, param2Mode);

            i += 4;
        }
        else if (opCode == 3)
        {
            int singleIntegerAsInput = int.Parse(Console.ReadLine());
            pProgram[pProgram[i + 1]] = singleIntegerAsInput;

            i += 2;
        }
        else if (opCode == 4)
        {
            Console.WriteLine(GetValue(pProgram, i + 1, param1Mode));

            i += 2;
        }
        else if (opCode == 5)
        {
            int firstParam = GetValue(pProgram, i + 1, param1Mode);

            if (firstParam != 0)
            {
                i = GetValue(pProgram, i + 2, param2Mode);
            }
            else
            {
                i += 3;
            }
        }
        else if (opCode == 6)
        {
            int firstParam = GetValue(pProgram, i + 1, param1Mode);

            if (firstParam == 0)
            {
                i = GetValue(pProgram, i + 2, param2Mode);
            }
            else
            {
                i += 3;
            }
        }
        else if (opCode == 7)
        {
            int firstParam = GetValue(pProgram, i + 1, param1Mode);
            int secondParam = GetValue(pProgram, i + 2, param2Mode);

            pProgram[pProgram[i + 3]] = (firstParam < secondParam) ? 1 : 0;

            i += 4;
        }
        else if (opCode == 8)
        {
            int firstParam = GetValue(pProgram, i + 1, param1Mode);
            int secondParam = GetValue(pProgram, i + 2, param2Mode);

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

    return pProgram[0];
}

// As mentioned in a real world setting the code above could definitely be refactored,
// to avoid all of the code duplication.

