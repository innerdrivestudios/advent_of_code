// Solution for https://adventofcode.com/2020/day/22 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of cards

string[] myInput = File.ReadAllLines(args[0]);

// Parse the input:

List<int> player1Deck = new();
List<int> player2Deck = new();
List<int> currentDeck = player1Deck;

foreach (string line in myInput)
{
	if (line.StartsWith("Player")) continue;
	if (string.IsNullOrEmpty(line))
	{
        currentDeck = player2Deck;
		continue;
	}

    currentDeck.Add (int.Parse(line));
}

// ** Part 1: Play the game...

int PlayTheGamePart1()
{

	Queue<int> player1Queue = new(player1Deck);
	Queue<int> player2Queue = new(player2Deck);

	while (player1Queue.Count > 0 && player2Queue.Count > 0)
	{
		int p1Card = player1Queue.Dequeue();
		int p2Card = player2Queue.Dequeue();

		if (p1Card > p2Card)
		{
			player1Queue.Enqueue(p1Card);
			player1Queue.Enqueue(p2Card);
		}
		else
		{
			player2Queue.Enqueue(p2Card);
			player2Queue.Enqueue(p1Card);
		}
	}

	Queue<int> winningQueue = player1Queue.Count == 0 ? player2Queue : player1Queue;
	List<int> winningCards = new List<int>(winningQueue);

	int result = 0;
	for (int i = 0; i < winningCards.Count; i++)
	{
		result += (winningCards.Count - i) * winningCards[i];
	}

	return result;
}

Console.WriteLine("Part 1: " + PlayTheGamePart1());

// ** Part 2: Play a game recursively? Simply execute the game as requested....

(bool p1Won, int result) PlayTheGamePart2(IEnumerable<int> pPlayer1Deck, IEnumerable<int> pPlayer2Deck)
{
	HashSet<string> roundHistory = new();

    Queue<int> player1Queue = new(pPlayer1Deck);
    Queue<int> player2Queue = new(pPlayer2Deck);

    bool p1Won = false;

    while (player1Queue.Count > 0 && player2Queue.Count > 0)
    {
		string currentDecksState = string.Join("-", player1Queue) + "X" + string.Join("-", player2Queue);
		
		if (roundHistory.Contains(currentDecksState))
		{
			//force player 1 win:
			player2Queue.Clear();
			break;
		}

		roundHistory.Add(currentDecksState);

        int p1Card = player1Queue.Dequeue();
        int p2Card = player2Queue.Dequeue();

		if (player1Queue.Count >= p1Card && player2Queue.Count >= p2Card)
		{
			(bool p1OneSubGame, int subGameResult) = PlayTheGamePart2(player1Queue.Take(p1Card), player2Queue.Take(p2Card));
			p1Won = p1OneSubGame;
		}
		else 
		{
			p1Won = p1Card > p2Card;
		}

		if (p1Won)
		{
			player1Queue.Enqueue(p1Card);
			player1Queue.Enqueue(p2Card);
		}
		else
		{
			player2Queue.Enqueue(p2Card);
			player2Queue.Enqueue(p1Card);
		}
    }

	p1Won = player1Queue.Count != 0;

    Queue<int> winningQueue = p1Won ? player1Queue : player2Queue;
    List<int> winningCards = new List<int>(winningQueue);

    int result = 0;
    for (int i = 0; i < winningCards.Count; i++)
    {
        result += (winningCards.Count - i) * winningCards[i];
    }

    return (p1Won,result);
}

Console.WriteLine("Part 2: " + PlayTheGamePart2(player1Deck, player2Deck).result);
