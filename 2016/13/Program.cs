//Solution for https://adventofcode.com/2016/day/13 (Ctrl+Click in VS to follow link)

using System.Numerics;
using Vec2i = Vec2<int>;

//Your input: a seed used to determine whether an x,y position in a grid is walkable or not

ulong myInput = 1352;

//Your task : find the minimum number of steps to get from 1,1 to 31,39

//Start and end poulong
Vec2i start = new Vec2i(1, 1);
Vec2i end = new Vec2i(31, 39);

//Allowed directions
Vec2i[] directions = [new Vec2i(1, 0), new Vec2i(-1, 0), new Vec2i(0, 1), new Vec2i(0, -1)];

//Part 1 - Assuming start and end are both walkable!
Console.WriteLine(
	"Part 1 - Finding the fewest steps required from " + start + " to " + end + " :" +
	GetFewestStepsCount(start, end, myInput)
);

//Part 2 - Flood fill as long as cost is < 51
Console.WriteLine(
	"Part 2 - Reachable locations from " + start + " in 50 steps :" +
	Saturate (start, myInput, 50)
);

Console.ReadKey();

//Basic Dijkstra/BFS
int GetFewestStepsCount (Vec2i pStart, Vec2i pEnd, ulong pInput)
{
	PriorityQueue<Vec2i, int> todoList = new();
	HashSet<Vec2i> visited = new();

	todoList.Enqueue(pStart,0);
	visited.Add(pStart);

	while (todoList.Count > 0)
	{
		todoList.TryPeek(out _, out int currentCost);
		Vec2i current = todoList.Dequeue();

		if (current == pEnd) return currentCost;

		foreach (Vec2i direction in directions) {
			Vec2i newPosition = current + direction;

			if (newPosition.X < 0 || newPosition.Y < 0) continue;
			if (visited.Contains(newPosition)) continue;

			//Find x*x + 3*x + 2*x*y + y + y*y
			//Add your puzzle input
			//Count the bits
			//even number of 1? walkable is true;

			ulong magicNumber =
				(ulong)(
				newPosition.X * newPosition.X +
				3 * newPosition.X +
				2 * newPosition.X * newPosition.Y +
				newPosition.Y +
				newPosition.Y * newPosition.Y
				);
				
			magicNumber += pInput;

			bool isWalkable = BitOperations.PopCount(magicNumber) % 2 == 0;

            if (isWalkable)
			{
				todoList.Enqueue(newPosition, currentCost+1);
				visited.Add(newPosition);
            }
		}
	}
	return 0;
}

//FloodFill with cost tracking
int Saturate(Vec2i pStart, ulong pInput, int pMaxCost)
{
	PriorityQueue<Vec2i, int> todoList = new();
	HashSet<Vec2i> visited = new();

	todoList.Enqueue(pStart, 0);
	visited.Add(pStart);

	while (todoList.Count > 0)
	{
		todoList.TryPeek(out _, out int currentCost);
		Vec2i current = todoList.Dequeue();

		//from start to next is cost 1 => 1 step, so cost 50 is 50 steps (aka done)
		if (currentCost >= 50) continue;

		foreach (Vec2i direction in directions)
		{
			Vec2i newPosition = current + direction;

			if (newPosition.X < 0 || newPosition.Y < 0) continue;
			if (visited.Contains(newPosition)) continue;

			//Find x*x + 3*x + 2*x*y + y + y*y
			//Add your puzzle input
			//Count the bits
			//even number of 1? walkable is true;

			ulong magicNumber =
				(ulong)(
				newPosition.X * newPosition.X +
				3 * newPosition.X +
				2 * newPosition.X * newPosition.Y +
				newPosition.Y +
				newPosition.Y * newPosition.Y
				);

			magicNumber += pInput;

			bool isWalkable = BitOperations.PopCount(magicNumber) % 2 == 0;

			if (isWalkable)
			{
				todoList.Enqueue(newPosition, currentCost + 1);
				visited.Add(newPosition);
			}
		}
	}

	return visited.Count;
}

