// Solution for https://adventofcode.com/2021/day/3 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input:
// The diagnostic report (your puzzle input) consists of a list of binary numbers which,
// when decoded properly, can tell you many useful things about the conditions of the submarine.
// The first parameter to check is the power consumption.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

List<string> numbersAsString = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .ToList();

// With these numbers, for every bit position, find the most common bit (1 or 0) and use that
// to construct a new number called gamma. With gamma, construct the opposite called epsilon.
// Multiply them to find the answer.

// First we'll create a method to get the most common for a position
// We could treat everything as numbers already, but it isn't really necessary yet

char GetMostCommon (int pIndex, List<string> pStringsToTest)
{
    int total = pStringsToTest.Count;
    int oneCount = 0;

    foreach (string s in pStringsToTest)
    {
        //zero if '0', one if '1'
        oneCount += s[pIndex] - '0';
    }

    //More than half are 1's?
    //Need to be careful of integer/2 rounding which gives false positives
    return 
        (oneCount == total - oneCount) ? 
            'X' :                                   //if EQUAL return X for error
            ((oneCount > total/2f) ? '1' : '0');    //otherwise decide
}

// Now we'll create gamma from the most common bits founds, assuming we'll never get X (or an exception will occur)

int charCount = numbersAsString[0].Length;
char[] gammaChars = new char[charCount];

for (int i = 0; i < charCount;i++)
{
    gammaChars[i] = GetMostCommon(i, numbersAsString);
}

string gammaString = new string(gammaChars);
Console.WriteLine("Gamma:    " + gammaString.PadLeft(charCount, '0'));

int gamma = Convert.ToInt32(gammaString, 2);

// And with gamma we can create epsilon, by taking the inverse, masked by the amount of bits required

int epsilon = ~gamma & (int)(Math.Pow(2, charCount) - 1);
Console.WriteLine("Epsilon:  " + Convert.ToString(epsilon, 2).PadLeft(charCount, '0'));

Console.WriteLine("Part 1 - Gamma * Epsilon: " + (gamma * epsilon));

// Part 2 - Basically the same as part 1 EXCEPT that after every count we need
// to reduce the list of strings we are testing:
// - 1 list containing only the values with the most common bit at position x
// - 1 list containing only the values with the least common bit at position x

// In other words gamma will no longer be the opposite from epsilon and every step the list reduces further.
//
// This is easily solved using some LINQ + Recursion:

// By default this filters on most common bit, but if you pass true for pLeastCommon it will flip the equation

string FindBinaryPattern (int pIndex, List<string> pStringsToTest, bool pLeastCommon)
{
    //First find the most common char at the given position for the given set
    char mostCommon = GetMostCommon(pIndex, pStringsToTest);

    //If X we need to check whether we are looking for the most or least common
    //By default X defaults to MOST COMMON 1 since we'll flip the test below automatically
    mostCommon = (mostCommon == 'X') ? '1' : mostCommon;

    //Then filter the given set on whether char[pIndex] is the most common (or flip it to look for the least common)
    List<string> newSet = pStringsToTest.Where (x => ((x[pIndex] == mostCommon ^ pLeastCommon))).ToList();

    if (newSet.Count == 1) return newSet[0];
    else return FindBinaryPattern(pIndex + 1, newSet, pLeastCommon);
}

int oxygen = Convert.ToInt32 (FindBinaryPattern(0, numbersAsString, false),2);
int co2 = Convert.ToInt32 (FindBinaryPattern(0, numbersAsString, true),2);

Console.WriteLine("Oxygen: " + oxygen);
Console.WriteLine("Co2: " + co2);

Console.WriteLine("Part 2 - Life support rating: " + (oxygen * co2));