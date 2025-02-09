//Solution for https://adventofcode.com/2020/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of binary search strings, e.g. BBFBBBBRRL

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings();

string[] boardingPasses = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Define the methods to get the row and column and subsequently the id

int GetRow (string pInput)
{
    int rowStart = 0;
    int rowEnd = 127;

    for (int i = 0; i < 7; i++)
    {
        if (pInput[i] == 'F')
        {
            rowEnd = (int)Math.Floor((double)(rowStart + rowEnd) / 2);
        }
        else
        {
            rowStart = (int)Math.Ceiling((double)(rowStart + rowEnd) / 2);
        }
    }

    return rowStart;
}

int GetColumn(string pInput)
{
    int columnStart = 0;
    int columEnd = 7;

    for (int i = 0; i < 3; i++)
    {
        if (pInput[pInput.Length - 3 + i] == 'L')
        {
            columEnd = (int)Math.Floor((double)(columnStart + columEnd) / 2);
        }
        else
        {
            columnStart = (int)Math.Ceiling((double)(columnStart + columEnd) / 2);
        }
    }

    return columnStart;
}

int GetID(string pInput)
{
    return GetRow(pInput) * 8 + GetColumn(pInput);
}

/*
Console.WriteLine(GetID("FBFBBFFRLR"));
Console.WriteLine(GetID("BFFFBBFRRR"));
Console.WriteLine(GetID("FFFBBBFRRR"));
Console.WriteLine(GetID("BBFFBBFRLL"));
*/

int highestID = boardingPasses.Max (x => GetID(x));
Console.WriteLine("Part 1 - Highest ID: " + highestID);

// ** Part 2 : Find your ID, it is the only missing ID in the list.

// Approach: first generate the ID's of all seats
List<int> ids = boardingPasses.Select (x => GetID(x)).ToList ();

//Then sort them and look for the gap...
ids.Sort ();

for (int i = 0; i < ids.Count - 1; i++)
{
    if (ids[i + 1] - ids[i] > 1)
    {
        //Missing id is the one in between those with a gap ;)
        Console.WriteLine("Part 2 - Missing id: " + ((ids[i] + ids[i+1])/2));
    }
}

