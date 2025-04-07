//Solution for https://adventofcode.com/2024/day/10 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid of digits representing a topographic map with trails,
// starting at value 0, increasing with 1 step increments, ending at a height of 9. 

// Let's start by parsing the input...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);
Grid<int> trailGrid = new Grid<int>(myInput, Environment.NewLine);

// And let's find the start of all trails...
List<Vec2i> trailStarts = new List<Vec2i>();
trailGrid.Foreach((position, value) =>
    {
        if (value == 0) trailStarts.Add(position);
    }
);

// ** Part 1 & 2: for each trail start, count how many valid ends it leads to 
// (by gradually increasing the height each step until the value is 9)

// We'll define two helper methods for this:

// CalculatePaths calculates the different ways we can get from 0 to 9 using
// the provided collection types to keep track of the different ways.
// If you pass in a list, every 9 counts as a unique 9
// If you pass in a hashset, only unique positions count as a unique 9

int CalculatePaths (ICollection<Vec2i> pPathGatherer)
{
	int totalValidDestinationsFound = 0;

    foreach (Vec2i v in trailStarts)
    {
		pPathGatherer.Clear();
		RunSearch(v, pPathGatherer);
		totalValidDestinationsFound += pPathGatherer.Count;
    }

	return totalValidDestinationsFound;
}

void RunSearch(Vec2i pPosition, ICollection<Vec2i> pPathGatherer)
{
	//Look for the value at the current position
	int gridValue = trailGrid[pPosition.X, pPosition.Y];

	//If it is 9, record it and return
	if (gridValue == 9)
	{
		pPathGatherer.Add(new Vec2i(pPosition.X, pPosition.Y));
		return;
	}

	//Look for all cells in our vicinity with a value that is one higher...
	int minX = Math.Max(pPosition.X - 1, 0);
	int minY = Math.Max(pPosition.Y - 1, 0);
	int maxX = Math.Min(trailGrid.width - 1, pPosition.X + 1);
	int maxY = Math.Min(trailGrid.height - 1, pPosition.Y + 1);

	for (int x = minX; x <= maxX; x++)
	{
		for (int y = minY; y <= maxY; y++)
		{
			//no diagonals
			if (x != pPosition.X && y != pPosition.Y) continue;
			//if we found a next possible step, recurse looking for the final destination 
			//(which is 9)
			if (gridValue + 1 == trailGrid[x, y]) RunSearch(new Vec2i(x, y), pPathGatherer);
		}
	}
}

Console.WriteLine("Part 1: "+CalculatePaths(new HashSet<Vec2i>()));
Console.WriteLine("Part 2: "+CalculatePaths(new List<Vec2i>()));

