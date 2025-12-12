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

// Alternative way...

// Real dirty

HashSet<string> GetMatches (HashSet<string> pInput, int pIteration)
{
    productionRules["8"] = string.Join(' ', Enumerable.Repeat("42", pIteration));
    productionRules["11"] = string.Join(' ', Enumerable.Repeat("42", pIteration)) + " " + string.Join(' ', Enumerable.Repeat("31", pIteration));

    HashSet<string> matches = new();

    foreach (string str in pInput)
    {
        int index = 0;
        if (CanMatch(str, ref index, "0")) matches.Add(str);
    }

    return matches;
}

bool debug = false;
bool pause = false;

bool CanMatch(string pInput, ref int pIndex, string pCurrentPattern, int pDepth = 0)
{
    string indent = new string(' ', pDepth);

    if (pDepth > 100) return false;

    if (debug) Console.WriteLine(indent + "Trying to match " + Alter(pInput, pIndex) + " at index " + pIndex + " with pattern " + pCurrentPattern);
    if (pause) Console.ReadKey();

    string[] parts = pCurrentPattern.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (parts.Length > 1)
    {
        if (debug) Console.WriteLine(indent + "Splitting pattern on | and repeating search for each part...");
        if (pause) Console.ReadKey();

        for (int i = parts.Length - 1; i >= 0; i--)
        {
            int currentIndex = pIndex;

            string part = parts[i];
            if (debug) Console.WriteLine(indent + "Trying to match " + Alter(pInput, pIndex) + " at index " + pIndex + " with subblock " + part);

            if (CanMatch(pInput, ref pIndex, part, pDepth + 1))
            {
                if (debug) Console.WriteLine(indent + "Matched subblock " + part + " of " + pCurrentPattern);
                if (pause) Console.ReadKey();
                return true;
            }

            if (debug) Console.WriteLine(indent + "Could not match subblock " + part + " of " + pCurrentPattern);
            if (pause) Console.ReadKey();

            pIndex = currentIndex;
        }

        if (debug) Console.WriteLine(indent + "Could not match any subblocks of " + pCurrentPattern);
        if (pause) Console.ReadKey();

        return false;
    }
    else
    {
        parts = pCurrentPattern.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (debug) Console.WriteLine(indent + "Pattern does not contain |, matching in order listed...");
        if (pause) Console.ReadKey();

        int savedIndex = pIndex;

        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            if (debug) Console.WriteLine(indent + "Trying to match " + Alter(pInput, pIndex) + " at index " + pIndex + " with part " + (i + 1) + "/" + parts.Length + " = \"" + part + "\"");
            if (pause) Console.ReadKey();

            if (part == "a" || part == "b")
            {
                if (pIndex >= pInput.Length)
                {
                    pIndex = savedIndex;
                    return false;  // can't match a character, no input left
                }

                if (pInput[pIndex] == part[0])
                {
                    if (debug) Console.WriteLine(indent + "Matched.");
                    if (pause) Console.ReadKey();

                    pIndex++;
                }
                else
                {
                    if (debug) Console.WriteLine(indent + "a or b not matched.");
                    if (pause) Console.ReadKey();
                    pIndex = savedIndex;
                    return false;
                }
            }
            else
            {
                if (debug) Console.WriteLine(indent + "Searching deeper.");
                if (pause) Console.ReadKey();

                if (CanMatch(pInput, ref pIndex, productionRules[part], pDepth + 1))
                {
                    if (i == parts.Length - 2) return true;
                    //if (debug) Console.WriteLine(i == parts.Length - 1);
                    if (debug) Console.WriteLine(indent + "Matched " + Alter(pInput, pIndex) + " at index " + pIndex + " with part " + (i + 1) + "/" + parts.Length + " = \"" + part + "\"");
                    if (pause) Console.ReadKey();
                }
                else
                {
                    if (debug) Console.WriteLine(indent + "Could not match " + Alter(pInput, pIndex) + " at index " + pIndex + " with part " + (i + 1) + "/" + parts.Length + " = \"" + part + "\"");
                    if (pause) Console.ReadKey();
                    pIndex = savedIndex;
                    return false;
                }
            }
        }

        if (debug) Console.WriteLine(indent + "Matched all consecutive parts of " + pCurrentPattern + " with input.");
        if (pause) Console.ReadKey();

        return false;
    }
}

string Alter(string pInput, int pIndex)
{
    if (pIndex >= pInput.Length)
    {
        return pInput;
    }
    char[] input = pInput.ToCharArray();
    input[pIndex] = char.ToUpper(pInput[pIndex]);
    return string.Concat(input);

}




int iteration = 0;
int total = 0;

while (iteration < 100)
{
    HashSet<string> matches = GetMatches(stringsToMatch, 0);

    total += matches.Count;
    stringsToMatch.ExceptWith (matches);
    iteration++;
    Console.Write(".");
}

Console.WriteLine("Part 2:" + total);

