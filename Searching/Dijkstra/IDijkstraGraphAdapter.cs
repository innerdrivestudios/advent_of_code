public interface IDijkstraGraphAdapter<T> where T : notnull
{
    IDictionary<T, long> GetNeighborsWithCosts(T pNode, long pCost);
	void Initialize(PriorityQueue<T, long> pQueue, Dictionary<T, long> pCosts, Dictionary<T, T> pParents);
	bool IsDone(T pCurrentNode);
}
