// Solution for https://adventofcode.com/2021/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//** Your input: a bunch of ocean floor depths as scanned by a sonar

long[] depths = ParseUtils.FileToNumbers<long>(args[0], Environment.NewLine);

// ** Part 1 - Detect how many time the depth increases from the previous scan

long GetIncreaseCount(long[] pDepths)
{
	long increaseCount = 0;
	
	for (int i = 1;  i < pDepths.Length; i++)
	{
		if (pDepths[i] > pDepths[i - 1]) increaseCount++;

	}

	return increaseCount;
}

Console.WriteLine("Part 1 - Increase count: "+GetIncreaseCount(depths));

// ** Part 2 - Detect how many time the depth increases from the previous scan in a 3 frame window

long GetIncreaseCount3FrameWindow(long[] pDepths)
{
	long increaseCount = 0;
	
	for (int i = 0; i < pDepths.Length - 3; i++)
	{
		long windowA = pDepths[i + 0] + pDepths[i + 1] + pDepths[i + 2];
		long windowB = pDepths[i + 1] + pDepths[i + 2] + pDepths[i + 3];

		if (windowB > windowA) increaseCount++;
	}

	return increaseCount;
}

Console.WriteLine("Part 2 - Increase count 3 frame window: " + GetIncreaseCount3FrameWindow(depths));

Console.ReadKey();
