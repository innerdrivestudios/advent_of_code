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

	// Keep in mind this doesn't clone the nodes or edges themselves!! 
	// Only the original graph linking them!
	public EdgedGraph<NodeType, EdgeData> Clone()
	{
		EdgedGraph<NodeType, EdgeData> clone = new ();

		foreach (NodeType node in adjacencyMatrix.Keys)
		{
			clone.adjacencyMatrix[node] = new(adjacencyMatrix[node]);
		}

		return clone;
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

	public HashSet<NodeType> GetNodes()
	{
		return new HashSet<NodeType>(adjacencyMatrix.Keys);
	}

	public HashSet<NodeType> GetNeighbors(NodeType pNode)
	{
		return new HashSet<NodeType>(adjacencyMatrix[pNode].Keys);
	}

	public IEnumerable<NodeType> GetNodesUnsafe ()
	{
		return adjacencyMatrix.Keys;
	}

    public IEnumerable<NodeType> GetNodesUnsafe(NodeType pNode)
    {
        return adjacencyMatrix[pNode].Keys;
    }
	
	public int GetNodesCount ()
	{
		return adjacencyMatrix.Count;
	}

	public int GetNodesCount (NodeType pNode)
	{
		return adjacencyMatrix[pNode].Count;
	}

	public bool HasNodes ()
	{
		return adjacencyMatrix.Count > 0;
	}

	public bool HasNodes (NodeType pNode)
	{
		return adjacencyMatrix.ContainsKey(pNode) && GetNodesCount(pNode) > 0;
	}

    public EdgeData GetEdgeData(NodeType pNodeA, NodeType pNodeB)
	{
		// Note we don't do any existence checks!!
		return adjacencyMatrix[pNodeA][pNodeB];
	}

    public bool HasEdgeData(NodeType pNodeA, NodeType pNodeB)
    {
        // Note we don't do any existence checks!!
        return adjacencyMatrix.ContainsKey(pNodeA) && adjacencyMatrix[pNodeA].ContainsKey(pNodeB);
    }

}
