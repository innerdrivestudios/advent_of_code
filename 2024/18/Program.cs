//Solution for https://adventofcode.com/2024/day/18 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of coordinates

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
List<Vec2i> coordinates = myInput
	.Split([Environment.NewLine, ","], StringSplitOptions.RemoveEmptyEntries)
	.Select(int.Parse)
	.Chunk(2)
	.Select(x => new Vec2i(x[0], x[1]))
	.ToList();

Vec2i min = Vec2i.Min(coordinates);
Vec2i max = Vec2i.Max(coordinates);

Console.WriteLine("Min position: " + min);
Console.WriteLine("Max position: " + max);

// ** Part 1: How long is the path with a certain amount of bytes corrupted?

// Set up the grid:

Grid<char> area = new Grid<char>(max.X+1, max.Y+1);

// Perform the search (basic BFS/Dijkstra):

int GetCost (int pCorruptedBitCount)
{
    int stepLimit = pCorruptedBitCount;
    for (int i = 0; i < stepLimit; i++)
    {
        area[coordinates[i]] = '#';
    }

    Vec2i start = min;
	Vec2i end = max;

	Vec2i[] directions = [new Vec2i(1, 0), new Vec2i(-1, 0), new Vec2i(0, 1), new Vec2i(0, -1)];

	PriorityQueue<Vec2i, int> todoList = new PriorityQueue<Vec2i, int>();
	todoList.Enqueue(start, 0);

	Dictionary<Vec2i, int> costs = new();
	costs.Add(start, 0);

	Dictionary<Vec2i, Vec2i> parents = new();

	while (todoList.Count > 0)
	{
		Vec2i current = todoList.Dequeue();
		int cost = costs[current];

		if (current == end)
		{
			return cost;
		}
		else
		{
			foreach (Vec2i direction in directions)
			{
				Vec2i newPosition = current + direction;
				if (area.IsInside(newPosition) && area[newPosition] != '#' && !costs.ContainsKey(newPosition))
				{
					int newCost = cost + 1;
					costs[newPosition] = newCost;
					todoList.Enqueue(newPosition, newCost);
				}
			}
		}
	}

	return -1;
}

Console.WriteLine("Part 1:" + GetCost(1024));

// Part 2: Just gonna try and brute force it...

int currentCorruptedBitIndex = 1024;

while (GetCost(currentCorruptedBitIndex) > 0)
{
	currentCorruptedBitIndex++;
}

Console.WriteLine("Part 2:" + coordinates[currentCorruptedBitIndex - 1]);

