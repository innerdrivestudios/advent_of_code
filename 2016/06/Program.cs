//Solution for https://adventofcode.com/2016/day/6 (Ctrl+Click in VS to follow link)


// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of weird strings

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] codedStrings = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine("Part 1:"+DecodeString(codedStrings, true));
Console.WriteLine("Part 2:"+DecodeString(codedStrings, false));

Console.ReadKey();

string DecodeString(string[] pInput, bool pMax)
{
	//assume every string has the same length, so get length based on the first string
	int characterCount = pInput[0].Length;

	string decodedString = "";

	//now for every "column"
	for (int character = 0; character < characterCount; character++)
	{
		//collect a count of each char		
		Dictionary<char, int> charCount = new();

		for (int word = 0; word < pInput.Length; word++)
		{
			char c = pInput[word][character];
			charCount.TryGetValue(c, out int count);
			charCount[c] = count + 1;
		}

		//and based on the max param find the max or min occurring char
		int maxOrMin = pMax ? charCount.Values.Max() : charCount.Values.Min();
		//from the dictionary select the key of the first item where the value matches our maxMin int
		decodedString += charCount.Where (x => x.Value == maxOrMin).First().Key;
	}

	return decodedString;
}
