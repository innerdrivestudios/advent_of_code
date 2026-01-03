// Solution for https://adventofcode.com/2023/day/23 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a map with hiking trails...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> hikingtrailMap = new Grid<char>(myInput, Environment.NewLine);

Dictionary<char, Vec2i> directionMap = new()
{
	{'>', new(1,0) },
	{'^', new(0,-1) },
	{'v', new(0,1) },
	{'<', new(-1,0) }
};

Vec2i[] directions = [new(1, 0), new(0, -1), new(0, 1), new(-1, 0)];

Vec2i start = new Vec2i(1, 0);
Vec2i end = new Vec2i(hikingtrailMap.width-2, hikingtrailMap.height-1);

// Basic depth first recursive search...

int GetLongestRoutePart1 (HashSet<Vec2i> pHistory, Vec2i pStart, Vec2i pEnd)
{
	if (pStart == pEnd) return pHistory.Count;
	
	int longest = 0;

	foreach (Vec2i direction in directions)
	{
		Vec2i newPos = pStart + direction;
		if (!hikingtrailMap.IsInside(newPos)) continue;
        if (pHistory.Contains(newPos)) continue;

        char newPosChar = hikingtrailMap[newPos];
		if (newPosChar == '#') continue;
		if (directionMap.ContainsKey(newPosChar) && directionMap[newPosChar] != direction) continue;

		pHistory.Add(newPos);
        int pathLength = GetLongestRoutePart1(pHistory, newPos, pEnd);
        if (pathLength > longest) longest = pathLength;
        pHistory.Remove(newPos);
    }

	return longest;
}

Console.WriteLine("Part 1: " + GetLongestRoutePart1(new(), start, end));

// ** Part 2:
//
// For part 2 I tried a gazillion things:
// - junction switching, reducing the set of junctions as we moved 'down' the path,
//   closing and opening up selective roads from the junctions
// - junction chokepoint detection, trying to limit the amount of recursive options
//
// Nothing worked (at least not within the time that I wanted the method to complete).
// Although I don't have the feeling that the grid based approach is THAT slow, 
// I'll try one last thing... reducing the grid to a graph...

// First I'll gather all 'junctions'
// using a method to count the amount of junctions chars so we can detect them...

int CountSlides(Vec2i pPos)
{
    int count = 0;
    foreach (Vec2i direction in directions)
    {
        Vec2i positionToTest = pPos + direction;
        if (!hikingtrailMap.IsInside(positionToTest)) continue;
        if (directionMap.ContainsKey(hikingtrailMap[positionToTest])) count++;
    }

    return count;
}

// Then a method to find all junctions (nodes)...

List<Vec2i> GetJunctions()
{
    List<Vec2i> junctions = new List<Vec2i>();
    hikingtrailMap.Foreach(
        (pos, value) =>
        {
            if (value == '#' || directionMap.ContainsKey(value)) return;
            if (CountSlides(pos) > 2) junctions.Add(pos);
        }
    );
    return junctions;
}

// Get the junctions and add start and end since those should also be nodes...
List<Vec2i> junctions = GetJunctions();
junctions.Add(start);
junctions.Add(end);

// Now we floodfill the dungeon to create the graph...

EdgedGraph<Vec2i, int> graph = new();

void CreateGraph (Vec2i pStart)
{
    // We queue the current position, the last junction we've seen and the cost since the last junction
    Stack<(Vec2i, Vec2i, int)> queue = new();
    queue.Push((pStart, pStart, 0));

    HashSet<Vec2i> visited = new();
    visited.Add(pStart);
    
    while (queue.Count > 0)
    {
        (Vec2i currentPos, Vec2i lastJunction, int cost) = queue.Pop();

        foreach (Vec2i direction in directions)
        {
            //This was tricky since certain things DO need to happen even though we already visited
            //the new position and some things should not happen ...

            Vec2i newPos = currentPos + direction;

            // If the new pos is not valid at all, skip it
            if (!hikingtrailMap.IsInside(newPos) || hikingtrailMap[newPos] == '#') continue;
            
            // Since we need to make sure two roads that are being floodfilled meet at a junction,
            // we only floodfill 'outward' from the junction (good hint from part 1!)
            char newPosChar = hikingtrailMap[newPos];
            if (directionMap.ContainsKey(newPosChar) && directionMap[newPosChar] != direction) continue;

            // Then we check if the next position IS a junction so we can make the connection, 
            // ignoring whether that node was already visited or not...
            int newCost = cost + 1;

            if (junctions.Contains(newPos) && newPos != lastJunction)
            {
                //Add the edge and set up the values for the rest...
                graph.AddEdge(lastJunction, newPos, newCost, true);
                lastJunction = newPos;
                newCost = 0;
            }

            // IF there is a rest, since we might have already processed this node so we don't 
            // need to create 'outgoing' roads again

            if (visited.Contains(newPos)) continue;
            visited.Add(newPos);

            queue.Push((newPos, lastJunction, newCost));
        }
    }
}

// Fill the actual graph
CreateGraph(start);

// And now define a method similar to what we had for part 1 but then WAY simpler performance wise...
int GetLongestRoutePart2(HashSet<Vec2i> pHistory, Vec2i pStart, Vec2i pEnd, int pLengthSoFar = 0)
{
    if (pStart == pEnd) return pLengthSoFar;

    int longest = 0;

    // Don't waste time searching for other stuff if we can find the end right away
    HashSet<Vec2i> neighbors = graph.GetNeighbors(pStart);
    if (neighbors.Contains(pEnd)) return pLengthSoFar + graph.GetEdgeData(pStart, pEnd);

    foreach (Vec2i neighbor in neighbors)
    {
        if (pHistory.Contains(neighbor)) continue;
        pHistory.Add(neighbor);
        int pathLength = GetLongestRoutePart2(pHistory, neighbor, pEnd, pLengthSoFar + graph.GetEdgeData(pStart, neighbor));
        if (pathLength > longest) longest = pathLength;
        pHistory.Remove(neighbor);
    }

    return longest;
}

Console.WriteLine("Part 2: " + GetLongestRoutePart2(new(), start, end));
