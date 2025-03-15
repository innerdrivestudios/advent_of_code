// Solution for https://adventofcode.com/2024/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: The unusual data (your puzzle input) consists of many reports,
// one report per line. Each report is a list of numbers called levels that are separated by spaces.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

List<List<int>> numberLists = myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)	//Split the input into strings of ints
	.Select (
		x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries)		//split the ints on space and convert them
		.Select(int.Parse)
		.ToList()
	)
	.ToList();

// ** Part 1 - 
// The engineers are trying to figure out which reports are safe.
// The Red-Nosed reactor safety systems can only tolerate levels that are either
// gradually increasing or gradually decreasing.
// So, a report only counts as safe if both of the following are true:

// The levels are either all increasing or all decreasing.
// Any two adjacent levels differ by at least one and at most three.

// How many reports are safe?

int safeCount = 0;

foreach (List<int> numbers in numberLists)
{
	safeCount += IsSafeNumberList(numbers) ? 1 : 0;
}

bool IsSafeNumberList(List<int> pNumbers)
{
	int direction = 0;
	bool safe = true;

	// Go through all numbers, establish that the direction is maintained once set
	for (int i = 0; i < pNumbers.Count - 1; i++)
	{
		int diff = (pNumbers[i + 1] - pNumbers[i]);
		int sign = Math.Sign(diff);
		int absdiff = Math.Abs(diff);

		//Set or maintain direction
		if (direction == 0) { 
			direction = sign; 
		}
		else if (direction != sign) { 
			safe = false; 
			break; 
		}

		//Validate min max diff
		if (absdiff < 1 || absdiff > 3) { 
			safe = false; 
			break; 
		}
	}

	return safe;
}

Console.WriteLine("Part 1 - Safe number count: "+safeCount);

// ** Part 2 - Do the same thing, but see if unsafe reports can be "fixed" and if so include them in the count
// Fixed is done by removing one number from the list and checking if that fixes anything 

safeCount = 0;

foreach (List<int> numbers in numberLists)
{
	safeCount += IsSafe(numbers) ? 1 : 0;
}

bool IsSafe(List<int> pNumbers)
{
	//original check
	bool safe = IsSafeNumberList(pNumbers);

	if (!safe)
	{
		//Try original check on copies of the list with 1 item removed
		for (int i = 0; i < pNumbers.Count; i++)
		{
			List<int> clone = new List<int>(pNumbers);
			clone.RemoveAt(i);
			safe = IsSafeNumberList(clone);
			if (safe) break;
		}
	}
	return safe;
}

Console.WriteLine("Part 2 - Safe number count including fixes: "+safeCount);




