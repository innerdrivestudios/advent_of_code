using Vec2i = Vec2<int>;

public class DijkstraGridAdapterPart1 : IDijkstraGraphAdapter<Vec2i>
{
    private Grid<int> grid;

    // Cardinal directions: right, left, down, up
    Vec2i [] directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];

    public DijkstraGridAdapterPart1(Grid<int> pGrid)
    {
        grid = pGrid;
    }

    public IDictionary<Vec2i, long> GetNeighborsWithCosts(Vec2i pNode, long pCost)
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
}
