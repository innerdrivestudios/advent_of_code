// Solution for https://adventofcode.com/2019/day/24 (Ctrl+Click in VS to follow link)

using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: Simulate bugs living and dying

// Create two grids so we can use a front and a back buffer...

Grid<char> active = new Grid<char>(myInput, Environment.NewLine);
Grid<char> buffer = new Grid<char>(myInput, Environment.NewLine);

// Declare all directions

List<Vec2i> directions = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)]; 

int IterateAndEncode (Grid<char> pGrid1, Grid<char> pGrid2)
{
	int result = 0;
	int bitIndex = 0;

	pGrid1.Foreach(
		(pos, value) =>
		{
			int bugCount = GetNeighborBugCount(pGrid1, pos);

			if (value == '#' && bugCount != 1) pGrid2[pos] = '.';
			else if (value == '.' && (bugCount == 1 || bugCount == 2)) pGrid2[pos] = '#';
			else pGrid2[pos] = value;

			result |= (pGrid2[pos] == '#' ? 1 : 0) << bitIndex;
			bitIndex++;
		}
	);

	return result;
}

int GetNeighborBugCount (Grid<char> pGrid, Vec2i pPosition)
{
	int count = 0;
	foreach (Vec2i direction in directions)
	{
		Vec2i newPosition = pPosition + direction;
		if (pGrid.IsInside(newPosition) && pGrid[newPosition] == '#') count++;
	}

	return count;
}

HashSet<int> gridConfigurations = new();

while (true)
{
	int code = IterateAndEncode(active, buffer);

	var temp = active;
	active = buffer;
	buffer = temp;

	if (!gridConfigurations.Add(code))
	{
		Console.WriteLine("Part 1:" + code);
		break;
	}
}

// ** Part 2: The grids are actually recursively nested and never ending ...

// This requires a fair amount of setup... so I'll start by defining some helper methods and variables...

// We'll store the grids in a dictionary from level to grid...
// but just as previously we'll store both an active and buffer grid,
// plus the resulting biodiversity rating after processing the grid:

Dictionary<int, (Grid<char> active, Grid<char> buffer, int bioRating)> grids = new();

// I'll also generate an empty grid:

Grid<char> emptyGrid = new Grid<char>(5, 5);
emptyGrid.Foreach((pos, value) => emptyGrid[pos] = '.');

grids[0] =
	(
		new Grid<char>(myInput, Environment.NewLine),
		emptyGrid.Clone(),
		1												// initial value doesn't matter
	);

// When a grid is not empty, it might have items along it's inner or outer border, 
// so'll we need to make sure those grids are there!

void EnsureGridLevels()
{
	List<int> allKeys = grids.Keys.ToList();

	foreach (var key in allKeys)
	{
		if (grids[key].bioRating > 0)
		{
			if (!grids.ContainsKey(key - 1)) grids[key - 1] = (emptyGrid.Clone(), emptyGrid.Clone(), 0); //outside
			if (!grids.ContainsKey(key + 1)) grids[key + 1] = (emptyGrid.Clone(), emptyGrid.Clone(), 0); //inside
		}
	}
}

EnsureGridLevels();

// Then when we are processing all cells in a grid, we'll need to know whether a cell is on the outside or inside:

bool IsCenter (Vec2i pPosition)
{
	return pPosition == new Vec2i(2,2);
}

bool IsOutside (Vec2i pPosition)
{
	return pPosition.X < 0 || pPosition.X > 4 || pPosition.Y < 0 || pPosition.Y > 4;
}

// Also we'll need to be able to get the inside or outside count of a grid position:

int GetNeighborBugCountPart2 (int pLevel, Vec2i pPosition)
{
	Grid<char> grid = grids[pLevel].active;

	int count = 0;

	foreach (var direction in directions)
	{
		Vec2i newPosition = pPosition + direction;

		if (IsCenter(newPosition))
		{
			//when we are going deeper into the dungeon, we need to get
			//the bug count on the outside of the deeper dungeon for a specific side
			count += GetOutsideBugCount(pLevel + 1, direction);
		}
		else if (IsOutside(newPosition))
		{
			//if we are going to the outside we need to get the bug count on the inside
			//of the outer dungeon for a specific side
            count += GetInsideBugCount(pLevel - 1, direction);
        }
		else
		{
			count += grid[newPosition] == '#' ? 1 : 0;
		}
    }

	return count;
}

// So if we enter the center, we need to count the outside of the inner grid.
// E.g. we step to the left entering the center, we need to count the items on the right side of the inner grid

int GetOutsideBugCount (int pLevel, Vec2i pDirection)
{
    if (!grids.ContainsKey(pLevel)) return 0;

    Grid<char> grid = grids[pLevel].active;

    //List<Vec2i> directions = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];
    //Let's turn our given direction into an index of 0 - 3
    int index = directions.IndexOf(pDirection);

	//So if the direction is X = -1, we need to count the bugs on the +X side, X=+1 => -X side, etc
	//In addition we need to count them in the right direction
	//Let's set up some helper values for that:

								//right			//bottom	//left		//top
	Vec2i[] startingPoint =		[new(4, 0),		new(0, 4),	new(0, 0),	new(0, 0)];
	Vec2i[] countDirection =	[new(0, 1),		new(1, 0),	new(0, 1),	new(1, 0)];

	Vec2i start = startingPoint[index];
	int count = 0;

	for (int i = 0; i < 5; i++)
	{
        count += grid[start] == '#' ? 1 : 0;
		start += countDirection[index];
    }

	return count;
}

// If we enter the outside of our grid we need to check what is on the inside
// of our outer grid, if we exit our grid to the right, we check what is at 2,2 + (1,0) in the outergrid

int GetInsideBugCount (int pLevel, Vec2i pDirection) {
	if (!grids.ContainsKey(pLevel)) return 0;

    Grid<char> grid = grids[pLevel].active;
	Vec2i positionToTest = new Vec2i(2, 2) + pDirection;
	return grid[positionToTest] == '#' ? 1 : 0;
}

// Now that we have everything, we can rewrite our iterate and encode for part 2:

int IterateAndEncodePart2(int pLevel)
{
	Grid<char> active = grids[pLevel].active;
	Grid<char> buffer = grids[pLevel].buffer;

    int result = 0;
    int bitIndex = 0;

    active.Foreach(
        (pos, value) =>
        {
			//Skip the center since we are already taking care of that in another grid
			if (pos == new Vec2i(2, 2)) return;

            int bugCount = GetNeighborBugCountPart2(pLevel, pos);

            if (value == '#' && bugCount != 1) buffer[pos] = '.';
            else if (value == '.' && (bugCount == 1 || bugCount == 2)) buffer[pos] = '#';
            else buffer[pos] = value;

            result |= (buffer[pos] == '#' ? 1 : 0) << bitIndex;
            bitIndex++;
        }
    );

    return result;
}

int iterationCount = 200;

for (int i = 0; i < iterationCount; i++)
{
	// Run through all grids updating them and updating their biovalues
	foreach (var kv in grids)
	{
		var value = kv.Value;
		value.bioRating = IterateAndEncodePart2(kv.Key);
		grids[kv.Key] = value;
	}

	//Then swap all grids for the next round unless we are at the end
    foreach (var kv in grids)
    {
        var value = kv.Value;
		var tmp = value.active;
		value.active = value.buffer;
		value.buffer = tmp;
        grids[kv.Key] = value;
    }

	EnsureGridLevels();
}

// Now after doing this, we'll need to count the bugs in every grid level!

int CountBugsInGrid (Grid<char> pGrid)
{
	int count = 0;
	pGrid.Foreach(
		(pos, value) =>
		{
			if (pos == new Vec2i(2, 2)) return;

			count += pGrid[pos] == '#' ? 1 : 0;
		}
	);

	return count;
}

Console.WriteLine("Part 2:" + grids.Sum (x => CountBugsInGrid(x.Value.active)));



