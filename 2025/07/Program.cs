// Solution for https://adventofcode.com/2025/day/7 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

Grid<char> diagram = new Grid<char>(myInput,Environment.NewLine);
Vec2i start = new Vec2i(diagram.width/2,0);

if (diagram[start] != 'S')
{
    Console.WriteLine("No start found");
    return;
}

// ** Part 1: Splitting the beams

Queue<Vec2i> beamsTodo = new ();
beamsTodo.Enqueue(start);

// For part 1
HashSet<Vec2i> splittersHit = new ();

// For part 2
Graph<Vec2i> graph = new Graph<Vec2i>();

// For part 1 & 2 we'll track/trace all beams downwards,
// creating new beams as we encounter splitters, but no double beams...

while (beamsTodo.Count > 0)
{
    // Get the beam and store its start
    Vec2i currentPosition = beamsTodo.Dequeue();
    Vec2i edgeStart = currentPosition;

    // Trace the beam downwards ...
    while (diagram.IsInside(currentPosition) && diagram[currentPosition] != '^')
    {
        currentPosition += new Vec2i(0, 1);
    }

    // If we are still inside the grid, we hit a splitter on our way down ...
    if (diagram.IsInside(currentPosition))
    {
        // Create the left and right beam ...
        Vec2i left = currentPosition + new Vec2i(-1, 0);
        Vec2i right = currentPosition + new Vec2i(1, 0);

        // So for part 1 we only need to know how many times we hit a splitter...
        // And if we hit a splitter that counts as 1 times splitting :)
        // Also we'll only queue NEW beams, so hitting a splitter multiple times
        // doesn't create new beams ...
        if (splittersHit.Add(currentPosition))
        {
            beamsTodo.Enqueue(left);
            beamsTodo.Enqueue(right);
        }

        // Every beam that hits a splitter though also builds a graph from the
        // start of the beam to the new split position, no matter whether the
        // splitter was already hit by another beam higher up or lower down...
        graph.AddEdge(edgeStart, left, false);
        graph.AddEdge(edgeStart, right, false);
    }
}

Console.WriteLine("Part 1: " + splittersHit.Count);

// ** Part 2: Count options:

Dictionary<Vec2i, long> cache = new();

long CountOptions(Vec2i pStart)
{
    if (cache.ContainsKey(pStart)) return cache[pStart];

    IEnumerable<Vec2i> neighbors = graph.GetNeighborsUnsafe(pStart);
    long countOptions = neighbors.Any() ? neighbors.Sum (CountOptions) : 1;

    cache[pStart] = countOptions;
    return countOptions;
}

Console.WriteLine("Part 2: " + CountOptions(start));

