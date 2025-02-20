// Solution for https://adventofcode.com/2021/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers representing X positions for crabs

int[] numbers = ParseUtils.CSVFileToNumbers<int>(args[0]);

// ** Part 1 : What is the position we should move all crabs to so that
// - All X positions are equal
// - The sum of all movements is the lowest possible

int GetCheapestXPositionPart1(int[] pPositions)
{
	int min = pPositions.Min();
	int max = pPositions.Max();

	int cheapestXPosition = int.MaxValue;

	for (int x = min ; x <= max; x++)
	{
		cheapestXPosition =
			int.Min(
				cheapestXPosition,
				pPositions.Sum(pos => Math.Abs(pos - x))
			);
	}

	return cheapestXPosition;
}

Console.WriteLine("Part 1 - CheapestXPosition:" + GetCheapestXPositionPart1(numbers));

// ** Part 2: Those damn crabs :), moving further is more expensive ...
// my guess is newton would not agree, but anyway ! 
//
// Let's say you want to move x, what is the total?

// 1 -> 1			-> 1
// 2 -> 1,2			-> 3
// 3 -> 1,2,3		-> 6
// 4 -> 1,2,3,4		-> 10
// 5 -> 1,2,3,4,5   -> 15

// In other words: this is simply the sum of a sequence of numbers:
// n				-> n * (n+1) / 2

int GetCheapestXPositionPart2(int[] pPositions)
{
	int min = pPositions.Min();
	int max = pPositions.Max();

	int cheapestXPosition = int.MaxValue;

	for (int x = min; x <= max; x++)
	{
		cheapestXPosition =
			int.Min(
				cheapestXPosition,
				pPositions.Sum(pos => PositionCostHelper(pos, x))
			);
	}

	return cheapestXPosition;
}

int PositionCostHelper (int pCurrentPosition, int pTargetPosition)
{
	int delta = int.Abs(pCurrentPosition - pTargetPosition);
	return delta * (delta + 1) / 2;
}

Console.WriteLine("Part 2 - CheapestXPosition:" + GetCheapestXPositionPart2(numbers));