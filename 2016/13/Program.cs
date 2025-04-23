//Solution for https://adventofcode.com/2016/day/13 (Ctrl+Click in VS to follow link)

using System.Numerics;
using Vec2i = Vec2<int>;

// In visual studio you can modify the input used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

// ** Your input: a seed used to determine whether an x,y position in a grid is walkable or not

ulong myInput = ulong.Parse(args[0]);

// ** Your task : find the minimum number of steps to get from grid cell 1,1 to cell 31,39

// Start and end positions

Vec2i start = new Vec2i (1, 1);
Vec2i end = new Vec2i (31, 39);

// Allowed directions

Vec2i[] directions = [new Vec2i(1, 0), new Vec2i(-1, 0), new Vec2i(0, 1), new Vec2i(0, -1)];

// ** Part 1 - Assuming start and end are both walkable!
Console.WriteLine(
	"Part 1 - Finding the fewest steps required from " + start + " to " + end + ": " +
	BFS (start, end, myInput)
);

// ** Part 2 - Flood fill as long as cost is < 51
Console.WriteLine(
	"Part 2 - Reachable locations from " + start + " in 50 steps: " +
	Saturate (start, myInput, 50)
);

//Basic Dijkstra/BFS
int BFS (Vec2i pStart, Vec2i pEnd, ulong pSeed)
{
	Queue<(Vec2i pos, int cost)> todoList = new();
	HashSet<Vec2i> visited = new();

	todoList.Enqueue((pStart,0));
	visited.Add(pStart);

	while (todoList.Count > 0)
	{
		var current = todoList.Dequeue();

		if (current.pos == pEnd) return current.cost;

		foreach (Vec2i direction in directions) {
			Vec2i newPosition = current.pos + direction;

			if (newPosition.X < 0 || newPosition.Y < 0) continue;

            if (visited.Contains(newPosition) || !IsWalkable(pSeed, newPosition)) continue;

            todoList.Enqueue((newPosition, current.cost + 1));
            visited.Add(newPosition);
		}
	}
	return 0;
}

//FloodFill with cost tracking
int Saturate(Vec2i pStart, ulong pSeed, int pMaxCost)
{
    Queue<(Vec2i pos, int cost)> todoList = new();
    HashSet<Vec2i> visited = new();

    todoList.Enqueue((pStart, 0));
    visited.Add(pStart);

	int visitedCount = 0;

    while (todoList.Count > 0)
    {
        var current = todoList.Dequeue();

		if (current.cost <= 50) visitedCount++;
		else break;

        foreach (Vec2i direction in directions)
        {
            Vec2i newPosition = current.pos + direction;

            if (newPosition.X < 0 || newPosition.Y < 0) continue;

            if (visited.Contains(newPosition) || !IsWalkable(pSeed, newPosition)) continue;

            todoList.Enqueue((newPosition, current.cost + 1));
            visited.Add(newPosition);
        }
    }
    return visitedCount;
}

bool IsWalkable (ulong pSeed, Vec2i pPosition)
{
    //Find x*x + 3*x + 2*x*y + y + y*y
    //Add your puzzle input
    //Count the bits
    //even number of 1? walkable is true;

    ulong magicNumber =
        (ulong)(
        pPosition.X * pPosition.X +
        3 * pPosition.X +
        2 * pPosition.X * pPosition.Y +
        pPosition.Y +
        pPosition.Y * pPosition.Y
        );

    magicNumber += pSeed;

    return BitOperations.PopCount(magicNumber) % 2 == 0;
}

