//Solution for https://adventofcode.com/2015/day/11 (Ctrl+Click in VS to follow link)

//Your input: Santa's current password
string myInput = "vzbxkghb";

//Your task: find the next valid password by 'incrementing' chars like this xx, xy, xz, ya, yb etc matching these requirements:
//1. Passwords must include one increasing straight of at least three letters, like abc, bcd, cde, and so on, up to xyz.
//   They cannot skip letters; abd doesn't count.
//2. Passwords may not contain the letters i, o, or l, as these letters can be mistaken for other characters and are therefore confusing.
//3. Passwords must contain at least two different, non-overlapping pairs of letters, like aa, bb, or zz.

string nextPassword = FindNextValidPassword(myInput);
Console.WriteLine("Part 1 - Next valid password:"+ nextPassword);

nextPassword = FindNextValidPassword(nextPassword);
Console.WriteLine("Part 2 - Next valid password:"+ nextPassword);

Console.ReadKey();

string FindNextValidPassword (string pInput)
{
    char[] myPassword = pInput.ToCharArray();

    do NextPassword(myPassword); while (!IsCorrectPassword(myPassword));

    return new string(myPassword);
}

void NextPassword(char[] pInputChars)
{
    for (int i = pInputChars.Length - 1; i >= 0; i--)
    {
        int currentCharCode = pInputChars[i] - 'a';         //convert a..z to 0..25
        int newCharCode =     (currentCharCode + 1) % 26;   //add one and wrap it around
        pInputChars[i] =      (char) (newCharCode + 'a');   //convert 0..25 back to a..z

        //if we didn't overflow, we are done, otherwise continue loop to increase the next char as well
        if (currentCharCode < newCharCode) break;
    }
}

bool IsCorrectPassword(char[] pPassword)
{
	//Passwords may not contain the letters i, o, or l, as these letters can be mistaken for other characters and are therefore confusing.
	return !ContainsChars(pPassword, [ 'i', 'o', 'l' ])

    //Passwords must include one increasing straight of at least three letters, like abc, bcd, cde, and so on, up to xyz. They cannot skip letters; abd doesn't count.
    && HasAtLeastOneStraight(pPassword)

    //Passwords must contain at least two different, non - overlapping pairs of letters, like aa, bb, or zz.
    && HasAtLeastXTwins (pPassword, 2);
}

bool ContainsChars(char[] pPassword, char[] chars)
{
    return pPassword.Intersect(chars).Any();
}

bool HasAtLeastOneStraight(char[] pPassword)
{
    for (int i = 0; i < pPassword.Length - 2; i++)
    {
        if (pPassword[i] + 1 == pPassword[i + 1] && pPassword[i + 1] + 1 == pPassword[i + 2])
        {
            return true; // Found a run of increasing characters
        }
    }
    return false; // No run of increasing characters found
}

bool HasAtLeastXTwins(char[] pPassword, int pRequiredAmountOfTwins)
{
    int twinsFound = 0;
    
    for (int i = 0; i < pPassword.Length - 1; i++)
    {
        //if pairs found, increase i additionaly to make sure the twins don't overlap
        if (pPassword[i] == pPassword[i + 1])
        {
            twinsFound++;
            i++;
        }
    }

    return twinsFound == pRequiredAmountOfTwins;
}

