//Solution for https://adventofcode.com/2021/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// * Step 1: Parse the input ...

string[] myInput = File.ReadAllLines(args[0]);

// Parse the positions from the input and move it back by one to get the range 0..9 (easier to track like this)

int[] position =
[
    int.Parse(myInput[0].Substring(myInput[0].IndexOf(": ") + 2)) - 1,
    int.Parse(myInput[1].Substring(myInput[1].IndexOf(": ") + 2)) - 1,
];

int[] playerScores = new int [2];

// The total sum for a roll is for example 1+2+3 or 4+5+6 etc...
// To calculate that fast we can use the principle of total 1..n == ((n+1) * n) / 2
// So to calculate the roll for 4+5+6 we calculate the sum of 1..6 minus 1..3

long TotalSum (int pRoll)
{
    long n = pRoll * 3;
    long sumN = (n * (n + 1)) / 2;

    long previousN = (pRoll - 1) * 3;
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
        // Just add and wrap ...
        position[i] = (int)(position[i] + TotalSum(++turns)) % 10;
        // But then for the score pretend the range is 1-10 instead of 0-9
        playerScores[i] += (position[i] + 1);
        losingScore = playerScores[1-i] * turns * 3;
        if (playerScores[i] >= 1000) { done = true; break; }
    }
}

Console.WriteLine("Part 1:" + losingScore);

// ** Part 2: Play the game with a splitting die

Dictionary<string, (long, long)> cache = new();

(long winCountP1, long winCountP2) CalculateMostWinningGames(List<int> pPlayerPositions, List<int> pPlayerScores, List<int> pRolls)
{
    // This cache key tripped me up for a while until I realized the order of the dice doesn't matter... only how many of a certain roll we have atm
    string key = string.Join("-", pPlayerPositions) + "_" + string.Join("-", pPlayerScores) + "_" + pRolls.Count(x => x == 1) + "_" + pRolls.Count(x => x == 2) + "_" + pRolls.Count(x => x == 3);
    
    if (cache.ContainsKey(key)) return cache[key];

    // If any player has made 3 rolls... we need to take the rolls, increase the player position, calculate the score and check the win...
    if (pRolls.Count > 0 && pRolls.Count % 3 == 0)
    {
        int rollSum = pRolls[^3..].Sum();

        //who gets the score? This method is called each time Count % 3 == 0, so if Count % 6 != 0 we are dealing with player 0, 1 otherwise
        int playerIndex = pRolls.Count % 6 != 0 ? 0:1;
        pPlayerPositions[playerIndex] = (pPlayerPositions[playerIndex] + rollSum) % 10;
        pPlayerScores[playerIndex] += (pPlayerPositions[playerIndex]+1);

        if (pPlayerScores[playerIndex] >= 21)
        {
            (long,long) result = (pPlayerScores[0] >= 21 ? 1 : 0, pPlayerScores[1] >= 21 ? 1 : 0);
            return result;
        }
    }


    (long winCountP1, long winCountP2) winCount = (0, 0);

    var result1 = CalculateMostWinningGames(new(pPlayerPositions), new(pPlayerScores), new(pRolls) { 1 });
    var result2 = CalculateMostWinningGames(new(pPlayerPositions), new(pPlayerScores), new(pRolls) { 2 });
    var result3 = CalculateMostWinningGames(new(pPlayerPositions), new(pPlayerScores), new(pRolls) { 3 });

    winCount.winCountP1 = result1.winCountP1 + result2.winCountP1 + result3.winCountP1;
    winCount.winCountP2 = result1.winCountP2 + result2.winCountP2 + result3.winCountP2;

    cache[key] = winCount;

    return winCount;
}

List<int> playerPositions = [
        int.Parse(myInput[0].Substring(myInput[0].IndexOf(": ") + 2)) - 1,
        int.Parse(myInput[1].Substring(myInput[1].IndexOf(": ") + 2)) - 1
    ];

var wonGames = CalculateMostWinningGames(playerPositions, [0, 0], new());
Console.WriteLine("Part 2: " + wonGames);
