// Solution for https://adventofcode.com/2023/day/2 (Ctrl+Click in VS to follow link)

// A game is a sequence of colors to counts
using Game = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, int>>;

// Your input: a list of games
string myInput = "Game 1: 3 green, 1 blue, 3 red; 3 blue, 1 green, 3 red; 2 red, 12 green, 7 blue; 1 red, 4 blue, 5 green; 7 green, 2 blue, 2 red\r\nGame 2: 1 green, 19 blue, 1 red; 8 blue, 4 red; 3 red, 6 blue; 1 green, 1 red, 12 blue\r\nGame 3: 3 green, 1 blue, 9 red; 1 blue, 2 green, 8 red; 1 blue, 2 red\r\nGame 4: 6 green, 2 red; 2 red, 16 green; 3 red, 1 blue\r\nGame 5: 5 blue, 1 green; 3 blue, 3 green, 3 red; 8 red, 1 blue, 2 green; 7 blue, 6 red; 4 red, 4 blue\r\nGame 6: 5 red, 20 blue, 3 green; 4 red, 20 blue, 3 green; 12 blue, 3 green, 1 red; 3 red, 3 green, 19 blue\r\nGame 7: 5 red, 3 blue, 9 green; 12 red, 3 blue; 5 green, 3 blue, 19 red; 6 red, 1 green, 3 blue\r\nGame 8: 9 red; 2 green, 1 blue, 7 red; 5 red, 2 blue; 3 blue, 1 green; 1 green, 14 red, 1 blue; 3 blue, 4 red, 1 green\r\nGame 9: 11 red, 2 green; 13 red, 8 green; 15 green, 3 red; 1 blue, 9 red, 18 green; 2 green, 12 red; 15 green, 9 red\r\nGame 10: 1 green; 16 green, 3 red, 2 blue; 1 blue, 16 green, 4 red; 16 green, 5 red, 2 blue\r\nGame 11: 2 red, 18 blue, 5 green; 4 green, 12 blue, 9 red; 6 red, 4 green, 5 blue; 8 red, 16 blue, 2 green; 1 green, 18 blue, 13 red; 13 blue, 9 red\r\nGame 12: 5 red, 10 green, 4 blue; 8 green, 8 red, 14 blue; 10 green, 17 blue, 13 red; 7 blue, 9 red, 13 green; 6 red, 16 blue, 4 green; 16 blue, 14 red, 16 green\r\nGame 13: 6 green, 1 red, 1 blue; 10 blue, 15 green; 1 blue, 2 red, 5 green; 2 blue, 1 red, 20 green; 3 blue, 3 red, 10 green\r\nGame 14: 2 green, 2 blue; 2 green, 3 red, 4 blue; 8 red, 1 blue, 1 green\r\nGame 15: 3 blue, 10 green, 1 red; 16 red, 1 blue, 20 green; 7 green, 6 blue, 13 red; 8 green, 20 red, 5 blue; 8 blue, 8 red, 18 green; 17 green, 8 red, 10 blue\r\nGame 16: 6 blue, 5 red; 6 red, 16 blue, 11 green; 1 red, 3 green, 13 blue; 1 red, 5 green, 1 blue; 3 red, 14 green, 16 blue; 1 red, 1 green, 3 blue\r\nGame 17: 8 green, 5 red, 7 blue; 2 blue, 2 green, 6 red; 3 green, 4 blue, 15 red\r\nGame 18: 5 blue; 2 red, 9 blue, 3 green; 4 green, 20 blue, 2 red; 4 green, 2 red, 5 blue; 16 blue\r\nGame 19: 15 red, 1 blue; 3 green, 16 red, 4 blue; 1 blue, 3 green, 4 red; 9 red, 2 green, 6 blue; 2 green, 5 blue, 4 red\r\nGame 20: 12 red, 7 blue; 11 blue, 7 red, 1 green; 1 green, 10 red, 4 blue\r\nGame 21: 9 blue, 1 green, 1 red; 4 blue, 2 green; 1 blue, 2 red\r\nGame 22: 1 red, 10 green; 6 blue, 4 green, 1 red; 6 blue, 12 green, 1 red; 3 red, 4 blue, 10 green; 1 blue, 13 green, 1 red\r\nGame 23: 14 red, 2 blue, 3 green; 8 green, 2 blue, 4 red; 2 blue, 7 green, 4 red; 4 blue, 7 red; 1 blue, 8 green, 13 red\r\nGame 24: 1 blue, 6 green, 7 red; 6 green, 2 blue, 5 red; 1 blue, 3 green; 2 blue, 9 green; 1 green, 4 red; 5 green, 4 red\r\nGame 25: 8 red, 2 green, 6 blue; 3 blue, 15 red, 1 green; 8 blue, 2 red; 2 blue, 1 green; 2 green, 18 red, 1 blue\r\nGame 26: 9 red, 11 green, 6 blue; 1 blue, 2 red, 16 green; 15 green, 11 red, 6 blue; 3 red, 13 green, 6 blue; 20 red, 2 blue, 4 green\r\nGame 27: 9 red, 10 blue, 17 green; 8 green, 15 blue; 4 green, 3 red; 11 blue; 14 green, 1 blue, 8 red; 10 blue, 5 green, 3 red\r\nGame 28: 2 green, 17 red; 7 red, 6 green, 6 blue; 12 green, 16 red; 7 red, 7 green, 7 blue; 7 green, 8 red, 5 blue; 7 red, 5 blue\r\nGame 29: 2 red, 2 blue, 3 green; 3 blue, 1 red; 3 green, 2 blue, 1 red; 6 red, 1 green, 4 blue\r\nGame 30: 8 red, 15 blue, 4 green; 5 green, 9 red, 15 blue; 1 green, 1 blue, 11 red\r\nGame 31: 6 blue, 2 red, 1 green; 2 blue, 2 red, 8 green; 2 blue, 1 red, 7 green\r\nGame 32: 6 red, 7 green, 6 blue; 9 red, 6 blue, 6 green; 1 green, 13 red, 4 blue\r\nGame 33: 3 green, 1 blue, 9 red; 2 blue, 12 red, 4 green; 1 blue, 5 red, 1 green; 4 green, 5 red, 2 blue; 1 red, 2 blue, 3 green; 3 green, 3 red, 1 blue\r\nGame 34: 1 blue, 9 red; 3 blue, 4 red; 3 blue, 5 green, 10 red; 2 blue, 9 red, 5 green\r\nGame 35: 3 red, 2 blue; 1 green, 10 blue, 4 red; 1 blue, 5 red, 2 green; 5 blue, 2 green, 1 red\r\nGame 36: 9 green, 6 blue, 1 red; 16 blue, 8 green, 3 red; 9 green, 8 blue, 2 red; 3 green, 3 blue, 1 red; 16 blue, 3 red, 3 green\r\nGame 37: 1 green, 1 red; 2 blue, 3 green; 1 red, 1 blue, 5 green; 1 red, 9 green, 2 blue; 12 green, 2 blue\r\nGame 38: 16 blue, 12 red, 4 green; 15 blue, 5 green, 6 red; 7 red, 12 blue; 19 blue, 15 red, 1 green\r\nGame 39: 1 red, 2 blue; 1 green, 10 red, 3 blue; 1 green, 2 red; 1 blue, 3 red\r\nGame 40: 11 blue, 6 red, 3 green; 2 blue, 12 green, 1 red; 16 green, 5 red; 5 red, 10 green, 6 blue; 3 red, 13 green, 1 blue; 13 green, 3 blue, 7 red\r\nGame 41: 19 red, 1 blue; 9 blue, 6 red; 10 red, 1 green, 17 blue\r\nGame 42: 1 red, 8 green, 12 blue; 8 blue, 10 red, 12 green; 9 blue, 8 green, 9 red; 8 red, 11 green; 12 blue, 5 red, 2 green\r\nGame 43: 6 blue, 7 red, 9 green; 4 blue, 6 red; 3 red, 4 blue, 5 green; 7 green, 15 blue; 15 blue, 9 green, 6 red; 6 green, 8 red, 7 blue\r\nGame 44: 12 blue, 5 red; 7 red, 16 blue; 2 red, 4 blue, 8 green; 3 red, 10 blue, 3 green; 5 blue\r\nGame 45: 3 green, 4 red, 6 blue; 1 green, 2 red, 11 blue; 6 red, 9 blue, 1 green; 8 blue, 3 green\r\nGame 46: 1 blue, 9 green, 1 red; 1 blue, 2 red, 6 green; 10 green, 3 blue\r\nGame 47: 2 green, 4 red; 2 green, 4 blue, 2 red; 2 blue, 3 green, 12 red; 12 red, 3 blue\r\nGame 48: 4 blue, 3 green, 16 red; 1 green, 2 blue, 2 red; 9 green, 7 blue, 13 red\r\nGame 49: 4 blue, 5 green, 17 red; 1 blue, 13 red, 2 green; 15 red, 1 blue, 5 green; 4 blue, 7 green, 19 red; 4 blue, 3 green; 2 green, 2 red\r\nGame 50: 2 red, 3 green, 7 blue; 1 green, 9 blue, 1 red; 19 blue, 4 red; 1 green, 13 blue\r\nGame 51: 2 blue, 4 green, 14 red; 8 blue, 17 green, 7 red; 1 blue, 6 green, 19 red; 20 red, 17 green, 6 blue; 2 green, 1 red, 9 blue\r\nGame 52: 13 green, 17 blue, 2 red; 18 red, 12 blue, 10 green; 11 green, 17 red, 9 blue; 7 green, 11 red, 9 blue; 12 red, 15 blue; 7 green, 4 blue, 5 red\r\nGame 53: 2 green, 1 red, 3 blue; 1 red, 1 blue; 1 blue; 1 blue, 1 green, 1 red\r\nGame 54: 2 red, 5 green; 3 blue, 3 red, 2 green; 1 blue, 3 red, 5 green\r\nGame 55: 7 green, 5 blue, 4 red; 8 blue, 7 red, 8 green; 12 blue, 2 red, 16 green; 3 green, 8 blue\r\nGame 56: 9 green, 2 red, 1 blue; 1 blue, 11 red, 3 green; 9 red, 1 blue, 8 green; 10 red, 16 green\r\nGame 57: 1 red, 5 blue, 9 green; 19 blue, 2 green, 5 red; 15 green, 3 red, 7 blue; 2 blue, 15 green, 9 red; 5 red, 9 green, 15 blue\r\nGame 58: 5 green, 1 blue; 3 red, 2 blue; 2 blue, 1 red, 12 green; 8 green; 12 green, 2 blue; 4 green, 4 red\r\nGame 59: 11 blue, 5 red, 4 green; 6 red, 1 green, 3 blue; 7 red, 10 blue, 4 green; 12 blue, 1 red, 1 green\r\nGame 60: 3 green, 10 red, 10 blue; 10 green, 6 blue, 10 red; 1 blue, 6 green, 7 red; 3 red; 8 blue, 7 green, 8 red; 3 red, 19 green\r\nGame 61: 11 green, 3 blue, 20 red; 3 green, 3 blue, 20 red; 10 green, 12 red, 8 blue; 4 green, 8 blue, 6 red; 7 blue, 10 red, 5 green; 6 green, 6 red\r\nGame 62: 10 green, 9 red; 2 green, 2 blue, 5 red; 4 blue, 11 green, 12 red\r\nGame 63: 5 blue, 4 green, 2 red; 5 blue, 3 red, 2 green; 6 blue, 2 green, 2 red; 1 red, 5 blue; 1 green, 3 blue\r\nGame 64: 5 blue, 4 green, 8 red; 8 blue, 12 red, 10 green; 8 red, 7 blue; 7 green, 7 red; 5 blue, 1 red, 2 green\r\nGame 65: 3 blue, 3 red, 15 green; 12 green, 3 blue, 12 red; 13 green, 6 red, 2 blue; 1 red, 7 blue, 3 green; 9 red, 5 green, 7 blue; 1 green, 5 blue, 9 red\r\nGame 66: 1 green, 6 blue; 7 blue, 8 green; 2 blue, 9 red, 14 green\r\nGame 67: 1 blue, 8 red, 1 green; 7 red, 10 green, 4 blue; 3 blue, 1 red, 4 green\r\nGame 68: 8 blue, 8 green, 10 red; 4 red, 5 green; 4 blue, 12 red, 15 green\r\nGame 69: 2 red, 3 blue, 2 green; 1 blue, 15 green, 4 red; 15 red, 20 green; 8 red, 4 green\r\nGame 70: 6 red, 4 blue, 10 green; 5 blue, 6 red, 16 green; 9 green, 1 red, 1 blue; 2 blue, 6 green; 1 green, 3 blue, 5 red\r\nGame 71: 9 red, 9 green, 4 blue; 1 blue, 5 green; 4 red, 2 blue, 5 green; 1 blue, 3 red, 2 green\r\nGame 72: 14 blue, 1 red, 4 green; 18 blue, 1 red, 3 green; 1 red, 1 green, 10 blue\r\nGame 73: 7 red, 6 green, 1 blue; 14 green, 1 blue, 4 red; 7 red, 18 green; 1 red, 5 green\r\nGame 74: 9 green; 1 red, 7 blue, 4 green; 10 blue\r\nGame 75: 4 red, 1 green; 1 green, 4 red, 2 blue; 3 green, 2 red, 7 blue\r\nGame 76: 16 green, 7 blue, 1 red; 2 blue, 6 red, 2 green; 7 blue, 17 green; 5 red, 15 blue, 15 green\r\nGame 77: 1 red, 7 blue, 8 green; 1 red, 6 blue, 5 green; 1 red, 5 blue, 4 green; 8 green, 1 blue; 2 blue\r\nGame 78: 9 green, 3 blue; 6 red, 12 green; 5 red, 3 blue, 10 green; 3 blue, 14 green, 13 red\r\nGame 79: 20 green, 1 blue, 3 red; 11 green, 4 red, 2 blue; 11 red, 1 blue, 5 green\r\nGame 80: 14 red; 3 green, 2 blue, 7 red; 1 blue, 6 red\r\nGame 81: 1 red; 11 blue; 11 blue; 9 blue, 5 green, 1 red\r\nGame 82: 13 red, 17 blue, 9 green; 1 blue, 2 green; 9 red, 5 green, 6 blue; 10 green, 14 blue, 14 red; 5 green, 2 blue, 10 red; 4 blue, 4 green, 2 red\r\nGame 83: 6 blue, 3 red, 5 green; 3 blue, 6 green; 13 red, 11 green, 1 blue; 7 blue, 1 green, 14 red; 9 green, 2 blue, 3 red; 8 green, 3 red, 2 blue\r\nGame 84: 5 green, 8 blue; 7 red, 7 blue, 10 green; 7 blue, 7 green, 7 red; 7 blue, 1 green, 11 red\r\nGame 85: 12 blue, 1 red, 2 green; 3 green, 13 red; 17 red, 1 blue, 2 green; 4 blue, 15 red; 9 blue, 7 red; 2 green, 11 red, 4 blue\r\nGame 86: 15 green, 1 blue, 8 red; 1 blue, 18 green, 3 red; 3 red, 1 blue, 16 green\r\nGame 87: 9 red, 17 blue, 9 green; 4 green, 6 red, 2 blue; 6 red, 5 blue\r\nGame 88: 8 red, 6 blue, 17 green; 17 green, 5 blue, 12 red; 2 red, 14 green, 1 blue\r\nGame 89: 14 red, 5 blue, 6 green; 1 blue, 6 green; 4 red, 9 green, 8 blue; 2 blue, 4 red, 11 green; 12 red, 1 green, 8 blue; 3 blue, 2 green, 5 red\r\nGame 90: 3 red, 3 blue; 14 green, 8 blue; 4 red, 12 green, 8 blue\r\nGame 91: 2 blue; 2 blue, 8 red; 4 red; 8 red, 1 blue; 1 green, 2 blue\r\nGame 92: 16 green, 16 red; 5 green, 2 blue; 14 red, 16 green; 17 red, 1 blue, 12 green\r\nGame 93: 9 blue, 14 red, 6 green; 2 blue, 6 red, 3 green; 1 green, 2 blue, 12 red; 6 green, 8 red, 5 blue; 5 blue, 9 green, 10 red; 7 blue, 10 green, 3 red\r\nGame 94: 2 blue, 13 green, 7 red; 5 red, 2 blue, 14 green; 8 red, 9 green; 2 blue, 8 green, 1 red; 7 red, 12 green; 2 blue, 3 green\r\nGame 95: 1 red, 8 blue, 4 green; 1 green, 3 blue, 2 red; 6 blue, 2 red, 1 green; 3 blue, 4 green; 3 green, 1 red\r\nGame 96: 15 blue, 8 red, 5 green; 15 green, 16 blue, 4 red; 11 blue, 8 red; 16 blue, 6 green, 1 red; 10 blue, 9 red; 1 red, 3 green, 3 blue\r\nGame 97: 11 green, 8 blue, 4 red; 12 green, 11 blue, 1 red; 4 red, 1 blue, 11 green; 6 green, 1 red, 7 blue; 5 blue, 12 green, 4 red; 5 blue, 8 green\r\nGame 98: 4 green, 15 blue; 13 blue, 8 green; 10 blue, 6 green; 1 red, 7 green\r\nGame 99: 1 green, 3 blue, 18 red; 8 blue, 19 red, 5 green; 7 red, 2 blue, 2 green; 10 red, 1 blue, 2 green\r\nGame 100: 4 red, 3 green, 4 blue; 8 green, 5 red, 2 blue; 1 red, 2 blue, 7 green; 3 blue, 8 green, 5 red\r\n";

// Let's parse 'm first

string[] gameStrings = // array of 3 green, 1 blue, 3 red; 3 blue, 1 green, 3 red; 2 red, 12 green, 7 blue;
	myInput
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)		//get every game
	.Select (x => x.Split(":")[1].Trim())						//but grab only everything after :
	.ToArray();

// Convert these strings to games

List<Game> games = new List<Game>();

foreach (string gameString in gameStrings)
{
	Game game = new Game();
	games.Add(game);

	//get each color cube sequence eg array of "3 red, 4 green", "5 red, 2 green"
	string[] subsets = gameString.Split(";", StringSplitOptions.RemoveEmptyEntries);

	foreach (string subset in subsets)
	{
		game.Add(
			subset.Split(",", StringSplitOptions.RemoveEmptyEntries)			//get "5 red", "2 green"
			.Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))	//split "5 red" into 5 and red
			.ToDictionary(x => x[1].Trim(), x => int.Parse(x[0].Trim()))		//map these values into a dictionary
		);
	}
}

//Now all games are stored as a list of dictionaries that maps color to counts

// Part 1 - Count all ids for all games that are using less cubes than the ones below
// (Keep in mind the game index is game id - 1)
Dictionary<string, int> validGame = new() { { "red", 12 }, { "green", 13 }, { "blue", 14 } };

long idSum = 0;
for (int i = 0; i < games.Count; i++)
{
	Game game = games[i];
	idSum += IsValid(game, validGame) ? (i + 1) : 0;
}

bool IsValid (Game pGame, Dictionary<string, int> pValidGame)
{
	foreach (Dictionary<string, int> cubeCounts in pGame)
	{
		foreach (KeyValuePair<string, int> cubeCount in cubeCounts)
		{
			//if any game requires more cubes than we have, it is invalid
			if (cubeCount.Value > pValidGame[cubeCount.Key]) return false;
		}
	}

	return true;
}

Console.WriteLine("Part 1 - Sum of Ids of valid games: " + idSum);

// Part 2:
// - What is the fewest number of cubes of each color that could have been in the bag to
//   make the game possible?
// - Calculate these minimums, multiple them together per game (this is called the 'power' of a game
// - Add all the powers together

long totalPower = 0;

foreach (Game game in games)
{
	totalPower += GetPower(game);
}

int GetPower (Game pGame)
{
	//Gather the minimum required amount of cube over all cube counts in a game
	Dictionary<string, int> minRequiredCube = new();

	foreach (Dictionary<string, int> cubeCounts in pGame)
	{
		foreach (KeyValuePair<string,int> cubeCount in cubeCounts)
		{
			minRequiredCube.TryGetValue(cubeCount.Key, out int currentMin);
			minRequiredCube[cubeCount.Key] = Math.Max(cubeCount.Value, currentMin);
		}
	}

	int power = 1;
	foreach (int value in minRequiredCube.Values) power *= value;
	return power;
}

Console.WriteLine("Part 2 - Sum of powers of all games: " + totalPower);