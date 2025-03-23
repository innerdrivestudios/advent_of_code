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

/*
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
*/

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

// So given a (string[], string[]) set, what we need is a mapping of all the elements on the left side to
// an actual number... e.g. if var a = (string[], string[]), then we need to map all elements in a.Item1
// The result will be a dictionary of strings to numbers.
// Having achieved that we can decipher the strings on the right, e.g. in a.Item2
// AND having all deciphered numbers, we can add them all together and we can solve the puzzle...
// 
// Do we even need to know how the actual wires have been crossed ?
// Or is knowing which pattern maps to which number enough?

// Consider the following table and the description below it:
//
//     *  **
//    8687497
//  0-abc efg   => 6
//  1-  c  f    => 2*
//  2-a cde g   => 5
//  3-a cd fg   => 5
//  4- bcd f    => 4*
//  5-ab d fg   => 5
//  6-ab defg   => 6
//  7-a c  f    => 3*
//  8-abcdefg   => 7*
//  9-abcd fg   => 6

// On the left are all digits, followed by the string that represents them seperated by a -
// On the right are the amount of chars in that string, followed by an * if that amount is unique
// At the top is that occurance count of a specific char over all strings, again with an * on top if 
// it is unique.

// The question is: given this information can we figure out which string maps to which number?
// I wrote a solver class to offload the complexity of this class, 

// If you run this with the test data you can see how it works

long totalValue = 0;
foreach (var value in twosetsOfStrings)
{
    Solver solver = new Solver(value);
    //solver.PrintSolvedTable();
    totalValue += solver.GetValue();
    //Console.WriteLine(solver.GetValue());
}

Console.WriteLine("Part 2: " + totalValue);
