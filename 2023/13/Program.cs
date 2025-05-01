// Solution for https://adventofcode.com/2023/day/13 (Ctrl+Click in VS to follow link)

using System.Diagnostics;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

string[] blocks = File.ReadAllText(args[0])
    .ReplaceLineEndings(Environment.NewLine)
    .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Create an array of grids so we can figure out where the mirror line lies...
Grid<char>[] grids = blocks.Select(x => new Grid<char>(x, Environment.NewLine)).ToArray();

// Note the pSkip parameter, this is required for the brute force part 2 :)

Vec2i FindMirrorLine (Grid<char> pGrid, Vec2i pSkip)
{
    //Let's test columns first...
    Vec2i result = FindMirroredColumn (pGrid, pSkip);
    //If that fails, the rows...
    if (result.Magnitude() == 0) result = FindMirroredRow(pGrid, pSkip);

    return result;
}

Vec2i FindMirroredColumn(Grid<char> pGrid, Vec2i pSkip)
{
    //how many mirrorlines are possible?
    //there has to be at least one line to mirror on both sides...
    //soooo... if we have 0, 1, 2, 3 colums, we can be between 0 & 1, 1 & 2, 2 & 3 -> 3 lines
    //we'll use 1 to indicate the column between 0 & 1, 2 for 1 & 2 etc
    //(note we could also have doubled the coordinates or switched to floats, all are possible))

    for (int column = 1; column < pGrid.width; column++)
    {
        if (pSkip.X == column) continue;

        bool allMirrored = true;
        
        //then for each column, we will be testing all rows:
        for (int row = 0; row < pGrid.height; row++)
        {
            if (!TestRowForMirroring (pGrid, row, column))
            {
                allMirrored = false;
                break;
            }
        }

        if (allMirrored) return new Vec2i(column, 0);
    }

    //Nothing found
    return new Vec2i(0,0);
}

bool TestRowForMirroring (Grid<char> pGrid, int pRow, int pColumn)
{
    //ok so now we first need to figure out HOW many tests to do
    //if we have 0, 1, 2, 3 colums, we can be between 0 & 1, 1 & 2, 2 & 3 -> 3 lines
    //if column = 1 we are between 0 and 1, we can do 1 test
    //if column = 2 we are between 1 and 2, we can do 2 tests
    //if column = 3 we are between 2 and 3, we can do 1 tests
    int tests = int.Min(pColumn, pGrid.width - pColumn);

    for (int i = 0; i < tests; i++)
    {
        if (pGrid[pColumn - i - 1, pRow] != pGrid[pColumn + i, pRow]) return false;
    }

    return true;
}

// Theoretically we can code both mirroring directions using one method but this was faster

Vec2i FindMirroredRow(Grid<char> pGrid, Vec2i pSkip)
{
    for (int row = 1; row < pGrid.height; row++)
    {
        if (pSkip.Y == row) continue;

        bool allMirrored = true;
        //then for each row, we will be testing all columns:
        for (int column = 0; column < pGrid.width; column++)
        {
            if (!TestColumnForMirroring(pGrid, row, column))
            {
                allMirrored = false;
                break;
            }
        }

        if (allMirrored) return new Vec2i(0, row);
    }

    return new Vec2i(0, 0);
}

bool TestColumnForMirroring(Grid<char> pGrid, int pRow, int pColumn)
{
    int tests = int.Min(pRow, pGrid.height - pRow);

    for (int i = 0; i < tests; i++)
    {
        if (pGrid[pColumn, pRow-i-1] != pGrid[pColumn, pRow + i]) return false;
    }

    return true;
}

Vec2i total = new Vec2i();

for (int i = 0; i < grids.Length; i++)
{
    Grid<char> grid = grids[i]; 
    Vec2i mirroredLine = FindMirrorLine(grid, new Vec2i(0, 0));
    total += mirroredLine;
}

Console.WriteLine("Part 1: " + total * new Vec2i(1, 100));

// Part 2: Find a different line of reflection as part of a "fix"
// Approach: brute force the crap out of it :)

Vec2i FindAlternativeMirrorLine(Grid<char> pGrid)
{
    Vec2i originalLineOfReflection = FindMirrorLine(pGrid, new Vec2i(0,0));

    for (int row = 0; row < pGrid.height; row++)
    {
        for (int col = 0; col < pGrid.width; col++)
        {
            //toggle
            char original = pGrid[col, row];
            pGrid[col, row] = pGrid[col, row] == '#' ? '.' : '#';

            Vec2i newMirrorLine = FindMirrorLine(pGrid, originalLineOfReflection);

            if (newMirrorLine.Magnitude() > 0)
            {
                return newMirrorLine;
            }

            //toggle back
            pGrid[col, row] = original;
        }
    }

    throw new Exception("No alternative mirror line found");
}

Stopwatch stopwatch = Stopwatch.StartNew();

total = new Vec2i();

for (int i = 0; i < grids.Length; i++)
{
    Grid<char> grid = grids[i];
    Vec2i mirroredLine = FindAlternativeMirrorLine(grid);
    total += mirroredLine;
}

Console.WriteLine("Part 2: " + total * new Vec2i(1, 100));
Console.WriteLine("Calculated in " + stopwatch.ElapsedMilliseconds + " ms.");
