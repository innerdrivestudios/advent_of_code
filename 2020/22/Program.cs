// Solution for https://adventofcode.com/2020/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of cards

string[] myInput = File.ReadAllLines(args[0]);

// Parse the input:

Queue<int> player1Queue = new();
Queue<int> player2Queue = new();
Queue<int> currentQueue = player1Queue;

foreach (string line in myInput)
{
	if (line.StartsWith("Player")) continue;
	if (string.IsNullOrEmpty(line))
	{
		currentQueue = player2Queue;
		continue;
	}

	currentQueue.Enqueue (int.Parse(line));
}

// ** Part 1: Play the game...

while (player1Queue.Count > 0 && player2Queue.Count > 0)
{
	int p1Card = player1Queue.Dequeue();
	int p2Card = player2Queue.Dequeue();

	if (p1Card > p2Card)
	{
		player1Queue.Enqueue (p1Card);
		player1Queue.Enqueue (p2Card);
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
	result += (winningCards.Count -i) * winningCards[i];
}

Console.WriteLine("Part 1: " + result);