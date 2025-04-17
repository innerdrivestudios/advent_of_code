//Solution for https://adventofcode.com/2018/day/12 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of plants and their replacement rules

using System.Diagnostics.Metrics;

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

string[] splitData = myInput.Split(
		Environment.NewLine + Environment.NewLine,
		StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
	);

// Some observations before we start... the plant data can grow left and right...
// We don't know how far yet, but what are feasible options to store this?
// - array with offset 
// - dictionary instead of array
// - linkedlist

// Because of the ease of use and direct access, I'll use a dictionary while keeping
// track of the min max index used.

// Secondly, 5 plantpots determine the outcome for the current plantpot.
// A pot can be filled or not, so we'll have 2*2*2*2*2 combinations -> 32 rules

// Thirdly, compared to all keys in the dictionary we COULD have, only a limited subset
// of integers is actually mapped, in other words all other integers basically map to
// an empty pot. If 5 empty pots would map to anything other than an empty pot, 
// the algorithm would go insane, so ..... always maps to .

// Fourth, continuing with the "there are 32 options" bit, this whole process is
// kinda like "Game Of Life" (where we go in iterations, requiring a previous and next state)
// and "Marching Squares" (where we march over a sequence of items)
// Just like a regular marching square setup we can convert the state of a pot
// (using the pot's index plus -2 -1 0 1 2) to an integer using 5 bits for the pot states
// and a mapping table from integer to the new pot state.

// We'll just map plant index to 0 or 1 (instead of bool),
// easier for the "marching squares" mapping
string plantPotsStr = splitData[0].Replace("initial state: ", "");

int minPlantIndex = plantPotsStr.IndexOf("#");
int maxPlantIndex = plantPotsStr.LastIndexOf("#");

Dictionary<int, int> plantPots = new Dictionary<int, int>();

for(int i = minPlantIndex; i <= maxPlantIndex; i++)
{
	plantPots[i] = plantPotsStr[i] == '.' ? 0 : 1;
}

// Next we'll build ourselves a lookup table:

// Get all separate #.#.# => # strings
string[] patternStrings = splitData[1].Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// And for each pattern, get the lookup value and map it to its result
int[] lookupTable = new int[32];
foreach (string patternStr in patternStrings)
{
	//left part is #.#.# right is # or .
	string[] patternParts = patternStr.Split(" => ");

	lookupTable[GetLookupValue(patternParts[0])] = patternParts[1] == "." ? 0:1 ;
}

//Convert #.#.# into an int through bit manipulation...
int GetLookupValue (string pPatternStr)
{
    int lookupValue = 0;

	for (int i = 0; i < 5; i++)
	{
		lookupValue |= (pPatternStr[i] == '.' ? 0 : 1) << (4-i);
	}

	return lookupValue;
}

/* // For debugging...
for (int i = 0; i < lookupTable.Length; i++)
{
	Console.WriteLine(patternStrings[i] + " => " + GetLookupValue(patternStrings[i]));
}
*/

// Simulate one generation of plant growth...
(Dictionary<int, int> plantData, int start, int end) RunGeneration((Dictionary<int, int> plantData, int start, int end) pInput)
{
	Dictionary<int, int> result = new();
	int start = int.MaxValue;
	int end = int.MinValue;

	//So plants are influenced by the two plants before them and after them
	for (int i = pInput.start - 2; i <= pInput.end + 2; i++)
	{
		int lookupValue =
			pInput.plantData.GetValueOrDefault(i - 2, 0) << 4 |
			pInput.plantData.GetValueOrDefault(i - 1, 0) << 3 |
			pInput.plantData.GetValueOrDefault(i - 0, 0) << 2 |
			pInput.plantData.GetValueOrDefault(i + 1, 0) << 1 |
			pInput.plantData.GetValueOrDefault(i + 2, 0) << 0;
		
		result[i] = lookupTable[lookupValue];

		//if the value was 1 we might need to adapt the start and end values
		if (result[i] == 1)
		{
			start = int.Min(start, i);
			end = int.Max(end, i);
		}
	}

	return (result, start, end);
}

// Now run the 20 generations...
var firstGeneration = (plantPots, minPlantIndex, maxPlantIndex);

var result = firstGeneration;
for (int i = 0; i < 20; i++)
{
	result = RunGeneration(result);
}

// After 20 generations, what is the sum of the numbers of all pots which contain a plant?

Console.WriteLine(
	"Total amount of plants:" +
	result.plantPots.Where(x => x.Value == 1).Sum(x => x.Key)
);

// ** Part 2: Do the same but now for 50000000000 generations

Console.WriteLine("Part 2 - Pattern test:");

// In other words: again, our base approach fails dramatically :)
// Let's run this again and see if we can find a pattern...

firstGeneration = (plantPots, minPlantIndex, maxPlantIndex);

result = firstGeneration;
for (int i = 0; i < 1000; i++)
{
	result = RunGeneration(result);

	Console.WriteLine(
		(i+1) + " => " +
		result.plantPots.Where(x => x.Value == 1).Sum(x => x.Key)
	);

}

// For my input, one sort of pattern I see is:

// 100 -> 8898
// 200 -> 16998
// 300 -> 25098
// 400 -> 33198
// 500 -> 41298

// In other words, ignoring the last 98:
// 100 -> 88    (7+81)
// 200 -> 169	(+81)
// 300 -> 250	(+81)
// 400 -> 331   (+81)
// 500 -> 412   (+81)

// So after a certain point the value pattern seems to stabilize
// and not go down anymore...
// Prediction without looking at the table:
// 1000 -> 412 + 5 * 81 = 817 +"98" = 81798 (=> correct!)

// In other words for 50000000000 generations, we need to do:
long total = 7 + (50000000000 / 100) * 81;
Console.WriteLine("Part 2:" + total + "98");