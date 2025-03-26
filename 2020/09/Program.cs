// Solution for https://adventofcode.com/2020/day/9 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers

long[] myInput = ParseUtils.FileToArrayOf<long>(args[0], Environment.NewLine);

// ** Part 1: Find the first number in the list which is not a valid combination of numbers in the preamble

long FindWeakestNumber (long[] pInput, int pPreamble)
{
    //we start testing after the preamble
    int current = pPreamble + 1;
    while (IsValid(myInput, pPreamble, current)) current++;
    return current < myInput.Length? myInput[current] : -1;
}

bool IsValid(long[] pInput, int pPreamble, int pCurrent)
{
    if (pPreamble + pCurrent >= pInput.Length) return false;
    
    //Do a double collision loop (so to speak) to find a sum that matches the current value
    for (int i = 1; i <= pPreamble-1; i++)
    {
        for (int j = i+1; j <= pPreamble; j++)
        {
            if (pInput[pCurrent] == pInput[pCurrent-i] + pInput[pCurrent-j]) return true;
        }
    }
    return false;
}

long result = FindWeakestNumber(myInput, 25);
Console.WriteLine("Part 1:" + result);

// ** Part 2: First find the contiguous set of numbers that make up the weakest number
// then add the first and list element in that list.

long FindEncryptionWeakness(long[] pInput, long pWeakness)
{
    //basic principle:
    //we start looping from index 'start' onwards, looking for the correct sum...
    //if we go OVER the sum, we stop looking, and restart the proces from start+1, etc
    //until we find the exact sum we are looking for.

    //while we are doing this, we keep track of the min and max value encountered during the current loop
    //so we can use them to calculate the requested result

    //some initial values
    int start = 0;
    long result = 0;

    //while we are not at the end of the list...
    while (start < pInput.Length)
    {
        //initialize the sum to zero 
        long sum = 0;
        long smallest = long.MaxValue;
        long largest = long.MinValue;
        int current = start;

        while (sum < pWeakness)
        {
            long nextValue = pInput[current];
            smallest = Math.Min(nextValue, smallest);
            largest = Math.Max(nextValue, largest);
            sum += nextValue;
            current++;
        }

        if (sum == pWeakness)
        {
            result = smallest + largest;
            break;
        }

        start++;
    }

    return result;
}

Console.WriteLine("Part 2:" + FindEncryptionWeakness(myInput, result));