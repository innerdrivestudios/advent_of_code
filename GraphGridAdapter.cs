/// <summary>
/// Adapter that converts a <see cref="Grid{T}"/> into a graph structure for pathfinding purposes.
/// Treats certain tile values as walkable and exposes 4-directional movement.
/// TODO: make 4/8 directional movement configurable
/// </summary>
public class GraphGridAdapter : IGraph<Vec2<int>>
{
    private Grid<char> grid;
    private HashSet<char> walkable = new();

    // Cardinal directions: right, left, down, up
    Vec2<int>[] directions = [new(1, 0), new(-1, 0), new(0, 1), new(0, -1)];

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphGridAdapter"/> class.
    /// </summary>
    /// <param name="pGrid">The grid to be adapted into a graph.</param>
    /// <param name="pWalkable">The set of characters considered walkable (e.g., '.', ' ', etc.).</param>
    public GraphGridAdapter(Grid<char> pGrid, HashSet<char> pWalkable)
    {
        grid = pGrid;
        walkable = pWalkable;
    }

    /// <inheritdoc />
    public ICollection<Vec2<int>> GetNeighbours(Vec2<int> pNode)
    {
        HashSet<Vec2<int>> neighbours = new();

        foreach (Vec2<int> direction in directions)
        {
            Vec2<int> position = pNode + direction;
            if (grid.IsInside(position) && walkable.Contains(grid[position]))
            {
                neighbours.Add(position);
            }
        }

        return neighbours;
    }
}
