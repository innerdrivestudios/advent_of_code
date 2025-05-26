//Solution for https://adventofcode.com/2017/day/14 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input value will be used by going to Debug/Debug Properties
// and specifying the value as a command line argument, e.g. amgozmfv 
// This value will be passed to the built-in args[0] variable

// ** Your input: a partial hash key

using System.Numerics;

string myInput = args[0];

// Let's make a quick bit lookup table ...

var charToBitCount = Enumerable.Range(0, 16)                // Generate numbers 0-15 (16 numbers)
    .ToDictionary(                                          // and construct a dictionary by taking this number
        i => i.ToString("x")[0],                            // Converting it to hex taking the first char as key
        i => Convert.ToString(i, 2).Count(c => c == '1')    // Converting it to binary and counting the bits
    );

// ** Part 1: Count the bits / blocks in a 128*128 grid using your given key:

Console.WriteLine(
    "Part 1:" + //Note range uses start, count, not start, end
    Enumerable.Range(0, 128).Sum(x => KnotHashLib.KnotHash(myInput + "-" + x).Sum(y => charToBitCount[y]))
);

// ** Part 2: How many different block regions are created by these patterns?

// Step 1. Create a grid containing these blocks as true/false values:

Grid<bool> diskMap = new Grid<bool>(128, 128);
BigInteger bitOne = new BigInteger(1);

for (int y = 0; y < 128; y++)
{
    //get the knot hash, 32 chars, 4 bits per char
    string knotHash = KnotHashLib.KnotHash (myInput + "-" + y);
    //convert to biginteger, prepend the 0 to make sure this value is interpreted as positive
    BigInteger bigInteger = BigInteger.Parse("0"+knotHash, System.Globalization.NumberStyles.HexNumber);

    for (int x = 0; x < 128; x++)
    {
        diskMap[x, y] = (bigInteger & (bitOne << x)) > 0;
    }
}

// Get all the regions indicated by the true values and count them
Console.WriteLine("Part 2:" + diskMap.GetRegions([true]).Count);
