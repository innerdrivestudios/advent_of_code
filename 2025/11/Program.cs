// Solution for https://adventofcode.com/2025/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

using System.Diagnostics;

string[] myInput = File.ReadAllLines(args[0]);

Graph<string> downGraph = new Graph<string>();
Graph<string> upGraph = new Graph<string>();

foreach (string line in myInput)
{
    string[] input = line.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    for (int i = 1;  i < input.Length; i++)
    {
        downGraph.AddEdge(input[0], input[i], false);
        upGraph.AddEdge(input[i], input[0], false);
    }
}

// ** Part 1: Count all paths

int GetPathCountPart1 (string pStart, string pEnd, Graph<string> pGraph)
{
    if (pStart == pEnd) return 1;

    int paths = 0;

    foreach (string p in pGraph.GetNeighborsUnsafe(pStart))
    {
        paths += GetPathCountPart1(p, pEnd, pGraph);
    }

    return paths;
}

Console.WriteLine("Part 1: " + GetPathCountPart1("you", "out", downGraph));

// ** Part 2: 

Stopwatch stopwatch = Stopwatch.StartNew();

// Doing a little test... which nodes are leaf nodes?
// Those with no children... 

foreach (string node in downGraph.GetNodes())
{
    if (downGraph.GetNeighborsUnsafe(node).Count() == 0)
    {
        Console.WriteLine(node);
    }
}

// So only "out"

// Doing a little test... which nodes are root nodes?
// Those with no parents... 

foreach (string node in upGraph.GetNodes())
{
    if (upGraph.GetNeighborsUnsafe(node).Count() == 0)
    {
        Console.WriteLine(node);
    }
}

// So only "svr"


// In other words... all roads leads from svr to out...

// Now what I want to do to clean the graph up and reduce the amount of options.
// I'll do that by severing connections from the bottom up if that node cannot lead to
// a certain parent node.

// First I'll define a HasParent using memoization

bool HasParent (string pStart, string pParent, Dictionary<string, bool> pCache = null)
{
    pCache = pCache ?? new Dictionary<string, bool>();

    if (pStart == pParent)
    {
        pCache[pStart] = true;
        return true;
    }

    bool hasParent = false;

    foreach (string child in upGraph.GetNeighbors(pStart))
    {
        if (pCache.ContainsKey(child)) hasParent |= pCache[child];
        else hasParent |= HasParent(child, pParent, pCache);
    }

    pCache[pStart] = hasParent;
    return hasParent;
}

// Then a similar method to sever connections...

void SeverConnections (string pStart, string pParent, HashSet<string> pProcessed = null)
{
    if (pStart == pParent) return;

    pProcessed = pProcessed ?? new HashSet<string>();

    foreach (string child in upGraph.GetNeighbors(pStart))
    {
        if (HasParent(child, pParent))
        {
            if (!pProcessed.Contains(child))
            {
                pProcessed.Add(child);
                SeverConnections(child, pParent, pProcessed);
            }
        }
        else
        {
            downGraph.RemoveNode(child);
            upGraph.RemoveNode(child);
        }
    }
}

// Sever all connections which cannot reach either dac or fft

SeverConnections("out", "dac");
SeverConnections("out", "fft");

// Followed by a brute force dangling node cleaner...

void CleanLeafNodes ()
{
    int nodesRemoved = 0;

    while (true)
    {
        nodesRemoved = 0;

        foreach (string node in downGraph.GetNodes())
        {
            if (node == "out") continue;

            if (downGraph.GetNeighbors(node).Count() == 0)
            {
                downGraph.RemoveNode(node);
                nodesRemoved++;
            }
        }

        if (nodesRemoved == 0) break;
    }
}

CleanLeafNodes();

// Now our whole graph is clean and every route is going over fft and dac

long CountOptions(string pStart, Graph<string> pGraph, Dictionary<string, long> pCache = null)
{
    pCache = pCache ?? new Dictionary<string, long>();

    if (pCache.ContainsKey(pStart)) return pCache[pStart];

    IEnumerable<string> neighbors = pGraph.GetNeighborsUnsafe(pStart);

    long countOptions = 0;

    if (neighbors.Count() == 0)
    {
        countOptions = 1;
    }
    else
    {
        foreach (string neighbor in neighbors)
        {
            countOptions += CountOptions(neighbor, pGraph, pCache);
        }
    }

    pCache[pStart] = countOptions;
    return countOptions;
}

Console.WriteLine("Part 2: " + CountOptions("svr", downGraph));
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " ms.");