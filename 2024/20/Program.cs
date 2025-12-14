// Solution for https://adventofcode.com/2024/day/20 (Ctrl+Click in VS to follow link)

using System.Diagnostics;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** First let's parse the input...

string input = File.ReadAllText(args[0]).ReplaceLineEndings();

Vec2i[] directions = [new Vec2i(1, 0), new Vec2i(-1, 0), new Vec2i(0, 1), new Vec2i(0, -1)];

Vec2i startPosition = new Vec2i();
Vec2i endPosition = new Vec2i();

Grid<char> grid = new Grid<char> (input, Environment.NewLine, null, ProcessContent);

char ProcessContent (Vec2i position, string content)
{
	if (content[0] == 'S')
	{
		startPosition = position;
		return '.';
	}
	else if (content[0] == 'E')
	{
		endPosition = position;
		return '.';
	}

	return content[0];
}

// ** Part 1: How many cheats of disabling the walls for 2 steps, would save you at least 100 picoseconds?

// Approach: calculate the whole path, since that is the shortest path without cheats,
// and check which shortcuts we could take...

List<Vec2i> FindPath (Vec2i pStart, Vec2i pEnd)
{
    Queue<Vec2i> priorityQueue = new () {};

	Dictionary<Vec2i, Vec2i> parentMap = new();
	parentMap[startPosition] = startPosition;

	priorityQueue.Enqueue(pStart);

	HashSet<Vec2i> visited = new() { pStart };

	while (priorityQueue.Count > 0)
	{
		Vec2i current = priorityQueue.Dequeue();

		if (current == pEnd) return ReconstructPath(parentMap, current);

		foreach (Vec2i direction in directions)
		{
			Vec2i nextPosition = current + direction;

			if (visited.Contains(nextPosition)) continue;

			if (grid.IsInside(nextPosition) && (grid[nextPosition] == '.'))
			{
				priorityQueue.Enqueue(nextPosition);
				parentMap[nextPosition] = current;
				visited.Add(nextPosition);
			}
		}
	}
	return null;
}

List<Vec2i> ReconstructPath (Dictionary<Vec2i, Vec2i> pParentMap,  Vec2i pEnd)
{
	List<Vec2i> path = new() { pEnd };
	Vec2i iterator = pEnd;

	while (iterator != pParentMap[iterator])
	{
		Vec2i parent = pParentMap[iterator];
		path.Add(parent);
		iterator = parent;
	}

	path.Reverse();

	return path;
}

// Assuming we have the path, how do we get the amount of possible cheats?

long GetCheatCount (List<Vec2i> pPath, int pMaxCheatTime, int pMinimumSavings)
{
	long shortCuts = 0;

	// For each part of the path...
	for (int i = 0; i < pPath.Count - 1; i++)
	{
		// Look ahead in the path, starting at the next node plus the amount of cheats we have,
		// since the minimum amount we want to skip is our cheat time
		for (int j = i + pMaxCheatTime+1; j < pPath.Count; j++)
		{
			// Our main path is the shortest path...
			// BUT IF we would cheat we might be able to take direct route from i to j
			Vec2i delta = pPath[i] - pPath[j];
			int manhattanDistance = Math.Abs(delta.X) + Math.Abs(delta.Y);

			// If that route is shortest or equal to the time we are allowed to cheat, we've found a cheat...
			if (manhattanDistance > pMaxCheatTime) continue;

			// But how much did we save?
			// Well, we skipped j-i steps that is great, but we got manhattanDistance steps in return
			int savings = (j-i) - manhattanDistance;
			shortCuts += (savings >= pMinimumSavings) ? 1 : 0;
		}
	}

	return shortCuts;
}

Stopwatch stopwatch = Stopwatch.StartNew();

List<Vec2i> path = FindPath(startPosition, endPosition);

Console.WriteLine("Part 1: " + GetCheatCount(path, 2, 100));
Console.WriteLine("Part 2: " + GetCheatCount(path, 20, 100));
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " ms");

