class ClassicDijkstraEdgedGraphAdapter<T> : IDijkstraGraphAdapter<T> 
{
    private EdgedGraph<T> edgedGraph;

    private T start;
    private T end;

    public ClassicDijkstraEdgedGraphAdapter(EdgedGraph<T> pEdgedGraph, T pStart, T pEnd)
    {
        edgedGraph = pEdgedGraph;
        start = pStart;
        end = pEnd;
    }

	public void Initialize(PriorityQueue<T, long> pQueue, Dictionary<T, long> pCosts, Dictionary<T, T> pParents)
	{
		pQueue.Enqueue(start, 0);
		pCosts[start] = 0;
		pParents[start] = start;
	}

	public IDictionary<T, long> GetNeighborsWithCosts(T pNode, long pCost)
    {
        List<T> neighbors = edgedGraph.GetNeighbors(pNode);

        Dictionary<T, long> costs = new ();

        foreach (T neighbor in neighbors)
        {
            costs[neighbor] = pCost + long.CreateChecked(edgedGraph.GetEdgeCost(pNode, neighbor));
        }

        return costs;
    }

	public bool IsDone(T pCurrentNode)
	{
		return end.Equals(pCurrentNode);
	}
}
