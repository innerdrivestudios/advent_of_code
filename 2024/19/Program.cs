//Solution for https://adventofcode.com/2024/day/19 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of sub patterns and full patterns we need to create from those sub patterns

// ** First let's parse the input...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
string[] inputParts = myInput.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

string[] subPatterns = inputParts[0].Split(", ", StringSplitOptions.RemoveEmptyEntries).ToArray();
string[] fullPatterns = inputParts[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// ** Part 1: Given the list of full patterns and subpatterns, how many of the 
// full patterns can be made using the given subpatterns

// First define some helper methods:

// Simple method to test whether the given FullPattern contains the
// given SubPattern starting at the given StartIndex
bool SubPatternMatches (string pFullPattern, string pSubPattern, int pStartIndex)
{
	// If the sub pattern is too long to be able to match, return false
	if (pStartIndex + pSubPattern.Length > pFullPattern.Length) return false;

	// Test each char until a mismatch
	for (int i = 0; i < pSubPattern.Length; i++)
	{
		if (pFullPattern[pStartIndex + i] != pSubPattern[i]) return false;
	}

	// All chars passed
	return true;
}

// Now WITH that SubPattern match operations, we implement the main workhouse:

// CanMake recursively tests whether we can match the pFullPattern using the provided
// subpattern by trying to match the part starting at pIndex, followed by a CanMake test
// on everything that follows after it...

bool CanMake (string pFullPattern, string[] pSubPatterns, int pIndex = 0) {
	// If we were able to match the whole pattern, we are done!
	if (pIndex == pFullPattern.Length) return true;

	// Otherwise test whether we can match the fullpattern at pIndex using 
	// our subPatterns
    foreach (string subPattern in pSubPatterns)
    {
        if (
				SubPatternMatches(pFullPattern, subPattern, pIndex) &&
				CanMake(pFullPattern, pSubPatterns, pIndex + subPattern.Length)
		) return true;
	}

	return false;
}

Console.WriteLine("Part 1: " + fullPatterns.Count(x => CanMake(x, subPatterns)));

// ** Part 2: Basically do the same, but now count in HOW MANY WAYS each full pattern can be
// created from the sub patterns and calculate that sum. 

// Memoization table otherwise this won't be able to run at all

Dictionary<string, long> stringToWays = new Dictionary<string, long>();

long CanMakeWaysCount(string pFullPattern, string[] pSubPatterns, int pIndex = 0)
{
	if (pIndex == pFullPattern.Length) return 1;

	string subStringToMatch = pFullPattern.Substring(pIndex);
	if (stringToWays.TryGetValue(subStringToMatch, out long ways)) return ways;

	long canMakeCount = 0;

	for (int i = 0; i < pSubPatterns.Length; i++)
	{
		string pattern = pSubPatterns[i];
		int patternLength = pattern.Length;

		if (SubPatternMatches(pFullPattern, pattern, pIndex))
		{
			long newWaysFound = CanMakeWaysCount(pFullPattern, pSubPatterns, pIndex + patternLength);
			canMakeCount += newWaysFound;

			stringToWays[pFullPattern.Substring(pIndex + patternLength)] = newWaysFound;
		}
	}

	return canMakeCount;
}

Console.WriteLine("Part 2: "+fullPatterns.Sum (x => CanMakeWaysCount(x, subPatterns)));

