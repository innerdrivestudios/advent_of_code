// Solution for https://adventofcode.com/2017/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of jump offsets like 0 3 0 1 -4 etc

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: How many steps does it take to reach the exit ?

int[] jumpOffsets = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

int[] jumpOffsetCopy = (int[]) jumpOffsets.Clone();

int jumpPointer = 0;
int stepCount = 0;

// While not exited...
while (jumpPointer < jumpOffsetCopy.Length)
{
    // Update the jumppointer while incrementing the jumpoffset afterwards
    jumpPointer += jumpOffsetCopy[jumpPointer]++;
    stepCount++;
}

Console.WriteLine("Part 1 (Step count to exit):" + stepCount);

// ** Part 2: How many steps does it now take to reach the exit under the new rules?

// Don't forget to restore the input !
jumpOffsetCopy = (int[])jumpOffsets.Clone();

jumpPointer = 0;
stepCount = 0;

// While not exited...
while (jumpPointer < jumpOffsetCopy.Length)
{
    // Update the jumppointer while incrementing the jumpoffset afterwards
    int jumpOffset = jumpOffsetCopy[jumpPointer];

    if (jumpOffset >= 3) jumpOffsetCopy[jumpPointer] -= 1;
    else jumpOffsetCopy[jumpPointer] += 1;

    jumpPointer += jumpOffset;
    stepCount++;
}

Console.WriteLine("Part 2 (Step count to exit):" + stepCount);