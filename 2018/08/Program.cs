// Solution for https://adventofcode.com/2018/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers which depending on how you interpret them form a graph

int[] numbers = ParseUtils.FileToArrayOf<int>(args[0], " ");

// ** Part 1: Sum all media entries...

//@returns the end index after parsing a node's children starting at pStartIndex
int SumAllMediaEntriesPart1(int[] pNumbers, int pStartIndex, ref int pSumOfMediaEntries, int pDepth = 0)
{
    // How is an entry constructed?
    //
    // A header, which is always exactly two numbers:
    // The quantity of child nodes.
    // The quantity of metadata entries.
    // Zero or more child nodes (as specified in the header).
    // One or more metadata entries (as specified in the header).

    // Make a copy of the start index so we don't modify it in lower recursion levels
    int currentStart = pStartIndex;

    int quantityOfChildNodes = pNumbers[currentStart++];
    int quantityOfMetaDataEntries = pNumbers[currentStart++];

    for (int i = 0; i < quantityOfChildNodes; i++)
    {
		currentStart = SumAllMediaEntriesPart1(pNumbers, currentStart, ref pSumOfMediaEntries, pDepth+1);
    }

    for (int i = 0; i < quantityOfMetaDataEntries; i++)
    {
        pSumOfMediaEntries += pNumbers[currentStart + i];
    }

    currentStart += quantityOfMetaDataEntries;

    return currentStart;
}

int total1 = 0;
int checksum1 = SumAllMediaEntriesPart1(numbers, 0, ref total1);

Console.WriteLine("Checksum:" + numbers.Length + " / " + checksum1);
Console.WriteLine("Part 1:" + total1);

// ** Part 2: Basically just follow the instructions to the letter...
// (Thank god we had to do part 1 first, otherwise this one would probably've been hell)

//@returns the end index after parsing a node's children starting at pStartIndex
int SumAllMediaEntriesPart2(int[] pNumbers, int pStartIndex, ref int pSumOfMediaEntries, int pDepth = 0)
{
	// How is an entry constructed?
	//
	// A header, which is always exactly two numbers:
	// The quantity of child nodes.
	// The quantity of metadata entries.
	// Zero or more child nodes (as specified in the header).
	// One or more metadata entries (as specified in the header).

	// Make a copy of the start index so we don't modify it in lower recursion levels
	int currentStart = pStartIndex;

	int quantityOfChildNodes = pNumbers[currentStart++];
	int quantityOfMetaDataEntries = pNumbers[currentStart++];

	pSumOfMediaEntries = 0;

	int[] sumsPerChild = new int[quantityOfChildNodes];

	for (int i = 0; i < quantityOfChildNodes; i++)
	{
		int childSum = 0;
		currentStart = SumAllMediaEntriesPart2(pNumbers, currentStart, ref childSum, pDepth + 1);

		sumsPerChild[i] = childSum;
	}

	if (quantityOfChildNodes == 0)
	{
		for (int i = 0; i < quantityOfMetaDataEntries; i++)
		{
			pSumOfMediaEntries += pNumbers[currentStart + i];
		}
	}
    else
    {
		for (int i = 0; i < quantityOfMetaDataEntries; i++)
		{
			int metaDataEntryIndex = pNumbers[currentStart + i] - 1;
			if (metaDataEntryIndex < 0 || metaDataEntryIndex >= sumsPerChild.Length) continue;
			pSumOfMediaEntries += sumsPerChild[metaDataEntryIndex];
		}
	}

	currentStart += quantityOfMetaDataEntries;

	return currentStart;
}

int total2 = 0;
int checksum2 = SumAllMediaEntriesPart2(numbers, 0, ref total2);

Console.WriteLine("Checksum:" + numbers.Length + " / " + checksum2);
Console.WriteLine("Part 2:" + total2);
