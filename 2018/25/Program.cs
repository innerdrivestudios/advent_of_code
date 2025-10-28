//Solution for https://adventofcode.com/2018/day/25 (Ctrl+Click in VS to follow link)

using Vec4i = Vec4<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of 4d points

string[] myInput = File.ReadAllLines(args[0]);

List<Vec4i> points = myInput
    .Select(x => x.Split(',')) //
    .Select(x => x.Select(int.Parse).ToArray())
    .Select(x => new Vec4i(x[0], x[1], x[2], x[3]))
    .ToList();

// ** Part 1: First create the graph:

Graph<Vec4i> graph = new Graph<Vec4i>();

foreach (var point in points) graph.AddNode(point);

for (int i = 0; i < points.Count - 1; i++)
{
    for (int j = i+1; j < points.Count; j++)
    {
        Vec4i a = points[i];
        Vec4i b = points[j];

        if ((a - b).ManhattanDistance() < 4) graph.AddEdge(a, b);

    }
}

// Now simply count the subgraphs...

int subGraphCount = 0;

while (points.Count > 0)
{
    HashSet<Vec4i> subGraph = graph.BFS(points[0]);
    points = points.Except(subGraph).ToList();
    subGraphCount++;
}

Console.WriteLine("Part 1: " + subGraphCount);



