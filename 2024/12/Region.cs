using Vec2i = Vec2<int>;

class Region
{
	public int area => positions.Count;
	public int perimeter { get; private set; }

	Dictionary<Vec2i, HashSet<Vec2i>> graph = new Dictionary<Vec2i, HashSet<Vec2i>>();
	private HashSet<Vec2i> positions = new();

	public Region (HashSet<Vec2i> pPositions)
	{
		positions = pPositions;
		RunEdgeDetection();
    }

	private void RunEdgeDetection()
	{
		foreach (Vec2i v in positions)
		{
			//build up a grid of points we'll use to walk the outer edge
			ToggleEdge(v + new Vec2i(0, 0), v + new Vec2i(1, 0));
			ToggleEdge(v + new Vec2i(1, 0), v + new Vec2i(1, 1));
			ToggleEdge(v + new Vec2i(1, 1), v + new Vec2i(0, 1));
			ToggleEdge(v + new Vec2i(0, 1), v + new Vec2i(0, 0));
		}
	}

	private void ToggleEdge(Vec2i pNodeA, Vec2i pNodeB)
	{
		// Make sure to delete internal edges (every edge that is visited twice)
		// However edges visited twice are caused by another square which means
		// the order of the edges will be reversed

		if (graph.TryGetValue(pNodeB, out var existingEdges) && existingEdges.Remove(pNodeA))
		{
			perimeter--;
			if (existingEdges.Count == 0) graph.Remove(pNodeB);
		}
		else
		{
			var newEdges = graph.GetValueOrDefault(pNodeA, new());
			newEdges.Add(pNodeB);
			graph[pNodeA] = newEdges;
			perimeter++;
		}
	}


	public int GetOptimizedPerimeterSize()
	{
		int totalPerimeter = 0;

		var graphCopy = new Dictionary<Vec2i, HashSet<Vec2i>> (graph);

		while (graphCopy.Count > 0)
		{
			Vec2i startPosition = graphCopy.First().Key;
			Vec2i currentPosition = startPosition;
			Vec2i lastDirection = new Vec2i(-1000, -1);

			int count = 0;

			do
			{
				HashSet<Vec2i> connections = graphCopy[currentPosition];
				Vec2i nextPoint = connections.First();
				connections.Remove(nextPoint);

				if (connections.Count == 0)
				{
					graphCopy.Remove(currentPosition);
				}

				Vec2i delta = nextPoint - currentPosition;

				if (!delta.Equals(lastDirection))
				{
					count++;
					lastDirection = delta;
				}

				currentPosition = nextPoint;

			} while (!currentPosition.Equals(startPosition));

			//If you don't start in the corner of an edge loop you'll count the same piece twice.
			//Now every area always HAS an even amount of perimeters so if it is uneven, we counted one too many
			if (count % 2 == 1) count--;
			totalPerimeter += count;
		}

		return totalPerimeter;
	}

	public void PrintEdgeMap ()
	{
		foreach (var kv in graph)
		{
            Console.WriteLine(kv.Key);
			foreach (var edge in kv.Value)
			{
				Console.WriteLine("\t" + edge);
			}
        }
	}

}

