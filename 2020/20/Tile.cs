using System.Text.RegularExpressions;

class Tile
{
	public readonly HashSet<int> sideIds = new();
	public readonly int tileID;
	
	public readonly Grid<char> tileData;
	private Regex tileNrParser = new Regex(@"Tile (\d+):");

	public Tile (string pTileData)
	{
		string firstLine = pTileData.Substring(0, pTileData.IndexOf(Environment.NewLine));
		tileID = int.Parse(tileNrParser.Match(firstLine).Groups[1].Value);

		tileData = new Grid<char>(pTileData.Substring(pTileData.IndexOf(Environment.NewLine) + Environment.NewLine.Length), Environment.NewLine);

		GatherSideData();
	}

	private void GatherSideData()
	{
		var left = GatherLeftSideIds();
		var right = GatherRightSideIds();
		var top = GatherTopSideIds();
		var bottom = GatherBottomSideIds();

		sideIds.Add(left.forward);
		sideIds.Add(left.backward);
		sideIds.Add(right.forward);
		sideIds.Add(right.backward);
		sideIds.Add(top.forward);
		sideIds.Add(top.backward);
		sideIds.Add(bottom.forward);
		sideIds.Add(bottom.backward);
	}

	public (int forward, int backward) GatherTopSideIds()
	{
		int forward = 0;
		int backward = 0;

		for (int x = 0; x < tileData.width; x++)
		{
			if (tileData[x, 0] == '#') forward |= 1 << x;
			if (tileData[tileData.width - x - 1, 0] == '#') backward |= 1 << x;
		}

		return (forward, backward);
	}

	public (int forward, int backward) GatherBottomSideIds()
	{
		int forward = 0;
		int backward = 0;

		for (int x = 0; x < tileData.width; x++)
		{
			if (tileData[x, tileData.height-1] == '#') forward |= 1 << x;
			if (tileData[tileData.width - x - 1, tileData.height - 1] == '#') backward |= 1 << x;
		}

		return (forward, backward);
	}

	public (int forward, int backward) GatherLeftSideIds()
	{
		int forward = 0;
		int backward = 0;

		for (int y = 0; y < tileData.height; y++)
		{
			if (tileData[0, y] == '#') forward |= 1 << y;
			if (tileData[0, tileData.height - 1 - y] == '#') backward |= 1 << y;
		}

		return (forward, backward);
	}

	public (int forward, int backward) GatherRightSideIds()
	{
		int forward = 0;
		int backward = 0;

		for (int y = 0; y < tileData.height; y++)
		{
			if (tileData[tileData.width - 1, y] == '#') forward |= 1 << y;
			if (tileData[tileData.width - 1, tileData.height - y - 1] == '#') backward |= 1 << y;
		}

		return (forward, backward);
	}

}
