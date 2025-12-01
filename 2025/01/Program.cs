// Solution for https://adventofcode.com/2025/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string myInput = File.ReadAllText(args[0]);
myInput = myInput.Replace("R", "+");
myInput = myInput.Replace("L", "-");
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

int[] numbers = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(int.Parse)
    .ToArray();

int Wrap (int pInput, int pWrapValue)
{
    return (((pInput % pWrapValue) + pWrapValue) % pWrapValue);
}

int startValue = 50;
int zeroCount = 0;
foreach (int number in numbers)
{
    startValue += (number);
    startValue = Wrap(startValue, 100);
    if (startValue == 0) zeroCount++;
}

Console.WriteLine("Part 1: " + zeroCount);

startValue = 50;
zeroCount = 0;
foreach (int number in numbers)
{
    for (int i = 0; i < Math.Abs(number); i++)
    {
        startValue += Math.Sign(number);
        startValue = Wrap(startValue, 100);
        if (startValue == 0) zeroCount++;
    }

}

Console.WriteLine("Part 2: " + zeroCount);