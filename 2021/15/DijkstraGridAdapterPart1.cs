using Vec2i = Vec2<int>;

public class DijkstraGridAdapterPart1 : IDijkstraGraphAdapter<Vec2i>
{
    protected Grid<int> grid;
    protected Vec2i start;
    protected Vec2i end;

    // Cardinal directions: right, left, down, up
    protected Vec2i [] directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];

    public DijkstraGridAdapterPart1(Grid<int> pGrid, Vec2i pStart, Vec2i pEnd)
    {
        grid = pGrid;
        start = pStart;
        end = pEnd;
    }

	public virtual void Initialize(PriorityQueue<Vec2i, long> pQueue, Dictionary<Vec2i, long> pCosts, Dictionary<Vec2i, Vec2i> pParents)
	{
        pQueue.Enqueue(start, 0);
        pCosts[start] = 0;
        pParents[start] = start;
	}

	public virtual IDictionary<Vec2i, long> GetNeighborsWithCosts(Vec2i pNode, long pCost)
    {
        Dictionary<Vec2i, long> neighbors = new();

        foreach (Vec2i direction in directions)
        {
            Vec2i newPosition = pNode + direction;
            if (grid.IsInside(newPosition))
            {
                //In this puzzle, the value indicates the cost
                neighbors[newPosition] = pCost + grid[newPosition];
            }

        }

        return neighbors;
    }


	public virtual bool IsDone(Vec2i pCurrentNode)
	{
        return pCurrentNode == end;
	}
}
