// Solution for https://adventofcode.com/2024/day/4 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with XMAS sequences hidden in the text...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<char> grid = new Grid<char>(myInput, Environment.NewLine);
//grid.Print();

// ** Part 1: Count the total amount of XMAS strings...

int totalXmasCount = 0;
Vec2i[] directions = { 
	new (1,0), 
	new (-1,0), 
	new (0,1), 
	new (0,-1), 
	new (1,1), 
	new (-1,-1),
	new (-1,1), 
	new (1,-1)
};
string xmas = "XMAS";

grid.Foreach((position, character) =>
{
	totalXmasCount += XmasCount(position);
});

int XmasCount (Vec2<int> pPosition)
{
	int xmasFound = 0;
	foreach (var direction in directions)
	{
		if (ContainsXmasInDirection(pPosition, direction)) xmasFound++;
    }
	return xmasFound;
}

bool ContainsXmasInDirection (Vec2i pPosition, Vec2i pDirection)
{
	int count = 0;

	Vec2i start = pPosition;

	while (count < 4)
	{
		if (grid.IsInside(start) && grid[start] == xmas[count])
		{
			start += pDirection;
            count++; 
		} 
		else
		{
			return false;
		}
	}

	return true;
}

Console.WriteLine("Part 1 - XMAS Count: " +totalXmasCount);

// ** Part 2: Actually find X-MAS

totalXmasCount = 0;

grid.ForeachRegion(
	1, 1, grid.width - 1, grid.height - 1,
	(position, character) =>
	{
		totalXmasCount += XmasFound(position)?1:0;
	}
);

bool XmasFound (Vec2i pPosition)
{
	if (grid[pPosition] != 'A') return false;

	string a = "" + grid[pPosition + new Vec2i(-1, -1)] + grid[pPosition + new Vec2i(1, 1)];
	if (a != "MS" && a != "SM") return false;

	string b = "" + grid[pPosition + new Vec2i(-1, 1)] + grid[pPosition + new Vec2i(1, -1)];
	if (b != "MS" && b != "SM") return false;

	return true;
}

Console.WriteLine("Part 2 - XMAS Count: " +totalXmasCount);