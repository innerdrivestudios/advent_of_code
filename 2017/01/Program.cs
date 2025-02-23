// Solution for https://adventofcode.com/2017/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers...

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// ** Part 1 & 2
//
// Calculate a "Captcha" by adding up all numbers that match a next number in the sequence.
// wrapping around at the end of the sequence.

// For example:

// 1122 produces a sum of 3 (1 + 2) because the first digit (1) matches the second digit and the third digit (2) matches the fourth digit.
// 1111 produces 4 because each digit (all 1) matches the next.
// 1234 produces 0 because no digit matches the next.
// 91212129 produces 9 because the only digit that matches the next one is the last digit, 9.

// Part 2 is the same as part 1 except the offset for the next item is no longer 1 (the next) but have the length of the sequence.

int CalculateCaptcha(string pInput, int pOffset)
{
    int total = 0;

    for (int i = 0; i < pInput.Length; i++)
    {
        if (pInput[i] == pInput[(i + pOffset) % pInput.Length]) total += (pInput[i] - '0');
    }

    return total;
}

Console.WriteLine("Part 1 - Comparing any number with the next: " + CalculateCaptcha(myInput, 1));
Console.WriteLine("Part 2 - Comparing any number with the next halfway around: " + CalculateCaptcha(myInput, myInput.Length/2));


