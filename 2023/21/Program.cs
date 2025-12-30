// Solution for https://adventofcode.com/2023/day/21 (Ctrl+Click in VS to follow link)

using System.Reflection.Metadata.Ecma335;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a garden plot...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> garden = new Grid<char>(myInput, Environment.NewLine);

garden = garden.Duplicate(11, 11);

Vec2i center = new Vec2i(garden.width / 2, garden.height / 2);

if (garden[center] != 'S') Console.WriteLine("Whoops...");

long GetMaxOptionsAtCost (Vec2i pStart, long pCost)
{
	Queue<Vec2i> queue = new Queue<Vec2i>();
	queue.Enqueue(pStart);

	Dictionary<Vec2i, long> costs = new();
	costs[pStart] = 0;

	Dictionary<Vec2i, Vec2i> parent = new ();
	parent[pStart] = pStart;

	Vec2i[] directions = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];

	while (queue.Count>0)
	{
		Vec2i currentPosition= queue.Dequeue();
		long currentCost = costs[currentPosition];

		// Done with this route...
		if (currentCost == pCost) continue;

		foreach (Vec2i direction in directions)
		{
			Vec2i newPos = currentPosition + direction;
			long newCost = currentCost + 1;

			if (!garden.IsInside(newPos) || garden[newPos] == '#') continue;

			if (!costs.ContainsKey(newPos) || costs[newPos] > newCost)
			{
				queue.Enqueue(newPos);
				costs[newPos] = newCost;
				parent[newPos] = currentPosition;
			}
		}
	}

	// Basically, at any point in the path we can decide to turn around,
	// e.g. if we have 1 step left, we can go 1 step back instead (2 steps compared to the end point)
	//		2 steps left, 2 steps back (4 steps compared to the end point)
	// i.e. all nodes with an even path cost:
	int hastagCount = 0;
	//garden.Foreach((pos, value) => { if (garden[pos] == '#' && pos.ManhattanDistance() % 2 == 0) hastagCount++; garden[pos] = '.'; });
	HashSet<Vec2i> uniqueStoppingPoints = new();

	foreach (var kv in costs)
	{
		if (kv.Value % 2 != pCost%2) continue;
		uniqueStoppingPoints.Add(kv.Key);
		garden[kv.Key] = 'O';
    }

	return uniqueStoppingPoints.Count;
}

Console.WriteLine("Part 1: " + GetMaxOptionsAtCost(center, 9));

for (int i = 0; i < 50;i++)
{
    Console.WriteLine(i + " " + GetMaxOptionsAtCost(center, i));
}

garden.Print();

// ** Part 2: Let's simulate 3 x 3 first to see if there are any clear patterns...
//Console.WriteLine(garden.width);
//Console.WriteLine(garden.height);