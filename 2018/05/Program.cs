//Solution for https://adventofcode.com/2018/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Input: a polymer string like dabAcCaCBAcCcaDA  

using System.Diagnostics;

string myInput = File.ReadAllText(args[0]);
// Very important: replace line endings with nothing explicitly !!
myInput = myInput.ReplaceLineEndings("");

// ** Part 1: Count the length of a reduced polymer string

// There are several ways that you could approach this... to name a few:
// - brute force string replacements, e.g. Replace all aA, Aa, bB, Bb until the string reduces no further.
// - regular expression pattern matching (at least I think that should be possible), 
//   but this approach is very similar and probably not faster than brute force string replacement
// - convert all chars in the string to a List, scan the list for the pattern,
//   (resetting the scan index here and there when a match is found to backtrack)
//   this is a solid approach, but involves a lot of O(n) removal operations if you approach it naively
// - convert all chars in the string to a LinkedList, and then the previous approach, 
//   this prevents the O(n) removal operations, since LinkedList removal is only O(1)
//   but still causes a lot of LinkedList manipulation
// - A more optimized scan approach, which is the approach I decided to go with and outlined below

// What is the idea?

// Looking at dabAcCaCBAcCcaDA, we simply start scanning the string at the start,
// keeping track of / counting any matches we might find.
//
// Vv                   => V is the index of the left char we are comparing (also called leftIndex)
// dabAcCaCBAcCcaDA     => v is the index of the right char we are comparing it with (also called rightIndex)

// If we don't have a match, we simply increase the scan index:
//
//  Vv
// dabAcCaCBAcCcaDA
//   Vv
// dabAcCaCBAcCcaDA
//
// etc until we get to this situation:
//
//     Vv
// dabAcCaCBAcCcaDA
//
// Now we found a match (cC) which we should 'remove'.
// But how do we remove it without modifying the string?
// Well, the basic idea is that AS we find a match, we expand our left and right indices outward, like this:
//
//    V<>v
// dabAcCaCBAcCcaDA
//
// So V moved one to the left, v moved one to the right, where we find another match: Aa
// Expanding again gives us:
//
//   V<<>>v
// dabAcCaCBAcCcaDA
//
// Now we are comparing b and C which is no match, so we stop expanding and return to the normal comparison process,
// starting at where v is now (since everything before that has been matched:
//
//        Vv
// dabAcCaCBAcCcaDA
//
// In other words, the leftIndex becomes the last rightIndex
//
// This works great, except for ONE caveat. 
// Check the string below:
//
// dabCBAcCcCaDA
//
// While scanning and matching, we end up scanning and matching the first cC:
//
//       Vv
// dabCBAcCcCaDA
//
// Expanding outward, we scan:
//
//      V<>v
// dabCBAcCcCaDA, which is no match... so we move on with V=v:
//
//         Vv
// dabCBAcCcCaDA, which is a match...
//
// now expanding SHOULD compare A with a, since the previous cC was a match and should have been removed...
// but since we are not actually modifying the string ... it is still there, causing the expansion to compare
// C with a instead of A with a, so it fails...
//
// To remedy this, every time we match, we overwrite the match with ..
// Any time we expand our left and right indices we skip the .'s
//
// Putting this all together:

int GetReactedPolymerLength(string pInput)
{
    int baseLength = pInput.Length;                         //Store the original length of the string
    int baseIndex = 0;                                      //Define where we start with checking in the string
    int matchedPairCount = 0;                               //Keep track of how many pairs we already matched

    int baseDeltaForMatch = Math.Abs('a' - 'A');            //Calculate how far chars need to be apart to match

    char[] chars = pInput.ToCharArray();                    //Convert the whole string to a char array so we can overwrite elements in O(1)

    //To match a char, we need two elements, the last being baseIndex and baseIndex + 1
    while (baseIndex + 1 < pInput.Length)
    {
        //Set up the left and right indices for comparison
        int leftIndex = baseIndex;
        int rightIndex = baseIndex + 1;

        do
        {
            //Calculate the "distance" between the left and right chars... 
            //E.g. 'a' - 'a' == 0, 'b' - 'a' = 1, etc
            int delta = Math.Abs(pInput[leftIndex] - pInput[rightIndex]);

            //If the delta between the left and right chars indicate a pair like aA, Aa, bB, Bb, etc pairs ...
            if (delta == baseDeltaForMatch)
            {
                //Marked those chars as 'visited'
                chars[leftIndex] = '.';
                chars[rightIndex] = '.';

                //Register we got a match
                matchedPairCount++;
                
                //And expand the left and right index, skipping any '.'
                while (leftIndex > 0 && chars[--leftIndex] == '.') { };
                while (rightIndex < chars.Length - 1 && chars[++rightIndex] == '.') { };

            }
            else
            {
                //Stop the expansion and ...
                break;
            }
        }
        while (leftIndex > -1 && rightIndex < pInput.Length);

        //Pick it up starting at the rightIndex
        baseIndex = rightIndex;
    }

    //What is the resulting length of the reduced string?
    return baseLength - 2 * matchedPairCount;
}

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine("Part 1 - Reduced string length:" + GetReactedPolymerLength(myInput));
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms.");

// ** Now for Part 2, first gonna do a brute force approach, and measure how fast it is...

stopwatch.Restart();

string lookupTable = "abcdefghijklmnopqrstuvwxyz";
int diff = 'A' - 'a';

int minimumLength = int.MaxValue;

foreach (char c in lookupTable)
{
    string alteredString = myInput.Replace(""+c, null);
    alteredString = alteredString.Replace(""+(char)(c+diff), null);
    minimumLength = Math.Min(minimumLength, GetReactedPolymerLength(alteredString));
}

Console.WriteLine("Part 2 - Improved string length:" + minimumLength);
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms.");

// Turns out it is still very fast and that the delay is in the GetReactedPolymerLength method
// which we have to execute 26 times... in other words, the string replacement isn't causing
// the actual delay here, so we are not going to try and optimize it any further.

