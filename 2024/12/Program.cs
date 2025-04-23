//Solution for https://adventofcode.com/2024/day/12 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with characters that form regions

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// Parsing the input:

Grid<char> grid = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1 & 2: What is the total price of fencing all regions on your map?

// Basically for each position in the grid that hasn't been visited yet,
// we'll run a region detection algorithm...

HashSet<Vec2i> visited = new();
List<Region> regions = new List<Region>();

grid.Foreach(
	(position, id) =>
	{
		if (!visited.Contains(position))
		{
			HashSet<Vec2i> filledPositions = grid.FloodFill(position, x => grid[x] == id);
			visited.UnionWith(filledPositions);
			regions.Add(new Region(filledPositions));
		}
	}
);

Console.WriteLine("Part 1:" + regions.Sum(x => x.area * x.perimeter));
Console.WriteLine("Part 2:" + regions.Sum(x => x.area * x.GetOptimizedPerimeterSize()));

