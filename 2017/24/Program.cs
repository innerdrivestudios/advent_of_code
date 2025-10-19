//Solution for https://adventofcode.com/2017/day/24 (Ctrl+Click in VS to follow link)

using Component = (int a, int b);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of components, this is like a set of domino bricks

string[] myInput = File.ReadAllLines(args[0]);

HashSet<Component> components = myInput
    .Select(x => x.Split('/'))
    .Select(x => (int.Parse(x[0]), int.Parse(x[1])))
    .ToHashSet();

// And then we'll make a list of ints to all the possible end values it is connected to

Dictionary<int, HashSet<int>> componentMap = new();

foreach (var component in components)
{
    var aList = componentMap.GetValueOrDefault(component.a, new());
    aList.Add(component.b);
    componentMap[component.a] = aList;

    var bList = componentMap.GetValueOrDefault(component.b, new());
    bList.Add(component.a);
    componentMap[component.b] = bList;
}

// Now we'll do a recursive search for the most expensive path

int FindStrongestBridge (int pCurrentNode = 0, int pCurrentStrength = 0, HashSet<Component> pVisited = null)
{
    pVisited = pVisited ?? new ();

    int newLinks = 0;
    HashSet<int> connections = componentMap[pCurrentNode];

    int strongestBridge = 0;

    foreach (var connection in connections)
    {
        if (pVisited.Contains((pCurrentNode, connection)) || pVisited.Contains((connection, pCurrentNode))) continue;

        pVisited.Add((pCurrentNode, connection));
        newLinks++;

        strongestBridge = int.Max (strongestBridge, FindStrongestBridge(connection, pCurrentStrength + pCurrentNode + connection, pVisited));

        pVisited.Remove((pCurrentNode, connection));
    }

    return (newLinks == 0)? pCurrentStrength : strongestBridge;
}

Console.WriteLine("Part 1:" + FindStrongestBridge());

// ** Part 2: What is the longest strongest bridge?

// Modified version of the previous method that also takes length into account

(int length, int strength) FindLongestStrongestBridge(int pCurrentNode = 0, int pCurrentStrength = 0, HashSet<Component> pVisited = null)
{
    pVisited = pVisited ?? new();

    int newLinks = 0;
    HashSet<int> connections = componentMap[pCurrentNode];

    (int length, int strength) longestStrongestBridge = (0, 0);

    foreach (var connection in connections)
    {
        if (pVisited.Contains((pCurrentNode, connection)) || pVisited.Contains((connection, pCurrentNode))) continue;

        pVisited.Add((pCurrentNode, connection));
        newLinks++;

        (int length, int strength) childBridge = FindLongestStrongestBridge(connection, pCurrentStrength + pCurrentNode + connection, pVisited);

        if (childBridge.length > longestStrongestBridge.length)
        {
            longestStrongestBridge = childBridge;
        }
        else if (childBridge.length == longestStrongestBridge.length) { 
            longestStrongestBridge.strength = int.Max (childBridge.strength, longestStrongestBridge.strength);
        }

        pVisited.Remove((pCurrentNode, connection));
    }

    return (newLinks == 0) ? (pVisited.Count+1, pCurrentStrength) : longestStrongestBridge;
}

Console.WriteLine("Part 2:" + FindLongestStrongestBridge().strength);
