// Solution for https://adventofcode.com/2015/day/25 (Ctrl+Click in VS to follow link)

// So, the premise for this one is an ending grid:

/*
   | 1   2   3   4   5   6   ...
-- - +---+---+---+---+---+---+
 1 | 1   3   6  10  15  21
 2 | 2   5   9  14  20
 3 | 4   8  13  19
 4 | 7  12  18
 5 | 11  17
 6 | 16
..
*/

// That means that for a given row and column, we get a specific number N.
// Now INSTEAD of that number N, we start out with the number 20151125 instead of 1
// and for each number after that, we take the last number and:
// * 252533
// % 33554393

// Question is: which number can be found in your specific column and row?

// Couple of observations... let's call a diagonal sequence of number a line...
// So we see lines:
// 1
// 2, 3,
// 4, 5, 6
// 7, 8, 9, 10

// In other words
// Line 1 start at 1
// Line 2 start at 1 + 1
// Line 3 start at 1 + 1 + 2
// Line 4 start at 1 + 1 + 2 + 3
// etc

// So if we can figure out which LINE row 2947, column 3029 is in....
// we can figure out which number it represents and thus how many times we should repeat the equations above to find the code

// For example take (3,4).
// (3,4) is in the diagonal that starts at line (3 + 4 - 1) -> line 6
// Line 6 has at start number 1 + 1 + 2 + 3 + 4 + 5 = 16
// Adding the original (4-1) steps back in we get 19
// (which is correct according to the provided table)

//So in code:

(int row, int column) rc = (2947, 3029);
long startNumber = FindStartNumber(rc);
long actualNumber = startNumber + rc.column - 1;

Console.WriteLine("The number in sequence: "+actualNumber);
Console.WriteLine("The code matching this number: " + Encode(actualNumber));

long FindStartNumber ((int row, int column) pCoordinate)
{
    int line = pCoordinate.row + pCoordinate.column - 1;
    int v = 1;
    for (int i = 1; i <= line; i++) v += (i-1);
    return v;
}

long Encode (long pNumber)
{
    long result = 20151125;
    for (int i = 1; i < pNumber; i++)
    {
        result *= 252533;
        result %= 33554393;
    }
    return result;
}

