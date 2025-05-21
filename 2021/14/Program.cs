//Solution for https://adventofcode.com/2021/day/14 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: an original string plus a list of expansion instructions

// Parse the input
string[] myInput = ParseUtils.SplitBlocks(File.ReadAllText(args[0]));

string codeToExpand = myInput[0];

Dictionary<(char, char), char> expansionTable = 
    ParseUtils.StringToTuples<string, char> (myInput[1], "->")      //get tuples of string, char based on ->
    .ToDictionary(x => (x.Item1[0], x.Item1[1]), x=> x.Item2);      //convert it to a dictionary of (string[0], string[1]) -> char

// ** Part 1: Expand the given codeToExpand 10 times and calculate #mostcommonelement - #leastcommonelement

// First create and initialize the data structure to keep track of the char counts...

Dictionary<char, long> charCountMap = new();

// Initialize our memoization table (result lookup cache).
// This is not strictly needed for part 1, but part 2 won't run without it
Dictionary<(char, char, int), Dictionary<char, long>> memoizationTable = new();

// Helper method to update the char count without crashing etc

void UpdateCharCount(Dictionary<char, long> pCharCountMap, char pChar, long pCount)
{
    pCharCountMap[pChar] = pCharCountMap.GetValueOrDefault(pChar, 0) + pCount;
}

// Now create a recursive method that actually calculates the char counts without creating/expanding the string itself

void StartRecursion (string pCodeToExpand, int pMaxDepth = 10)
{
    // Set up the initial char count map and memoization table
    charCountMap.Clear();
    memoizationTable.Clear();

    // Store the initial char that were given as puzzle input
    foreach (char pChar in codeToExpand) UpdateCharCount(charCountMap, pChar, 1);

    // Run the recursive char count calculation process on each char pair
    for (int i = 0; i < pCodeToExpand.Length-1; i++)
    {
        var subResult = ProcessCharCombo(pCodeToExpand[i], pCodeToExpand[i + 1], pMaxDepth);
        foreach (var keyValuePair in subResult) UpdateCharCount(charCountMap, keyValuePair.Key, keyValuePair.Value);
    }
}

Dictionary<char, long> ProcessCharCombo (char pCharA, char pCharB, int pMaxDepth)
{
    // If already cached for this combo at this level, return the cache
    if (memoizationTable.TryGetValue((pCharA, pCharB, pMaxDepth), out Dictionary<char, long> result))
    {
        return result;
    }

    // If not, create a dictionary with the count for the resulting char from this lookup...
    Dictionary<char, long> charCountMap = new Dictionary<char, long>();

    char newChar = expansionTable[(pCharA, pCharB)];
    UpdateCharCount(charCountMap, newChar, 1);

    // And if needed, process the child combo's and add their counts to our own
    if (pMaxDepth > 1)
    {
        var leftResult = ProcessCharCombo(pCharA, newChar, pMaxDepth - 1);
        var rightResult = ProcessCharCombo(newChar, pCharB, pMaxDepth - 1);

        foreach (var keyValuePair in leftResult) UpdateCharCount(charCountMap, keyValuePair.Key, keyValuePair.Value);
        foreach (var keyValuePair in rightResult) UpdateCharCount(charCountMap, keyValuePair.Key, keyValuePair.Value);
    }

    // Don't forget to cache the results...
    memoizationTable[(pCharA, pCharB, pMaxDepth)] = charCountMap;

    return charCountMap;
}

StartRecursion(codeToExpand, 10);
Console.WriteLine("Part 1:" + (charCountMap.Values.Max() - charCountMap.Values.Min()));

// ** Part 2: Same but with 40 iterations...
StartRecursion(codeToExpand, 40);
Console.WriteLine("Part 2:" + (charCountMap.Values.Max() - charCountMap.Values.Min()));
