// Solution for https://adventofcode.com/2022/day/4 (Ctrl+Click in VS to follow link)

using Range = (int min, int max);

// Your input: pairs of ranges

// In visual studio you can modify which file by going to Debug/Debug Properties
// and setting $(SolutionDir)input.txt as the command line, this will be passed to args[0]

string myInput = File.ReadAllText(args[0]);

List<(Range, Range)> ranges = myInput
	//Split into separate strings describing range pairs
	.Split ("\r\n", StringSplitOptions.RemoveEmptyEntries)
	//Split into 4 strings describing the start and end of the ranges
	.Select (line => line.Split (new char[] {'-', ','}, StringSplitOptions.RemoveEmptyEntries))
	//Convert them into pairs of ranges
	.Select (
		numbers => (
			( int.Parse(numbers[0]), int.Parse(numbers[1]) ),
			( int.Parse(numbers[2]), int.Parse(numbers[3]) )
		)
	)
	.ToList ();

// Part 1 - Find all ranges that encompass/overlap

bool FullyOverlaps (Range pRangeA, Range pRangeB)
{
	return
		// A [------------]
		// B [---------------]
		(pRangeA.min >= pRangeB.min && pRangeA.max <= pRangeB.max)
		||
		// A [---------------]
		// B    [------------]
		(pRangeB.min >= pRangeA.min && pRangeB.max <= pRangeA.max);
}

Console.WriteLine(
	"Part 1 - Full overlap count: " +
	ranges.Count(rangePair => FullyOverlaps(rangePair.Item1, rangePair.Item2))
);

// Part 2 - Find all ranges that partially overlap:

bool PartiallyOverlaps(Range pRangeA, Range pRangeB)
{
	return
		// A [-----------]
		// B      [----------]
		(pRangeA.min <= pRangeB.min && pRangeA.max >= pRangeB.min)
		||
		// A      [----------]
		// B [----------]
		(pRangeB.min <= pRangeA.min && pRangeB.max >= pRangeA.min);
}

Console.WriteLine(
	"Part 2 - Partial overlap count: " +
	ranges.Count(rangePair => PartiallyOverlaps(rangePair.Item1, rangePair.Item2))
);

