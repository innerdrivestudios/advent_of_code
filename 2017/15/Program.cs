//Solution for https://adventofcode.com/2017/day/15 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input will be used by going to Debug/Debug Properties
// and specifying two values as command line arguments, e.g. 591 393 (no quotes). 
// These values will be passed to the built-in args[0/1] variables

long aStart = int.Parse(args[0]);
long bStart = int.Parse(args[1]);

// ** Part 1 & 2 are exactly the same with a different modulo:

// The generators both work on the same principle.
// To create its next value, a generator will take the previous value it produced,
// multiply it by a factor (generator A uses 16807; generator B uses 48271),
// and then keep the remainder of dividing that resulting product by 2147483647.
// That final remainder is the value it produces next.

// Define some helper methods:

long NextValue (long pInput, long pFactor, long pMultipleOf = 1)
{
    while (true)
    {
        pInput = (pInput * pFactor) % 2147483647;
        if (pInput % pMultipleOf == 0) break;
    }

    return pInput;
}

// ** Part 1:

long matchCount = 0;

for (int i = 0; i < 40000000; ++i)
{
    aStart = NextValue (aStart, 16807);
    bStart = NextValue (bStart, 48271);

    if ((aStart & 0xFFFF) == (bStart & 0xFFFF)) matchCount++;
}

Console.WriteLine("Part 1:" + matchCount);

// ** Part 2:

// Reset a & b:

aStart = int.Parse(args[0]);
bStart = int.Parse(args[1]);

matchCount = 0;

for (int i = 0; i < 5000000; ++i)
{
    aStart = NextValue(aStart, 16807, 4);
    bStart = NextValue(bStart, 48271, 8);

    if ((aStart & 0xFFFF) == (bStart & 0xFFFF)) matchCount++;
}

Console.WriteLine("Part 2:" + matchCount);

