// Solution for https://adventofcode.com/2020/day/14 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of masks and storage instructions

using System.Text.RegularExpressions;

string[] myInput = File.ReadAllLines(args[0]);

(long maskOr, long maskAnd) ConvertMask (string pMask)
{
	string patternOr = pMask.Replace('X', '0');
	string patternAnd = pMask.Replace('X', '1');
	return (Convert.ToInt64(patternOr, 2), Convert.ToInt64(patternAnd, 2));
}

//Console.WriteLine(0 & pattern.maskAnd | pattern.maskOr);

Regex maskParser = new Regex(@"mask = (.+)", RegexOptions.Compiled);
Regex memoryParser = new Regex(@"mem\[(\d+)\] = (\d+)", RegexOptions.Compiled);

Dictionary<long, long> memory = new();
(long maskOr, long maskAnd) currentMask = (0, 0);

foreach (string line in myInput)
{
	Match match = maskParser.Match(line);
	if (match.Success)
	{
		//Console.WriteLine(match.Groups[1]);
		currentMask = ConvertMask(match.Groups[1].Value);
	}
    else
    {
		match = memoryParser.Match(line);
		if (match.Success)
		{
			long address = long.Parse(match.Groups[1].Value);
			long value = long.Parse (match.Groups[2].Value);
			long maskedValue = value & currentMask.maskAnd | currentMask.maskOr;
			memory[address] = maskedValue;
        }
    }
}

Console.WriteLine("Part 1:" + memory.Values.Sum());

// ** Part 2: Couple of changes:
// - looking at the bitmask, we replace X with 0 and only do an OR
// - all original X positions need to be modified in order using all possible combinations
//   in practice, let's say you have XXX, then we need to overwrite the original pattern
//   with 8 possible variations of 0 and 1 assigned to XXX

memory.Clear();

foreach (string line in myInput)
{
	Match match = maskParser.Match(line);
	if (match.Success)
	{
		//get the mask again even though we'll only use the orMask
		currentMask = ConvertMask(match.Groups[1].Value);
	}
	else
	{
		match = memoryParser.Match(line);
		if (match.Success)
		{
			long address = long.Parse(match.Groups[1].Value);
			long value = long.Parse(match.Groups[2].Value);
			long maskedAddressBase = address | currentMask.maskOr;

			//Find out where all the ones are...
            long xValues = currentMask.maskAnd ^ currentMask.maskOr;

			List<long> addressVariations = GenerateAllAddressVariations (maskedAddressBase, xValues);
			foreach (long addressVariation in addressVariations) {
				memory[addressVariation] = value;
			}
        }
	}
}

List<long> GenerateAllAddressVariations(long pMaskedAddressBase, long pXValues)
{
	// calculate all indices all x's
	List<int> xIndices = new ();
	int currentIndex = 0;

	while (pXValues > 0)
	{
		if ((pXValues & 1) > 0) xIndices.Add(currentIndex);
		pXValues >>= 1;
		currentIndex++;
	}

    // now that we have the indices:
    // - clear all these bits in the original number 
	// Note the tricky cast to long, by default 1 is int, which is 32 bit,
	// which can't be shifted more than 32 bits without LOSING all bits!!
	// (Same for the long casts below)

    foreach (int index in xIndices)
	{
		pMaskedAddressBase &= ~(1L << index);
	}

    // - generate the amount of variations we need to consider
    int amountOfVariations = 1 << xIndices.Count;

    // now for every variations (0, 1, 2, etc) walk over every index deciding
    // if that bit need to be 0 or 1 in the original number

    // E.g. imagine the base number is 10000
    // And the x indices are marked with X => 10X0X
    // This means we have two X's, so we have 4 variations for XX = 00, 01, 10, 11
    //
    // So for the numbers 0, 1, 2, 3, .. we AND that number with every 1 << index
    // and if it is 1 we set it...

    List<long> variations = new();

	for (long i = 0; i < amountOfVariations; i++)
	{
		long newAddress = pMaskedAddressBase;	

		for (int index = 0; index < xIndices.Count; index++)
		{
			//if index i is set, set 1 at the actual index
			long bitIsSet = (1 << index) & i;
			long bitValue = (bitIsSet > 0 ? 1L : 0L) << xIndices[index];
			newAddress |= bitValue;
		}
        variations.Add(newAddress);
	}

    return variations;
}

Console.WriteLine("Part 2:" + memory.Values.Sum());
