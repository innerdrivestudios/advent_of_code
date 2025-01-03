//Solution for https://adventofcode.com/2015/day/14 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;

//Your input: a list of reindeer "specs"

string myInput = "Vixen can fly 8 km/s for 8 seconds, but then must rest for 53 seconds.\r\nBlitzen can fly 13 km/s for 4 seconds, but then must rest for 49 seconds.\r\nRudolph can fly 20 km/s for 7 seconds, but then must rest for 132 seconds.\r\nCupid can fly 12 km/s for 4 seconds, but then must rest for 43 seconds.\r\nDonner can fly 9 km/s for 5 seconds, but then must rest for 38 seconds.\r\nDasher can fly 10 km/s for 4 seconds, but then must rest for 37 seconds.\r\nComet can fly 3 km/s for 37 seconds, but then must rest for 76 seconds.\r\nPrancer can fly 9 km/s for 12 seconds, but then must rest for 97 seconds.\r\nDancer can fly 37 km/s for 1 seconds, but then must rest for 36 seconds.\r\n";

//Your task: calculating some stats for the different reindeer

//Step 1. Convert all input into a list of reindeer

List<Reindeer> allReindeer = ConvertInput();

//Step2. Run the different challenges

Console.WriteLine("Part 1: Winning distance = " + GetWinningDistance(allReindeer, 2503));
Console.WriteLine("Part 2: Winning points = " + GetWinningPoints(allReindeer, 2503));

Console.ReadKey();

List<Reindeer> ConvertInput()
{
    List<Reindeer> reindeers = new List<Reindeer>();

	string pattern = @"(\w+) can fly (\d+) km/s for (\d+) seconds, but then must rest for (\d+) seconds\.\r\n";
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

int GetWinningPoints(List<Reindeer> pDeer, int pSeconds)
{
    int[] reindeerScores = new int[pDeer.Count];  

    for (int i = 1; i < pSeconds; i++)
    {
        int winningDistance = GetWinningDistance(pDeer, i);

        for(int j = 0; j < pDeer.Count; j++)
        {
            if (pDeer[j].GetDistanceTravelled(i) == winningDistance) reindeerScores[j]++;
        }
    }

    return reindeerScores.Max();
}
