//Solution for https://adventofcode.com/2015/day/5 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of separate strings to check for matching certain requirements, e.g.
// uxcplgxnkwbdwhrp
// nsuerykeptdsutidb etc

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] stringsToCheck = myInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

/**
 * Part 1 - Check all strings to see if they match these requirements:
 * 
 * A) It contains at least three vowels (aeiou only), like aei, xazegov, or aeiouaeiouaeiou.
 * B) It contains at least one letter that appears twice in a row, like xx, abcdde (dd), or aabbccdd (aa, bb, cc, or dd).
 * C) It does not contain the strings ab, cd, pq, or xy, even if they are part of one of the other requirements.
 */
void Part1(string[] pStringsToCheck)
{
    int niceStrings = 0;

    string vowels = "aeiou";
    string[] notAllowed = { "ab", "cd", "pq", "xy" };

    foreach (string stringToCheck in pStringsToCheck)
    {
        if (
                stringToCheck.Count(t => vowels.Contains(t)) > 2 &&     //A
                Regex.Matches(stringToCheck, @"(.)\1").Count > 0 &&     //B
                !notAllowed.Any(x => stringToCheck.Contains(x))         //C
            )
        {
            niceStrings++;
        }
    }

    Console.WriteLine("Part 1:" + niceStrings);
}

Part1(stringsToCheck);

/**
 * Part 2 - Check all strings to see if they match these requirements:
 * 
 * A) It contains a pair of any two letters that appears at least twice in the string without overlapping, like xyxy (xy) or aabcdefgaa (aa), but not like aaa (aa, but it overlaps).
 * B) It contains at least one letter which repeats with exactly one letter between them, like xyx, abcdefeghi (efe), or even aaa.
 */
void Part2(string[] pStringsToCheck)
{
    int niceStrings = 0;

    foreach (string stringToCheck in pStringsToCheck)
    {
        if (
            Regex.Matches(stringToCheck, @"(..).*\1").Count > 0 &&  //A
            Regex.Matches(stringToCheck, @"(.).\1").Count > 0       //B
            )
        {
            niceStrings++;
        }
    }

    Console.WriteLine("Part 2:" + niceStrings);
}

Part2(stringsToCheck);

