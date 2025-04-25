//Solution for https://adventofcode.com/2017/day/13 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: fire wall layer specifications and their size

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine);

// First get all firewall specs as (index, size)

List<(int layer, int size)> intTuples = myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
	.Select(x => x.Split(": "))
	.Select(x => (int.Parse(x[0]), int.Parse(x[1])))
	.ToList();

// Then convert that to an actual list of firewall specifications we can go through one at a time

int[] firewall = new int[intTuples.Last().layer+1];
foreach (var intTuple in intTuples) firewall[intTuple.layer] = intTuple.size;

// Define a helper method to simulate the position of a scanner at time x

int PingPong(int pInput, int pMin, int pMax)
{
    int pWrapValue = pMax - pMin;
    return int.Abs((pInput + pWrapValue) % (2 * pWrapValue) - pWrapValue) + pMin;
}

// And a method to check whether we collide at timestep x with layer x
// Optional delay already built in for part 2

bool CollisionWithScannerAt (int pTimeStep, int pDelay = 0)
{
	//no firewall present at this step
	if (firewall[pTimeStep] == 0) return false;
	//fire wall present, check if the scanner is not at our position
	return PingPong(pTimeStep + pDelay, 0, firewall[pTimeStep] - 1) == 0;
}

int severity = 0;
for (int i = 0; i < firewall.Length; i++)
{
	if (CollisionWithScannerAt(i)) severity += i * firewall[i];
}

Console.WriteLine("Part 1: "+severity);

// ** Part 2: Simple bruteforce test:

bool AnyCollisionWithScannerUsingDelay (int pDelay)
{
    for (int i = 0; i < firewall.Length; i++)
    {
        if (CollisionWithScannerAt(i, pDelay)) return true;
    }

	return false;
}

int delay = 0;
while (AnyCollisionWithScannerUsingDelay(delay)) delay++;

Console.WriteLine("Part 2: "+delay);
