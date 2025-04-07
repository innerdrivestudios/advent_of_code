// Solution for https://adventofcode.com/2015/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

// ** Your input: Santa's current password
string myInput = args[0];

// ** Part 1: Find the next valid password by 'incrementing' chars like this
//            xx, xy, xz, ya, yb etc matching these requirements:
//
//      1. Passwords must include one increasing straight of at least three letters,
//         like abc, bcd, cde, and so on, up to xyz.
//         They cannot skip letters; abd doesn't count.
//      2. Passwords may not contain the letters i, o, or l,
//         as these letters can be mistaken for other characters and are therefore confusing.
//      3. Passwords must contain at least two different, non-overlapping pairs of letters,
//         like aa, bb, or zz.

string nextPassword = FindNextValidPassword(myInput);
Console.WriteLine("Part 1 - Next valid password:"+ nextPassword);

nextPassword = FindNextValidPassword(nextPassword);
Console.WriteLine("Part 2 - Next valid password:"+ nextPassword);

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
	return 
        
	//Passwords may not contain the letters i, o, or l,
    //as these letters can be mistaken for other characters and are therefore confusing.
    !ContainsChars(pPassword, [ 'i', 'o', 'l' ])

    //Passwords must include one increasing straight of at least three letters,
    //like abc, bcd, cde, and so on, up to xyz. They cannot skip letters; abd doesn't count.
    && HasAtLeastOneStraight(pPassword)

    //Passwords must contain at least two different,
    //non - overlapping pairs of letters, like aa, bb, or zz.
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
			// Found a run of increasing characters
			return true;         
        }
    }

	// No run of increasing characters found
	return false; 
}

bool HasAtLeastXTwins(char[] pPassword, int pRequiredAmountOfTwins)
{
    int twinsFound = 0;
    
    for (int i = 0; i < pPassword.Length - 1; i++)
    {
        //if pairs found, increase i additionally to make sure the twins don't overlap
        if (pPassword[i] == pPassword[i + 1])
        {
            twinsFound++;
            i++;
        }
    }

    return twinsFound == pRequiredAmountOfTwins;
}

