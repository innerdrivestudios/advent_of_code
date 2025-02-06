//Solution for https://adventofcode.com/2016/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of password scrambling instructions

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// first define all individual methods, the reverse parameter is for part 2

void SwapPosition (string pInstruction, List<char> pInput, bool pReverse = false) {
    Regex parser = new Regex(@"position (\d)");
    MatchCollection matches = parser.Matches(pInstruction);

    //Match will only be a single digit 0-9, subtracting '0' gives us the int value
    int positionA = matches[0].Groups[1].Value[0] - '0';
    int positionB = matches[1].Groups[1].Value[0] - '0';

    SwapChars(pInput, positionA, positionB);
}

void SwapLetter (string pInstruction, List<char> pInput, bool pReverse = false)
{
    Regex parser = new Regex(@"letter ([a-z])");
    MatchCollection matches = parser.Matches(pInstruction);

    //Match will only be a single digit 0-9, subtracting '0' gives us the int value
    int positionA = pInput.IndexOf(matches[0].Groups[1].Value[0]);
    int positionB = pInput.IndexOf(matches[1].Groups[1].Value[0]);

    SwapChars(pInput, positionA, positionB);
}

void MovePosition(string pInstruction, List<char> pInput, bool pReverse = false)
{
    Regex parser = new Regex(@"position (\d)");
    MatchCollection matches = parser.Matches(pInstruction);

    //Match will only be a single digit 0-9, subtracting '0' gives us the int value
    int positionA = matches[0].Groups[1].Value[0] - '0';
    int positionB = matches[1].Groups[1].Value[0] - '0';

    // abcdefg 0 - 4 --> bcdeafg
    // bcdeafg 4 - 0

    if (pReverse)
    {
        int tmp = positionA;
        positionA = positionB;
        positionB = tmp;
    }

    char a = pInput[positionA];
    pInput.RemoveAt(positionA);
    pInput.Insert(positionB, a);
}

void Rotate (string pInstruction, List<char> pInput, bool pReverse = false)
{
    bool left = pInstruction[ 7 ] == 'l';
    int amount = pInstruction[ pInstruction.IndexOf(" step") - 1 ] - '0';

    int endIndex = pInput.Count - 1;
    int grabIndex = (left ^ pReverse) ? 0 : endIndex;
    int putIndex = endIndex - grabIndex;

    for (int i = 0; i < amount; i++)
    {
        char a = pInput[grabIndex];
        pInput.RemoveAt(grabIndex);
        pInput.Insert(putIndex, a);
    }
}


//This bit of code is for part 2, based on where a char ends up, we'll store how to reverse it
int[] reversePositionTable = new int[8];

for (int i = 0; i < 8; i++)
{
    int charIndex = i;
    int originalAmount = charIndex + 1 + (charIndex >= 4 ? 1 : 0);
    int newPosition = (charIndex + originalAmount) % 8;
    reversePositionTable[newPosition] = originalAmount;
}

void RotateCharBased(string pInstruction, List<char> pInput, bool pReverse = false)
{
    char c = pInstruction.Last();
    int charIndex = pInput.IndexOf(c);

    // assume we are going forward ....
    int amount = charIndex + 1 + (charIndex >= 4 ? 1 : 0);

    // however if we are going backward ....
    // the issue here is that the amount depends on the original position, which not easily reversed
    if (pReverse) amount = reversePositionTable[charIndex];

    int grabIndex = pInput.Count - 1;
    int putIndex = 0;

    if (pReverse)
    {
        putIndex = grabIndex;
        grabIndex = 0; 
    }

    for (int i = 0; i < amount; i++)
    {
        char a = pInput[grabIndex];
        pInput.RemoveAt(grabIndex);
        pInput.Insert(putIndex, a);
    }
}

void ReversePositions (string pInstruction, List<char> pInput, bool pReverse = false)
{
    Regex parser = new Regex(@" (\d)");
    MatchCollection matches = parser.Matches(pInstruction);

    //Match will only be a single digit 0-9, subtracting '0' gives us the int value
    int positionA = matches[0].Groups[1].Value[0] - '0';
    int positionB = matches[1].Groups[1].Value[0] - '0';

    int middleIndex = (positionB - positionA) / 2;

    for (int i = 0; i < middleIndex+1; i++)
    {
        char a = pInput[positionA + i];
        pInput[positionA + i] = pInput[positionB - i];
        pInput[positionB - i] = a;
    }
}

void SwapChars (List<char> pInput, int pIndexA, int pIndexB)
{
    char tmp = pInput[pIndexA];
    pInput[pIndexA] = pInput[pIndexB];
    pInput[pIndexB] = tmp;
}

void TestMethod (Action<string, List<char>, bool> pFunc, string pInstruction, List <char> pInput, bool pReverse = true)
{
    List<char> inputClone = new (pInput);
    pFunc(pInstruction, inputClone, false);
    pFunc(pInstruction, inputClone, true);

    Console.WriteLine(string.Concat(pInput) + " ==> " + pInstruction + " ==> " + string.Concat(inputClone));
}

List<char> passwordToScramble = "abcdefgh".ToList();

if (false)
{
    TestMethod(SwapPosition, "swap position 0 with position 7", passwordToScramble);
    TestMethod(SwapPosition, "swap position 7 with position 0", passwordToScramble);
    TestMethod(SwapPosition, "swap position 4 with position 4", passwordToScramble);

    TestMethod(SwapLetter, "swap letter a with letter c", passwordToScramble);
    TestMethod(SwapLetter, "swap letter d with letter f", passwordToScramble);
    TestMethod(SwapLetter, "swap letter d with letter d", passwordToScramble);

    TestMethod(MovePosition, "move position 4 to position 0", passwordToScramble);
    TestMethod(MovePosition, "move position 0 to position 4", passwordToScramble);

    TestMethod(Rotate, "rotate left by 3 steps", passwordToScramble);
    TestMethod(Rotate, "rotate right by 2 steps", passwordToScramble);

    TestMethod(ReversePositions, "reverse positions 0 through 7", passwordToScramble);
    TestMethod(ReversePositions, "reverse positions 2 through 7", passwordToScramble);
    TestMethod(ReversePositions, "reverse positions 2 through 5", passwordToScramble);
    TestMethod(ReversePositions, "reverse positions 2 through 2", passwordToScramble);
    TestMethod(ReversePositions, "reverse positions 2 through 3", passwordToScramble);

    TestMethod(RotateCharBased, "rotate based on position of letter f", passwordToScramble);
}


// Now actually execute all the instructions
string Encode (string pPassword)
{
    List<char> passwordToScramble = pPassword.ToList();

    foreach (string instruction in instructions)
    {
        if (instruction.StartsWith("swap position")) SwapPosition(instruction, passwordToScramble);
        else if (instruction.StartsWith("swap letter")) SwapLetter(instruction, passwordToScramble);
        else if (instruction.StartsWith("move position")) MovePosition(instruction, passwordToScramble);
        else if (instruction.StartsWith("rotate based")) RotateCharBased(instruction, passwordToScramble);
        else if (instruction.StartsWith("rotate")) Rotate(instruction, passwordToScramble);
        else if (instruction.StartsWith("reverse")) ReversePositions(instruction, passwordToScramble);
        else Console.WriteLine(instruction + " not supported.");
    }

    return string.Concat(passwordToScramble);
}

Console.WriteLine("Part 1 - Scrambled Password:" + Encode ("abcdefgh"));

// Part 2 : Sigh, nice :)
// How I fixed this: add a pReverse parameter to all operations that are not symmetric

// and now run everything in reverse...

// Now actually execute all the instructions
string Decode(string pPassword)
{
    List<char> passwordToUnscramble = pPassword.ToList();

    for (int i = instructions.Length - 1; i >= 0; i--)
    {
        string instruction = instructions[i];

        if (instruction.StartsWith("swap position")) SwapPosition(instruction, passwordToUnscramble);
        else if (instruction.StartsWith("swap letter")) SwapLetter(instruction, passwordToUnscramble);
        else if (instruction.StartsWith("move position")) MovePosition(instruction, passwordToUnscramble, true);
        else if (instruction.StartsWith("rotate based")) RotateCharBased(instruction, passwordToUnscramble, true);
        else if (instruction.StartsWith("rotate")) Rotate(instruction, passwordToUnscramble, true);
        else if (instruction.StartsWith("reverse")) ReversePositions(instruction, passwordToUnscramble);
        else Console.WriteLine(instruction + " not supported.");
    }

    return string.Concat(passwordToUnscramble);
}

Console.WriteLine("Part 2 - Unscrambled Password:" + Decode ("fbgdceah"));

// Alternatively we could brute force it... (which is slow of course since we have to iterate over all possible permutations)

Console.WriteLine("\nBrute forcing part 2 ...");

var permutations = "abcdefgh".ToList().GetPermutations();

foreach (var permutation in permutations)
{
    string password = string.Concat(permutation);
    if (Encode(password) == "fbgdceah")
    {
        Console.WriteLine("Found: " + password);
        break;
    }
}
