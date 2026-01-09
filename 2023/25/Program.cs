// Solution for https://adventofcode.com/2023/day/25 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a graphClone

string[] myInput = File.ReadAllLines(args[0]);

// This is a slow implementation of karger's algorithm.
// My initial approach was trying to select 3 edges to try and split the graphClone.
// Chances that you pick the 3 correct edges are so slim that it will run forever.
// On the up side, chances that you pick the wrong edges are very big.
// Karger's algorithm uses that principle to random pick edges and merge the nodes linking 
// them. 

// I didn't read the whole thing, there is something with Disjoint Union Sets etc that I still
// need to dive into. For the time being, here is a lazy man's approach to solving it...

EdgedGraph<Node, long> graph = new();
Dictionary<string, Node> nodeMap = new();

Node GetNode(string pNodeId)
{
    if (!nodeMap.ContainsKey(pNodeId)) nodeMap[pNodeId] = new Node();
    return nodeMap[pNodeId];
}

foreach (string line in myInput)
{
    string[] input = line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    for (int i = 1; i < input.Length; i++)
    {
        graph.AddEdge(GetNode(input[0]), GetNode(input[i]), 1, true);
    }
}

while (true)
{
    // Clone the original graph
    EdgedGraph<Node, long> graphClone = graph.Clone();

    Random random = new Random();

    void CollapseRandomEdge()
    {
        //first get a random node...
        List<Node> nodes = graphClone.GetNodesUnsafe().ToList();
        Node randomNode = nodes[random.Next(0, nodes.Count)];

        // Now get a random other node from this node...
        List<Node> connectedNodes = graphClone.GetNodesUnsafe(randomNode).ToList();
        Node randomOtherNode = connectedNodes[random.Next(0, connectedNodes.Count)];

        // Then calculate the total node value (this counts how many nodes this collapsed node represents)...
        int totalNodeValue = randomNode.count + randomOtherNode.count;
        Node newNode = new Node();
        newNode.count = totalNodeValue;
        // And add it to the graph...
        graphClone.AddNode(newNode);

        // Now transfer all edges and their cost from the original 2 nodes, to this new node...
        //
        // First copy all outgoing edges from randomNode to the newNode (except the edge to the randomOtherNode)
        foreach (Node node in connectedNodes)
        {
            if (node == randomOtherNode) continue;
            graphClone.AddEdge(newNode, node, graphClone.GetEdgeData(randomNode, node));
        }

        // Then copy all outgoing edges from randomOtherNode to the newNode (except the edge to the randomNode)
        HashSet<Node> randomConnectedNodes = graphClone.GetNeighbors(randomOtherNode);
        foreach (Node node in randomConnectedNodes)
        {
            if (node == randomNode) continue;

            long value = graphClone.GetEdgeData(randomOtherNode, node);

            if (graphClone.HasEdgeData(newNode, node))
            {
                graphClone.AddEdge(newNode, node, graphClone.GetEdgeData(newNode, node) + value);
            }
            else
            {
                graphClone.AddEdge(newNode, node, value);
            }
        }

        graphClone.RemoveNode(randomNode);
        graphClone.RemoveNode(randomOtherNode);
    }

    while (graphClone.GetNodesCount() > 2)
    {
        CollapseRandomEdge();
    }

    List<Node> remainingNodes = graphClone.GetNodesUnsafe().ToList();

    //Console.WriteLine("Get edge data:" + graphClone.GetEdgeData(remainingNodes[0], remainingNodes[1]));
    if (graphClone.GetEdgeData(remainingNodes[0], remainingNodes[1]) == 3)
    {
        Console.WriteLine("Part 1:" + remainingNodes[0].count * remainingNodes[1].count);
        break;
    }
}