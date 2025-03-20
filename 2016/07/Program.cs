//Solution for https://adventofcode.com/2016/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of super secret strings representing IPv7 (yes you read that right:))
// ip addresses (or not) with certain properties.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] stringsToScan = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

Console.WriteLine("Part 1 - TLS Supporting IP count: " + stringsToScan.Where (SupportsTLS).Count());
Console.WriteLine("Part 1 - SLS Supporting IP count: " + stringsToScan.Where (SupportsSLS).Count());
Console.ReadKey();

bool SupportsTLS (string pInput)
{
	//we could probably also do this with regular expressions, or at least part of it,
	//but that would probably just make it more complicated/unreadable

	//index 0 is outside [], 1 is inside []
	bool[] abbaFound = [false, false];
	int abbaFoundIndex = 0;

	//Confusing ain't it ;) but basic idea every string is aaa[aaa]aaaa[aaaa]aaaa,
	//so split the substrings on [] for easier checking
	string[] subStringsToScan = pInput.Split(['[', ']']);

	for (int subStringIndex = 0;  subStringIndex < subStringsToScan.Length; subStringIndex++)
	{
		//every even index is outside of [], even odd index is inside []
		abbaFoundIndex = subStringIndex % 2;
		string subString = subStringsToScan[subStringIndex];
		
		for (int i = 0; i < subString.Length - 3; i++)
		{
			if (
				subString[i] == subString[i+3] &&		//a__a == 
				subString[i+1] == subString[i+2] &&		//_bb_ ==
				subString[i] != subString[i+1]			//aa__ !=
				)
			{
				abbaFound[abbaFoundIndex] = true;
				break;
			}
		}

		//as soon as there is an ABBA within brackets, exit the loop completely
		if (abbaFound[1]) break;
	}

	return abbaFound[0] && !abbaFound[1];
}

bool SupportsSLS(string pInput)
{
	//we could probably also do this with regular expressions, or at least part of it
	//but that would probably just make it more complicated/unreadable

	//Slightly other approach for this one, we'll store the outerstrings & innerStrings found
	//and see if we can find a match with an inner string 
	HashSet<string> outerABASequences = new HashSet<string>();
	HashSet<string> innerABASequences = new HashSet<string>();
	
	//Confusing ain't it ;) But basic idea every string is aaa[aaa]aaaa[aaaa]aaaa,
	//so split the substrings on [] for easier checking
	string[] subStringsToScan = pInput.Split(['[', ']']);

	for (int subStringIndex = 0; subStringIndex < subStringsToScan.Length; subStringIndex++)
	{
		string subString = subStringsToScan[subStringIndex];

		for (int i = 0; i < subString.Length - 2; i++)
		{
			//try and find an ABA sequence ....
			if (
				subString[i] == subString[i + 2] &&     //a_a == 
				subString[i] != subString[i + 1]        //aa_ !=
				)
			{
				//if outside of [] store the string
				if (subStringIndex % 2 == 0)
				{
					//store aba
					outerABASequences.Add(subString.Substring(i, 3));
				}
				else 					
				{
					//if inside of [] and we have a match, record the sequence in reverse
					//basically aba from the bab that we have and store it
					//so we can intersect it with the actual outer sequences later
					string babSequence = "" + subString[i + 1] + subString[i] + subString[i + 1];
					innerABASequences.Add(babSequence);
				}
			}
		}
	}

	//check if there is any sequence in both the inner and the outer sequences table
	return outerABASequences.Intersect(innerABASequences).Count() > 0;
}
