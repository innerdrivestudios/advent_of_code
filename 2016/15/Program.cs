//Solution for https://adventofcode.com/2016/day/15 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of disc specifications e.g.
// Disc #1 has 13 positions; at time=0, it is at position 10.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Converting the input to a list of discs

List<Disc> ConvertInput (string pInput) {
	List<Disc> discs = new();

	Regex discParser = new Regex(@"Disc #\d has (\d+) positions; at time=0, it is at position (\d+).");
	foreach (Match match in discParser.Matches(myInput))
	{
		discs.Add(new Disc(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
	}
	
	return discs;
}

// Part 1: What is the first time you can press the button to get a capsule? (e.g where every disc is a 0)

int GetTime (List<Disc> pDiscs)
{
	int time = 0;

	while (true)
	{
		// Simulate the fall through process for the current set of discs
		// Every while loop we look at the current state and simulate the capsule falling
		// position + 0 + 1 for disc 1
		// position + 1 + 1 for disc 2 etc

		int total = 0;

		for (int i = 0; i < pDiscs.Count; i++)
		{
			Disc disc = pDiscs[i];
			total += (disc.position + i + 1) % disc.positions;
		}

		if (total == 0) break;

		// Then after, we move time forward by one for every disc

		for (int i = 0; i < pDiscs.Count; i++)
		{
			Disc disc = pDiscs[i];
			disc.position = (disc.position + 1) % disc.positions;
		}

		time++;
	}

	return time;
}

Console.WriteLine("Part 1 - First time where the sum of all discs is zero:" + GetTime(ConvertInput(myInput)));

// Part 2 : Same but with an additional disc

// Reset the discs...

List<Disc> discs = ConvertInput(myInput);
discs.Add (new Disc(11, 0));

Console.WriteLine("Part 2 - First time where the sum of all discs is zero:" + GetTime(discs));
