static class SearchUtils
{
	public static List<T> BFSSearch<T> (IGraph<T> pGraph, T pStart, T pEnd)
	{
		Queue<T> todo = new();
		HashSet<T> visited = new HashSet<T>();
		Dictionary<T, T> parentMap = new Dictionary<T, T>();

		todo.Enqueue(pStart);
		visited.Add(pStart);

		while (todo.Count > 0)
		{
			T current = todo.Dequeue();
			if (current.Equals(pEnd)) return GetPath(parentMap, current);

			var neighbours = pGraph.GetNeighbours(current);
			
			foreach (T neighbour in neighbours)
			{
				if (visited.Contains(neighbour)) continue;
				todo.Enqueue(neighbour);
				visited.Add(neighbour);
				parentMap[neighbour] = current;
			}
		}

		return null;
	}

	public static List<T> DijkstraSearch<T>(IGraph<T> pGraph, T pStart, T pEnd)
	{
		PriorityQueue<T, long> todo = new();
		Dictionary<T, T> parentMap = new Dictionary<T, T>();
		Dictionary<T, long> costMap = new Dictionary<T, long>();

		todo.Enqueue(pStart, 0);
		costMap[pStart] = 0;

		while (todo.Count > 0)
		{
			T current = todo.Dequeue();
			if (current.Equals(pEnd)) return GetPath(parentMap, current);

			var neighbours = pGraph.GetNeighbours(current);
			long currentCost = costMap[current];

			foreach (T neighbour in neighbours)
			{
				if (costMap.ContainsKey(neighbour)) continue;

				long newCost = currentCost + 1;
				todo.Enqueue(neighbour, newCost);
				parentMap[neighbour] = current;
				costMap[neighbour] = newCost;
			}
		}

		return null;
	}

	public static List<T> GetPath<T>(Dictionary<T, T> pParentMap, T pEndNode)
	{
		List<T> path = new();

		while (pParentMap.TryGetValue(pEndNode, out T parentNode))
		{
			path.Add(pEndNode);
			pEndNode = parentNode;
		}

		path.Reverse();

		return path;
	}

}