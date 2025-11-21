// Solution for https://adventofcode.com/2019/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 21,
// which reuses the IntCode computer from day 19,
// which reuses the IntCode computer from day 17,
// which reuses the IntCode computer from day 15,
// which reuses the IntCode computer from day 13,
// which reuses the IntCode computer from day 11,
// which reuses the IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9
// https://adventofcode.com/2019/day/11
// https://adventofcode.com/2019/day/13
// https://adventofcode.com/2019/day/15
// https://adventofcode.com/2019/day/17
// https://adventofcode.com/2019/day/19
// https://adventofcode.com/2019/day/21

// ** Part 1:

// Create 50 computers:

List<IntCodeComputer> computers = new ();
List<NetworkInterface> networkInterfaces = new ();

for (int i = 0; i < 50; i++)
{
	NetworkInterface networkComputer = new NetworkInterface();
	IntCodeComputer intCodeComputer = new IntCodeComputer(myInput, networkComputer);

	networkInterfaces.Add(networkComputer);
	computers.Add(intCodeComputer);
}

// Run all computers at the same time ...

Console.WriteLine("Run computers...");

bool hasEnded = false;
HashSet<long> collectedYValues = new();

while (!hasEnded)
{
	//Assume we end
	hasEnded = true;

	//For any computer not ended, AND its value with hasEnded (Run returns true if it has ended)
	for (int i = 0;i < 50; i++)
	{
		if (!computers[i].hasEnded) hasEnded &= computers[i].Run();
	}

	if (networkInterfaces.Any (x  => x.hasEnded)) break;

	if (networkInterfaces.All(x => x.isIdle))
	{
		//Console.WriteLine("Network is idle...");
		if (NetworkInterface.HasNatMemory())
		{
			var values = networkInterfaces[0].ConsumeNatMemory();
			if (!collectedYValues.Add (values.Item2))
			{
				Console.WriteLine("Part 2: " + values.Item2);
				break;
			}
		}
	}
}

Console.WriteLine("All ended");
