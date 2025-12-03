// Solution for https://adventofcode.com/2025/day/3 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: joltage ratings

using System.Diagnostics;

string[] myInput = File.ReadAllLines(args[0]);

// ** Part 1: Run a basic "collision" loop

int CalculateMaxJoltagePart1 (string pInput)
{
    int max = 0;

    for (int x = 0; x < pInput.Length - 1; x++)
    {
        for (int y = x+1; y < pInput.Length; y++)
        {
            int newMax = (pInput[x]-'0') * 10 + (pInput[y]-'0');
            if (newMax > max) max = newMax;
        }
    }
    return max;
}

Console.WriteLine("Part 1: " + myInput.Sum(CalculateMaxJoltagePart1));

// * Part 2:

// We need to write a loop similar to the one above but now not nested 2 deep (since we were looking for 2 numbers)
// but x deep where x is the amount of numbers we are looking for. 


long CalculateMaxJoltagePart2 (string pInput, int pNumberCount, int pIndex = 0)
{
    // If we've reached the end return 0; 
    if (pNumberCount == 0) return 0;

    // Instead of blindly iterating over everything, only consider the highest index in our remain run
    int indexToConsider = new ();
    int highestNumberFound = -1;

    for (int i = pIndex; i < pInput.Length - (pNumberCount - 1); i++)
    {
        int digit = pInput[i] - '0';

        if (digit > highestNumberFound)
        {
            indexToConsider = i;
            highestNumberFound = digit;
        }
    }

    // Calculate the resulting number
    return  highestNumberFound * (long)Math.Pow(10, pNumberCount - 1) + 
            CalculateMaxJoltagePart2(pInput, pNumberCount - 1, indexToConsider + 1);
}

Stopwatch sw = Stopwatch.StartNew();

long total = 0;
foreach (string input in myInput)
{
    total += CalculateMaxJoltagePart2(input, 12);
}

Console.WriteLine("Part 2: " + total); 
Console.WriteLine("Calculated in " + sw.ElapsedMilliseconds + " ms");

