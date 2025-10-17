/// <summary>
/// Provides utility methods for graph traversal, including BFS, Dijkstra, and flow map generation.
/// </summary>
static class SearchUtils
{
    /// <summary>
    /// Performs a breadth-first search on a graph to find a path from a start node to an end node.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of nodes in the graph.</typeparam>
    /// 
    /// <param name="pGraph">The graph to search.</param>
    /// <param name="pStart">The starting node.</param>
    /// <param name="pEnd">The destination node.</param>
    /// 
    /// <returns>A list representing the shortest path from start+1 to end, or null if no path exists.</returns>
    public static List<T> BFSSearch<T>(IGraph<T> pGraph, T pStart, T pEnd)
    {
        Queue<T> todo = new();
        Dictionary<T, T> parentMap = new();

        todo.Enqueue(pStart);
        parentMap[pStart] = pStart;

        while (todo.Count > 0)
        {
            T current = todo.Dequeue();

            if (current.Equals(pEnd)) break;

            foreach (T neighbour in pGraph.GetNeighbours(current))
            {
                if (parentMap.ContainsKey(neighbour)) continue;

                parentMap[neighbour] = current;
                todo.Enqueue(neighbour);
            }
        }

        return GetPath(parentMap, pEnd);
    }

    /// <summary>
    /// Performs Dijkstra’s algorithm to find the shortest path between two nodes.
    /// Assumes all edges have equal weight (1).
    /// </summary>
    /// 
    /// <typeparam name="T">The type of nodes in the graph.</typeparam>
    /// 
    /// <param name="pGraph">The graph to search.</param>
    /// <param name="pStart">The starting node.</param>
    /// <param name="pEnd">The destination node.</param>
    /// 
    /// <returns>The shortest path from start to end, or null if no path exists.</returns>
    /// 
    /// TODO: this needs to be updated, coded like this, it's is a BFS search...
    public static List<T> DijkstraSearch<T>(IGraph<T> pGraph, T pStart, T pEnd)
    {
        PriorityQueue<T, long> todo = new();
        Dictionary<T, long> costMap = new();
        Dictionary<T, T> parentMap = new();

        todo.Enqueue(pStart, 0);
        costMap[pStart] = 0;
        parentMap[pStart] = pStart;

        while (todo.Count > 0)
        {
            T current = todo.Dequeue();

            if (current.Equals(pEnd)) break;

            var neighbours = pGraph.GetNeighbours(current);
            long currentCost = costMap[current];

            foreach (T neighbour in neighbours)
            {
                if (costMap.ContainsKey(neighbour)) continue;

                long newCost = currentCost + 1;
                todo.Enqueue(neighbour, newCost);
                costMap[neighbour] = newCost;
                parentMap[neighbour] = current;
            }
        }

        return GetPath(parentMap, pEnd);
    }

    /// <summary>
    /// Reconstructs a path from a parent map built during search.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of nodes in the graph.</typeparam>
    /// 
    /// <param name="pParentMap">A dictionary mapping each node to its parent in the search path.</param>
    /// <param name="pEndNode">The end node whose path should be reconstructed.</param>
    /// 
    /// <returns>A list of nodes representing the path from the start node to the end node (INCLUDING THE START NODE).</returns>
    public static List<T> GetPath<T>(Dictionary<T, T> pParentMap, T pEndNode)
    {
        List<T> result = new();

        T current = pEndNode;

        while (!current.Equals(pParentMap[current]))
        {
            result.Add(current);
            current = pParentMap[current];
        }

        result.Add(current);
        result.Reverse();
        return result;
    }

    /// <summary>
    /// Generates a flow map (flood fill) from a given starting node.
    /// Each node in the result is associated with the number of steps required to reach it.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of nodes in the graph.</typeparam>
    /// 
    /// <param name="pGraph">The graph to flood-fill.</param>
    /// <param name="pStart">The starting node for the fill.</param>
    /// 
    /// <returns>A dictionary mapping nodes to their distance (in steps) from the start node.</returns>
    /// 
    /// TODO: Take the ACTUAL distance into account and not just 1, this only works for H/Z movements of 1!
    public static Dictionary<T, long> FlowMap<T>(IGraph<T> pGraph, T pStart)
    {
        PriorityQueue<T, long> todo = new();
        Dictionary<T, long> costMap = new();

        todo.Enqueue(pStart, 0);
        costMap[pStart] = 0;

        while (todo.Count > 0)
        {
            T current = todo.Dequeue();

            var neighbours = pGraph.GetNeighbours(current);
            long currentCost = costMap[current];

            foreach (T neighbour in neighbours)
            {
                if (costMap.ContainsKey(neighbour)) continue;

                long newCost = currentCost + 1;
                todo.Enqueue(neighbour, newCost);
                costMap[neighbour] = newCost;
            }
        }

        return costMap;
    }
}
