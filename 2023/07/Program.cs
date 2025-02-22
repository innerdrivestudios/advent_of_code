// Solution for https://adventofcode.com/2023/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of hands and bids

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Step 1: Parse the input ...

List<(string hand, int bid)> hands = myInput
	.Split(new string[] { Environment.NewLine, " " }, StringSplitOptions.RemoveEmptyEntries)
	.Chunk(2)
	.Select(x => (x[0], int.Parse(x[1])))
	.ToList();

// ** Step 2: Run the challenges ... 

Console.WriteLine(new Part1().Run(hands));
Console.WriteLine(new Part2().Run(hands));
