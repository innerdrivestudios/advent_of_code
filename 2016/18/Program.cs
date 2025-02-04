//Solution for https://adventofcode.com/2016/day/18 (Ctrl+Click in VS to follow link)

using System.Text;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: the first row of a dungeon tile set

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// ** Part 1: For a dungeon the width of the given first row and length (height) 40, calculate how
// many safe tiles there are, by following the rules given in the puzzle

// We can solve this by simply keeping track of only of the current and next row...
// Note that this solution is a little bit more complex than it had to be because of the char[] optimization
// A stringbuffer would also work just fine and make everything a bit simpler.
// Also a last row is generated while the last currentrow is being counted, which could be improved.

long GetSafePlacesCountFor (int pRows) { 
	char[] currentRow = myInput.ToCharArray();
	char[] nextRow = new char[currentRow.Length];
	char[] tmp;

	int rowCount = 0;
	long safePlacesCount = 0;

	while (rowCount < pRows)
	{
		//fill the next row and count how many safe places there were in this row
		safePlacesCount += GetNextRow(nextRow, currentRow);
		rowCount++;

		tmp = currentRow;
		currentRow = nextRow;
		nextRow = tmp;
	}

	return safePlacesCount;
}

int GetNextRow(char[] pNextRow, char[] pCurrentRow)
{
	int safePlaces = 0;
	for (int i = 0; i < pCurrentRow.Length; i++)
	{
		int leftIndex = i - 1;
		int rightIndex = i + 1;

		bool trapLeft = leftIndex >= 0 && pCurrentRow[leftIndex] == '^';
		bool trapCenter = pCurrentRow[i] == '^';
		bool trapRight = rightIndex < pCurrentRow.Length && pCurrentRow[rightIndex] == '^';

		//Its left and center tiles are traps, but its right tile is not.
		//Its center and right tiles are traps, but its left tile is not.
		//Only its left tile is a trap.
		//Only its right tile is a trap.

		safePlaces += (pCurrentRow[i] == '.') ? 1 : 0;

		bool isTrap =
			(trapLeft && trapCenter && !trapRight) ||
			(!trapLeft && trapCenter && trapRight) ||
			(trapLeft && !trapCenter && !trapRight) ||
			(!trapLeft && !trapCenter && trapRight);

		pNextRow[i] = (isTrap ? '^' : '.');
	}

	return safePlaces;
}

Console.WriteLine("Part 1 - Safe places count for 40 rows:" + GetSafePlacesCountFor(40));

// ** Part 2: The same question but now for 400000 rows ...

Console.WriteLine("Part 2 - Safe places count for 400000 rows:" + GetSafePlacesCountFor(400000));
