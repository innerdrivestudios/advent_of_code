// Solution for https://adventofcode.com/2023/day/1 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of messed up numbers combined with text such as "95three6threendpqpjmbpcblone"

string[] weirdStrings = ParseUtils.FileToArrayOf<string>(args[0], Environment.NewLine);

// ** Part 1 - Find the real numbers hidden in the weird texts, combine them and add them

Console.WriteLine("Part 1 - Sum all hidden numbers: " + SumFirstAndLastDigits(weirdStrings));

long SumFirstAndLastDigits(string[] pInput)
{
	//this expression says consume everything that is not a number followed by a number
	Regex regexp = new Regex(@"[^\d]*(\d+)");

	long sum = 0;

	foreach (string weirdString in pInput)
	{
		//match that combination as many times as possible
		MatchCollection matches = regexp.Matches(weirdString);

		//default values, use a char which doesn't occur in the string
		char firstDigit = ' ';
		char lastDigit = ' ';

		for (int i = 0; i < matches.Count; i++)
		{
			//if the match is the first, overwrite the first digit
            if (i == 0) firstDigit = matches[i].Groups[1].Value.First();
			//if the match is the last, overwrite the last digit
			if (i == matches.Count - 1) lastDigit = matches[i].Groups[1].Value.Last();
		}

		//add them together as a string and parse them!
		sum += long.Parse("" + firstDigit + lastDigit);
	}

	return sum;
}

//** Part 2: Now we don't only look for 01,2,3,4 etc but also for zero, one, two, etc

Console.WriteLine("Part 2 - Sum all numbers inc written ones: " + SumFirstAndLastDigitsIncWritten(weirdStrings));

long SumFirstAndLastDigitsIncWritten(string[] pInput)
{
	long sum = 0;

	string[] digitArray = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

	foreach(string weirdString in pInput)
	{
		(int digit, int index) firstDigit = (0, -1);
		(int digit, int index) lastDigit = (0, -1);

        // first scan for regular digits
        for (int i = 0; i < weirdString.Length; i++)
		{
			//if we found a digit and the first digit hasn't been set, set both the first and last digit
			//to an initial value. IF a value has been set only overwrite the last digit
			if (char.IsAsciiDigit(weirdString[i]))
			{
				if (firstDigit.index < 0) firstDigit = lastDigit = (weirdString[i] - '0', i);
				else lastDigit = (weirdString[i] - '0', i);
			}
		}

        //Console.WriteLine("Initial values: " + firstDigit + " / " + lastDigit);

		//now see if there is a written digit, earlier than first digit
        for (int i = 0; i < digitArray.Length; i++)
		{
			int digitIndex = weirdString.IndexOf(digitArray[i]);

			if (digitIndex > -1)
			{
				if (firstDigit.index < 0 || digitIndex < firstDigit.index)
				{
					firstDigit = (i, digitIndex);
					//Console.WriteLine("Setting first digit " + firstDigit);
					//if (lastDigit.index < 0) lastDigit = firstDigit;
				}
			}
		}

		//or a written digit later than the last digit
		for (int i = 0; i < digitArray.Length; i++)
		{
			int digitIndex = weirdString.LastIndexOf(digitArray[i]);

			if (digitIndex > -1)
			{
				if (lastDigit.index < 0 || digitIndex > lastDigit.index)
				{
					lastDigit = (i, digitIndex);
					//Console.WriteLine("Setting last digit " + lastDigit);
				}
			}
		}

        //Console.WriteLine("Final values: " + firstDigit + " / " + lastDigit);
		//Console.ReadKey();

        sum += (firstDigit.digit * 10 + lastDigit.digit);
	}

	return sum;
}

Console.ReadKey();

