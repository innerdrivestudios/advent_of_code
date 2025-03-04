// Solution for https://adventofcode.com/2015/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of box dimensions specified like 4x23x21

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: Calculate the required wrapping paper according to the challenge specs

// Convert the input to an array of int arrays representing the sorted dimensions of each box 

int[][] dimensionArrays =
    myInput
        //Get each separate box dimension specification
        .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
        //Take each separate line and convert it into a sorted int[]
        .Select(line => line.Split('x').Select(int.Parse).OrderBy(n => n).ToArray())
        // Convert the whole list of int[] to int[][]
        .ToArray();

// And calculate the requested data:

int totalWrappingPaper = 0;

foreach (int[] dimensions in dimensionArrays)
{
    //Required wrapping paper is 2*l*w + 2*w*h + 2*h*l ...
    totalWrappingPaper += 2 * (dimensions[0] * dimensions[1] + dimensions[1] * dimensions[2] + dimensions[0] * dimensions[2]);
    //PLUS the AREA of the SMALLEST side for extra paper
    totalWrappingPaper += dimensions[0] * dimensions[1];
}

Console.WriteLine("Part 1 (Required amount of wrapping paper):" + totalWrappingPaper);

// ** Part 2: Calculate the required ribbon length according to the challenge specs

int totalRibbon = 0;

foreach (int[] dimensions in dimensionArrays)
{
    //Required ribbon is 2 times the smallest side...
    totalRibbon += 2 * (dimensions[0] + dimensions[1]);
    //PLUS the l*w*h (according to specs)
    totalRibbon += dimensions[0] * dimensions[1] * dimensions[2];
}

Console.WriteLine("Part 2 (Required amount of ribbon):" + totalRibbon);


