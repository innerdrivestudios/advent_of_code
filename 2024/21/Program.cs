// Solution for https://adventofcode.com/2024/day/21 (Ctrl+Click in VS to follow link)

using KeyPad = System.Collections.Generic.Dictionary<char, Vec2<int>>;
using TransitionTable = System.Collections.Generic.Dictionary<(char, char), string[]>;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** First let's parse the input...

string[] myInput = File.ReadAllLines(args[0]);

// ** Part 1: Find the fewest number of button presses you'll need to
// perform in order to cause the robot in front of the door to type each code.
// What is the sum of the complexities of the five codes on your list?

// Define the keypad as described:

KeyPad numericKeypadPositions = new KeyPad();
numericKeypadPositions['7'] = new Vec2i(0, 0);
numericKeypadPositions['8'] = new Vec2i(1, 0);
numericKeypadPositions['9'] = new Vec2i(2, 0);
numericKeypadPositions['4'] = new Vec2i(0, 1);
numericKeypadPositions['5'] = new Vec2i(1, 1);
numericKeypadPositions['6'] = new Vec2i(2, 1);
numericKeypadPositions['1'] = new Vec2i(0, 2);
numericKeypadPositions['2'] = new Vec2i(1, 2);
numericKeypadPositions['3'] = new Vec2i(2, 2);
//gap at 0,3
numericKeypadPositions['0'] = new Vec2i(1, 3);
numericKeypadPositions['A'] = new Vec2i(2, 3);

// Define the directional keypad as described:

KeyPad controlKeypadPositions = new KeyPad();
//gap at 0,0
controlKeypadPositions['^'] = new Vec2i(1, 0);
controlKeypadPositions['A'] = new Vec2i(2, 0);
controlKeypadPositions['<'] = new Vec2i(0, 1);
controlKeypadPositions['v'] = new Vec2i(1, 1);
controlKeypadPositions['>'] = new Vec2i(2, 1);

// Define a table that allows us to convert a number of directions (to navigate around the keypad) back to characters
Dictionary<Vec2i, char> controlKeypadDirection2Value = new Dictionary<Vec2i, char>();
controlKeypadDirection2Value[new Vec2i(1, 0)] = '>';
controlKeypadDirection2Value[new Vec2i(0, -1)] = '^';
controlKeypadDirection2Value[new Vec2i(-1, 0)] = '<';
controlKeypadDirection2Value[new Vec2i(0, 1)] = 'v';

// Generate two tables that based on a given keypad give us the shortest character sequences to go from 1 character to another
TransitionTable numericKeypadTransitions = new TransitionTable();
TransitionTable controlKeypadTransitions = new TransitionTable();

FillTable(numericKeypadTransitions, numericKeypadPositions);
FillTable(controlKeypadTransitions, controlKeypadPositions);

void FillTable(TransitionTable table, KeyPad keyPad)
{
    foreach (char s in keyPad.Keys)
    {
        foreach (char e in keyPad.Keys)
        {
            if (s == e)
            {
                table[(s, e)] = ["A"];
            }
            else
            {
                Vec2i startPosition = keyPad[s];
                Vec2i endPosition = keyPad[e];

                // We need to get all possible paths... since even if a string is longer, there might be less changes in it
                List<List<Vec2i>> paths = GetPaths(startPosition, endPosition, keyPad);

                int leastChangeCount = int.MaxValue;
                List<string> directionStrings = new List<string>();

                foreach (List<Vec2i> path in paths)
                {
                    string directionString = GetDirectionString(path);
                    int changes = CountChanges(directionString);

                    if (changes <= leastChangeCount)
                    {
                        if (changes < leastChangeCount)
                        {
                            directionStrings.Clear();
                            leastChangeCount = changes;
                        }

                        directionStrings.Add(directionString);
                    }
                }

                table[(s, e)] = directionStrings.ToArray();
            }
        }
    }
}

// Get all the possible paths on the given keypad given the given start and end position

List<List<Vec2i>> GetPaths(Vec2i pStartPosition, Vec2i pEndPosition, Dictionary<char, Vec2i> pKeyPadToUse, int pDepth = 0)
{
    //If we are at the end, then there is only one path to add to the list
    if (pStartPosition.Equals(pEndPosition))
    {
        return new List<List<Vec2i>>() { new List<Vec2i>() { pEndPosition } };
    }

    List<List<Vec2i>> paths = new List<List<Vec2i>>();

    Vec2i delta = pEndPosition - pStartPosition;

    int directionX = Math.Sign(delta.X);
    int directionY = Math.Sign(delta.Y);

    if (directionX != 0)
    {
        Vec2i nextStep = pStartPosition + new Vec2i(directionX, 0);
        if (pKeyPadToUse.ContainsValue(nextStep))
        {
            List<List<Vec2i>> subPaths = GetPaths(nextStep, pEndPosition, pKeyPadToUse, pDepth + 1);

            foreach (var path in subPaths)
            {
                path.Insert(0, pStartPosition);
                paths.Add(path);
            }
        }
    }

    if (directionY != 0)
    {
        Vec2i nextStep = pStartPosition + new Vec2i(0, directionY);
        if (pKeyPadToUse.ContainsValue(nextStep))
        {
            List<List<Vec2i>> subPaths = GetPaths(nextStep, pEndPosition, pKeyPadToUse, pDepth + 1);

            foreach (var path in subPaths)
            {
                path.Insert(0, pStartPosition);
                paths.Add(path);
            }
        }
    }

    return paths;
}

string GetDirectionString(List<Vec2i> pPath)
{
    string directionString = "";

    for (int i = 0; i < pPath.Count - 1; i++)
    {
        directionString += controlKeypadDirection2Value[pPath[i + 1] - pPath[i]];
    }

    return directionString+'A';
}

// Every direction changes requires a bunch of extra button presses,
// so let's count the changes in an input sequence, so we can minimize it

int CountChanges(string pInput)
{
    int count = 0;
    char current = pInput[0];
    for (int i = 1; i < pInput.Length; i++)
    {
        if (current != pInput[i]) count++;
        current = pInput[i];
    }
    return count;
}

// Then to translate a code, we need to know what presses are required to enter the code, starting from the A char

//iterative method that can be used to convert a given code to the "next" code
List<string> TranslateCode(string pCode, TransitionTable pTable)
{
    //the idea is we convert the code char by char, but each char-char conversion may take multiple paths 
    //(e.g. we can go from A to 9 in different ways)

    //instead of doing this recursively, we start with an empty string and create a new list where each item is:
    //"" + a single possible conversion

    //then that list becomes the new result list, so in the next step we create a new list again where we combine
    //EVERY item in the results list with EVERY possible conversion, rinse and repeat

    //we start with an empty list
    List<string> results = [""];

    List<string> nextResult = new List<string>();

    //the control pads always start from 'A'
    char currentChar = 'A';

    foreach (char nextChar in pCode)
    {
        string[] translations = pTable[(currentChar, nextChar)];

        foreach (string translation in translations)
        {
            foreach (string result in results)
            {
                nextResult.Add(result + translation);
            }
        }

        currentChar = nextChar;
        List<string> tmp = results;
        results = nextResult;
        nextResult = tmp;
        //make the list ready for the next loop
        nextResult.Clear();
    }

    return results;
}

// Translate the given codes using the control pad, selecting the shortest translations

List<string> ProcessLevel(List<string> pCodes)
{
    int leastChanges = int.MaxValue;
    List<string> results = new List<string>();

    foreach (string code in pCodes)
    {
        List<string> translations = TranslateCode(code, controlKeypadTransitions);
        foreach (string translation in translations)
        {
            int changes = CountChanges(translation);
            if (changes <= leastChanges)
            {
                if (changes < leastChanges) results.Clear();
                results.Add(translation);
                leastChanges = changes;
            }
        }
    }

    return results;
}


// Now go through all codes

long total = 0;
foreach (string code in myInput)
{
    // Initial step from numeric to control pad
    List<string> baseCodes = TranslateCode(code, numericKeypadTransitions);
    
    // Two directional keypads that robots are using
    baseCodes = ProcessLevel(baseCodes);
    baseCodes = ProcessLevel(baseCodes);

    int value = int.Parse(code[0..^1]);
    total += value * baseCodes[0].Length;
}

Console.WriteLine("Part 1: " + total);


// ** Part 2:

Dictionary<(char, char, int), long> cache = new ();

//Instead of generating all the actual codes, now we only calculate the cost of if we WOULD generate it
long GetCost(string pCode, int pMaxDepth, int pCurrentDepth = 0)
{
    //are we at the depth requested? then just return the length of the string requested
    if (pCurrentDepth == pMaxDepth) return pCode.Length;

    // if not we go through every transition in our code and we calculate what that transition might cost us
    // down the line. out of all future possibilities we pick the minimum value.
    // AND WE CACHE IT! Very important :)

    long total = 0;
    char currentChar = 'A';

    foreach (char nextChar in pCode)
    {
        string[] translations = controlKeypadTransitions[(currentChar, nextChar)];

        long minCost;

        if (!cache.TryGetValue((currentChar, nextChar, pCurrentDepth), out minCost))
        {
            minCost = long.MaxValue;
            foreach (string translation in translations)
            {
                minCost = Math.Min(minCost, GetCost(translation, pMaxDepth, pCurrentDepth + 1));
            }
            cache[(currentChar, nextChar, pCurrentDepth)] = minCost;
        }

        currentChar = nextChar;
        total += minCost;
    }

    return total;
}

// ProcessNumericCode performs the first translation from numeric code to <>^V<>><A codes
// (can be multiple) then passes that code into the GetCost method and gets the minimum of the result

long ProcessNumericCode(string pCode)
{
    List<string> controlPadCodes = TranslateCode(pCode, numericKeypadTransitions);
    return controlPadCodes.Select(c => GetCost(c, 25)).Min();
}

total = 0;

foreach (string numericCode in myInput)
{
    long value = long.Parse(numericCode.TakeWhile(char.IsDigit).ToArray());
    total += value * ProcessNumericCode(numericCode);
}

Console.WriteLine("Part 2: " + total);




