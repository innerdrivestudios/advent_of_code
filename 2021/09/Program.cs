// Solution for https://adventofcode.com/2021/day/9 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid describing a lava cave.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<int> lavaCave = new Grid<int>(myInput, Environment.NewLine);

// ** Part 1: Find all numbers that are lower than their surroundings, take their Sum(x => x+1)

Vec2i[] directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];
int riskLevelSum = 0;

// Required for part 2...
HashSet<Vec2i> lowestPoints = new HashSet<Vec2i>();

lavaCave.Foreach(CalculateRiskLevel);
Console.WriteLine("Part 1 - Risk level:" + riskLevelSum);

void CalculateRiskLevel (Vec2i pPosition, int pValue)
{
	foreach (var direction in directions)
	{
		Vec2i testPosition = pPosition + direction;
		if (lavaCave.IsInside(testPosition) && lavaCave[testPosition] <= pValue) return;
	}

	lowestPoints.Add(pPosition);
	riskLevelSum += pValue + 1;
}

// ** Part 2: Multiply the sizes of the three largest floodfilled basins

int multipliedSizes = 
	lowestPoints
		//Run the flood fill on every point in the lowestPoint set and take its Count
		.Select(point => lavaCave.FloodFill(point, a => lavaCave[a] != 9).Count)
		//Order it descending
		.OrderByDescending(size => size)
		//And the first three
		.Take(3)
		//Then aggregate the results of these 3 items by multiplying them
		.Aggregate((x, y) => x * y);

Console.WriteLine("Part 2 - Multiplied sizes of the 3 largest basins:" + multipliedSizes);

