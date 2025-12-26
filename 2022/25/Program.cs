// Solution for https://adventofcode.com/2022/day/25 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: SNAFU numbers :)

string[] myInput = File.ReadAllLines(args[0]);

Dictionary<char, long> char2Value = new Dictionary<char, long>() { { '=', -2 }, { '-', -1 }, { '0', 0 }, { '1', 1 }, { '2', 2 } };

long SnafuToLong (string pInput)
{
    long factor = 1;
    long result = 0;

    int iterator = pInput.Length - 1;

    while (iterator >= 0)
    {
        result += char2Value[pInput[iterator]] * factor;
        factor *= 5;
        iterator--;
    }

    return result;
}

// ** Part 1: Calculating the result:

Console.WriteLine("Part 1 as long:" + myInput.Sum (SnafuToLong));

// How do we convert this to SNAFU?
// When we look at how we convert a normal number to binary, we basically fill up "bits" from right to left, doing:
//
// while (number != 0) {
//   nextbit = number % 2;
//   number =/ 2;
// }
// 
// In this case we can do exactly the same, however if we do:
//
// while (number != 0) {
//   nextbit = number % 5;
//   number =/ 5;
// }
//
// We are not taking our allowed range of -2..2 into account...
// So IF our number turns out to be 3 or 4 we need to bring it back into the allowed range by subtracting 5.
// For the first digit this actually represents 5*1, for the second 5*5, third 125 etc...
// This is nice, but then we do need to make up for it on the next digit, e.g. we subtract 5 from this digit, 
// we'll need to carry this over to the next and add it to what we already had...
// Now I'm just hoping this doesn't add up to 3 anywhere ;)

Dictionary<long, char> reverseMap = new() { { -2, '=' }, { -1, '-' }, { 0, '0' }, { 1, '1' }, { 2, '2' } };

string LongToSnafu(long pInput)
{
    string result = "";
    int carry = 0;

    while (pInput != 0)
    {
        long falloff = pInput % 5;
        int nextCarry = 0;

        if (falloff > 2) { falloff -= 5; nextCarry = 1; }

        result = reverseMap[falloff + carry] + result;
        pInput = pInput / 5;
        carry = nextCarry;
    }

    if (carry != 0) result = reverseMap[carry] + result;

    return result;
}

Console.WriteLine("Part 2 as Snafu: " + LongToSnafu(myInput.Sum(SnafuToLong)));

