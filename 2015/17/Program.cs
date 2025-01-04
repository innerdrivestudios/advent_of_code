//Solution for https://adventofcode.com/2015/day/16 (Ctrl+Click in VS to follow link)

//Your input: a list of container capacities
List<int> input = new List<int>()	{ 33, 14, 18, 20, 45, 35, 16, 35, 1, 13, 18, 13, 50, 44, 48, 6, 24, 41, 30, 42 };

//Your task:
// Part 1 - Find how many different combinations of containers can exactly fit 150 liters of eggnog.
// Part 2 - Find the minimum number of containers that can exactly fit all 150 liters of eggnog.
//			How many different ways can you fill that number of containers and still hold exactly 150 litres?

//Approach: iterate all possible combinations of containers to find:
// 1. the AMOUNT of combinations of containers that store the requested amount
// 2. a mapping from the amount of containers that store the requested amount to how many of those combinations there are

int possibleCombinationsCount = 0;
Dictionary<int, int> containerCount2Uses = new Dictionary<int, int>();
IterateCombinations(input, 150, ref possibleCombinationsCount, containerCount2Uses);

Console.WriteLine("Part 1: " + possibleCombinationsCount);

int minimumAmountOfContainersUsed = containerCount2Uses.Keys.Min();
Console.WriteLine("Part 2: " + containerCount2Uses[minimumAmountOfContainersUsed]);

///////////////////////////// HELPER METHOD /////////////////////////////

void IterateCombinations(
	List<int> pInput, 
	int pRequiredTotal, 
	ref int possibleCombinationsCount,
	Dictionary<int, int> pContainerCount2Uses,
	List<int> pContainersUsed = null)
{
	//We need to keep track of containers used so far, if none passed create an empty list
	pContainersUsed = pContainersUsed ?? new List<int>();

	int filledSoFar = pContainersUsed.Sum();

	if (filledSoFar == pRequiredTotal)
	{
		possibleCombinationsCount++;

		containerCount2Uses.TryGetValue(pContainersUsed.Count, out int uses);
		containerCount2Uses[pContainersUsed.Count] = uses+1;

		return;
	}
	else if (filledSoFar > pRequiredTotal)
	{
		return;
	}
	else
	{
		//for each element in the given input...
		for (int i = 0; i < pInput.Count; i++)
		{			
			//...add that element to a clone of the containers used (see last line)
			//and with that cloned list iterate all combinations of all next items in the list			
			IterateCombinations(
				pInput.GetRange(i + 1, pInput.Count - i - 1),  //index, count 
				pRequiredTotal, 
				ref possibleCombinationsCount, 
				pContainerCount2Uses,
				new List<int>(pContainersUsed) { pInput[i] }
			);
		}

		//In other words, if you input 1,2,3,4,5, what happens is that we iterate all combinations for:
		//1 + all combinations of 2-5
		//2 + all combinations of 3-5
		//3 + all combinations of 4-5
		//etc

		//1 + all combinations of 2-5 -> becomes
		//	1,2 + all combinations of 3-5
		//	1,3 + all combinations of 4-5
		//	1,4 + all combinations of 5

		//In the end we iterate over all possible combinations of containers
	}
}

