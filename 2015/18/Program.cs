// Solution for https://adventofcode.com/2015/day/18 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A 100x100 grid of on/off cells

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

//** Your task: perform some "game of life" operations on the grid and print data about the results

Grid<bool> grid = new Grid<bool>(myInput, "\r\n", null, (a, b) => b == "#");

Part1(grid);
Part2(grid);
Console.ReadKey();

void Part1(Grid<bool> pGrid)
{
    pGrid = pGrid.Clone();

    for (int steps = 1; steps <= 100; steps++)
    {
        pGrid = ProcessGrid(pGrid);
    }

    int total = 0;
    pGrid.Foreach((a, b) => total += (b ? 1 : 0));
    Console.WriteLine("Part 1: " + total);
}

void Part2(Grid<bool> pGrid)
{
    pGrid = pGrid.Clone();

    for (int steps = 1; steps <= 100; steps++)
    {
        BreakGrid(pGrid);

        pGrid = ProcessGrid(pGrid);
    }

    BreakGrid(pGrid);

    int total = 0;
    pGrid.Foreach((a, b) => total += (b ? 1 : 0));
    Console.WriteLine("Part 2: " + total);
}

///////////////////////////////////////////////////////////////////////
/// HELPER METHODS BELOW THIS LINE
///////////////////////////////////////////////////////////////////////

Grid<bool> ProcessGrid(Grid<bool> pGrid)
{
    Grid<bool> newGrid = new Grid<bool>(pGrid.width, pGrid.height);

    for (int x = 0; x < pGrid.width; x++)
    {
        for (int y = 0; y < pGrid.height; y++)
        {
            int neighbourCount = GetNeighbourCount(pGrid, x, y);

            //A light which is on stays on when 2 or 3 neighbors are on, and turns off otherwise.
            //A light which is off turns on if exactly 3 neighbors are on, and stays off otherwise.
            if (pGrid[x, y])
            {
                newGrid[x, y] = neighbourCount == 2 || neighbourCount == 3;
            }
            else
            {
                newGrid[x, y] = neighbourCount == 3;
            }
        }
    }

    return newGrid;
}

void BreakGrid (Grid<bool> pGrid)
{
    pGrid[0, 0] = true;
    pGrid[pGrid.width - 1, 0] = true;
    pGrid[pGrid.width - 1, pGrid.height - 1] = true;
    pGrid[0, pGrid.height - 1] = true;
}

int GetNeighbourCount(Grid<bool> pGrid, int pX, int pY)
{
    int count = 0;

    for (int column = pX - 1; column <= pX + 1; column++)
    {
        for (int row = pY - 1; row <= pY + 1; row++)
        {
            if (!pGrid.IsInside(column, row) || (column == pX && row == pY)) continue;
            
            if (pGrid[column, row]) count++;
        }
    }

    return count;
}



