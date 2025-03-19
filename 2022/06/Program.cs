// Solution for https://adventofcode.com/2022/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a stream of characters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 & 2:
// Get the character index where we detect X DIFFERENT characters in a sliding window for the first time...

int GetStartOfPacketMarkerIndex (string pInput, int pMarkerLength)
{
	// Going to use a really lazy approach for this ...
	LinkedList<char> fourCharMarker = new ();

	for (int i = 0; i < pInput.Length; i++)
	{
		fourCharMarker.AddLast(pInput[i]);
		if (fourCharMarker.Count > pMarkerLength) fourCharMarker.RemoveFirst();

		if (fourCharMarker.Distinct().Count() == pMarkerLength)
		{
			//We want the NEXT index (according to the specs)
			return i + 1;
		}
	}

	return -1;
}

Console.WriteLine("Part 1 - Packet index:" + GetStartOfPacketMarkerIndex(myInput, 4));
Console.WriteLine("Part 2 - Packet index:" + GetStartOfPacketMarkerIndex(myInput, 14));