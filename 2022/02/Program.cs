// Solution for https://adventofcode.com/2022/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a collection of char pairs, representing a Rock Paper Scissor strategy guide
// ABC is what the opponent plays, XYZ is your recommended response:

// A & X for Rock, B & Y for Paper, C & Z for Scissors

// where

// - X Rock defeats Scissors C
// - Y Paper defeats Rock A
// - Z Scissors defeats Paper B
// e.g. AX < BY < CZ < AX

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] charPairs = myInput 
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
	.ToArray();

// ** Part 1 - 

// Following the given strategy guide for the moves you have to make, what is your predicted score?
// How do you score?
// - Add your chosen move => X=1, Y=2, Z=3
// - Add the result of your move: 0 for loss, 3 for draw, 6 for win

long GetPredictedScoresForGivenDesiredMoves (string[] pStrategyGuide) {
	long score = 0;

	//AX < BY < CZ < AX
	//we lookup the index of the given pair and divide the index by 3 to get the result (int rounddown)
	//0 is loss, 1 is draw, 2 is win, multiplying this by 3 gives us the correct result
	List<string> resultLookup = [
		"B X", "C Y", "A Z", //losses
		"A X", "B Y", "C Z", //draws
		"A Y", "B Z", "C X"  //wins
	];

	foreach (var pair in pStrategyGuide) {
		int battleScore = (resultLookup.IndexOf(pair) / 3) * 3;
		int moveScore = (pair[2] - 'X' + 1);                      //plus 1 for X, 2 for Y, 3 for X

        //Console.WriteLine($"Battle score:{battleScore} Move score:{moveScore}");
        score += battleScore + moveScore;
	}

	return score;
}

Console.WriteLine("Part 1 - Get predicted score: " + GetPredictedScoresForGivenDesiredMoves(charPairs));

// ** Part 2 - 

// Following the given strategy guide for how the battle must end, what is your predicted score?
// How do you score?
// - First you make the battle end as desired indicated by => X = lose, Y = draw, Z = win
//   adding points for the move you made
//   (ABC would now be more logical, or even directly map ABC to 012)
// - Then you add the result of the response: 0 for loss, 3 for draw, 6 for win

long GetPredictedScoresForGivenDesiredOutcomes(string[] pStrategyGuide)
{
	long score = 0;

	//Let's say opponent plays B and we need to lose, this means X (rock) should be played:
	//The easiest way to do this is using a list with dictionaries
	//First get the specific dictionary based on desired end result, 
	//then use the move made to lookup the desired response and score it
	//Note that we only need the score, but mapping that directly makes the code really hard to understand

	List<Dictionary<char, char>> resultLookup = [
		new() { { 'B', 'X'}, { 'C', 'Y'}, { 'A', 'Z'} }, //losses
		new() { { 'A', 'X'}, { 'B', 'Y'}, { 'C', 'Z'} }, //draws
		new() { { 'A', 'Y'}, { 'B', 'Z'}, { 'C', 'X'} }, //wins
	];

	foreach (var pair in pStrategyGuide)
	{
		//XYZ indicate the required end result, so map it to 0, 1, 2 and multiply it with 3
		int desiredResult = pair[2] - 'X';
		int battleScore = desiredResult * 3;
		//now look up the matching directionary, and use the opponents move to get our move and score it
		//again, we could also have mapped ABC to ABC here or ABC to 012
		int moveScore = resultLookup[desiredResult][pair[0]] - 'X' + 1;                         

		//Console.WriteLine($"Battle score:{battleScore} Move score:{moveScore}");
		score += battleScore + moveScore;
	}

	return score;
}

Console.WriteLine("Part 2 - Get predicted score: " + GetPredictedScoresForGivenDesiredOutcomes(charPairs));