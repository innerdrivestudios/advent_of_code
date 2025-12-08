// Solution for https://adventofcode.com/2025/day/8 (Ctrl+Click in VS to follow link)

using Vec3l = Vec3<long>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

List<Vec3l> points = myInput
    .Split([Environment.NewLine, ","], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select (long.Parse)
    .Chunk(3)
    .Select (x => new Vec3l(x[0], x[1], x[2]))
    .ToList ();

// ** Part 1: Generate all pairs...

//int pairsToConnectCount = 10; //for the test data
int pairsToConnectCount = 1000;

List<(Vec3l, Vec3l)> pairs = new();

for (int i = 0; i < points.Count-1; i++)
{
    for (int j = i+1; j < points.Count; j++)
    {
        pairs.Add((points[i], points[j]));
    }
}

// Sort all pairs on distance, don't optimize if it turns out we don't have to :)
pairs.Sort((a, b) => double.Sign((a.Item1 - a.Item2).Magnitude() - (b.Item1 - b.Item2).Magnitude()));

// Build the graph for the requested amount of shortest connections
Graph<Vec3l> graph = new();
for (int i = 0; i < int.Min(pairsToConnectCount, pairs.Count); i++)
{
    graph.AddEdge(pairs[i].Item1, pairs[i].Item2);
}

// Find all sizes of the first x nodes that were part of an edge

HashSet<Vec3l> processed = new();
List<int> sizes = new();

for (int i = 0; i < int.Min(pairsToConnectCount, pairs.Count); i++)
{
    if (!processed.Contains(pairs[i].Item1))
    {
        HashSet<Vec3l> found = graph.BFS(pairs[i].Item1);
        sizes.Add(found.Count);
        processed.UnionWith(found);
    }
}

sizes = sizes.OrderByDescending(x => x).ToList();
if (sizes.Count > 2) Console.WriteLine("Part 1: " + sizes[0] * sizes[1] * sizes[2]);
else Console.WriteLine("Whoopsie!");

// ** Part 2: 

// Start by building a graph with all the points and no edges
Graph<Vec3l> graphPart2 = new();
points.ForEach(graphPart2.AddNode);

// Loop until our graph is fully connected
int connectionIndex = 0;
while (connectionIndex < pairs.Count)
{
    graphPart2.AddEdge(pairs[connectionIndex].Item1, pairs[connectionIndex].Item2);
    if (graphPart2.BFS(points[0]).Count == graphPart2.GetNodeCount()) break;
    connectionIndex++;
}

Console.WriteLine("Part 2: " + pairs[connectionIndex].Item1.X * pairs[connectionIndex].Item2.X);
