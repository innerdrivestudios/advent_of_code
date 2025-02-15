// Solution for https://adventofcode.com/2021/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of lanternfish ages...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

List<long> lanternFish = myInput
	.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
	.Select (long.Parse)
	.ToList();

// ** Part 1: (naively) simulate all lanternfish and get their count

long LanternFishCountAfterXDays (int pDays, List<long> pInitialLanternFish)
{
	// Clone the fish
	pInitialLanternFish = new(pInitialLanternFish);

	while (pDays > 0)
	{
		int lanternFishCount = pInitialLanternFish.Count;

		for (int i = 0; i < lanternFishCount; i++)
		{
			pInitialLanternFish[i]--;
			if (pInitialLanternFish[i] == -1)
			{
				pInitialLanternFish[i] = 6;
				pInitialLanternFish.Add(8);
			}
		}

		pDays--;
    }

	return pInitialLanternFish.Count;
}

Console.WriteLine("Part 1: "+ LanternFishCountAfterXDays(80, lanternFish));

// Part 2: How many fish are there after 256 days?

// OF COURSE! WHY NOT! :)
// The issue of course is that the number of lantern fish grows WAY too f-ing fast
// to compute in a reasonable time frame, BUT...
// The trick is of course that we do not need to know the exact fish in question that multiplies...
// we only need to know the amount of fish that multiply... if that makes sense...

// In other words, if we know how MANY fish we have that are 0 days from multiplying, 1 day, 2 day, etc
// we can calculate everything else...

long LanternFishCountAfterXDaysOptimized(int pDays, List<long> pInitialLanternFish)
{
	// So we have 9 days to register (0 to 8)
	List<long> lanternFish = [0,0,0,0,0,0,0,0,0];

	// First we map the days to procreate to the amount of fish for that day ...
	foreach (int fish in pInitialLanternFish)
	{
		lanternFish[fish]++;
	}

	while (pDays > 0)
	{
		// Every day we take all fish at day 0, remove them and add them to day 6
		// While adding the same amount of children to the end of the list (day 8)
		// This could even be optimized further using a linked list, or ring buffer 
		// but that would only unnecesarily complicate things
		long fish = lanternFish[0];
		lanternFish.RemoveAt(0);
		lanternFish[6] += fish;
		lanternFish.Add(fish);

		pDays--;
	}

	return lanternFish.Sum();
}

Console.WriteLine("Part 2: "+ LanternFishCountAfterXDaysOptimized(256, lanternFish));




