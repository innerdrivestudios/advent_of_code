// Solution for https://adventofcode.com/2020/day/2 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a password list with restrictions (required charcount, char, password)

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] passwordRequirements = myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Part 1 - Count how many passwords meet the requirements:
// - 6-9 z: fsderzvxcvwer indicates that in order for the given password to be valid
//   it needs to contain 6-9 occurances of the given character

int GetValidPasswordCountPart1(string[] pPasswordRequirements)
{
	int validPasswordCount = 0;
	Regex passwordParser = new Regex(@"(\d+)-(\d+) ([a-z]): (.+)");

	foreach (string passwordRequirement in pPasswordRequirements)
	{
		Match match = passwordParser.Match(passwordRequirement);
		if (match.Success)
		{
			int min = int.Parse(match.Groups[1].Value);
			int max = int.Parse(match.Groups[2].Value);
			char c = match.Groups[3].Value[0];
			string password = match.Groups[4].Value;

			int charCount = password.Count(x => x == c);
			if (charCount >= min && charCount <= max) validPasswordCount++;
		}
	}

	return validPasswordCount;
}

Console.WriteLine("Part 1 - Valid password count: " + GetValidPasswordCountPart1(passwordRequirements));

// Part 2 - Count how many passwords meet the new requirements:
// - Each policy actually describes two positions in the password,
//   where 1 means the first character, 2 means the second character, and so on.
//   (Be careful; Toboggan Corporate Policies have no concept of "index zero"!)
//   Exactly one of these positions must contain the given letter.
//   Other occurrences of the letter are irrelevant for the purposes of policy enforcement.

int GetValidPasswordCountPart2(string[] pPasswordRequirements)
{
	int validPasswordCount = 0;
	Regex passwordParser = new Regex(@"(\d+)-(\d+) ([a-z]): (.+)");

	foreach (string passwordRequirement in pPasswordRequirements)
	{
		Match match = passwordParser.Match(passwordRequirement);
		if (match.Success)
		{
			int position1 = int.Parse(match.Groups[1].Value);
			int position2 = int.Parse(match.Groups[2].Value);
			char c = match.Groups[3].Value[0];
			string password = match.Groups[4].Value;

			int matchCount =
				(password[position1-1] == c ? 1 : 0) +
				(password[position2-1] == c ? 1 : 0);

			validPasswordCount += matchCount == 1 ? 1 : 0;
		}
	}

	return validPasswordCount;
}

Console.WriteLine("Part 2 - Valid password count according to new rules: " + GetValidPasswordCountPart2(passwordRequirements));