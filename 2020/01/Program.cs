//Solution for https://adventofcode.com/2020/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of numbers for an "expense report"

long[] numbers = ParseUtils.FileToNumbers<long>(args[0], Environment.NewLine);

// Part 1 - Find the two numbers that add up to 2020 and return their product

long GetExpenseChecksumFor2(long[] pNumbers)
{

	for (int i = 0; i < pNumbers.Length - 1; i++)
	{
		long entryA = pNumbers[i];

		if (entryA >= 2020) continue; //prune for speed
		
		for (int j = i + 1; j < pNumbers.Length; j++)
		{
			long entryB = pNumbers[j];

            if (entryA + entryB == 2020)
			{
				return entryA * entryB;
			}
		}
	}

	return -1;
}

Console.WriteLine("Part 1 - Expense report checksum for 2 numbers: " + GetExpenseChecksumFor2(numbers));

// Part 2 - Find the three numbers that add up to 2020 and return their product

long GetExpenseChecksumFor3(long[] pNumbers)
{

	for (int i = 0; i < pNumbers.Length - 2; i++)
	{
		long entryA = pNumbers[i];

		if (entryA >= 2020) continue; //prune for speed

		for (int j = i + 1; j < pNumbers.Length - 1; j++)
		{
			long entryB = pNumbers[j];

			if (entryA + entryB >= 2020) continue; //prune for speed

			for (int k = j + 1; k < pNumbers.Length; k++)
			{
				long entryC = pNumbers[k];

				if (entryA + entryB + entryC == 2020)
				{
					return entryA * entryB * entryC;
				}

			}
		}
	}

	return -1;
}

Console.WriteLine("Part 2 - Expense report checksum for 3 numbers: " + GetExpenseChecksumFor3(numbers));

