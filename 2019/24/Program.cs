// Solution for https://adventofcode.com/2019/day/24 (Ctrl+Click in VS to follow link)

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

Vec2i[] directions = [new(-1, 0), new(1, 0), new(0, -1), new(0, 1)]; 

int IterateAndEncode (ref Grid<char> pGrid1, ref Grid<char> pGrid2)
{
	var tempGrid1 = pGrid1; 
	var tempGrid2 = pGrid2;

	int result = 0;
	int bitIndex = 0;

	pGrid1.Foreach(
		(pos, value) =>
		{
			int bugCount = GetBugCount(tempGrid1, pos);

			if (value == '#' && bugCount != 1) tempGrid2[pos] = '.';
			else if (value == '.' && (bugCount == 1 || bugCount == 2)) tempGrid2[pos] = '#';
			else tempGrid2[pos] = value;

			result |= (tempGrid2[pos] == '#' ? 1 : 0) << bitIndex;
			bitIndex++;
		}
	);

	Grid<char> temp = pGrid1;
	pGrid1 = pGrid2;
	pGrid2 = temp;

	return result;
}

int GetBugCount (Grid<char> pGrid, Vec2i pPosition)
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
	int code = IterateAndEncode(ref active, ref buffer);
	if (!gridConfigurations.Add(code))
	{
		Console.WriteLine("Part 1:" + code);
		break;
	}

	//active.Print("");
	//Console.WriteLine(Convert.ToString(code,2));
	//Console.ReadKey();
}


Console.WriteLine();



