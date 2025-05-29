/// <summary>
/// Represents a generic graph abstraction that exposes neighbor relationships for nodes.
/// </summary>
/// <typeparam name="T">The type of node in the graph.</typeparam>
interface IGraph<T>
{
    /// <summary>
    /// Gets all directly connected neighbors of a node.
    /// </summary>
    /// 
    /// <param name="pNode">The node whose neighbors should be retrieved.</param>
    /// 
    /// <returns>A collection of neighboring nodes.</returns>
    ICollection<T> GetNeighbours(T pNode);
}
