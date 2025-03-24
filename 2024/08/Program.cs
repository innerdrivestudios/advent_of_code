//Solution for https://adventofcode.com/2024/day/8 (Ctrl+Click in VS to follow link)

using System.Xml.Linq;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with signal towers

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<char> area = new Grid<char> (myInput, Environment.NewLine);

// ** Part 1: Antennas create anti nodes at the same distance from them as between them
// How many unique locations within the bounds of the map contain an antinode?

// For this problem it is easier to iterate over all grid elements in order using a single for loop
// instead of using a separate x and y using a double for loop

HashSet<Vec2i> uniqueNodePositions = new HashSet<Vec2i>();

int maxElements = area.totalElements;

for (int  i = 0;  i < maxElements-1;  i++)
{
    for (int j = i + 1; j < maxElements; j++)
	{
		if (area[i] == '.' || area[i] != area[j]) continue;

		//Get the vector from node i to j
		Vec2i iPos = new Vec2i(i % area.width, i / area.width);
		Vec2i jPos = new Vec2i(j % area.width, j / area.width);
		Vec2i delta = jPos - iPos;

		//Use that delta to get the antenna positions
		Vec2i nodeA = iPos - delta;
		Vec2i nodeB = jPos + delta;

		if (area.IsInside(nodeA)) uniqueNodePositions.Add(nodeA);
		if (area.IsInside(nodeB)) uniqueNodePositions.Add(nodeB);
	}
}

Console.WriteLine("Part 1 - Unique node count:" + uniqueNodePositions.Count);

// ** Part 2: Same, but now we don't have two nodes, instead frequencies repeat forever ...

uniqueNodePositions.Clear();

for (int i = 0; i < maxElements - 1; i++)
{
	for (int j = i + 1; j < maxElements; j++)
	{
		if (area[i] == '.' || area[i] != area[j]) continue;

		// Get the vector from node i to j
		Vec2i iPos = new Vec2i(i % area.width, i / area.width);
		Vec2i jPos = new Vec2i(j % area.width, j / area.width);
		Vec2i delta = jPos - iPos;

		// With those vectors, keep walking as long as we are inside the area
		while (area.IsInside(iPos))
		{
			uniqueNodePositions.Add(iPos);
			iPos -= delta;
		}

		while (area.IsInside(jPos))
		{
			uniqueNodePositions.Add(jPos);
			jPos += delta;
		}

	}
}

Console.WriteLine("Part 2 - Unique node count:" + uniqueNodePositions.Count);

