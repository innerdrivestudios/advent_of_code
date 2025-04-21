using Vec2i = Vec2<int>;

class HeightmapGridGraphAdapter : IGraph<Vec2i>
{
	private Grid<int> heightmap;

	public HeightmapGridGraphAdapter(Grid<int> pHeightMap)
	{
		heightmap = pHeightMap;
	}

	public ICollection<Vec2i> GetNeighbours(Vec2i pNode)
	{
		List<Vec2i> neighbours = new();

		int minX = int.Max(0, pNode.X - 1);
		int maxX = int.Min(heightmap.width-1, pNode.X + 1);

		int minY = int.Max(0, pNode.Y - 1);
		int maxY = int.Min(heightmap.height - 1, pNode.Y + 1);

		int currentNodeValue = heightmap[pNode];

        for (int x = minX; x <= maxX; x++)
		{
			for (int y = minY; y <= maxY; y++)
			{
				//skip center and the diagonals
				if (x == pNode.X ^ y == pNode.Y)
				{
                    if (heightmap[x, y] - currentNodeValue <= 1)
					{
						neighbours.Add(new Vec2i(x, y));
					}
				}
			}
		}

		return neighbours;
	}
}

