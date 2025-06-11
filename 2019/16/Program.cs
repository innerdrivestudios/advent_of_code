// Solution for https://adventofcode.com/2019/day/16 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a long line of int digits

using System.Diagnostics;
using System.Text;

int[] GetDigits (string pDigits)
{
	return pDigits.ReplaceLineEndings("").Select(x => int.Parse("" + x)).ToArray();
}

int[] basePattern = [0, 1, 0, -1];

// ** Part 1: Perform the FFT algorithm as described, perform it a 100 times and get the first 8 digits...

// First define some helpers methods...

// pDigits      -> the array with digits to perform the FFT algorithm on
// pResultStore -> the array to store the results in, to avoid constant allocation,
//                  we use a double buffering mechanism, constantly swapping the array
//                  being passed in, with the last array being updated

// Note:            pResultStore needs to be the same size as pDigits, even though not all numbers
//                  might valid 

void FFT (int[] pDigits, int[] pResultStore)
{
    for (int i = 0; i < pDigits.Length; i++)
    {
        pResultStore[i] = CalculateDigit(pDigits, i);
    }
}

int CalculateDigit (int[] pDigits, int pDigitIndex)
{
    //Console.WriteLine();
    //Console.Write($"[{pDigitIndex}] => ");
    int total = 0;

    for (int i = 0; i < pDigits.Length; i++)
    {
        int baseMultiplier = basePattern[((i+1) / (pDigitIndex + 1)) % basePattern.Length];
		//Console.Write(""+ baseMultiplier + "*" + pDigits[i] + " ");
        total += pDigits[i] * baseMultiplier;
    }
    return int.Abs(total % 10);
}

int[] GetPart1Result(int[] pDigits, int pIterations)
{
	int[] result = new int[pDigits.Length];

	for (int i = 0; i < pIterations; i++)
	{
		FFT(pDigits, result);

		int[] tmp = result;
		result = pDigits;
		pDigits = tmp;
    }

    return pDigits;
}

Console.WriteLine();

//string inputString = "123456789123456789";
string inputString = File.ReadAllText(args[0]);
int[] inputDigits = GetDigits(inputString);
Console.WriteLine("Part 1: " + string.Concat(GetPart1Result(inputDigits, 100).Take(8)));

// ** Part 2:

// Our original input string is 650
// Times * 10.000 = 6.500.000
// Our original process loops over 6.500.000 numbers to calculate 6.500.000 numbers...
// And that a 100 times... in other words, that is a lot...

// HOWEVER if you look at the pattern, the first x-1 numbers while calculating digit x are multiplied with 0
// The second x numbers * 1, the 3rd set of x * 0 and the last set of x times - 1.

// Looking at our first 7 digits of our input (5974901 in my case), this means:
// 5974900 zeroes
// 5974901 ones...
// 5974901 nobody cares, since we're already waayyy passed the total of 6.500.000 numbers...
//
// Now if you look at the build of say 18 numbers, it looks like this:

/*
[0] => 1*1 0*2 -1*3 0*4 1*5 0*6 -1*7 0*8 1*9 0*1 -1*2 0*3 1*4 0*5 -1*6 0*7 1*8 0*9
[1] => 0*1 1*2 1*3 0*4 0*5 -1*6 -1*7 0*8 0*9 1*1 1*2 0*3 0*4 -1*5 -1*6 0*7 0*8 1*9
[2] => 0*1 0*2 1*3 1*4 1*5 0*6 0*7 0*8 -1*9 -1*1 -1*2 0*3 0*4 0*5 1*6 1*7 1*8 0*9
[3] => 0*1 0*2 0*3 1*4 1*5 1*6 1*7 0*8 0*9 0*1 0*2 -1*3 -1*4 -1*5 -1*6 0*7 0*8 0*9
[4] => 0*1 0*2 0*3 0*4 1*5 1*6 1*7 1*8 1*9 0*1 0*2 0*3 0*4 0*5 -1*6 -1*7 -1*8 -1*9
[5] => 0*1 0*2 0*3 0*4 0*5 1*6 1*7 1*8 1*9 1*1 1*2 0*3 0*4 0*5 0*6 0*7 0*8 -1*9
[6] => 0*1 0*2 0*3 0*4 0*5 0*6 1*7 1*8 1*9 1*1 1*2 1*3 1*4 0*5 0*6 0*7 0*8 0*9
[7] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 1*8 1*9 1*1 1*2 1*3 1*4 1*5 1*6 0*7 0*8 0*9
[8] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 1*9 1*1 1*2 1*3 1*4 1*5 1*6 1*7 1*8 0*9
[9] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 1*1 1*2 1*3 1*4 1*5 1*6 1*7 1*8 1*9
[10] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 1*2 1*3 1*4 1*5 1*6 1*7 1*8 1*9
[11] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 1*3 1*4 1*5 1*6 1*7 1*8 1*9
[12] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 1*4 1*5 1*6 1*7 1*8 1*9
[13] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 0*4 1*5 1*6 1*7 1*8 1*9
[14] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 0*4 0*5 1*6 1*7 1*8 1*9
[15] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 0*4 0*5 0*6 1*7 1*8 1*9
[16] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 0*4 0*5 0*6 0*7 1*8 1*9
[17] => 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 0*9 0*1 0*2 0*3 0*4 0*5 0*6 0*7 0*8 1*9 
*/

// In other words: if we go forward, we have to loop over EVERY DIGIT for EVERY DIGIT!
// BUT if we go BACKWARD, we can calculate digit x-1 by using the result of digit x
// Which turns O(n^2*m) into O(n*m), which would already be better...

// Let's see if we can put that in practice...

Console.WriteLine("");
Console.WriteLine("Starting part 2...");

Console.WriteLine("Original input string length:" + inputString.Length);

StringBuilder newInputStringBuilder = new StringBuilder();
for (int i = 0; i < 10000; i++) newInputStringBuilder.Append(inputString);

Console.WriteLine("New input string length:" + newInputStringBuilder.Length);

inputString = newInputStringBuilder.ToString();
int digitsToSkip = int.Parse(inputString.Substring(0, 7));
Console.WriteLine("Digits to skip:" + digitsToSkip);
inputDigits = GetDigits(inputString);

Stopwatch stopWatch = Stopwatch.StartNew();

for (int  i = 0;  i < 100; i++)
{
    //Console.Write(".");
    int runningTotal = 0;
    for (int j = inputDigits.Length - 1; j >= digitsToSkip; j--)
    {
        //No abs needed, since all numbers will be positive in the range we are looking at
        runningTotal = (runningTotal + inputDigits[j]) % 10;
        inputDigits[j] = runningTotal;
    }
}

Console.WriteLine("");

Console.WriteLine("Part 2: " + string.Concat(inputDigits.Skip(digitsToSkip).Take(8)));
Console.WriteLine("Part 2 took " + stopWatch.ElapsedMilliseconds + " ms");





