public interface IDijkstraGraphAdapter<T>
{
    IDictionary<T, long> GetNeighborsWithCosts(T pNode, long pCost);
}
