// Solution for https://adventofcode.com/2025/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ranges and numbers

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
string[] myInputParts = myInput.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries| StringSplitOptions.TrimEntries);

List<(long, long)> ranges = ParseUtils.StringToTuples<long, long>(myInputParts[0], "-");
List<long> ingredients = myInputParts[1].Split(Environment.NewLine).Select(long.Parse).ToList();

// ** Part 1: How many of the list ingredients fall within the given ranges?

HashSet<long> freshIngredients = new ();

foreach (long number in ingredients)
{
	foreach (var range in ranges)
	{
		if (number >= range.Item1 && number <= range.Item2) freshIngredients.Add(number);
	}
}

Console.WriteLine("Part 1: " + freshIngredients.Count);

// ** Part 2: How many ingredients fall within the given ranges overall?

// Now we want to collapse / join overlapping ranges ...
// Fastest way to do that is to first sort on the start of the range...

ranges.Sort((x, y) => x.Item1.CompareTo(y.Item1));

// And then actually collapse the ranges ...

for (int i = 0; i < ranges.Count - 1; i++)
{
	for (int j = i + 1; j < ranges.Count;)
	{
		// if the end of the first range we are checking is equal or
		// goes past the start of the second, merge them

		if (ranges[i].Item2 >= ranges[j].Item1)
		{
			ranges[i] = (ranges[i].Item1, Math.Max(ranges[i].Item2, ranges[j].Item2));
			ranges.RemoveAt(j);
		}
		else
		{
			j++;
		}
	}
}

// Sum the ranges (inclusive)
Console.WriteLine("Part 2: " + ranges.Sum (x => x.Item2 - x.Item1 + 1));
