// Solution for https://adventofcode.com/2023/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: 2 seperate sets of data:
// 1) a string of LRLRLRLLRL instructions
// 2) a set of AAA -> (BBB, CCC) instructions that tell us where we went up from AAA if we go L (BBB) or R (CCC)

(string, string) myInput = ParseUtils.FileToStringTuple(args[0]);

string leftRightInstructions = myInput.Item1;

Dictionary<string, (string left, string right)> map = myInput.Item2
    //Just split on every element in the string so that only triplets of steering instructions remain
    .Split([Environment.NewLine, "(", ",", ")", "="], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    //Chunk m into triplets
    .Chunk(3)
    //And map m as AAA => (BBB,CCC)
    .ToDictionary(x => x[0], x => (x[1], x[2]));

// *** Part 1: Starting at AAA how many steps are required to end up at ZZZ by following the LR instructions on the map?

int GetRequiredStepCountPart1 (string pStartNode, string pEndNode)
{
	string currentNode = pStartNode;

	int currentInstructionIndex = 0;
	int steps = 0;

	while (true)
	{
		steps++;

		currentNode = GetNextNode(currentNode, leftRightInstructions[currentInstructionIndex]);

		if (currentNode == pEndNode) break;

		currentInstructionIndex = (currentInstructionIndex + 1) % leftRightInstructions.Length;
	}

	return steps;
}


string GetNextNode (string pNodeName, char pDirection)
{
	return pDirection == 'L' ?
		map[pNodeName].left :
		map[pNodeName].right;
}

Console.WriteLine("Part 1 - Amount of steps required:" + GetRequiredStepCountPart1("AAA", "ZZZ"));

// ** Part 2: Follow all __A paths until they've lead to all __Z paths

// Ok, so my first try was to simply try and brute force this,
// which turns out to take waaayyy too long.
// So we'll do it in a different way

// First update GetRequiredStepCount to look for __Z instead of ZZZ
int GetRequiredStepCountPart2(string pStartNode)
{
	string currentNode = pStartNode;

    int currentInstructionIndex = 0;
	int steps = 0;

	while (true)
	{
		steps++;

		currentNode = GetNextNode(currentNode, leftRightInstructions[currentInstructionIndex]);

		if (currentNode[2] == 'Z') break;

		currentInstructionIndex = (currentInstructionIndex + 1) % leftRightInstructions.Length;
	}

	return steps;
}

// Then find all __A nodes and count how many steps each one requires...

Console.WriteLine("Part 2 ...");
Console.WriteLine("Compute these nodes ...");

string[] __ANodes = map.Keys.Where(x => x[2] == 'A').ToArray();
Console.WriteLine("Roads we'll need to walk:" + string.Join(',', __ANodes));

int[] stepsRequired = new int[__ANodes.Length];

for (int i = 0; i < __ANodes.Length; i++)
{
	stepsRequired[i] = GetRequiredStepCountPart2(__ANodes[i]);
	Console.WriteLine(stepsRequired[i] + " steps required to get from " + __ANodes[i] + " to the end");
}

// Now we have all the steps required PER item and the question is can we use those individual results
// to find the total amount of steps required after which ALL of them end up at __Z at the same time.

// If all results were PRIME numbers, this would be easy, we could just multiply all results together.
// This is often done for animations, e.g. let's say you have an animation consisting of 3 subanimations:
// 1 of 3 frames, 1 of 5 frames, 1 of 7 frames then this complete animation will repeat after 3 * 5 * 7 frames.
//
// However if the numbers are NOT prime, that will not work:
// 1 of 2 frames, 1 of 4 frames, 1 of 6 frames -> will repeat for the 1st time after 12 frames, not 48 

// To get 12 though, we CAN look at the biggest common denominator, which is 2 and do:
// (2/2) * (4/2) * (6/2) * 2 = 12

// Officially this value '12' is called the Least Common Multiple:
// The smallest positive number that is divisible by all of them.
// So for 2, 4, and 6, this is 12, since 12 can be divided by 2, 4 and 6.

// This works because after 12 frames all of them repeat at the same time!

// So, let's see if we can find those numbers and do this math:

HashSet<int> FindFactors (int pNumber)
{
	HashSet<int> factors = new ();

	int max = (int)Math.Sqrt(pNumber);

    for (int i = 2; i < max; i++)
	{
		if (pNumber % i == 0)
		{
			factors.Add(i);
			factors.Add(pNumber/i);
		}
	}
	return factors;
}

// Get a factor that they all share:
IEnumerable<int> factors = FindFactors(stepsRequired[0]);

for (int i = 0; i < stepsRequired.Length; i++)
{
	Console.WriteLine("All factors:"+ string.Join(',',FindFactors(stepsRequired[i])));
	factors = factors.Intersect(FindFactors(stepsRequired[i]));
}

int commonFactor = factors.First();
Console.WriteLine("Common factor:"+commonFactor);

// Now we multiply each value in stepsRequired, but divided by their common factor...

long result = 1;

for (int i = 0; i < stepsRequired.Length; i++)
{
	result *= stepsRequired[i]/commonFactor;
}

// The end result is the result multiplied with the commonFactor again...
result *= commonFactor;

Console.WriteLine("Part 2 - Steps required:" + result);

// Just for fun, the brute force version
// Brute force try 1 : Start following them all, doesn't work: too slow  

// for stepping
int currentInstructionIndex = 0;
long steps = 0;

// for debugging
long granularity = 10000000;
long last = 0;

Console.WriteLine("Expected iterations:" + result / granularity);

while (true)
{
	steps++;

    for(int i = 0; i < __ANodes.Length; i++)
    {
        __ANodes[i] = GetNextNode(__ANodes[i], leftRightInstructions[currentInstructionIndex]);
    }

    int matchingCount = __ANodes.Count(x => x[2] == 'Z');

    if (matchingCount == __ANodes.Length) break;

	currentInstructionIndex = (currentInstructionIndex + 1) % leftRightInstructions.Length;

	if (steps/granularity != last)
	{
		last = steps / granularity;
        Console.WriteLine("Currently at: " + steps + " steps");
    }
}

Console.WriteLine("Part 2 - Amount of steps required:" + steps);
