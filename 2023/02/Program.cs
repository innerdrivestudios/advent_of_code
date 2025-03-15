// Solution for https://adventofcode.com/2023/day/2 (Ctrl+Click in VS to follow link)

// A game is a sequence of colors to counts
using Game = System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, int>>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of games

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Let's parse 'm first

string[] gameStrings = // array of 3 green, 1 blue, 3 red; 3 blue, 1 green, 3 red; 2 red, 12 green, 7 blue;
	myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)		//get every game
	.Select (x => x.Split(":")[1].Trim())									//but grab only everything after :
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

// ** Part 1 - Count all ids for all games that are using less cubes than the ones below
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

// ** Part 2:
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