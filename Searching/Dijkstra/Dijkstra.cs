/// <summary>
/// Provides a generic implementation of Dijkstra's shortest path algorithm.
/// Works with any graph model that supports cost-based neighbor lookup.
/// </summary>
public static class Dijkstra
{
    /// <summary>
    /// Executes Dijkstra's algorithm to find the lowest-cost path from a starting node to an end node.
    /// Also returns the cost to reach all visited nodes from the start.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the node identifiers (e.g., coordinates, strings, etc.).</typeparam>
    /// 
    /// <param name="pDijkstraGraphAdapter">Graph adapter that supplies neighbors and their associated costs.</param>
    /// <param name="pStart">The starting node.</param>
    /// <param name="pEnd">The destination node.</param>
    /// 
    /// <returns>
    /// A <see cref="DijkstraResult{T}"/> containing the shortest path (if found) and the cost to reach each node.
    /// </returns>
    public static DijkstraResult<T> Search<T>(IDijkstraGraphAdapter<T> pDijkstraGraphAdapter, T pStart, T pEnd)
    {
        var queue = new PriorityQueue<T, long>();       // Maps nodes to cost priorities
        var costs = new Dictionary<T, long>();          // Tracks the lowest known cost to each node
        var parents = new Dictionary<T, T>();           // Tracks the parent of each node for path reconstruction

        queue.Enqueue(pStart, 0);
        costs[pStart] = 0;
        parents[pStart] = pStart;                       // Root node points to itself

        while (queue.Count > 0)
        {
            T currentNode = queue.Dequeue();
            if (currentNode.Equals(pEnd)) break;        // End node reached — terminate early

            long currentCost = costs[currentNode];

            foreach (var neighborAndCost in pDijkstraGraphAdapter.GetNeighborsWithCosts(currentNode, currentCost))
            {
                T neighbor = neighborAndCost.Key;
                long newCost = neighborAndCost.Value;

                // Only update if new path is cheaper or node hasn't been visited yet
                if (!costs.ContainsKey(neighbor) || newCost < costs[neighbor])
                {
                    costs[neighbor] = newCost;
                    parents[neighbor] = currentNode;
                    queue.Enqueue(neighbor, newCost);
                }
            }
        }

        var path = ReconstructPath(parents, pEnd);
        return new DijkstraResult<T> { path = path, costs = costs };
    }

    /// <summary>
    /// Reconstructs the path from the start node to the specified end node based on parent relationships.
    /// </summary>
    /// <typeparam name="T">The node type.</typeparam>
    /// <param name="pParentMap">A dictionary mapping each node to its parent node in the search tree.</param>
    /// <param name="pEndNode">The node at the end of the desired path.</param>
    /// <returns>The ordered list of nodes representing the shortest path.</returns>
    private static List<T> ReconstructPath<T>(Dictionary<T, T> pParentMap, T pEndNode)
    {
        var path = new List<T>();
        T current = pEndNode;

        while (!current.Equals(pParentMap[current]))
        {
            path.Add(current);
            current = pParentMap[current];
        }

        // Start node is not included here — optionally uncomment if needed
        // path.Add(current);

        path.Reverse();
        return path;
    }
}

/// <summary>
/// Contains the result of a Dijkstra search operation.
/// Includes the computed shortest path and cost mapping for all reachable nodes.
/// </summary>
/// <typeparam name="T">The type of the nodes in the graph.</typeparam>
public class DijkstraResult<T>
{
    /// <summary>
    /// The list of nodes in the shortest path from start to end.
    /// </summary>
    public List<T> path;

    /// <summary>
    /// A dictionary mapping each visited node to its cumulative cost from the start node.
    /// </summary>
    public Dictionary<T, long> costs;
}
