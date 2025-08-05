// Solution for https://adventofcode.com/2023/day/12 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: patterns with holes and checksum values like:
// ?.#?#?????.? 			1,1,3
//
// This tells us that whatever we put on the ? we need to 
// end up with a pattern like #..#..###
// (and there might be multiple ways to do so)

// Let's first parse the input into the most basic usable translation

string[] myInput = File.ReadAllLines(args[0]);

List<Pattern> patternList = myInput
	.Select(ConvertStringToPattern)									
	.ToList();

Pattern ConvertStringToPattern (string pPattern)
{
	string[] parts = pPattern.Split(' ');

	return new(
		parts[0],
		parts[1].Split(',').Select(int.Parse).ToList()
	);
}

// ** Part 1: Figure out in how many ways we can complete a pattern so that the pattern matches the checksum.

// Let's work through an example case:
//
// ?#..#??.?#.? 2,1,1,2
//
// If we add the indices we get this:
// 0  1  2  3  4  5  6  7  8  9  10 11
// ?  #  .  .  #  ?  ?  .  ?  #   .  ?		2,1,1,2
//
// In other words, the indices we need to fill are:
// 0, 5, 6, 8, 11 -> 5 indices.
//
// What do we need to do with these indices?
// We need to find all the ways we can assign a # to these indices
// while matching the 2,1,1,2 requirements.
//
// There are different ways to do so, here are 3 "obvious" ones:
// 1. Generate ALL permutations of 0,5,6,8,11:
//    - this is really brute force but would include a lot of 
//      nonsensical variations such as 0,5,6,8,11 vs 5,0,6,8,11.
//		This is nonsensical for two reasons:
//		- the order in which you assign #'s doesn't matter
//		- this will result in way too many # being assigned as well
// 2. Generate only permutations that make sense:
//   - this is the best option of course but might be very difficult,
//     e.g. it would involve filtering out the index 11, since
//     11 is in a group of 1, where as 2,1,1,2 indicates the last group
//     needs to be a group of 2.
// 3. Generate ALL permutations, filtering out the ones that are 
//    obviously wrong as mentioned under step 1, meaning:
//    - permutation indices can only ever increase
//    - a permutation sequence length is limited by the amount of # we need to place
//
// Looking at ?#..#??.?#.? 2,1,1,2 we have 5 ? to fill BUT
// seeing 2,1,1,2 tells us we can only have 6 # in total, 
// and 3 spots are already filled, we know we can only use sequences
// that are 6-3 = 3 in length.
//
// Given we have 5 indices (0, 5, 6, 8, 11) and we have to create
// runs that are at least 3 long, we need to iterate over the first
// 0 .. (5 - 3) elements, in other words index 0 .. 2, values 0, 5 & 6
// 
// What do we do with values 0, 5, 6?
// Each of those values has possible follow up values:
// 0 -> 5,6,8	resulting in: (0,5) (0,6) (0,8)
// 5 -> 6,8		resulting in: (5,6) (5,8)
// 6 -> 8		resulting in: (6,8)
//
// And each of these sequences has several other follow up values:
// (0,5) -> 6,8,11
// (0,6) -> 8,11
// (0,8) -> 11

// Anyway we can see it is a recursive process influenced by 
// the length of the sequence we have and the amount of elements
// we need to pick.
//
// In total this process will result in the following sequences:
// (0,5,6)		(5,6,8)
// (0,5,8)		(5,6,11)
// (0,5,11)		(5,8,11)
// (0,6,8)		(6,8,11)
// (0,6,11)
// (0,8,11)

// Putting this into practice, we'll need a couple of methods.
// First a method gather some info about a specific pattern, 
// in particular, which indices are free and how many of them we'll need to fill:

(List<int> freeIndices, int placesToFill) GetPatternInfo(Pattern pPattern)
{
	List<int> result = new();
	int hashtagsFound = 0;
	for (int i = 0; i < pPattern.pattern.Length; ++i)
	{
		if (pPattern.pattern[i] == '?') result.Add(i);
		else if (pPattern.pattern[i] == '#') hashtagsFound++;
	}
	return (result, pPattern.checksum.Sum () - hashtagsFound);
}

// Then a method to actually take the free indices and the placesToFill and
// generate all possible variations in which we can do that...

List<HashSet<int>> GenerateSequences(List<int> pInput, int pStart, int pSequenceLength, Pattern pPattern)
{
	if (pSequenceLength < 1) throw new Exception("Sequence length has to be at least 1");

	List<HashSet<int>> results = new();

	// At each level we iterate over the elements in the list
	// starting from the given start, up until the end minus
	// the amount items still to pick... 

	// Why? Since if we still need to pick 2 elements after this one
	// I need to stop 2 elements before the end otherwise there will
	// never be enough elements left...

	for (int i = pStart; i <= pInput.Count - pSequenceLength; i++)
	{
		//If there is 1 number left, just create a sequence with that number
		if (pSequenceLength == 1)
		{
            results.Add([pInput[i]]);
		}
		else
		{
			//Get all child sequences and insert ourselves...
			List<HashSet<int>> childResults = GenerateSequences(pInput, i + 1, pSequenceLength - 1, pPattern);
			foreach (HashSet<int> childResult in childResults)
			{
				//Note that the order doesn't matter
				HashSet<int> clonedChildResult = new HashSet<int>(childResult);
                clonedChildResult.Add(pInput[i]);
				results.Add(clonedChildResult);
			}
		}
	}

	return results;
}

// Then a method to check if a generated sequence is actually valid:

bool IsValidSequence (Pattern pPattern, HashSet<int> pReplacementSequence)
{
	int currentSequence = 0;
	int currentSequenceLength = 0;
	bool inSequence = false;

	for (int i = 0; i <= pPattern.pattern.Length; i++)
	{
		if (i < pPattern.pattern.Length && 
			(
				pPattern.pattern[i] == '#' || 
				pReplacementSequence.Contains(i)
			)
		)
		{
			currentSequenceLength++;
			inSequence = true;
		} 
		else if (inSequence)
		{
			inSequence = false;
			if (currentSequence >= pPattern.checksum.Count || pPattern.checksum[currentSequence] != currentSequenceLength) return false;
			currentSequence++;
			currentSequenceLength = 0;
		}
	}

	return true;
}

// And last but least a method to tie everything together

long GetPossibleArrangementsCount (Pattern pPattern)
{
    //Console.Write(".");
    var patternInfo = GetPatternInfo(pPattern);

	//Even if there are no spaces to fill, this still counts as 1 valid configuration
	if (patternInfo.placesToFill == 0) return 1;

	List<HashSet<int>> possibleConfigurations = 
		GenerateSequences(patternInfo.freeIndices, 0, patternInfo.placesToFill, pPattern);

	int validConfigurations = 0;
	foreach (var configuration in possibleConfigurations)
	{
		if (IsValidSequence(pPattern, configuration)) validConfigurations++;
	}

	return validConfigurations;
}

Console.WriteLine("Part 1:" + patternList.Sum(x => GetPossibleArrangementsCount(x)));

// I wasn't able to solve part 2 yet!

Console.WriteLine(GetPossibleArrangementsCount(ConvertStringToPattern("?###???????? 3,2,1")));

int len = 0;
foreach (var pattern in  myInput)
{
    Console.WriteLine(pattern + "\t\t\t" + GetPossibleArrangementsCount(ConvertStringToPattern(pattern)));
	len = int.Max(len, pattern.IndexOf(' '));
}

Console.WriteLine(len);