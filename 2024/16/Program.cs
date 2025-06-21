//Solution for https://adventofcode.com/2024/day/16 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a maze for the Reindeer olympics, with start and end points for the reindeer

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// ** Parse our input...
Vec2i start = new Vec2i();
Vec2i end = new Vec2i();

Grid<char> grid = new Grid<char>(myInput, "\r\n", null, ProcessContent);

char ProcessContent(Vec2i pPosition, string pContent)
{
    if (pContent[0] == 'S')
    {
        start = pPosition;
        return '.';
    }

    if (pContent[0] == 'E')
    {
        end = pPosition;
        return '.';
    }

    return pContent[0];
}

// Part 1: Given that moving forward costs 1 and rotating costs a 1000, what is the lowest score to the final path?
// Note that this was written before writing the Dijkstra helper classes and it was easier just to keep it this way,
// while refactoring...

// Set up al
Vec2i[] directions = { new Vec2i(1, 0), new Vec2i(0, 1), new Vec2i(-1, 0), new Vec2i(0, -1) };

int[] steeringOptions = { -1, 0, 1 };
// The cost of moving and rotating !
int[] steeringCosts = { 1001, 1, 1001 };

int GetDirectionIndex(int pDirectionIndex)
{
    return ((pDirectionIndex % directions.Length) + directions.Length) % directions.Length;
}

// Run basic dijkstra:
Dictionary<Vec2i, int> visited = new ();
PriorityQueue<Node, int> todoList = new PriorityQueue<Node, int>();

// Start facing east...
Node startNode = new Node() { cost = 0, position = start, directionIndex = 0, parent = null };
todoList.Enqueue(startNode, startNode.cost);

// For part 2, we need ALL final nodes...
List<Node> finalNodes = new();

while (todoList.Count > 0)
{
    Node current = todoList.Dequeue();

    if (current.position.Equals(end))
    {
        if (finalNodes.Count == 0 || current.cost == finalNodes[0].cost) finalNodes.Add(current);
    }
    else
    {
        for (int i = 0; i < steeringOptions.Length; i++)
        {
            int newDirectionIndex = GetDirectionIndex(current.directionIndex + steeringOptions[i]);
            Vec2i newPosition = current.position + directions[newDirectionIndex];

            if (!grid.IsInside(newPosition) || grid[newPosition] == '#') continue;

            int newCost = current.cost + steeringCosts[i];

            //This one is tricky... routes can be cheaper at point X until rudolf has taken a turn on route A,
            //while route B continues straight, making the costs equal again at the next location
            //so we have to add the leeway of one additional step (max cost 1001)
            if (!visited.ContainsKey(newPosition) || visited[newPosition] + 1001 > newCost)
            {
                Node newNode = new Node() { 
                    cost = newCost, 
                    position = newPosition, 
                    directionIndex = newDirectionIndex, 
                    parent = current 
                };
                todoList.Enqueue(newNode, newCost);

                visited[newPosition] = newCost;
            }

        }
    }
}

int lowestCost = finalNodes[0].cost;

HashSet<Vec2i> uniquePositions = new HashSet<Vec2i>(); 

while (finalNodes.Count > 0 && finalNodes[0].cost == lowestCost)
{
    Node node = finalNodes[0];
    while (node != null)
    {
        uniquePositions.Add(node.position);
        node = node.parent;
    }
    finalNodes.RemoveAt(0);
}

Console.WriteLine("Part 1: " + lowestCost);
Console.WriteLine("Part 2: " + uniquePositions.Count);

