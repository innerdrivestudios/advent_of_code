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
        results.UnionWith(GetPossibleSingleStringExpansions(part, pMaxLength, pDepth + 1));
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
                crossResults.Add(newString);
            }
        }

        results = crossResults;
    }

    return results;
}

HashSet<string> allPossibleExpansions = GetPossibleDoubleStringExpansions("0");
int possibleExpansionCountPart1 = stringsToMatch.Count(x => allPossibleExpansions.Contains(x));

Console.WriteLine("Part 1:" + possibleExpansionCountPart1);

// ** Part 2:

productionRules["8"] = "42 | 42 8";
productionRules["11"] = "42 31 | 42 11 31";

// The idea is that we need to try and match the pInput string.
// When we start out, we only have one index to start from, index 0, meaning we want to match the first character in pInput
// with our current pattern...

// However, our pattern might split in two, e.g. 8 -> 42 | 42 8
// meaning firstPart 1 might match more than firstPart 2...
// If this pattern was called by another pattern, eg 8 9 10 that means that by the time we get to 9,
// we might have different starting indices to try...

// All of this leads to a situation where we need to account for multiple IN starting indices we need to try to match,
// that might result in multiple OUT starting indices for the step up above...

int maxRecursion = stringsToMatch.Max(x => x.Length);
Console.WriteLine("Max recursion:" + maxRecursion);

HashSet<int> CanMatch(string pInput, HashSet<int> pInIndices, string pCurrentPattern, int pDepth = 0)
{
    // Right recursive grammar, we gotta stop somewhere, counting this as a NO result
    if (pDepth > maxRecursion) return [];

    string[] parts = pCurrentPattern.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    HashSet<int> outIndices = new();

    if (parts.Length > 1)
    {
        // we split the process into each specific firstPart using the same starting indices as we got ourselves
        for (int i = 0; i < parts.Length; i++)
        {
            HashSet<int> matchIndices = CanMatch(pInput, pInIndices, parts[i], pDepth + 1);
            //if there is already a match in a sub block stop processing the whole thing and only return the match...
            if (matchIndices.Contains(pInput.Length)) return [pInput.Length];
            
            outIndices.UnionWith(matchIndices);
        }
    }
    else
    {
        // Otherwise get the firstPart and the rest...
        parts = pCurrentPattern.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        // Lazy sorry...
        string firstPart = string.Concat(parts.Take(1));
        string rest = string.Join (" ", parts.Skip(1)).Trim();
        bool isMore = !string.IsNullOrWhiteSpace(rest);

        foreach (int inIndex in pInIndices)
        {
            // We got shit to parse but no string left, meaning this index does not result in something valid, since the string is done, but the grammar isn't
            if (inIndex >= pInput.Length) continue;

            // a & b always come alone... so if we got one of these, it...
            if (firstPart == "a" || firstPart == "b")
            {
                // ...needs to match with the current index
                if (pInput[inIndex] == firstPart[0])
                {
                    // ... and if we match we are one step closer to our goal
                    outIndices.Add(inIndex+1);
                    // early exit?
                    if (outIndices.Contains(pInput.Length)) return [pInput.Length];
                }
            }
            else
            {
                // ... if the thing we need to match is not an a or b, try to match the subchild
                HashSet<int> firstPartMatch = CanMatch(pInput, [inIndex], productionRules[firstPart], pDepth + 1);

                // If that was it
                if (!isMore)
                {
                    if (firstPartMatch.Contains(pInput.Length)) return [pInput.Length];
                    outIndices.UnionWith(firstPartMatch);
                }
                else
                {
                    foreach (int i in firstPartMatch)
                    {
                        HashSet<int> restPartMatch = CanMatch(pInput, [i], rest, pDepth + 1);
                        if (restPartMatch.Contains(pInput.Length)) return [pInput.Length];
                        outIndices.UnionWith(restPartMatch);
                    }

                }
            }
        }
    }

    return outIndices;
}

int total = 0;

foreach(string str in stringsToMatch)
{
    //Console.WriteLine("Matching " + str);
    HashSet<int> matchResults = CanMatch(str, [0], "0", 0);
    if (matchResults.Count == 1 && matchResults.First() == str.Length) total++;
}

Console.WriteLine("Part 2:" + total);

