// Solution for https://adventofcode.com/2016/day/16 (Ctrl+Click in VS to follow link)

// ** Your input: a binary sequence...

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

using System.Text;

string myInput = args[0];

// Part 1: Apply a modified dragon curve to generate random data

string ApplyDragonCurve (string pInput)
{
    char[] original = pInput.ToCharArray();
    char[] reversedAndInverted = new char[original.Length];

    for (int i = 0; i < original.Length; i++)
    {
        reversedAndInverted[original.Length - i - 1] = (char)('0' + ('1' - original[i]));
    }

    return pInput + "0" + new string (reversedAndInverted);
}

// Method to calculate the checksum, make sure you use a string builder for performance !

string CalculateChecksum (string pInput)
{
    Console.WriteLine("Calculating checksum for "+pInput.Length + " bytes...");
    StringBuilder newString = new StringBuilder();

    for (int i = 0; i < pInput.Length; i += 2)
    {
        newString.Append(pInput[i] == pInput[i + 1] ? "1" : "0");
    }

    if (newString.Length % 2 == 0) return CalculateChecksum(newString.ToString());
    else return newString.ToString();
}

string FillDisk (string pInput, int pLength)
{
    // Generate until the "disk" is "full"

    string randomData = pInput;
    while (randomData.Length < pLength)
    {
        randomData = ApplyDragonCurve(randomData);
    }

    // Get only the disk portion
    randomData = randomData.Substring(0, pLength);

    Console.WriteLine("Disk filled with " + pLength + " bytes of data...");
    return randomData;
}

Console.WriteLine("Part 1 - What is the correct checksum?:" + CalculateChecksum(FillDisk(myInput, 272)));
Console.WriteLine("Part 2 - What is the correct checksum?:" + CalculateChecksum(FillDisk(myInput, 35651584)));


