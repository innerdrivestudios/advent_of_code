//Solution for https://adventofcode.com/2017/day/12 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: links between ids

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// Step 1, convert the input to a graph...

Graph<int> graph = ConvertTextToGraph(myInput);

Graph<int> ConvertTextToGraph(string pInput)
{
    var graph = new Graph<int>();

    string[] lines = pInput.Split(Environment.NewLine);

    foreach (var line in lines)
    {
        int[] elements = line
            .Split([ "<->", ","], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select (int.Parse)
            .ToArray();

        for (int i = 1; i < elements.Length; i++)
        {
            graph.AddEdge(elements[0], elements[i]);
        }
    }

    return graph; 
}

// ** Part 1: "How many programs are in the group that contains program ID 0?"
// In other words: run a search to cover all nodes connected to 0

Console.WriteLine("Part 1: " + graph.BFS(0).Count);

// ** Part 2: "How many groups are there?"
// In other words:
// - Get all nodes
// - Pick the first one, run a search, remove all nodes in the result from the original group
// - Repeat until empty and count the repetitions

// Use hashset since removal is O(1) vs O(n) for a regular list
HashSet<int> nodes = graph.GetNodes().ToHashSet();
int groupCount = 0;

while (nodes.Count > 0)
{
    nodes.ExceptWith(graph.BFS(nodes.First()));
    groupCount++;
}

Console.WriteLine("Part 2: " + groupCount);

