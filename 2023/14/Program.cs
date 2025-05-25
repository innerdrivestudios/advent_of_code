// Solution for https://adventofcode.com/2023/day/14 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A dish with rounded and cube shaped rocks

List<Vec2i> roundRocks = new();

Grid<char> platform =
	new Grid<char>(
		File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine),
		Environment.NewLine
	);

// Find all blocks
platform.Foreach(
	(pos, value) =>
	{
		if (value == 'O') roundRocks.Add(pos);
	}
);

// ** Part 1: Tilt the platform so all rounded rocks roll north,
// a calculate the score...

// Options:
// 1. Iterate over all rounded rocks and move each one upwards one by one until  they block
// 2. Iterate over all rounded rocks and move each one to a pre-calculated upward position

// Solution 1 should be somewhere around O(m*n) (m for the rocks, n for the height of the grid)
// Solution 2 should be somewhere around O(m) since moving is "instantenaous"

// Since solution 1 is way easier we'll try that first...

// Let's define some helper methods first to move any item to the first free spot above it

Vec2i Move (Vec2i pPosition, Vec2i pDirection)
{
	Vec2i previous = pPosition;
	Vec2i next = previous + pDirection;

	while (platform.IsInside(next) && platform[next] == '.')
	{
		platform[next] = platform[previous];
		platform[previous] = '.';

		previous = next;
		next = previous + pDirection;
	}

	return previous;
}

long GetScore (Vec2i pPosition)
{
	return platform.height - pPosition.Y;
}

void MoveAll (Vec2i pDirection)
{
	if (pDirection.X == -1) roundRocks.Sort(SortWest);
	else if (pDirection.X == 1) roundRocks.Sort(SortEast);
	else if (pDirection.Y == -1) roundRocks.Sort(SortNorth);
	else if (pDirection.Y == 1) roundRocks.Sort(SortSouth);

	for (int i = 0; i < roundRocks.Count; i++)
	{
		roundRocks[i] = Move(roundRocks[i], pDirection);
	}
}

int SortNorth (Vec2i pA, Vec2i pB) => pA.Y - pB.Y;
int SortSouth (Vec2i pA, Vec2i pB) => pB.Y - pA.Y;
int SortWest (Vec2i pA, Vec2i pB) => pA.X - pB.X;
int SortEast (Vec2i pA, Vec2i pB) => pB.X - pA.X;

MoveAll(new Vec2i(0, -1));

long totalScore = roundRocks.Sum (x => GetScore(x));
Console.WriteLine("Part 1:" + totalScore);

// ** Part 2: Keep moving the rocks round and round in different directions
// do that 1000000000 and calculate the final score...

// Reinitialize the grid and roundRounds list:

roundRocks.Clear();

platform =
	new Grid<char>(
		File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine),
		Environment.NewLine
	);

platform.Foreach(
	(pos, value) =>
	{
		if (value == 'O') roundRocks.Add(pos);
	}
);

// Define a helper method to run a whole cycle...

void RunCycle()
{
	MoveAll(new Vec2i(0, -1));  //north
    MoveAll(new Vec2i(-1, 0));  //west
	MoveAll(new Vec2i(0, 1));   //south
	MoveAll(new Vec2i(1, 0));   //east
}

// Running 1000000000 iterations will take way to far, so we'll try to detect
// a repeating loop and extrapolate from there ...

// Running a full cycle detection algorithm might be pretty slow, 
// but inspecting my output, indicated that there is a specific marker,
// two repeating total scores, that keeps on repeating over time.
// If we can find these markers, we can find the cycle.

long lastScore = -1;
long lastRepeatedMarker = -1;
long iteration = 0;
long startIndex = 0;

List<long> values = new List<long>();

while (true)
{
	RunCycle();
    iteration++;

    long newScore = roundRocks.Sum(x => GetScore(x));
    values.Add(newScore);

	//If we found the same score...
	if (newScore == lastScore)
	{
		// ... but are not keeping track of scores... start tracking
		if (lastRepeatedMarker == -1)
		{
			lastRepeatedMarker = newScore;
		}
		else 
		{
			// if marker is the same, we are done
			if (newScore == lastRepeatedMarker) break;
			else //reset tracking to new score
			{
				lastRepeatedMarker = newScore;
				values.Clear();
				//we just cleared everything, so the first value will be added in the next iteration
				startIndex = iteration + 1;
			}
		}
	}

	lastScore = newScore;
}

long iterationsToStillComplete = 1000000000 - startIndex;
Console.WriteLine("Part 2:" + values[(int)(iterationsToStillComplete % values.Count)]);

