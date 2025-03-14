// Solution for https://adventofcode.com/2018/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of weird strings

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] splitStrings = myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

//Part 1
// - Count how many times a char occurs exactly 2 or 3 times in a string (count both things, but only once)
// - Multiply those occurances together

int twoTimesCount = 0;
int threeTimesCount = 0;

foreach (string s in splitStrings)
{
	//Count every element and check if any of them is exactly two or three
	if (s.Select(a => s.Count(b => a == b)).Any(c => c == 2)) twoTimesCount++;
	if (s.Select(a => s.Count(b => a == b)).Any(c => c == 3)) threeTimesCount++;
}

Console.WriteLine("Part 1 - Checksum: " + twoTimesCount * threeTimesCount);

// Part 2 
// - Find the strings that only differ by one character
// - Find the common characters in those two strings

// Iterate over all string pairs
for (int i = 0; i < splitStrings.Length-1; i++)
{
	for (int j = i + 1; j < splitStrings.Length; j++)
	{
		//count the difference, while building a difference string (lot of redundant work)
		int difference = 0;
		string a = splitStrings[i];
		string b = splitStrings[j];
		string equalChars = "";

		for (int k = 0; k < a.Length; k++)
		{
			int delta = a[k] == b[k] ? 0 : 1;
			if (delta == 0) equalChars += a[k];
			difference += delta;
		}

		if (difference == 1)
		{
            Console.WriteLine("Part 2 - Checksum: " + equalChars);
			return;
        }
    }
}