// Solution for https://adventofcode.com/2016/day/22 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid of nodes that can hold a certain amount of data.
// - we have direct access only to node 0,0
// - we can instruct another node to MOVE ALL of its data to a neighbour node (4 directions max) if the neighbour has enough space

// ** First parse the input...
string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
string[] nodeDescriptions = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// ** Get the max coordinate assuming correct input...
Regex partialNodeParser = new Regex("/dev/grid/node-x(\\d+)-y(\\d+)");

(int x, int y) maxCoord = (0, 0);

{
    Match result = partialNodeParser.Match(nodeDescriptions.Last());
    maxCoord.x = int.Parse(result.Groups[1].Value);
    maxCoord.y = int.Parse(result.Groups[2].Value);
}

// ** Now that we know the max coordinates, construct the whole grid and a list of all nodes...

Node[,] nodeGrid = new Node[maxCoord.x + 1, maxCoord.y + 1];
List<Node> allNodes = new List<Node>();
Regex fullNodeParser = new Regex("/dev/grid/node-x(\\d+)-y(\\d+)\\s+(\\d+)T\\s+(\\d+)T\\s+(\\d+)T\\s+(\\d+)%");

foreach (string nodeDescription in nodeDescriptions)
{
    Match result = fullNodeParser.Match(nodeDescription);
    if (result.Success)
    {
        int x = int.Parse(result.Groups[1].Value);
        int y = int.Parse(result.Groups[2].Value);
        int size = int.Parse(result.Groups[3].Value);
        int used = int.Parse(result.Groups[4].Value);
        int avail = int.Parse(result.Groups[5].Value);
        //avail is not used!
        Node node = new Node(size, used, new Vec2i(x,y));
        nodeGrid[x, y] = node;
        allNodes.Add(node);
    }
}

// ** Part 1: 

int viableNodeCount = 0;

for (int k = 0; k < allNodes.Count; k++)
{
    for (int l = 0; l < allNodes.Count; l++)
    {
        if (k != l)
        {
            Node a = allNodes[k];
            Node b = allNodes[l];

            if (a.used > 0 && a.used <= b.avail)
            {
                viableNodeCount++;
            }
        }
    }
}

Console.WriteLine("Part 1 - Viable node count:" + viableNodeCount);

void PrintGrid ()
{
    Console.WriteLine();
    for (int m = 0; m < nodeGrid.GetLength(1); m++)
    {
        for (int n = 0; n < nodeGrid.GetLength(0); n++)
        {
            Console.Write(nodeGrid[n, m]?.ToString() + "\t");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

// ** Part 2: How many moves are required to move the data in the top right node to the top left node?

// Main thing is that we want to be able to move all data from a node to an adjacent node.
// In order to be able to do that the node needs to be empty and big enough OR we need to be able to make it empty.
// In other words we need to find a "path" from our source node to the node where we want to go, 
// which might involve moving a bunch of other nodes.
// And on top of that we want the shortest path...

// My first attempt (later removed) was to treat this as a recursive-we-need-to-move-the-box puzzle,
// this worked great for the small example, but was infeasible for the larger data sets due to the amount
// of paths that would have to be evaluated.

// Reading part 1 again and realizing part 1 often holds a hint for solving part 2, I changed my approach,
// when we need to clear a space, I look for all viable nodes that could hold the data for that space,
// then do a BFS from all of those spaces to the node we need to clear and select the shortest path.

// So, let's start, we need to move all the content from our top left node to top right node:
Vec2i currentNodePosition = new Vec2i(maxCoord.x, 0);
Vec2i targetNodePosition = new Vec2i(0, 0);

bool failed = false;
long movedExecuted = 0;

while (currentNodePosition != targetNodePosition)
{
    Vec2i nextNodePosition = currentNodePosition + new Vec2i(-1, 0);

    Node currentNode = nodeGrid[currentNodePosition.X, currentNodePosition.Y];  
    Node nextNode = nodeGrid[nextNodePosition.X, nextNodePosition.Y];

    //Console.WriteLine("Moving "+currentNodePosition + " to " + nextNodePosition);

    //We will try and move ALL the data...
    if (nextNode.used == 0)
    {
        //Console.WriteLine("Fits...");
        nextNode.used = currentNode.used;
        currentNode.used = 0;
        movedExecuted++;
    }
    else
    {
        //Console.WriteLine("Target blocked, trying to clear " + nextNodePosition);
        List<Vec2i> path = FindShortestPathToClear(nextNodePosition, currentNodePosition);
        
        if (path == null)
        {
            //Console.WriteLine("Failed...");

            failed = true;
            break;
        }
        else
        {
            //Console.WriteLine("Moving " + string.Join(" ", path));

            // The path describes the route from source to the node that could hold our data
            // So we need to move the contents starting at the end of the path

            for (int i = path.Count - 1; i > 0; i--)
            {
                Node target = nodeGrid[path[i].X, path[i].Y];
                Node source = nodeGrid[path[i - 1].X, path[i - 1].Y];

                target.used += source.used;
                source.used = 0;
                movedExecuted++;
            }

            // Console.WriteLine("Moved content to cleared node...");
            nextNode.used = currentNode.used;
            currentNode.used = 0;
            movedExecuted++;
        }
    }

    currentNodePosition = nextNodePosition;

   // PrintGrid();
   // Console.ReadKey();
}

List<Vec2i> FindShortestPathToClear (Vec2i pNodeToClear, Vec2i pBlocked)
{
    List<Node> nodes = GetViableNodes(nodeGrid[pNodeToClear.X, pNodeToClear.Y]);

    List<Vec2i> shortestPath = null;

    foreach (Node node in nodes)
    {
        List<Vec2i> path = RunBFS(node.position, pNodeToClear, pBlocked, shortestPath == null? int.MaxValue : shortestPath.Count);
        if (path != null && (shortestPath == null || shortestPath.Count > path.Count)) shortestPath = path;
    }

    return shortestPath;
}

List<Node> GetViableNodes(Node pToMove)
{
    return allNodes.Where(n => n != pToMove && n.avail >= pToMove.used).ToList();
}

// Search from possible candidate to the node we want to move
List<Vec2i> RunBFS (Vec2i pSource, Vec2i pTarget, Vec2i pBlocked, int pMaxPathLength)
{
    Vec2i[] directions = [new (-1,0), new (0,-1), new (1,0), new (0,1)];

    Queue<Vec2i> todoList = new();
    Dictionary<Vec2i, int> costs = new();
    Dictionary<Vec2i, Vec2i> parents = new();

    todoList.Enqueue(pSource);
    costs[pSource] = 0;

    while (todoList.Count > 0)
    {
        Vec2i currentPosition = todoList.Dequeue();

        if (currentPosition == pTarget)
        {
            //Console.WriteLine("Path found!");

            Vec2i iterator = pTarget;
            List<Vec2i> path = [iterator];
            while (parents.ContainsKey(iterator))
            {
                iterator = parents[iterator];
                path.Add(iterator);
            }
            return path;
        }        
        
        int currentCost = costs[currentPosition];

        // If we already found a shorter path...
        if (currentCost > pMaxPathLength) continue;

        foreach (Vec2i direction in directions)
        {
            Vec2i newPosition = currentPosition + direction;

            //if the newPosition is a node we are not allowed to move, skip it
            if (newPosition == pBlocked) continue;

            //we need to stay inside the grid
            if (newPosition.X < 0 || newPosition.Y < 0 || newPosition.X > maxCoord.x || newPosition.Y > maxCoord.y) continue;

            //don't go back to where we came from
            if (costs.ContainsKey(newPosition)) continue;

            Node currentNode = nodeGrid[currentPosition.X, currentPosition.Y];
            Node nextNode = nodeGrid[newPosition.X, newPosition.Y];

            //if target node can hold the data, we are done, return the path to clear the given node
            //(Assume the node is cleared before so we can use the whole size)
            if (currentNode.size >= nextNode.used)
            {
                costs[newPosition] = currentCost + 1;
                todoList.Enqueue(newPosition);
                parents[newPosition] = currentPosition;
            }
        }

    }

    return null;
}


Console.WriteLine();

Console.WriteLine("Part 2: " + movedExecuted);

