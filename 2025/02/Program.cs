// Solution for https://adventofcode.com/2025/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of ranges

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("");

List<(long, long)> ranges = myInput
    .Split ([",", "-"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(long.Parse)
    .Chunk(2)
    .Select(x => (x[0], x[1]))
    .ToList();

// ** Part 1: Find the invalid numbers:

bool IsValid (long pLong)
{
    string numberAsString = pLong.ToString ();
    //uneven lengths can't repeat
    int length = numberAsString.Length;
    if (length % 2 == 1) return true;

    int halfLength = length / 2;

    for (int i = 0; i < halfLength; i++)
    {
        if (numberAsString[i] != numberAsString[i + halfLength]) return true;
    }

    return false;
}

HashSet<long> invalidIds = new();

foreach (var range in ranges)
{
    for (long i = range.Item1; i <= range.Item2; i++)
    {
        if (!IsValid(i)) invalidIds.Add(i);
    }
}

Console.WriteLine("Part 1: " + invalidIds.Sum());

// ** Part 2: Now id's are invalid if a pattern repeats any number of times...

// First let's write another helper method to find half repeating patterns, third repeating patterns etc

bool Repeats(string pNumberAsString, int pAmountOfBlocks)
{
    // 8
    int length = pNumberAsString.Length;

    // eg 8 / 2 = 4
    int factorizedLength = length / pAmountOfBlocks;
    
    // First check if the number is actually a factor...
    if ((length / pAmountOfBlocks) * pAmountOfBlocks != length) return false;

    //For each part of the factorized length (eg 4)
    for (int i = 0; i < factorizedLength; i++)
    {
        //The ith element of every block has to match
        for (int f = 1; f < pAmountOfBlocks; f++)
        {
            if (pNumberAsString[i] != pNumberAsString[i + f * factorizedLength]) return false;
        }
    }

    return true;
}

invalidIds = new();

foreach (var range in ranges)
{
    for (long i = range.Item1; i <= range.Item2; i++)
    {
        string numberAsString = i.ToString();

        for (int amountOfBlocks = 2; amountOfBlocks <= numberAsString.Length; amountOfBlocks++)
        {
            if (Repeats(numberAsString, amountOfBlocks))
            {
                invalidIds.Add(i);
                break;
            }
        }
    }
}

Console.WriteLine("Part 2: " + invalidIds.Sum());



