// Solution for https://adventofcode.com/2021/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: not sure yet...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Parse the whole input into and array of tuples consisting of two string arrays

(string[], string[])[] twosetsOfStrings = myInput
    //Get all lines
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    //Split each line based on "|" so now we have a list of string[]
    .Select(x => x.Split("|", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    //Take each string[] and split both parts on " " 
    .Select(
        x =>
        (
            x[0].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
            x[1].Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        )
    )
    .ToArray();

//Before we continue, sort all strings 

for (int i = 0; i< twosetsOfStrings.Length; i++)
{
    var element = twosetsOfStrings[i];

    for(int j = 0; j < element.Item1.Length; j++)
    {
        element.Item1[j] = string.Concat(element.Item1[j].OrderBy (c => c));
    }

    for (int j = 0; j < element.Item2.Length; j++)
    {
        element.Item2[j] = string.Concat(element.Item2[j].OrderBy(c => c));
    }
}


//Note the sort order
Dictionary<string, int> charToDigitMap = new Dictionary<string, int>()
{
    { "abcefg", 0 },            // 6
    { "cf", 1 },                // 2 *
    { "acdeg", 2 },             // 5
    { "acdfg", 3 },             // 5
    { "bcdf", 4 },              // 4 *
    { "abdfg", 5 },             // 5
    { "abdefg", 6 },            // 6
    { "acf", 7 },               // 3 *
    { "abcdefg", 8 },           // 7 *
    { "abcdfg", 9 }             // 6
};

//HashSet<string> easyValues = new HashSet<string>() { "cf", "bcdf", "acf", "abcdefg" };
HashSet<int> easyValues = new HashSet<int>() { 2,3,4,7 };

int countEasyValues = 0;
foreach (var entry in twosetsOfStrings)
{
    foreach (var value in entry.Item2)
    {
        if (easyValues.Contains(value.Length)) countEasyValues++;
    }
}

Console.WriteLine("Part 1:" + countEasyValues);

Console.WriteLine();

// ** Part 2: Now figure out which parts are mixed up and what the real numbers should be...