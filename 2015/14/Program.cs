// Solution for https://adventofcode.com/2015/day/14 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of reindeer "specs", e.g.:
// Vixen can fly 8 km/s for 8 seconds, but then must rest for 53 seconds.

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// Step 1. Let's convert all the input to actual Reindeer

List<Reindeer> ConvertInput()
{
    List<Reindeer> reindeers = new List<Reindeer>();

    string pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds.";
    MatchCollection matches = Regex.Matches(myInput, pattern);

    foreach (Match match in matches)
    {
        if (match.Success)
        {
            string name = match.Groups[1].Value;
            int flySpeed = int.Parse(match.Groups[2].Value);
            int flyDuration = int.Parse(match.Groups[3].Value);
            int restDuration = int.Parse(match.Groups[4].Value);

            reindeers.Add(new Reindeer(name, flyDuration, flySpeed, restDuration));
        }
    }

    return reindeers;
}

List<Reindeer> allReindeer = ConvertInput();

// ** Part 1: Calculate what distance has the winning reindeer traveled after 2503 seconds:

int GetWinningDistance(List<Reindeer> pDeer, int pSeconds)
{
    int winningDistance = int.MinValue;

    foreach (Reindeer reindeer in pDeer)
    {
        int travelDistance = reindeer.GetDistanceTravelled(pSeconds);

        if (travelDistance > winningDistance)
        {
            winningDistance = travelDistance;
        }
    }

    return winningDistance;
}

Console.WriteLine("Part 1: Winning distance = " + GetWinningDistance(allReindeer, 2503));

// ** Part 2: Calculate how many points the winning reindeer has after 2503 seconds 

// Brute force check, every step of the way...

int GetWinningPoints(List<Reindeer> pDeer, int pSeconds)
{
    int[] reindeerScores = new int[pDeer.Count];  

    for (int i = 1; i <= pSeconds; i++)
    {
        int winningDistance = GetWinningDistance(pDeer, i);

        for(int j = 0; j < pDeer.Count; j++)
        {
            if (pDeer[j].GetDistanceTravelled(i) == winningDistance) reindeerScores[j]++;
        }
    }

    return reindeerScores.Max();
}

Console.WriteLine("Part 2: Winning points = " + GetWinningPoints(allReindeer, 2503));
