// Solution for https://adventofcode.com/2020/day/10 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers representing output jolts (and thus input)

List<int> myInput = ParseUtils.FileToArrayOf<int>(args[0], Environment.NewLine).ToList();

// ** Part 1: Find the number of 1 and 3 differences in the list

// First insert start and end points
myInput.Insert(0, 0);
myInput.Add(myInput.Max() + 3);
myInput.Sort();

(int diff1, int diff3) differences = (0, 0);

for (int i = 0; i < myInput.Count-1; i++)
{
	int delta = myInput[i + 1] - myInput[i];
	if (delta == 1) differences.diff1++;
	else if (delta == 3) differences.diff3++;
	else Console.WriteLine("Whoops:" + delta);
}

Console.WriteLine("Part 1: " + differences.diff1 * differences.diff3);

/**

// ** Part 2: Count the different ways we can connect chargers while still
// meeting the requirements mentioned in part
//
// Initial approach:
// - When calling the method with a certain configuration count it as 1 possible config
// - Loop over the configuration using a sliding window of 3 items at a time 
//   checking the delta between element 0 and 2 of each window.
//   If it is <= 3 we can remove the middle one. 
// - If we CAN remove the middle one, we do that on a CLONE, we don't modify the given list,
//   because we need to continue looping over it. Given the CLONE with that element removed,
//   we call the count method recursively adding all the resulting variations to our own count.
// - A nice optimization here is that we can pass in a starting index into the recursive call
//   so we can avoid adding duplicate elements without using a visited list.
//
// This approach works PERFECTLY, except that it turns out to be way too slow,
// no matter which optimization I applied, BUT it did allow me to experiment with the results
// for different lists and deduct a better mechanism that way (see below)...
//
// If you want to test the method with the test data makes sure you comment out the writeline

int CountVariations (List<int> pInputList, int pStartIndex = 0, HashSet<string> visited = null)
{
    Console.WriteLine(string.Join(',', pInputList));

	int variationCount = 1; //we are one variation

    for (int i = pStartIndex; i < pInputList.Count-2; i++)
	{
		if (pInputList[i+2] - pInputList[i] <= 3)
		{
			List<int> alteredClone = new List<int>(pInputList);
			alteredClone.RemoveAt(i + 1);
            variationCount += CountVariations(alteredClone, i, visited);
        }
	}

	return variationCount;
}

// Look at what this prints:

Console.WriteLine(CountVariations([0,1,2,3,4,5]));					//sequence of 6
Console.WriteLine(CountVariations([10,11,12,13,14]));				//sequence of 5
Console.WriteLine(CountVariations([0,1,2,3,4,5,10,11,12,13,14]));	//sequence of 6 + sequence of 5

// In other words:
//  13
//  7
//  91 (13*7)
//
// Which leads to some nice insights:
// - delta's between numbers are ALWAYS either 1 or 3 (that is how the input data is constructed)
// - a run is a sequence of numbers with delta 1
// - runs are only interrupted by 1 or more delta 3's
// - the amount of variations is only influenced by the runs of delta 1's
// - the length of a run determines how many valid variations of that run we can construct
// - the total amount is all the amounts of variations multiplied

// BUT do we calculate the amount of variations for a run of a specific length?
// Through testing we see:

Console.WriteLine(CountVariations([0]));				//Sequence of	1 -> 1
Console.WriteLine(CountVariations([0,1]));				//Sequence of	2 -> 1
Console.WriteLine(CountVariations([0,1,2]));			//Sequence of	3 -> 2
Console.WriteLine(CountVariations([0,1,2,3]));			//Sequence of	4 -> 4
Console.WriteLine(CountVariations([0,1,2,3,4]));		//Sequence of	5 -> 7
Console.WriteLine(CountVariations([0,1,2,3,4,5]));		//Sequence of	6 -> 13
Console.WriteLine(CountVariations([0,1,2,3,4,5,6]));	//Sequence of	7 -> 24

// So we can try to reason this out, why it is this way, but the most important insight
// is that the total amount of variations for a run of length x is the total amount of variations
// for a length of x-3, x-2 and x-1 items:

/**/

int GetVariationCountForARunOfLength (int pLength)
{
    if (pLength <= 2) return 1;
	if (pLength == 3) return 2;
	return	GetVariationCountForARunOfLength(pLength-1) +
			GetVariationCountForARunOfLength(pLength-2) +
			GetVariationCountForARunOfLength(pLength-3); 
}

/*
Console.WriteLine(
	"Variation count for a run of length 7:" +
	GetVariationCountForARunOfLength(7)
);
*/

// So now all that is left to do is to write an algorithm that scans a list of numbers
// to deduct the run length for each run of delta 1's, get their variations count and 
// multiply them:

List<int> ScanRunLengths (List<int> pInput)
{
	List<int> runLengths = new List<int>();
	runLengths.Add(1);

	int currentRunIndex = 0;

	for (int i = 0; i < pInput.Count-1; i++)
	{
		if (pInput[i+1] - pInput[i] == 1)
		{
			runLengths[currentRunIndex]++;
		}
		else
		{
			runLengths.Add(1);
			currentRunIndex++;
		}
	}

	return runLengths;
}

List<int> runLengths = ScanRunLengths(myInput);
long totalVariations = 1;
foreach (int runLength in runLengths) totalVariations *= GetVariationCountForARunOfLength(runLength);
Console.WriteLine("Part 2: "+totalVariations);