// Solution for https://adventofcode.com/2020/day/19 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ...

string[] myInput = File.ReadAllLines(args[0]);

Dictionary<string, string> productionRules = new();
HashSet<string> stringsToMatch = new();

foreach (string line in myInput)
{
    if (string.IsNullOrEmpty(line)) continue;

    string[] parts = line.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (parts.Length == 2) productionRules[parts[0]] = parts[1].Replace("\"", "");
    else stringsToMatch.Add(parts[0]);
}

// ** Part 1:

HashSet<string> GetPossibleDoubleStringExpansions (string pInput, int pMaxLength = int.MaxValue, int pDepth = 0)
{
    //First assume there is a split | with multiple options
    string[] parts = pInput.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    //Merge the possible results for both paths
    HashSet<string> results = new HashSet<string>();

    foreach (string part in parts)
    {
        results.UnionWith(GetPossibleSingleStringExpansions(part, pMaxLength, pDepth+1));
    }

    return results;
}

HashSet<string> GetPossibleSingleStringExpansions(string pInput, int pMaxLength = int.MaxValue, int pDepth = 0)
{
    //if the input given is an end node, simply return it
    if (pInput == "a" || pInput == "b") return new HashSet<string>() { pInput };

    //Otherwise for this single set of "1 2 3", concatenate all possible results...

    string[] parts = pInput.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    HashSet<string> results = new HashSet<string>();

    results = GetPossibleDoubleStringExpansions(productionRules[parts[0]], pMaxLength, pDepth + 1);

    for (int i = 1; i < parts.Length; i++) 
    {
        HashSet<string> subResults = GetPossibleDoubleStringExpansions(productionRules[parts[i]], pMaxLength, pDepth + 1);

        //merge these results with what we already had
        HashSet<string> crossResults = new HashSet<string>();

        foreach (string a in results)
        {
            foreach (string b in subResults)
            {
                string newString = a + b;
                if (newString.Length <= pMaxLength) crossResults.Add(newString);
            }
        }

        results = crossResults;
    }

    return results;
}

//productionRules["8"] = "42";

HashSet<string> allPossibleExpansionsPart1 = GetPossibleDoubleStringExpansions("0");

Console.WriteLine("Part 1:" + stringsToMatch.Count (x => allPossibleExpansionsPart1.Contains(x)));

/////
///