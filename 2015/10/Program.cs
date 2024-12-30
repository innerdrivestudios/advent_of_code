//Solution for https://adventofcode.com/2015/day/10 (Ctrl+Click in VS to follow link)

using System.Text;

//Your input: a string of digits 0-9
string myInput = "1113122113";

//Your task: implement the look-and-say algoriths, e.g. 1113122113 -> 111 3 1 22 11 3 -> 31 13 11 22 21 13 -> 311311222113

//Approach: we're just gonna build the strings, but we'll use a stringbuilder to do so
//since it dramatically improves performance over concatenating regular strings
StringBuilder temp = new StringBuilder();

Part1(myInput);
Part2(myInput);
Console.ReadKey();

void Part1(string pInput)
{
    Console.WriteLine("Part 1:"+ApplyLookAndSay(pInput, 40).Length);
}

void Part2(string pInput)
{
    Console.WriteLine("Part 2:" + ApplyLookAndSay(pInput, 50).Length);
}

string ApplyLookAndSay (string pInput, int pTimes)
{
    while (pTimes-- > 0) pInput = LookAndSay(pInput);
    return pInput;
}

string LookAndSay (string pInput)
{
    temp.Clear();
    
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
