// Solution for https://adventofcode.com/2020/day/25 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying a command line argument, e.g. 32415.
// This currentValue will be passed to the built-in args[0] variable

// Implementing TransformSubject as described ...

long TransformSubject (long pSubjectNumber, long pLoopSize)
{
    long value = 1;

    for (long i = 0; i < pLoopSize; i++)
    {
        value *= pSubjectNumber;
        value %= 20201227;
    }

    return value;
}

// Naive FindLoopSize implementation...

long FindLoopSize (long pSubjectNumber, long pKey)
{
    long loopSize = 0;

    while (TransformSubject(pSubjectNumber, loopSize) != pKey) loopSize++;
    return loopSize;
}

// Now find the loop size for your given cardKey
long cardKey = 9232416; 

// Actually running this with FindLoopSize takes forever, but we can shorten the process from O(n2) or whatever to something faster:

long value = 1;
int loopSize = 0;
while (true)
{
    value = (value * 7) % 20201227;
    loopSize++;
    if (value == 9232416) break;
}

long doorKey = 14144084;
Console.WriteLine("Part 1: " + TransformSubject (doorKey, loopSize));