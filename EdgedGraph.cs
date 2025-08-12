// This is a lot like a regular graph, but now with explicit edge cost,
// so we can track edge cost NOT based on distance.


public class EdgedGraph<T> 
{
	//We need two nodes, to get their cost...
	private Dictionary<T, Dictionary<T, long>> adjacencyMatrix;

	public EdgedGraph()
	{
		adjacencyMatrix = new ();
	}

	public void AddNode(T pNode)
	{
		if (!adjacencyMatrix.ContainsKey(pNode)) adjacencyMatrix[pNode] = new ();
	}

    public void RemoveNode(T pNode)
    {
        if (adjacencyMatrix.ContainsKey(pNode))
        {
            adjacencyMatrix.Remove(pNode);
        }

        foreach (var key in adjacencyMatrix.Keys)
        {
            adjacencyMatrix[key].Remove(pNode);
        }
    }

    public void AddEdge(T pFromNode, T pToNode, long pCost, bool pBiDirectional = true)
	{
		//Ensure the from node is there
		AddNode(pFromNode);
		adjacencyMatrix[pFromNode][pToNode] = pCost;

		if (pBiDirectional)
		{
			AddNode(pToNode);
			adjacencyMatrix[pToNode][pFromNode] = pCost;
		}
	}

	public List<T> GetNodes()
	{
		return new List<T>(adjacencyMatrix.Keys);
	}

	public List<T> GetNeighbors(T pNode)
	{
		return new List<T>(adjacencyMatrix[pNode].Keys);
	}

	public long GetEdgeCost(T pNodeA, T pNodeB)
	{
		// Note we don't do any existence checks!!
		return adjacencyMatrix[pNodeA][pNodeB];
	}

}
