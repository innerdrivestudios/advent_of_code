public class Graph<T>
{
    private Dictionary<T, List<T>> adjacencyList;

    public Graph()
    {
        adjacencyList = new Dictionary<T, List<T>>();
    }
    
    public void Clear() 
    { 
        adjacencyList.Clear(); 
    }
    
    public void RemoveNode(T pNode)
    {
        if (adjacencyList.ContainsKey(pNode))
        {
            adjacencyList.Remove(pNode);
        }
        
        foreach (var key in adjacencyList.Keys)
        {
            adjacencyList[key].Remove(pNode);
        }
    }
    
    public List<T> GetNodes()
    {
        return new List<T>(adjacencyList.Keys);
    }
    
    public void AddNode(T pNode)
    {
        if (!adjacencyList.ContainsKey(pNode))
        {
            adjacencyList[pNode] = new List<T>();
        }
    }

    public void RemoveEdge(T pFromNode, T pToNode, bool pBiDirectional = true)
    {
        if (adjacencyList.ContainsKey(pFromNode))
        {
            adjacencyList[pFromNode].Remove(pToNode);
        }
        if (pBiDirectional && adjacencyList.ContainsKey(pToNode))
        {
            adjacencyList[pToNode].Remove(pFromNode);
        }
    }

    public void AddEdge(T pFromNode, T pToNode, bool pBiDirectional = true) { 
        if (!adjacencyList.ContainsKey(pFromNode))
        {
            AddNode(pFromNode);
        }
        if (!adjacencyList.ContainsKey(pToNode)) { 
            AddNode(pToNode);
        } 
        
        adjacencyList[pFromNode].Add(pToNode); 
        if (pBiDirectional) adjacencyList[pToNode].Add(pFromNode); 
    } 
    
    public List<T> GetNeighbors(T pNode) 
    { 
        return new List<T>(adjacencyList[pNode]); 
    }

    public int GetNodeCount()
    {
        return adjacencyList.Count;
    }
    
    public void PrintGraph()
    {
        foreach (var node in adjacencyList)
        {
            Console.WriteLine($"{node.Key}: {string.Join(", ", node.Value)}");
        }
    }
    
    // Breadth-First Search (BFS)
    public HashSet<T> BFS(T pStartNode)
    {
        Queue<T> queue = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();
        queue.Enqueue(pStartNode);
        visited.Add(pStartNode);

        while (queue.Count > 0)
        {
            T current = queue.Dequeue();

            foreach (T connection in GetNeighbors(current))
            {
                if (visited.Contains(connection)) continue;
                queue.Enqueue(connection);
                visited.Add(connection);
            }
        }

        return visited;
    }

}