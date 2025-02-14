// Solution for https://adventofcode.com/2020/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: collections of characters representing questions answered correctly

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: get the sum of all unique answers per group

int uniqueAnswerCount = myInput
	//Get the groups...
	.Split(
		Environment.NewLine + Environment.NewLine,
		StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
	)
	//Remove any left over newlines
	.Select (group => group.ReplaceLineEndings(""))
	//Get a string based on the distinct chars in the group
	.Select (group => string.Concat(group.Distinct()))
	//And sums the lengths of the unique strings
	.Sum(group => group.Length);

Console.WriteLine("Part 1 Unique answer count:"+ uniqueAnswerCount);

// ** Part 2: Get the overlapping answer count for each group

string[][] answerGroups = myInput
	//Get the groups...
	.Split(
		Environment.NewLine + Environment.NewLine,
		StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
	)
	.Select (x => x.Split (Environment.NewLine))
	.ToArray();

int overlappingAnswerCount = 0;	

foreach (string[] answerGroup in answerGroups)
{
	string startSet = answerGroup[0];

	// This is definitely not optimized
	for (int i = 1; i < answerGroup.Length; i++)
	{
		startSet = string.Concat(startSet.Intersect(answerGroup[i]));
	}

	overlappingAnswerCount += startSet.Length;
}

Console.WriteLine("Part 2 - Overlapping answer count:" + overlappingAnswerCount);

