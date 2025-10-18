// Solution for https://adventofcode.com/2017/day/21 (Ctrl+Click in VS to follow link)

using System.Text;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A list of pattern matches

string[] myInput = File.ReadAllLines(args[0]);

// Ok, how do we process them?
// There might be very quick ways to do this with a lot of math, not entirely sure, 
// but to keep it simple, I'm going to convert the provided patterns to 2*2 arrays,
// 3*3 arrays and matching binary numbers.
// In other words, we'll store binary number => binary number,
// and convert to and from arrays where needed.

// Helper method to turn a string into a 2d char array:

char[,] PatternToArray (string pPattern)
{
    // first lose all the /
    string input = pPattern.Replace("/", "");
    int dimension = (int)Math.Sqrt (input.Length);

    char[,] array = new char[dimension, dimension];

    for (int i = 0; i < input.Length; i++)
    {
        int x = i % dimension;
        int y = i / dimension;
        array[x,y] = input[i];
    }

    return array;
}

// Helper method to flip an array, note that combined with rotation we only need to flip in one direction,
// since flipping in two direction is simply rotating

char[,] FlipArrayHorizontal (char[,] pInput)
{
    int dimension = pInput.GetLength(0);
    char[,] copy = new char[dimension, dimension];

    for (int y = 0; y < dimension; y++)
    {
        for (int x = 0; x < dimension; x++)
        {
            copy[dimension - 1 - x, y] = pInput[x,y];
        }
    }

    return copy;
}

// Helper method to rotate an array

char[,] RotateArrayClockwise (char[,] pInput, int pTurns) {
    int dimension = pInput.GetLength(0);
    char[,] copy = new char[dimension, dimension];
    double degreesToRadians = double.DegreesToRadians((pTurns%4) * 90);
    double cos = Math.Cos(degreesToRadians);
    double sin = Math.Sin(degreesToRadians);
    double offset = (dimension-1) / 2d;

    for (int y = 0; y < dimension; y++)
    {
        for (int x = 0; x < dimension; x++)
        {
            double oX = x - offset;
            double oY = y - offset;
            double nX = oX * cos - oY * sin;
            double nY = oX * sin + oY * cos;

            nX += offset;
            nY += offset;

            copy[(int)Math.Round(nX), (int)Math.Round(nY)] = pInput[x, y];
        }
    }

    return copy;
}

// Helper method for debugging...

string ToString(char[,] pArray)
{
    StringBuilder sb = new StringBuilder();

    for (int y = 0; y < pArray.GetLength(1); y++)
    {
        for (int x = 0; x < pArray.GetLength(0); x++)
        {
            if (pArray[x, y] == 0) sb.Append(' ');
            else sb.Append(pArray[x, y]);
        }
        sb.AppendLine();
    }

    return sb.ToString();
}

// Convert a pattern to binary... include some parameters we can use to convert parts of a bigger array later

int ConvertToBinary (char[,] pArray, int pDimension = -1, Vec2i pOffset = default)
{
    pDimension = pDimension < 0 ? pArray.GetLength(0) : pDimension;
    int pattern = 0;
    
    for (int y = 0; y < pDimension; y++)
    {
        for (int x = 0; x < pDimension; x++)
        {
            pattern <<=1;
            pattern |= pArray[x + pOffset.X, y + pOffset.Y] == '#' ? 1 : 0;
        }
    }

    return pattern;
}

// With this in place we can build our pattern map however, we need to match both 2x2 patterns and 3x3 patterns, 
// whose IDs might overlap, so we will build two different maps:
Dictionary<int, int> patternMap2x2To3x3 = new();
Dictionary<int, int> patternMap3x3To4x4 = new();

// Now fill the pattern map
foreach (string input in myInput)
{
    string[] pairs = input.Split(" => ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    char[,] sourcePattern = PatternToArray(pairs[0]);
    char[,] targetPattern = PatternToArray(pairs[1]);

    Dictionary<int, int> conversionMap = sourcePattern.Length == 4 ? patternMap2x2To3x3 : patternMap3x3To4x4;

    char[,] flippedSourcePattern = FlipArrayHorizontal(sourcePattern);

    int convertedTargetPattern = ConvertToBinary(targetPattern);

    for (int i = 0; i < 4; i++)
    {
        char[,] pattern1 = RotateArrayClockwise(sourcePattern, i);
        char[,] pattern2 = RotateArrayClockwise(flippedSourcePattern, i);

        int binaryPattern1 = ConvertToBinary(pattern1);
        int binaryPattern2 = ConvertToBinary(pattern2);
        
        conversionMap[binaryPattern1] = convertedTargetPattern;
        conversionMap[binaryPattern2] = convertedTargetPattern;
    }
}

// ** Part 1: Process the grid 5 times and check how many pixels are on.

// Question is of course, how to best store the grid? 
// Are we going to store an ever expanding 2D array and process data from that and into that?
// Or store things as hashsets?

// Let's try using 2D arrays first, since it is simpler although it is more copying

// Next to what we already have we'll need two more things:
// 1. A method to convert a binary pattern back to an array
// 2. A method to chunk an array, convert the parts and construct a new array

// Convert a binary number back to a pattern
// Since we need to be able to do this in a target array, we don't return anything

void BinaryToArrayContent (char[,] pTargetArray, int pBinary, int pDimension, Vec2i pOffset = default)
{
    int numberOfBitsToProcess = pDimension * pDimension;

    for (int i = 0; i < numberOfBitsToProcess; i++)
    {
        int x = i % pDimension;
        int y = i / pDimension;

        int bit = pBinary & (1 << (numberOfBitsToProcess - i - 1));

        pTargetArray[pOffset.X + x, pOffset.Y + y] = bit > 0 ? '#' : '.';
    }
}

// And now the main work horse, a method to chunk the original array, do conversion and construct a new array

char[,] Iterate(char[,] pInput)
{
    int dimension = pInput.GetLength(0);
    bool sizeEven = (dimension % 2 == 0);
    Dictionary<int, int> conversionTable = sizeEven ? patternMap2x2To3x3 : patternMap3x3To4x4;

    int inputBlockSize = sizeEven ? 2 : 3;
    int outputBlockSize = sizeEven ? 3 : 4;

    Vec2i inputOffset = sizeEven ? new Vec2i(2, 2) : new Vec2i(3, 3);
    Vec2i outputOffset = sizeEven ? new Vec2i(3, 3) : new Vec2i(4, 4);

    int blockCount = pInput.GetLength(0) / inputBlockSize;

    char[,] outputArray = new char[blockCount * outputBlockSize, blockCount * outputBlockSize];

    for (int x = 0; x < blockCount; x++)
    {
        for (int y = 0; y < blockCount; y++)
        {
            int binaryValue = ConvertToBinary(pInput, inputBlockSize, inputOffset.Scale(new Vec2i(x, y)));
            int outputValue = conversionTable[binaryValue];
            BinaryToArrayContent(outputArray, outputValue, outputBlockSize, outputOffset.Scale(new Vec2i(x, y)));
        }
    }

    return outputArray;
}

char[,] startingBlock = new char[3, 3];
                                    //.#./..#/###
BinaryToArrayContent(startingBlock, 0b010001111, 3);

for (int i = 0; i < 5; i++)
{
    startingBlock = Iterate (startingBlock);
}

// Slow and lazy... but fast enough :)
int litPixels = ToString(startingBlock).Count(x => x == '#');
Console.WriteLine("Part 1:" + litPixels);

// Part 2: How many pixels are on after 18 iterations?

// We already have 5 iterations, so skip those... other than that, same thing
for (int i = 0; i < 18-5; i++)
{
    startingBlock = Iterate(startingBlock);
}

// Slow and lazy... but fast enough :)
litPixels = ToString(startingBlock).Count(x => x == '#');
Console.WriteLine("Part 2:" + litPixels);

