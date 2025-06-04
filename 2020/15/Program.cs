// Solution for https://adventofcode.com/2020/day/15 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: starting input for a memory game e.g. 9,3,1,0,8,4

int[] startingNumbers = args[0].Split(',').Select (int.Parse).ToArray();

// ** Part 1: what will be the 2020th number spoken?

// Let's build the starting structure, so we know when a number was spoken and spoken last:

Dictionary<int, (int previous, int last)> historyMap = new();

// Turns are 1 based...
for(int i = 0; i < startingNumbers.Length; i++)
{
	historyMap[startingNumbers[i]] = (i + 1, i + 1);
}

int lastNumber = startingNumbers.Last();
int lastTurn = startingNumbers.Length; //0,1,2,3 => 4 

void GenerateNextNumber ()
{
	//look at the last number:
	(int previous, int last) history = historyMap[lastNumber];
	int newNumber = history.last - history.previous;

	lastTurn++;
	(int previous, int last) historyForNewNumber = historyMap.GetValueOrDefault (newNumber, (lastTurn, lastTurn));
	historyForNewNumber.previous = historyForNewNumber.last;
	historyForNewNumber.last = lastTurn;
	historyMap[newNumber] = historyForNewNumber;

	lastNumber = newNumber;
}

while (lastTurn < 2020)	GenerateNextNumber();
Console.WriteLine(lastNumber);

// ** Part 2: What is the 30000000 number spoken?

while (lastTurn < 30000000) GenerateNextNumber();
Console.WriteLine(lastNumber);
