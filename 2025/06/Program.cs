// Solution for https://adventofcode.com/2025/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A loooong list of numbers... 3 rows of numbers, in columns, with a last row of operators...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: Initially I took the lazy approach, just converting everything to string
// and go from there until I reached part 2. Solved everything and then rewrote it.

// Get all the data into a grid
Grid<char> table = new Grid<char>(myInput, Environment.NewLine);

// Define some helper methods

int GetStartingIndexOfNextColumn (Grid<char> pTable, int pStartingIndex)
{
    int index = pStartingIndex + 1;

    while (index < pTable.width)
    {
        char op = pTable[index, pTable.height - 1];
        if (op == '*' || op == '+') return index;

        index++;
    }

    // Tricky one, each column has a space at the end except the last
    // to make the rest of the math simpler, return what the index of the next column WOULD have been
    return pTable.width + 1;
}

// Convert the numbers in a column to a list either interpreting them horizontally or vertically
long[] GetNumbers(Grid<char> pGrid, int pStartIndex, int pEndIndex, bool pHorizontal)
{
    // The amount of numbers we have depends on the reading order, 
    // if we interpret them from left to right, the amount of numbers is equal to the rows - 1
    // if we interpret them from top to bottom, the amount is equal the amount of char columns within the total column
    long[] numbers = new long[pHorizontal ? pGrid.height - 1 : pEndIndex - pStartIndex + 1];

    for (int y = 0; y < pGrid.height - 1; y++)
    {
        for (int x = pStartIndex; x <= pEndIndex; x++)
        {
            if (pGrid[x,y] == ' ') continue;
            int numberIndex = pHorizontal ? y : (x - pStartIndex);
            numbers[numberIndex] = 10 * numbers[numberIndex] + (pGrid[x,y] - '0');
        }
    }

    return numbers;
}

long CalculateTableResults (Grid<char> pTable, bool pHorizontal)
{
    int currentColumnStart = 0;
    long total = 0;

    while (true)
    {
        int currentColumnEnd = GetStartingIndexOfNextColumn(pTable, currentColumnStart) - 2;

        // Console.WriteLine(currentColumnStart + " " + currentColumnEnd);
        // Console.WriteLine(pTable[currentColumnStart,pTable.height-1]);

        long[] numbers = GetNumbers (pTable, currentColumnStart, currentColumnEnd, pHorizontal);
        
        // Console.WriteLine(string.Join(" ", numbers));

        char op = pTable[currentColumnStart, pTable.height - 1];

        if (op == '+') total += numbers.Sum();
        else total += numbers.Aggregate((x, y) => x * y);

        // Skip the space
        currentColumnStart = currentColumnEnd + 2;
        if (currentColumnStart >= pTable.width) break;
    }

    return total;
}

Console.WriteLine("Part 1:" + CalculateTableResults(table, true));
Console.WriteLine("Part 2:" + CalculateTableResults(table, false));
