// Solution for https://adventofcode.com/2018/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of + and - offsets

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - Calculate the end result when adding all of these numbers, using 0 as a starting point

List<int> ints = myInput
    .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
    .Select(int.Parse)
    .ToList();

Console.WriteLine("Part 1 -> " + ints.Sum());

// ** Part 2 - Keep on processing the numbers in the list until you encounter a duplicate

Console.WriteLine("Part 2 -> " + FindFirstDuplicateFrequency(ints));

int FindFirstDuplicateFrequency(List<int> ints)
{
    HashSet<int> frequencies = new HashSet<int>();

    int sum = 0;
    int index = 0;

    while (true)
    {
        sum += ints[index];

        if (!frequencies.Add(sum)) return sum;

        index++;
        index %= ints.Count; 
    }
}

Console.ReadKey();
