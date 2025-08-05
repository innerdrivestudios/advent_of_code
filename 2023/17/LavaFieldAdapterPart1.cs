using Vec2i = Vec2<int>;
using PathNode = (Vec2<int> position, Vec2<int> direction, int stepsInThisDirection);

class LavaFieldAdapterPart1 : IDijkstraGraphAdapter<PathNode>	
{
	Directions<Vec2i> directions = new([new(1, 0), new(0, 1), new(-1, 0), new(0, -1)]);

	//left, straight on, right
	int[] allowedDirections = [-1, 0, 1];

	private Grid<int> lavaField;
	private PathNode[] possibleStarts;
	private Vec2i destination;

	public LavaFieldAdapterPart1(Grid<int> pLavaField, PathNode[] pPossibleStarts, Vec2i pDestination)
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

			//steps reset when we turn (and we turn at i == 1)
			node.stepsInThisDirection = (i != 1) ? 1 : (node.stepsInThisDirection+1);

			//0, 1, 2,... whoops, we can't move more than 3 steps in any given direction
			if (node.stepsInThisDirection > 3) continue;

			//Update the actual direction
			node.direction = directions.Get(directions.directions.IndexOf(pNode.direction) + allowedDirections[i]);
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
		return pNodeA.position == destination;
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

