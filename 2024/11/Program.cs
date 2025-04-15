//Solution for https://adventofcode.com/2024/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input the algorithm uses by going to
// Debug/Debug Properties and specifying the input as a command line argument,
// e.g. "125 17" (include the quotes!!)
// This value will be passed to the built-in args[0] variable

List<long> myInput = args[0]
	.Trim('"')
	.Split(" ", StringSplitOptions.RemoveEmptyEntries)
	.Select(long.Parse)
	.ToList();

// Every time you blink, the stones each simultaneously change according to the
// first applicable rule in this list:

// - If the stone is engraved with the number 0,
//   it is replaced by a stone engraved with the number 1.
// - If the stone is engraved with a number that has an even number of digits,
//   it is replaced by two stones. The left half of the digits are engraved on
//   the new left stone, and the right half of the digits are engraved on the
//   new right stone. (The new numbers don't keep extra leading zeroes:
//   1000 would become stones 10 and 0.)
// - If none of the other rules apply, the stone is replaced by a new stone;
//   the old stone's number multiplied by 2024 is engraved on the new stone.

// ** Part 1: We simply follow the rules in a naive linkedlist implementation.

long BlinkXTimes (List<long> pNumbers, int pX)
{
	LinkedList<long> numbers = new LinkedList<long>(pNumbers);

	for (int i = 1; i <= 25; i++) Blink(numbers);

	return numbers.Count;
}

void Blink(LinkedList<long> pNumbers)
{
	LinkedListNode<long> start = pNumbers.First;
	
	while (start != null)
	{
		long value = start.Value;

		//Math.Floor(Log10 (0-9)) + 1 -> 1
		//Math.Floor(Log10 (10-99)) + 1 -> 2
		//etc
		int nrsOfDigits = (int)Math.Floor(Math.Log10(value) + 1);

		if (value == 0)
		{
			start.Value = 1;
        }
		else if (nrsOfDigits % 2 == 0)
		{ 
			//Cut the number in two...
			int digits = (int)Math.Pow(10, nrsOfDigits / 2);
			long leftStone = value / digits;
			long rightStone = value % digits;

			start.Value = leftStone;
			pNumbers.AddAfter(start, rightStone);

			start = start.Next;
        }
		else
		{
			start.Value = value * 2024;
		}

        start = start.Next;
	}
}

Console.WriteLine("Part 1 - " + BlinkXTimes(myInput, 25));

// ** Part 2: Same but now for 75 iterations...
// Issue is space complexity is insane for this algorithm,
// so our naive list approach doesn't work anymore.
//
// Instead we'll use a recursive prediction method, where we'll take of number
// handle it, count the resulting numbers from that at a specific level
// and apply memoization to speed it up:

// We store the value for which we want to determine
// in how many numbers it will split at the level we derived its value
// since the amount of times we still HAVE to split it will determine the outcome
Dictionary<(long, int), long> totalTable = new Dictionary<(long, int), long>();

long BlinkXTimesOptimized (List<long> pNumbers, int pX)
{
	long total = 0;

	foreach (long number in pNumbers)
	{
		//Blink each single number 75 times...
		total += BlinkOptimized(number, 0, 75);
	}

	return total;
}

long BlinkOptimized (long pValue, int pCurrentDepth, int pMaxDepth)
{
	//if we have reached the max depth, we don't split the number anymore
	//so the given value is the given value -> only 1 value
	if (pCurrentDepth >= pMaxDepth) return 1;

	long total = 0;
	if (totalTable.TryGetValue((pValue, pCurrentDepth), out total)) return total;

	int nrsOfDigits = (int)Math.Floor(Math.Log10(pValue) + 1);

	if (pValue == 0)
	{
		//simply replace the value
		total = BlinkOptimized(1, pCurrentDepth+1, pMaxDepth);
	}
	else if (nrsOfDigits % 2 == 0)
	{
		//get the result of two values
		int digits = (int)Math.Pow(10, nrsOfDigits / 2);
		long leftStone = pValue / digits;
		long rightStone = pValue % digits;
		total = BlinkOptimized(leftStone, pCurrentDepth + 1, pMaxDepth) + BlinkOptimized(rightStone, pCurrentDepth + 1, pMaxDepth);
	}
	else
	{
		//get the result of 1024 * the value
		total = BlinkOptimized(pValue * 2024, pCurrentDepth + 1, pMaxDepth);
	}

	//apply memoization
	totalTable[(pValue,  pCurrentDepth)] = total;	

	return total;
}

Console.WriteLine("Part 2 - " + BlinkXTimesOptimized(myInput, 75));


