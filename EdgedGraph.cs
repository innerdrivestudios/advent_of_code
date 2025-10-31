// This is a lot like a regular graph, but now with explicit edge cost,
// so we can track edge cost NOT based on distance.


public class EdgedGraph<NodeType, EdgeData> 
{
	//We need two nodes, to get their cost...
	private Dictionary<NodeType, Dictionary<NodeType, EdgeData>> adjacencyMatrix;

	public EdgedGraph()
	{
		adjacencyMatrix = new ();
	}

	public void AddNode(NodeType pNode)
	{
		if (!adjacencyMatrix.ContainsKey(pNode)) adjacencyMatrix[pNode] = new ();
	}

    public void RemoveNode(NodeType pNode)
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

    public void AddEdge(NodeType pFromNode, NodeType pToNode, EdgeData pEdgeData, bool pBiDirectional = true)
	{
		//Ensure the from node is there
		AddNode(pFromNode);
		adjacencyMatrix[pFromNode][pToNode] = pEdgeData;

		if (pBiDirectional)
		{
			AddNode(pToNode);
			adjacencyMatrix[pToNode][pFromNode] = pEdgeData;
		}
	}

	public List<NodeType> GetNodes()
	{
		return new List<NodeType>(adjacencyMatrix.Keys);
	}

	public List<NodeType> GetNeighbors(NodeType pNode)
	{
		return new List<NodeType>(adjacencyMatrix[pNode].Keys);
	}

	public EdgeData GetEdgeCost(NodeType pNodeA, NodeType pNodeB)
	{
		// Note we don't do any existence checks!!
		return adjacencyMatrix[pNodeA][pNodeB];
	}

}
