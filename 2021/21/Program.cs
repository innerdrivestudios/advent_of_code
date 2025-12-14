//Solution for https://adventofcode.com/2021/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a lookup table for pixels and an image that needs to be improved

// * Step 1: Parse the input and convert it into a faster format

string[] myInput = File.ReadAllLines(args[0]);

int[] position =
[
    int.Parse(myInput[0].Substring(myInput[0].IndexOf(": ") + 2)) - 1,
    int.Parse(myInput[1].Substring(myInput[1].IndexOf(": ") + 2)) - 1,
];

// Let's move everything back from 1-10 to 0-9 so we can modulo it...

int[] playerScores = new int [2];

long TotalSum (int pRoll)
{
    long n = pRoll * 3;
    long previousN = (pRoll - 1) * 3;
    long sumN = (n * (n + 1)) / 2;
    long sumNminus1 = (previousN * (previousN + 1)) / 2;

    return sumN - sumNminus1;
}

int turns = 0;
long losingScore = 0;

bool done = false;

while (!done)
{
    for (int i = 0; i < 2; i++)
    {
        position[i] = (int)(position[i] + TotalSum(++turns)) % 10;
        playerScores[i] += (position[i] + 1);
       // Console.WriteLine(playerScores[i]);
       // Console.ReadKey();
        losingScore = playerScores[1-i] * turns * 3;
        if (playerScores[i] >= 1000) { done = true; Console.WriteLine(playerScores[1-i] + " " + turns); break; }
    }
}

Console.WriteLine("Part 1:" + losingScore);
