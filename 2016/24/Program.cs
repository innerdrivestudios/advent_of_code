//Solution for https://adventofcode.com/2016/day/24 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a map

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> area = new Grid<char>(myInput, Environment.NewLine);

// Assuming we don't know the amount of locations to visit...
Dictionary<int, Vec2i> locationsToVisit = new();

area.Foreach(
    (pos, value) =>
    {
        if (char.IsDigit(value))
        {
            //Record the locations of this item and overwrite it with . to mark it as walkable
            locationsToVisit[value - '0'] = pos;
            area[pos] = '.';
        }
    }
);

// ** Part 1: Now that we have all locations, we can start pathfinding between all different options (yes going to try and brute force first :)).

// This stores the length of the route from index i to j (and backwards)
Dictionary<(int, int), int> pathLengths = new();

GraphGridAdapter gga = new GraphGridAdapter(area, ['.']);

for (int i = 0; i < locationsToVisit.Count-1; i++)
{
    for (int j = i + 1; j < locationsToVisit.Count; j++)
    {
        int pathLength = SearchUtils.BFSSearch(gga, locationsToVisit[i], locationsToVisit[j]).Count;
        //-1 to exclude the start node
        pathLengths[(i, j)] = pathLengths[(j, i)] =  pathLength-1;
    }
}

// Now we get a list of all options:
List<int> possibleOptions = locationsToVisit.Keys.ToList();

// We remove zero, since the start is fixed
possibleOptions.Remove(0);

// Get all permutations:
List<List<int>> possibleRoutes = possibleOptions.GetPermutations();

// And then we search for the shortest route

long GetShortestRoute (Func<List<int>, long> pEvaluator)
{
    long shortestRoute = long.MaxValue;

    foreach (var possibleRoute in possibleRoutes)
    {
        long routeLength = pEvaluator(possibleRoute);
        shortestRoute = long.Min(routeLength, shortestRoute);
    }

    return shortestRoute;
}

long CalculateRouteLengthPart1 (List<int> pPossibleRoute)
{
    //Get length from option 0 to the first option of the route
    long routeLength = pathLengths[(0, pPossibleRoute[0])];

    for (int i = 0; i < pPossibleRoute.Count-1;i++)
    {
        routeLength += pathLengths[(pPossibleRoute[i], pPossibleRoute[i+1])];
    }

    return routeLength;
}

Console.WriteLine("Part 1:" + GetShortestRoute(CalculateRouteLengthPart1));

// ** Part 2: What is the fewest number of steps required to start at 0,
// visit every non-0 number marked on the map at least once, and then return to 0?

long CalculateRouteLengthPart2(List<int> pPossibleRoute)
{
    //Get length from option 0 to the first option of the route
    long routeLength = pathLengths[(0, pPossibleRoute[0])];

    for (int i = 0; i < pPossibleRoute.Count - 1; i++)
    {
        routeLength += pathLengths[(pPossibleRoute[i], pPossibleRoute[i + 1])];
    }

    routeLength += pathLengths[(pPossibleRoute[pPossibleRoute.Count-1] , 0)];

    return routeLength;
}

Console.WriteLine("Part 2:" + GetShortestRoute(CalculateRouteLengthPart2));

