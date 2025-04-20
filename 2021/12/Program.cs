//Solution for https://adventofcode.com/2021/day/12 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: links between caves

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// Step 1, convert the input to a graph...

Graph<string> graph = ConvertTextToGraph(myInput);

Graph<string> ConvertTextToGraph(string pInput)
{
    var graph = new Graph<string>();

    string[] lines = pInput.Split(Environment.NewLine);

    foreach (var line in lines)
    {
        string[] elements = line
            .Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        graph.AddEdge(elements[0], elements[1]);
    }

    return graph; 
}

// ** Part 1: How many paths through this cave system are there
// that visit small caves at most once?

// In other words, run a basic recursive search,
// with a modified history check to allow reentering big caves...

HashSet<string> lowerCaseCaves = graph.GetNodes().Where(IsLowerCase).ToHashSet();

long GetPathCountPart1 (string pStart, string pEnd, List<string> pHistory)
{
    if (pStart == pEnd) return 1;

    pHistory.Add(pStart);

    long totalPathsFound = 0;

    foreach (string child in graph.GetNeighbors(pStart))
    {
        if (lowerCaseCaves.Contains(child) && pHistory.Contains(child)) continue;

        totalPathsFound += GetPathCountPart1(child, pEnd, new(pHistory));
    }

    return totalPathsFound;
}

bool IsLowerCase (string pInput)
{
    return pInput == pInput.ToLower();
}


Console.WriteLine("Part 1: " + GetPathCountPart1("start", "end", new()));

// ** Part 2: Same search approach, but with different conditions:

long GetPathCountPart2(string pStart, string pEnd, List<string> pHistory, bool pTimesUp = false)
{
    if (pStart == pEnd)
    {
        return 1;
    }

	long totalPathsFound = 0;

    foreach (string child in graph.GetNeighbors(pStart))
    {
        //ignore start always
        if (child == "start") continue;

        //now get the child count and whether the child is lower case
        int childCount = pHistory.Count(x => x == child);
        bool isLowerCase = lowerCaseCaves.Contains(child);

        //when can we continue the search?
        //- if the child is NOT in the history at all OR
        //- if it is in the history but NOT lowercase
        //- it is in the history and lowercase BUT occurs
        //  0 or 1 time AND there is still time
        //  (no other small cave visited twice)

        if (
            !pHistory.Contains(child) || 
            !isLowerCase ||
            (childCount < 2 && !pTimesUp)
            )
        {
            //This one is tricky... we should NEVER turn off timesUp
            //if it was already true, but we can turn a false one true
            //if we are lowercase and we WERE already in the list

            bool timesUp = pTimesUp | (isLowerCase && childCount > 0);
            pHistory.Add(child);
            totalPathsFound += GetPathCountPart2(child, pEnd, (pHistory), timesUp);
            pHistory.RemoveAt(pHistory.Count - 1);
        }
    }

	return totalPathsFound;
}

Console.WriteLine("Part 2: " + GetPathCountPart2("start", "end", new()));