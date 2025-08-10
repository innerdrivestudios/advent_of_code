// Solution for https://adventofcode.com/2022/day/16 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// Your input:
// - a description of a maze of tunnels (specified using a list of valves and their connections),
//   including each valve's flow rate if it were opened (in pressure per minute), e.g.:
//
//   Valve LY has flow rate=0; tunnels lead to valves IZ, EJ
//   Valve .. has flow rate=..; ...

using System.Diagnostics;
using System.Text.RegularExpressions;

string[] tunnelReport = File.ReadAllLines(args[0]);

// Step 1. Parse the input :)
// We will create a Graph from Valve to Valve,
// while keeping a dictionary of mapped valves so we can complete their info as we go...

Dictionary<string, Valve> valveNameToValveMap = new();

Valve GetValve (string pValveName)
{
    if (!valveNameToValveMap.ContainsKey(pValveName)) valveNameToValveMap[pValveName] = new Valve(pValveName);

    return valveNameToValveMap[pValveName];
}

Graph<Valve> graph = new ();

Regex regex = new Regex(@"^Valve (\w+) has flow rate=(\d+); tunnels? leads? to valves? (.+)$");

foreach (string tunnelReportLine in tunnelReport)
{
    //Valve LY has flow rate = 0; tunnels lead to valves IZ, EJ
    Match match = regex.Match(tunnelReportLine);
    if (match.Success)
    {
        Valve mainValve = GetValve(match.Groups[1].Value);
        mainValve.flowRate = int.Parse(match.Groups[2].Value);

        string[] destinationValves = match.Groups[3].Value.Split(",", StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries);
        
        foreach (string destinationValve in destinationValves)
        {
            //We make the edge non bidirectional since the input lines specify the connections back and forth
            graph.AddEdge (mainValve, GetValve (destinationValve), false);
        }
        
        Console.WriteLine(match.Groups[1].Value + " leads to " + string.Join (",", destinationValves));
    }
    else
    {
        throw new Exception("Every line should match!");
    }
}

// So, now we have a graph, which a bunch of nodes and connections, to be more precise...
Console.WriteLine();
Console.WriteLine("Nodes total:" + graph.GetNodes().Count);
// ... is the amount of nodes we have ...

// There are a couple of issues I foresee searching this graph:
// - there are a lot of junk nodes: e.g. valve nodes with 2 connections and a flowRate of 0
//      These are basically only there to incur additional steps and decision points, even though you will never stop there.
// - Finding the solution requires moving back and forth over all the valves in the graph,
//   in other words, the standard approach of exploring all possibilities using a BFS or Dijkstra search,
//   while keeping track of visited nodes is not going to work.

// HOWEVER if we kind of "reduce" the problem, this puzzle basically comes down to:
// "What is the order in which we should open up valves to get the highest flow rate within the first 30 minutes?"

// For that, we basically only need to know:
// - what is the shortest path between each NON ZERO flow rate valve and each other NON ZERO flow rate valve,
//   except for the starting AA node with flow rate 0, we do need to include that one.
// - Note that we actually don't even need to KNOW the actual path at this point,
//   only how much time it will take to go from A to B and open that valve.
// - Which permutations of valve order options can fit into 30 minutes
// - Which permutation results in the highest flow rate value

// Possibly we could prune permutations even earlier based on whether they could beat another permutation, 
// but I don't see it (yet).

// Ok, next step, figure out for which nodes we need to calculate path distances etc...
// So more testing data...

Console.WriteLine("Total connections count:" + graph.GetNodes().Sum(x => graph.GetNeighbors(x).Count));
Console.WriteLine("Removable nodes:" + graph.GetNodes().Count(x => x.flowRate == 0 && graph.GetNeighbors(x).Count == 2));
Console.WriteLine("0 nodes with > 2 connections:" + graph.GetNodes().Count(x => x.flowRate == 0 && graph.GetNeighbors(x).Count > 2));
Console.WriteLine("Connection for single 0 node with > 2 connections?");
Console.WriteLine(
	graph.GetNeighbors(
		graph.GetNodes().Where(x => x.flowRate == 0 && graph.GetNeighbors(x).Count > 2).First()
	).Count
);
Console.WriteLine();

// In my situation 41 out of 57 nodes can be removed/ignored... nice!
// Ok, so we have two options:
// 1) Convert our graph into an EdgedGraph first (a graph with edges that have a cost),
// by removing all nodes with a flowrate of 0 (except AA) and updating the costs of their neighbouring edges accordingly,
// followed by a 0.5 * N * N search from all nodes to all nodes to figure out the shortest path costs for the paths between them.
// 2) Don't optimize the input graph, just do the 0.5 * N * N search directly on the input graph

// On the one hand we shouldn't early optimize, but on the other hand, I haven't written an algorithm like 1) yet, so let's do that anyway...

// First we'll convert our basic graph into an EdgedGraph
Console.WriteLine("Setting up EdgedGraph");
EdgedGraph<Valve> edgedGraph = new ();

foreach (Valve node in graph.GetNodes())
{
    foreach (Valve neighbor in graph.GetNeighbors(node))
    {
        Console.WriteLine("Adding " + node.id + " to " + neighbor.id + " with initial cost of 1");
        //Note this is just traversal cost, no flow rate involved here 
        edgedGraph.AddEdge(node, neighbor, 1, false);
    }
}

// Let's just do a little bit of sanity check to make sure our current graph is correct...
Console.WriteLine();
Console.WriteLine("Total connections count:" + edgedGraph.GetNodes().Sum(x => edgedGraph.GetNeighbors(x).Count));
Console.WriteLine("Removable nodes:" + edgedGraph.GetNodes().Count(x => x.flowRate == 0 && edgedGraph.GetNeighbors(x).Count == 2));
Console.WriteLine("0 nodes with > 2 connections:" + edgedGraph.GetNodes().Count(x => x.flowRate == 0 && edgedGraph.GetNeighbors(x).Count > 2));
Console.WriteLine("Connection for single 0 node with > 2 connections?");
Console.WriteLine(
    edgedGraph.GetNeighbors(
        edgedGraph.GetNodes().Where(x => x.flowRate == 0 && edgedGraph.GetNeighbors(x).Count > 2).First()
    ).Count
);

// So now we actually get all those nodes that have a flow rate of zero and are not node AA
List<Valve> removableNodes = edgedGraph.GetNodes().Where(x => x.flowRate == 0 && x.id != "AA" && edgedGraph.GetNeighbors(x).Count == 2).ToList();

Console.WriteLine();
// For each of these nodes, we'll get their 2 connections (as verified above), create a new connection between them,
// with a cost that is the sum of the previous connection cost:
Console.WriteLine("Optimizing graph...");

foreach (var node in removableNodes)
{
    List<Valve> neighbors = edgedGraph.GetNeighbors(node);
    long totalCost = edgedGraph.GetEdgeCost(node, neighbors[0]) + edgedGraph.GetEdgeCost(node, neighbors[1]);
    edgedGraph.RemoveNode(node);
    edgedGraph.AddEdge(neighbors[0], neighbors[1], totalCost, true);
}

// Let's see if this went ok:
Console.WriteLine("Optimized graph...");
foreach (var node in edgedGraph.GetNodes())
{
    foreach (var neighbor in edgedGraph.GetNeighbors(node))
    {
        Console.WriteLine("Edge between:" + node.id + " and " + neighbor.id + " with cost " + edgedGraph.GetEdgeCost(node, neighbor));
    }
}

// Ok, now it is time to create the actual "graph" that tells us what the cost is to get from node X to node Y and open it's valve,
// since basically (looking at the provided example as well) if you want to open valve DD, what you'll want to know is WHEN is it
// going to release pressure. We can do that by getting the path cost + 1 for opening the valve, OR we just add that cost to the path 
// immediately...

Console.WriteLine();
Console.WriteLine("Calculating all individual path to open costs...");
EdgedGraph<Valve> costToOpenTable = new();
List<Valve> allValves = edgedGraph.GetNodes();

// We don't include paths from X to X and we only need to calculate the cost once
for (int i = 0; i < allValves.Count-1; i++)
{
    for (int j = i + 1;  j < allValves.Count; j++)
    {
        DijkstraResult<Valve> path = Dijkstra.Search(new ClassicDijkstraEdgedGraphAdapter<Valve>(edgedGraph, allValves[i], allValves[j]));
        if (path == null) throw new Exception("Not supposed to happen");

        Console.WriteLine(allValves[i].id + " to " + allValves[j].id + " path found with cost " + path.totalCost);
        costToOpenTable.AddEdge(allValves[i], allValves[j], path.totalCost + 1, true);
    }
}

Console.WriteLine();

Console.WriteLine("Final result table:");
foreach (var node in costToOpenTable.GetNodes())
{
    foreach (var neighbour in costToOpenTable.GetNeighbors(node))
    {
        Console.WriteLine("Cost to open " + node.id + " starting at " + neighbour.id + " = " + costToOpenTable.GetEdgeCost(node, neighbour));
    }
}

// Next step... starting at A (this luckily already severly limits the starting conditions)...
// Find which combination / order of opening the valves gives the highest end result,
// while never returning to a graph already opened.
// Now we don't need to know the order for the final answer, but to calculate the flow rate we do...
// We could keep track of this on the Valve, but since we are searching over a ton of different options,
// it is better to keep this data out of the Valve instance itself

long GetBiggestFlowRate (EdgedGraph<Valve> pCostToOpenTable, Dictionary<Valve, int> pValveOpenTimes, Valve pLastValveOpened, int pEndTime)
{
    long highestFlowRate = GetFlowRateForValveSetAtTime(pValveOpenTimes, pEndTime);

    int currentTime = pValveOpenTimes[pLastValveOpened];

    //Go through all connections in the last valve opened
    foreach (var nextValve in pCostToOpenTable.GetNeighbors(pLastValveOpened))
    {
        //If opening this valve would take too long ...
        int nextValveOpenTime = currentTime + (int) pCostToOpenTable.GetEdgeCost(pLastValveOpened, nextValve);
        if (nextValveOpenTime > pEndTime) continue;

        //If we already opened this valve, don't visit it again
        if (pValveOpenTimes.ContainsKey(nextValve)) continue;

        pValveOpenTimes.Add (nextValve, nextValveOpenTime);

        var childResult = GetBiggestFlowRate(
                    pCostToOpenTable,
                    pValveOpenTimes,
                    nextValve,
                    pEndTime
                );

        pValveOpenTimes.Remove(nextValve);

        if (childResult > highestFlowRate) highestFlowRate = childResult;
    }

    return highestFlowRate;
}

long GetFlowRateForValveSetAtTime (Dictionary<Valve, int> pValveOpenTimes, int pEndTime)
{
    long totalFlowRate = 0;

    foreach (var kvp in pValveOpenTimes)
    {
        totalFlowRate += kvp.Key.flowRate * (pEndTime - kvp.Value);
    }

    return totalFlowRate;
}

Stopwatch stopwatch = Stopwatch.StartNew();
var biggestFlowRatePart1 = GetBiggestFlowRate(costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 30);
Console.WriteLine("Part 1: " + biggestFlowRatePart1);
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " milliseconds");

// ** Part 2: Do the same but now together with an elephant :)

// In other words, we need to divide the valves we can visit between us and the elephant and test the division to see whether 
// it is the highest ranking. How can we do this? 
//
// For every node except AA we can make a choice: will we visit this node, or will the elephant visit this node.
// In other words, lets say we have 4 nodes BB, CC, DD, EE, we can make the following divisions:

//                        US              <-> ELEPHANT
//
// 00 0000                .., .., .., ..  <-> BB, CC, DD, EE
// 01 0001                .., .., .., EE  <-> BB, CC, DD, ..
// 02 0010                .., .., DD, ..  <-> BB, CC, .., EE
// 03 0011                .., .., DD, EE  <-> BB, CC, .., ..
// 04 0100                .., CC, .., ..  <-> BB, .., DD, EE
// 05 0101                .., CC, .., EE  <-> BB, .., DD, ..
// 06 0110                .., CC, DD, ..  <-> BB, .., .., EE
// 07 0111                .., CC, DD, EE  <-> BB, .., .., ..
// 08 1000                BB, .., .., ..  <-> .., CC, DD, EE
// 09 1001                BB, .., EE, ..  <-> .., CC, DD, ..
// 10 1010                BB, .., DD, ..  <-> .., CC, .., EE
// 11 1011                BB, .., DD, EE  <-> .., CC, .., ..
// 12 1100                BB, CC, .., ..  <-> .., .., DD, EE
// 13 1101                BB, CC, .., EE  <-> .., .., DD, ..
// 14 1110                BB, CC, DD, ..  <-> .., .., .., EE
// 15 1111                BB, CC, DD, EE  <-> .., .., .., ..

// SO what we see is:
// - 2^n (16 in this case) possible combinations
// - we can easily enumerate all divisions by numbering them using binary numbers
// - Not all combinations might make sense (e.g. division 0 and 15)
// - All combinations are mirrored around 2*n/2 (range 0000 to 0111 mirrors 1000 to 1111),
//   these are basically the same sets of divisions we are testing, in other words,
//   we only need to test half of the possible divisions.

// To actually implement this, we'll rewrite the method above to also take a dictionary of valves->valveindex, a bitmask and an inverter mask:

long GetBiggestFlowRatePart2 (
        EdgedGraph<Valve> pCostToOpenTable, Dictionary<Valve, int> pValveOpenTimes, Valve pLastValveOpened, int pEndTime,
        Dictionary<Valve, int> pAllowedValves, int pBitMask, int pNegationMask
    )
{
    long highestFlowRate = GetFlowRateForValveSetAtTime(pValveOpenTimes, pEndTime);

    int currentTime = pValveOpenTimes[pLastValveOpened];

    int bitMask = pBitMask ^ pNegationMask;

    //Go through all connections in the last valve opened
    foreach (var nextValve in pCostToOpenTable.GetNeighbors(pLastValveOpened))
    {
        //If opening this valve would take too long ...
        int nextValveOpenTime = currentTime + (int)(pCostToOpenTable.GetEdgeCost(pLastValveOpened, nextValve));
        if (nextValveOpenTime > pEndTime) continue;

        //If we already opened this valve, don't visit it again
        if (pValveOpenTimes.ContainsKey(nextValve)) continue;

        //Check the index of the valve against the allowed bitmask
        if ((1 << pAllowedValves[nextValve] & bitMask) == 0) continue;

        pValveOpenTimes.Add(nextValve, nextValveOpenTime);

        var childResult = GetBiggestFlowRatePart2(
                    pCostToOpenTable,
                    pValveOpenTimes,
                    nextValve,
                    pEndTime,
                    pAllowedValves,
                    pBitMask,
                    pNegationMask
                );

        pValveOpenTimes.Remove(nextValve);

        if (childResult > highestFlowRate) highestFlowRate = childResult;
    }

    return highestFlowRate;
}


List<Valve> valveList = costToOpenTable.GetNodes();
valveList.Remove(valveNameToValveMap["AA"]);

//Cache indices to avoid O(n) lookup
Dictionary<Valve, int> valveToIndexMap = valveList.ToDictionary(x => x, x => valveList.IndexOf(x));

int maxDivisionFlag = 1 << valveList.Count;
int negationBitMask = maxDivisionFlag - 1;

// As a sanity check we test whether our method actually works...
biggestFlowRatePart1 = GetBiggestFlowRatePart2(
                costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 30,
                valveToIndexMap, negationBitMask, 0
            );

Console.WriteLine("Testing new setup, should print same as part 1:" + biggestFlowRatePart1);

biggestFlowRatePart1 = GetBiggestFlowRatePart2(
                costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 30,
                valveToIndexMap, 0, 0
            );

Console.WriteLine("Testing new setup, should print 0:" + biggestFlowRatePart1);

// Now run the actual part 2, don't forget to use 26 instead of 30!!

var biggestFlowRatePart2 = 0L;

Console.WriteLine("Variations to test:" + maxDivisionFlag/2);

int variationsTested = 0;
int granularity = 1000;
int lastResult = 0;
object locker = new object();

/*
for (int i = 0; i < maxDivisionFlag/2; i++)
{
    biggestFlowRatePart2 = long.Max(
            biggestFlowRatePart2,
            GetBiggestFlowRatePart2(
                costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 26,
                valveList, i, 0
            ) +
            GetBiggestFlowRatePart2(
                costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 26,
                valveList, i, negationBitMask
            )
        );

    variationsTested++;
    if (variationsTested/granularity != lastResult)
    {
        lastResult= variationsTested/granularity;
        Console.WriteLine((lastResult * granularity) + " variations tested...");
    }
}
*/

stopwatch.Restart();
Parallel.For (0, maxDivisionFlag / 2, i =>
{
    long yourScore = GetBiggestFlowRatePart2(
        costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 26,
        valveToIndexMap, i, 0
    );

    long elephantScore = GetBiggestFlowRatePart2(
        costToOpenTable, new() { { valveNameToValveMap["AA"], 0 } }, valveNameToValveMap["AA"], 26,
        valveToIndexMap, i, negationBitMask
    );

    long combined = yourScore + elephantScore;

    lock (locker)
    {
        if (combined > biggestFlowRatePart2)
            biggestFlowRatePart2 = combined;

        variationsTested++;
        if (variationsTested / granularity > lastResult)
        {
            lastResult = variationsTested / granularity;
            Console.WriteLine($"{lastResult * granularity} variations tested...");
        }
    }
});

Console.WriteLine("Part 2:" + biggestFlowRatePart2);
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " milliseconds");