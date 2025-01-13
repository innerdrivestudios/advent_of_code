//Solution for https://adventofcode.com/2015/day/25 (Ctrl+Click in VS to follow link)

//So, the premise for this one is an ending grid:

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

//That means that for a given row and column, we get a specific number N.
//Now INSTEAD of that number we take 20151125 and for each number 2..N we:
// * 252533
// % 33554393

//Question is: which N is represent by a specific column and row?

//Couple of observations... let's call a diagonal sequence of number a line...
//So we see lines:
// 1
// 2, 3,
// 4, 5, 6
// 7, 8, 9, 10

//In other words, each line N starts at line(N-1).end+1 and has one element more than the previous range

/**/

//In code, as a demo, before we do the real stuff...

Console.WriteLine("Demo of range built up...");

(int start, int end) ranges = (0, 0);

for (int i = 1; i < 6; i++)
{
    ranges = (ranges.end + 1, ranges.end + i);
    Console.WriteLine(ranges);
}

/**/

//So if we can figure out which LINE row 2947, column 3029 is in....
//we can figure out which number it represents and thus how many times we should repeat the equations above to find the code

(int row, int column) rc = (2947, 3029);
//(int row, int column) rc = (3, 4);

//Look at (3, 4) (number 19), we can figure out which line this coordinate is in by doing:
//3 + 4 - 1 = 6
//For 6, we can figure out the start number (16). To get to 19, we simply do 16 + (4-1) = 19.

//So in code:

long startNumber = FindStartNumber(rc);
long actualNumber = startNumber + rc.column - 1;

Console.WriteLine("The number in sequence: "+actualNumber);
Console.WriteLine("The code matching this number: " + Encode(actualNumber));

long FindStartNumber ((int row, int column) pCoordinate)
{
    int line = pCoordinate.row + pCoordinate.column - 1;

    (int start, int end) ranges = (0, 0);

    for (int i = 1; i <= line; i++)
    {
        ranges = (ranges.end + 1, ranges.end + i);
    }

    return ranges.start;
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

