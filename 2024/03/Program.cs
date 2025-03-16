// Solution for https://adventofcode.com/2024/day/3 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of weird multiply instructions, intermingled with do/don't sequences

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - Find all the REAL mul instructions, execute them and add the results

int total = 0;
Regex mulInstructionParser = new Regex(@"mul\((\d+),(\d+)\)");

foreach (Match match in mulInstructionParser.Matches(myInput))
{
	total += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
}

Console.WriteLine("Part 1 - Multiplications summed: " + total);

// ** Part 2 - Similar to the previous one, BUT now we use the do and don't statements
// in the input to accept or ignore mul statements, starting in "accept" mode.

// First set up a matcher for mul / do and don't 
MatchCollection matches = Regex.Matches(myInput, @"mul\(\d+,\d+\)|do\(\)|don't\(\)");

bool enabled = true;
total = 0;

// Then for all of those matches, handle them accordingly
foreach (Match match in matches)
{
	if (match.Value.StartsWith("do()")) enabled = true;
	else if (match.Value.StartsWith("don't()")) enabled = false;

	// If it's a `mul(x,y)` instruction, extract the numbers using the regexp from part 1
	else if (match.Value.StartsWith("mul") && enabled)
	{
		Match mulMatch = mulInstructionParser.Match(match.Value);
		total += int.Parse(mulMatch.Groups[1].Value) * int.Parse(mulMatch.Groups[2].Value);
	}
}

Console.WriteLine("Part 2 - Alternative mul instruction parse total: " + total);
