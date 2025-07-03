//Solution for https://adventofcode.com/2015/day/10 (Ctrl+Click in VS to follow link)

using System.Diagnostics;
using System.Text;

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

// ** Your input: a string of digits 0-9

string myInput = args[0];

//** Part 1 & 2: Implement the look-and-say algoriths,
// e.g. 1113122113 is 111 3 1 22 11 3 which becomes 31 13 11 22 21 13 which is 311311222113

//Approach: we're just gonna build the strings, but we'll use a stringbuilder to do so
//since it dramatically improves performance over concatenating regular strings

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
Console.WriteLine("Part 1:" + ApplyLookAndSay(myInput, 40).Length);
Console.WriteLine("Part 2:" + ApplyLookAndSay(myInput, 50).Length);
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms");
Console.ReadKey();

string ApplyLookAndSay (string pInput, int pTimes)
{
    while (pTimes-- > 0) pInput = LookAndSay(pInput);
    return pInput;
}

string LookAndSay (string pInput)
{
    StringBuilder temp = new StringBuilder();
    
    char currentChar = pInput[0];
    int occurencesFound = 1;

    for (int i = 1; i < pInput.Length; i++)
    {
        //If we are encountering the same char as the previous one
        if (currentChar == pInput[i])
        {
            //Keep counting...
            occurencesFound++;
        }
        else
        {
            //Else 'say' how many occurences we saw of the current char
            temp.Append(occurencesFound);
            temp.Append(currentChar);

            //Set up correctly for the next bit
            currentChar = pInput[i];
            occurencesFound = 1;
        }
    }

    //Don't forget the last part    
    temp.Append(occurencesFound);
    temp.Append(currentChar);

    return temp.ToString();
}
