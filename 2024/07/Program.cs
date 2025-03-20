//Solution for https://adventofcode.com/2024/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers followed by a list of numbers you need to try and make them with

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Parse the input

string[] equations = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

List<Equation> equationsList = new List<Equation>();

foreach (string equation in equations)
{
	equationsList.Add(new Equation(equation));
}

// ** Part 1

long total = 0;
foreach (Equation equation in equationsList)
{
	if (equation.EvaluatePart1())
	{
		total += equation.result;
	}
}

Console.WriteLine("Part 1:" + total);

// ** Part 2

total = 0;
foreach (Equation equation in equationsList)
{
	if (equation.EvaluatePart2())
	{
		total += equation.result;
	}
}

Console.WriteLine("Part 2:" + total);

