// Solution for https://adventofcode.com/2022/day/1 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of calorie blocks 

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

long[][] numbers = myInput
	.Split(                                             //Split list into inventory blocks
		Environment.NewLine + Environment.NewLine, 
		StringSplitOptions.RemoveEmptyEntries
	)				
	.Select(	
		x => x.Split(                                   //Split inventory blocks into lines
			Environment.NewLine, 
			StringSplitOptions.RemoveEmptyEntries
		)			
		.Select (long.Parse).ToArray()					//and convert those lines to longs
	)
	.ToArray();

// ** Part 1 - Get the inventory block with the highest (calorie) count

Console.WriteLine("Part 1 - Highest calorie count: "+ numbers.Select(x => x.Sum()).Max());

// ** Part 2 - Get the total calorie count for the top 3 blocks

Console.WriteLine(
	"Part 2 - Calorie count for the top 3 elves: " +
	numbers.Select(x => x.Sum()).OrderByDescending(x => x).Take(3).Sum()
);