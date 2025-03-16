// Solution for https://adventofcode.com/2022/day/3 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: strings that describe rucksack content, each char is an item
// A string contains two halves, first half describes compartment 1, second half compartment 2
// Lowercase item types a through z have priorities 1 through 26.
// Uppercase item types A through Z have priorities 27 through 52.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

List<(string , string)> pairs = myInput
    .Split("\r\n", StringSplitOptions.RemoveEmptyEntries)
    .Select (x => (x.Substring(0, x.Length/2), x.Substring (x.Length/2)))
    .ToList ();

// ** Part 1 - Per backpack, find the item that is in both compartments, get its priority and multiply the 
// priorities of all duplicate items over all backpacks

List<char> duplicateItems = pairs.Select (x => x.Item1.Intersect(x.Item2).First()).ToList ();

// Now that we have all duplicate items, get their priorities (could be done in one call, but that's pretty unreadable):
// Note the space, this is required to make sure a == 1 and not 0

string lookupTable = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

List<int> priorities = duplicateItems.Select(x => lookupTable.IndexOf(x)).ToList ();

Console.WriteLine("Part 1 - Sum of all priorities: " + priorities.Sum());

// ** Part 2 - More of the same, but now we need to group three lines at a time instead of half of the "bag"

string[] bags = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

List<int> badgePriorities = new List<int> ();

for (int i = 0; i < bags.Length; i += 3)
{
    //Get the intersection of 3 bags, take the first element (to convert it to a char)
    //And then look up its index in the lookup table
    badgePriorities.Add( lookupTable.IndexOf ( bags[i].Intersect(bags[i + 1]).Intersect(bags[i + 2]).First()) );
}

Console.WriteLine("Part 2 - Sum of all priorities: " + badgePriorities.Sum());

