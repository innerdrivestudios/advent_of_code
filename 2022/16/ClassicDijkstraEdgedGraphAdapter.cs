class ClassicDijkstraEdgedGraphAdapter<T> : IDijkstraGraphAdapter<T> 
{

    private EdgedGraph<T> edgedGraph;

    public ClassicDijkstraEdgedGraphAdapter(EdgedGraph<T> pEdgedGraph)
    {
        edgedGraph = pEdgedGraph;
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
}
