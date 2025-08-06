using Vec2i = Vec2<int>;

public class DijkstraGridAdapterPart2 : DijkstraGridAdapterPart1
{

	public DijkstraGridAdapterPart2(Grid<int> pGrid, Vec2i pStart, Vec2i pEnd) : base(pGrid, pStart, pEnd)
	{
	}

	public override IDictionary<Vec2i, long> GetNeighborsWithCosts(Vec2i pNode, long pCost)
    {
        Dictionary<Vec2i, long> neighbors = new();

        foreach (Vec2i direction in directions)
        {
            Vec2i newPosition = pNode + direction;

            //to detect if this position is within the grid, we can't use grid.IsInside anymore ...
            //we'll need to define our own new IsInside ...
            int gridWidth = grid.width;
            int gridHeight = grid.height;

            int xBlock = newPosition.X / gridWidth;
            int yBlock = newPosition.Y / gridHeight;

            //Values need to be positive, but in the first 5 blocks (block index 0,1,2,3,4)
            if (newPosition.X >= 0 && newPosition.Y >= 0 && xBlock < 5 && yBlock < 5)
            {
                //Same cost math as in part 2 alternative 1
                neighbors[newPosition] = pCost + (1 + (grid[newPosition.X % grid.width, newPosition.Y % grid.height] + xBlock + yBlock - 1) % 9);
            }

        }

        return neighbors;
    }

}
