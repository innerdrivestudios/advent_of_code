// Solution for https://adventofcode.com/2023/day/1 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of messed up numbers :)

string[] weirdStrings = ParseUtils.FileToArrayOf<string>(args[0], Environment.NewLine);

// Task 1 - Find the numbers hidden in the weird texts, combine them and add them

Console.WriteLine("Part 1 - Sum all hidden numbers: " + SumFirstAndLastDigits(weirdStrings));

long SumFirstAndLastDigits(string[] pInput)
{
	Regex regexp = new Regex(@"[^\d]*(\d+)");

	long sum = 0;

	foreach (string weirdString in pInput)
	{
		MatchCollection matches = regexp.Matches(weirdString);

		char firstDigit = ' ';
		char lastDigit = ' ';

		for (int i = 0; i < matches.Count; i++)
		{
            if (i == 0) firstDigit = matches[i].Groups[1].Value.First();
			if (i == matches.Count - 1) lastDigit = matches[i].Groups[1].Value.Last();
		}

		sum += long.Parse("" + firstDigit + lastDigit);
	}

	return sum;
}

Console.WriteLine("Part 2 - Sum all numbers inc written ones: " + SumFirstAndLastDigitsIncWritten(weirdStrings));

long SumFirstAndLastDigitsIncWritten(string[] pInput)
{
	long sum = 0;

	string[] digitArray = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

	foreach(string weirdString in pInput)
	{
		(int digit, int index) firstDigit = (0, -1);
		(int digit, int index) lastDigit = (0, -1);

        //Console.WriteLine("Testing "+weirdString);

        // first scan for regular digits
        for (int i = 0; i < weirdString.Length; i++)
		{
			if (char.IsAsciiDigit(weirdString[i]))
			{
				if (firstDigit.index < 0) firstDigit = lastDigit = (weirdString[i] - '0', i);
				else lastDigit = (weirdString[i] - '0', i);
			}
		}

        //Console.WriteLine("Initial values: " + firstDigit + " / " + lastDigit);

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

