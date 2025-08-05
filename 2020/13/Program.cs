// Solution for https://adventofcode.com/2020/day/13 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a timestamp and a sequence of buslines

using System.Diagnostics;

string[] myInput = File.ReadAllLines(args[0]);

long earliestTimestamp = long.Parse(myInput[0]);
long[] busLines = ConvertStringToBusLineArray(myInput[1]);

long[] ConvertStringToBusLineArray (string pInput) {
	return pInput
	.Replace('x', '0')
	.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
	.Select(long.Parse)
	.ToArray();
}

(long time, long busLine) FindBusAndDepartureTime (long pEarliestTime, long[] pBusLines)
{
	long[] busLines = pBusLines.Where(x => x!= 0).ToArray();

	while (true)
	{
		foreach (long busLine in busLines)
		{
			if (pEarliestTime % busLine == 0)
			{
				return (pEarliestTime, busLine);
			}
		}
		pEarliestTime++;
	}
}

Stopwatch stopwatch = Stopwatch.StartNew();
var busLineAndDepartureTime = FindBusAndDepartureTime(earliestTimestamp, busLines);
Console.WriteLine("Part 1:"+(busLineAndDepartureTime.time - earliestTimestamp) * busLineAndDepartureTime.busLine);
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " ms.");

// ** Part 2: What is the earliest timestamp such that all of the listed bus IDs
// depart at offsets matching their positions in the list?

/** 

// First a brute force approach, so we can experiment...

// pEarliestTime is the minimum time we want to get back
// buslines are all the buslines
// offset multiplier is to experiment with 0 offset, or higher offsets easily
long GetSynchronizedTimestamp (long pEarliestTime, long[] pBusLines, long pOffsetMultiplier = 1)
{
	//First we'll create two optimized lists where we condense the buslines and their offsets
	List<long> factors = new();
	List<long> offsets = new();

	int biggestIndex = -1;

	for (long i = 0; i < pBusLines.Length; i++)
	{
		if (pBusLines[i] == 0) continue;

		factors.Add(pBusLines[i]);
		offsets.Add(i);
		if (biggestIndex == -1 || factors[factors.Count - 1] > factors[biggestIndex]) biggestIndex = factors.Count - 1;
	}

	// Now we'll run through the algorithm again, but this time using offsets...
	// The smallest stepsize is determined by the biggest factor (highest busline)
	// (smaller than that and things won't align anyway)

	long iteration = pEarliestTime / factors[biggestIndex];
	iteration = long.Max(0, iteration - 1);

	while (true)
	{
		//we start at a value that should provide us with a match at the biggest factor
		long timestamp = (iteration * factors[biggestIndex]) - offsets[biggestIndex] * pOffsetMultiplier;

		if (timestamp < pEarliestTime)
		{
			iteration++;
			continue;
		}

		//check for full match
		bool match = true;
		for (int i = 0; i < factors.Count; i++)
		{
			if ((timestamp + offsets[i] * pOffsetMultiplier) % factors[i] != 0)
			{
				match = false;
				break;
			}
		}

		if (match) return timestamp;
		iteration++;
	}
}

void RunTestData(long[] pBuslines, long pEarliestTime, long pExpectedValue, long pTimeOffsetMultiplier = 1)
{
	long timeStamp1 = GetSynchronizedTimestamp(pEarliestTime, pBuslines, pTimeOffsetMultiplier);
	Console.ForegroundColor = pExpectedValue == timeStamp1 ? ConsoleColor.Green: ConsoleColor.Red;	
    Console.WriteLine("Running test on: " + string.Join(",",pBuslines)+ " => Expected vs result:" +pExpectedValue+"/"+timeStamp1);

	Console.ForegroundColor = ConsoleColor.White;
	long timeStamp2 = GetSynchronizedTimestamp(timeStamp1+1, pBuslines, pTimeOffsetMultiplier);
	long aggregate = pBuslines.Where(x => x != 0).Aggregate((x,y) => x * y);
    Console.WriteLine("Running cycle detection:" + (timeStamp2-timeStamp1) + " vs expected " + aggregate);
}


Console.WriteLine();
Console.WriteLine();

//0 = from time 0, xxxx = expected result, 1 is time offset applied as advertised
RunTestData(ConvertStringToBusLineArray("17, x, 13, 19"), 0, 3417,1);
RunTestData(ConvertStringToBusLineArray("67,7,59,61"), 0, 754018,1);
RunTestData(ConvertStringToBusLineArray("67,x,7,59,61"), 0, 779210,1);
RunTestData(ConvertStringToBusLineArray("67,7,x,59,61"), 0, 1261476,1);
RunTestData(ConvertStringToBusLineArray("1789,37,47,1889"), 0, 1202161486,1);
RunTestData(ConvertStringToBusLineArray("7,13,x,x,59,x,31,19"), 0, 1068781,1);

Console.WriteLine();
Console.WriteLine();

// Commented out all this experimentation data, see conclusions below

RunTestData([1], 0, 0, 0);
RunTestData([1,3], 0, 0, 0);
RunTestData([1,3,5], 0, 0, 0);

Console.WriteLine();
RunTestData([1], 1, 1,0);
RunTestData([1, 3], 1, 3,0);
RunTestData([1, 3, 5], 1,15,0);
RunTestData([1, 3, 5, 7], 1, 105,0);

Console.WriteLine();
RunTestData([3, 5, 7], 1, 105,0);
RunTestData([5, 7, 3], 1, 105,0);
RunTestData([7, 3, 5], 1, 105,0);
RunTestData([5, 3, 7], 1, 105,0);

// Conclusion so far... if we don't apply the time offset, and all our numbers
// are prime... the earliest time after 0 is equal to cycle size which is the product
// of all prime numbers...

// Now how about if we do apply the time offset...
Console.WriteLine();

//Time scale value of 1...
RunTestData([3, 5, 7], 1, 105, 1);
Console.WriteLine();

//Showing that adding one additional zero between every value is same as doubling timescale 
RunTestData([3,0,5,0,7], 1, 105, 1);
RunTestData([3, 5, 7], 1, 105, 2);
Console.WriteLine();
RunTestData([3, 0,0, 5,0, 0, 7], 1, 105, 1);
RunTestData([3, 5, 7], 1, 105, 3);

//Notice that cycle remains the same no matter what... as long as primes stay the same
RunTestData([0,5,0,7,0,0,3,0,0], 1, 105, 1);
RunTestData([0,5,0,7,0,0,3,0,11], 1, 5*7*3*11, 1);

Console.Clear();

//Now let's try and figure out the relation between delay and numbers...
RunTestData([5,0], 1, 5, 1);
RunTestData([0,5], 1, 4, 1);
RunTestData([0,0,5], 1, 3, 1);
Console.WriteLine();

RunTestData([7,0], 1, 7, 1);
RunTestData([0,7], 1, 6, 1);
RunTestData([0,0,7], 1, 5, 1);
Console.WriteLine();
RunTestData([5,7], 1, 5*6, 1);
RunTestData([5,7], 1, 5*7, 0);

RunTestData([0,3], 1, 0, 1);
RunTestData([0,0,5], 1, 0, 1);
RunTestData([0,0,0,7], 1, 0, 1);
RunTestData([3,5,7], 1, 54, 1);

Console.WriteLine();
RunTestData([3,5], 1, 54, 1);
RunTestData([3,0,5], 1, 54, 1);
RunTestData([3,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,0,5], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,0,0,5], 1, 54, 1);

Console.WriteLine();
RunTestData([3,7], 1, 54, 1);
RunTestData([3,0,7], 1, 54, 1);
RunTestData([3,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,0,7], 1, 54, 1);
RunTestData([3,0,0,0,0,0,0,0,0,7], 1, 54, 1);

RunTestData([3,5], 1, 54, 1);
RunTestData([0,5,7], 1, 54, 1);

// Some more interesting tests and conclusions:

// With no time delay, 3 & 5 repeat at 3 * 5
Console.WriteLine(GetSynchronizedTimestamp(1, [3,5],0));
// With no time delay, 5 & 7 repeat at 5 * 7 (note all given numbers are prime numbers)
Console.WriteLine(GetSynchronizedTimestamp(1, [5,7],0));
// With no time delay, 3 & 5 & 7 repeat at 3 * * 7 (note all given numbers are prime numbers)
Console.WriteLine(GetSynchronizedTimestamp(1, [3, 5, 7], 0));

// But what if we do use a delay? This prints 9.
// Why? Since the first time 3 and 5 with an initial delay of 1 line up,
// is when 3 and 9 line up... at 9
Console.WriteLine(GetSynchronizedTimestamp(1, [3, 5], 1));

// What if there is one additional delay? This prints 3.
// Why? Since the first time 3 and 5-2 (=3) line up is at 3.
Console.WriteLine(GetSynchronizedTimestamp(1, [3, 0, 5], 1));

// How about this one? This prints 54. 
// Why? Since the first time 3 and 5 with a delay of 1 and 7 with a delay of 2
// line up is at 54. Now the question is, can we deduce this without brute forcing it?
Console.WriteLine(GetSynchronizedTimestamp(1, [3, 5, 7], 1));

// Not entirely, but we can go a long way towards an answer by approaching this 
// pair by pair. For example, check out these tests/values and the code below it:
Console.WriteLine(GetSynchronizedTimestamp(1, [3, 5], 1));		// prints 9
Console.WriteLine(GetSynchronizedTimestamp(1, [0, 5, 7], 1));	// prints 19

/**/

// (3,0) & (5,5-1) in the setup above denoted as (base, initialValue) repeats at 9
// (5,1) & (7,7-2) in the setup above denoted as (base, initialValue) repeats at 19
// Now we know 3 & 5 repeat every 15, and 5 & 7 every 35, so we can also ask:
// when do (3*5, 9) and (5*7, 19) repeat? We already know that answer, 54
// But this required way less steps, although being way more complicated, 
// for which we can use the method below:

long FirstRepeat ((long baseNr, long shiftedStart) pA, (long baseNr, long shiftedStart) pB)
{
	long a = pA.shiftedStart;
	long b = pB.shiftedStart;

	while (a != b)
	{
		//let's say a = 5 and b-a = 300 and a.baseNr = 65
		// ((b-a)/65) * 65

		if (a < b) a += long.Max(1,((b-a)/pA.baseNr)) * pA.baseNr;
		else b += long.Max (1,((a-b)/pB.baseNr)) * pB.baseNr;

        //Console.WriteLine(a + " vs " + b);
    }

	return a;
}

/** 

// Working through the example above again without using this slight less brute force method:

Console.WriteLine(FirstRepeat((3,0),(5,4)));
Console.WriteLine(FirstRepeat((5,4),(7,5)));
Console.WriteLine(FirstRepeat((3*5,9),(5*7,19)));

// Let's work through a more complicated example, using a slightly different setup:
// Values:			 1789,	37,	47,	1889
// With delays:			0,   1,  2,    3
// Delayed values:   1789,  36, 45, 1886

// So in the first iteration, we'll run the following pairs, while storing both
// the product of the initial values (e.g. 1789*37) and the outcome of our FirstRepeat call:
// 
// 0,1 
// 1,2
// 2,3

Console.WriteLine(FirstRepeat((1789, 0), (37, 36)));        //=> 30413
Console.WriteLine(FirstRepeat((37, 36), (47,45)));          //=> 1220
Console.WriteLine(FirstRepeat((47,45), (1889, 1886)));    //=> 39666

// Presented in the same format as above, we now have:
// 1789 vs 37, 37 vs 47, 47 vs 1889
// 30413,          1220,      39666

// Similar to the 3vs5 and 5vs7 we now need to find out where the outcome of 3vs5 and 5vs7 repeats...
// So in a similar manner...
Console.WriteLine(FirstRepeat((1789*37, 30413), (37*47, 1220)));    //=>1288080
Console.WriteLine(FirstRepeat((37*47, 1220), (47*1889, 39666)));    //=>3147071

// Presented in the same format as above, we now have:
//  1789 * 37 vs 37 * 47,	37 * 47 vs 47*1889 
//				 1288080,				3147071
//				
// However continuing this approach, we could end up in a situation where we compare:
// 1789 * 37 * 37 * 47 with 37 * 47 * 47 * 1889
// And we can immediately see the outcome would be 37 * 47 too high
// In other words, when ever we do something like this, we actually need to do:
// (1789 * 37 * 37 * 47 / GCD (1789 * 37, 37 * 47)) vs etc							
Console.WriteLine(FirstRepeat((1789*47*37, 1288080), (37*47*1889, 3147071)));

// This prints 1202161486
// which is exactly the value we are looking for... but now to code the whole damn thing :)

/**/

// First we'll extract the list condensing we already applied in the brute force method
// but we'll replace the delays with the initial times.

(List<long> factors, List<long> initialTimes) Condense(long[] pFactors)
{
	//First we'll create two optimized lists where we condense the buslines and their offsets
	List<long> factors = new();
	List<long> initialTimes = new();

	for (long i = 0; i < pFactors.Length; i++)
	{
		if (pFactors[i] == 0) continue;

		factors.Add(pFactors[i]);
		initialTimes.Add(pFactors[i]-i);
	}

	return (factors, initialTimes);
}

// Now we'll write a FirstRepeatSequence that processes our condensed sequence
// and returns a new one, for which we'll also need a GCD method:

long GCD(long a, long b)
{
	while (b != 0)
	{
		long temp = b;
		b = a % b;
		a = temp;
	}
	return a; 
}

(List<long> factors, List<long> initialTimes) ProcessFirstRepeatSequence((List<long> factors, List<long> initialTimes) pSequence)
{
	List<long> factors = new();
	List<long> initialTimes = new();

	for (int i = 0; i < pSequence.factors.Count - 1; i++)
	{
		long aFactor = pSequence.factors[i];
		long aInitial = pSequence.initialTimes[i];
		long bFactor = pSequence.factors[i+1];
		long bInitial = pSequence.initialTimes[i+1];

		//Note order matters, aFactor * bFactor overflows if we don't divide it first
		factors.Add(aFactor / GCD(aFactor, bFactor) * bFactor );
		initialTimes.Add (FirstRepeat((aFactor,aInitial), (bFactor, bInitial)));
	}

	return (factors, initialTimes);
}

long GetSynchronizedTimestampOptimized(long[] pInputList)
{
	var condensed = Condense(pInputList);
	while (condensed.initialTimes.Count > 1)
	{
		condensed = ProcessFirstRepeatSequence(condensed);
        //Console.WriteLine(condensed.factors.Count + " factors left.");
    }
	return condensed.initialTimes[0];
}

stopwatch.Restart();
Console.WriteLine("Part 2:" + GetSynchronizedTimestampOptimized(busLines));
Console.WriteLine("Calculated in "+stopwatch.ElapsedMilliseconds + " ms.");


// UPDATE: So! My general approach AFTER solving things the hard way 
// (yes, you read through my pain above, and believe me, it was $#@$&@#$& )
// is that I pass my code through ChatGPT for crits & comments to see if
// I've been stupid and whether there is actually a better way...
// Turns out this whole puzzle is built around a concept called 
// "The Chinese Remainder Theorem" aka CRT.
// 
// So my approach is not the best approach, although I am still pretty proud/happy
// that I figured out ANY approach after 1.5 day of banging my head against the wall.
//
// I've added the CRT to my todolist to research further at a later stage:
// https://www.google.com/search?client=firefox-b-d&q=chinese+remainder+theorem
// I don't want to add any of it now, since I would be copying stackoverflow code 
// without understanding the logic behind it.
