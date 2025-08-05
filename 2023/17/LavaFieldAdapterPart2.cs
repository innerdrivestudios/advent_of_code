using Vec2i = Vec2<int>;
using PathNode = (Vec2<int> position, Vec2<int> direction, int stepsInThisDirection);

class LavaFieldAdapterPart2 : IDijkstraGraphAdapter<PathNode>	
{
	Directions<Vec2i> directions = new([new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]);

	//left, middle, right
	int[] allowedDirections = [-1, 0, 1];

	private Grid<int> lavaField;
	private PathNode[] possibleStarts;
	private Vec2i destination;

	public LavaFieldAdapterPart2(Grid<int> pLavaField, PathNode[] pPossibleStarts, Vec2i pDestination)
	{
		lavaField = pLavaField;
		possibleStarts = pPossibleStarts;
		destination = pDestination;
	}

	public IDictionary<PathNode, long> GetNeighborsWithCosts(PathNode pNode, long pCost)
	{
		Dictionary<PathNode, long> result = new();

		for (int i = 0; i < allowedDirections.Length; i++)
		{
			PathNode node = pNode;

			//Once a crucible starts moving in a direction it needs to move a minimum of
			//four blocks in that direction before it can turn (or even before it can stop at the end)
			//so if it is currently (before moving) at 1, 2, 3 we can't turn yet,
			//but if it is 4 (or higher) we can
			if (node.stepsInThisDirection < 4 && i != 1) continue;

			//an ultra crucible can move a maximum of ten consecutive blocks without turning
			//if we've moved more than 9 (so 10 or higher), already, we'll make step 11 this round
			//so we can't move straight anymore
			if (node.stepsInThisDirection > 9 && i == 1) continue;

			//steps reset when we turn (and we turn at i == 1)
			node.stepsInThisDirection = (i != 1) ? 1 : (node.stepsInThisDirection + 1);

			//Update the actual direction
			node.direction = directions.Get(directions.directions.IndexOf(node.direction) + allowedDirections[i]);
			node.position += node.direction;

			if (lavaField.IsInside(node.position))
			{
				result[node] = pCost + lavaField[node.position];
			}
		}

		return result;
	}

	public bool IsDone(PathNode pNodeA)
	{
		return pNodeA.position == destination && pNodeA.stepsInThisDirection >= 4;
	}

	public void Initialize(PriorityQueue<PathNode, long> pQueue, Dictionary<PathNode, long> pCosts, Dictionary<PathNode, PathNode> pParents)
	{
		foreach (var s in possibleStarts)
		{
			pQueue.Enqueue(s, 0);
			pCosts[s] = 0;
			pParents[s] = s;
		}
	}

}

