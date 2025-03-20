//Solution for https://adventofcode.com/2016/day/8 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

//instructions will be rect, row or column (see below)
//values will be width, height | row, amount | column, amount
using Instruction = (string instruction, int value1, int value2);
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of instructions to enable cells in a grid and rotate them

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<bool> screen = new Grid<bool>(50, 6);
//explicitly set the screen to off
screen.Foreach((pos, value) => screen[pos] = false);

//convert the instructions to a more usable format, e.g.:
//rect 1x1                  => rect,1,1
//rotate row y=0 by 6       => row,0,6
//rotate column x=0 by 1    => column,0,1

List<Instruction> instructions = ParseInstructions(myInput);  

List<Instruction> ParseInstructions (string pInstructions)
{
    List<Instruction> instructions = new List<Instruction>();

    string[] instructionsAsString = pInstructions.Split(Environment.NewLine,StringSplitOptions.RemoveEmptyEntries);

    //rect 1x1
    //rotate row y=0 by 6
    //rotate column x=0 by 1
    //                           G1    G2                 G3        G4    G5       G6
    string pattern = @"(?:rect (\d+)x(\d+))|(?:rotate (row|column) (x|y)=(\d+) by (\d+))";
    Regex regex = new Regex(pattern);

    foreach (string instruction in instructionsAsString)
    {
        //Console.WriteLine("@"+instruction+"@");
        Match match = regex.Match(instruction);

        if (match.Groups[1].Success) // rect case
        {
            //Console.WriteLine($"Rect: {match.Groups[1].Value}x{match.Groups[2].Value}");
            instructions.Add(("rect", int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)));
        }
        else if (match.Groups[3].Success) // rotate row/column case
        {
            //Console.WriteLine($"Rotate: {match.Groups[3].Value} {match.Groups[4].Value} = {match.Groups[5].Value} by {match.Groups[6].Value}");
            //"column", "row"                        row/column                        //amount
			instructions.Add((match.Groups[3].Value, int.Parse(match.Groups[5].Value), int.Parse(match.Groups[6].Value)));
		}
    }

    return instructions;
}

Part1and2(instructions);
Console.ReadKey();

void Part1and2(List<Instruction> pInstructions)
{
	foreach (Instruction instruction in pInstructions)
	{
        ProcessInstruction(instruction);
        Console.Clear();
        Console.WriteLine(instruction);
        screen.Print("", "\n", (pos, value) => value?"#":".");
        Thread.Sleep(10);
	}

    int pixelLit = 0;
    screen.Foreach((pos, value) => pixelLit += value ? 1 : 0);
    Console.WriteLine("Pixels on:"+pixelLit);
}

void ProcessInstruction(Instruction pInstruction)
{
	switch (pInstruction.instruction) {
        case "rect"     : ProcessRectInstruction(pInstruction); return;
        case "column"   : ProcessShiftColumnInstruction(pInstruction); return;
        case "row"      : ProcessShiftRowInstruction(pInstruction); return;
    }
}

void ProcessRectInstruction (Instruction pInstruction)
{
	Vec2i topLeft = new Vec2i(0, 0);
	Vec2i widthHeight = new Vec2i(pInstruction.value1, pInstruction.value2);

    screen.ForeachRegion(topLeft, widthHeight, (pos, value) => screen[pos] = true);
}

void ProcessShiftColumnInstruction (Instruction pInstruction)
{
    Vec2i topLeft = new Vec2i(pInstruction.value1, 0);
    Vec2i widthHeight = new Vec2i(1, screen.height);

	Grid<bool> columnClone = screen.Clone (topLeft, widthHeight);

	columnClone.Foreach(
		(pos, value) => screen[WrapVec2(pos + topLeft + new Vec2i(0, pInstruction.value2))] = value
	);
}

void ProcessShiftRowInstruction(Instruction pInstruction)
{
	Vec2i topLeft = new Vec2i(0, pInstruction.value1);
	Vec2i widthHeight = new Vec2i(screen.width, 1);

	Grid<bool> columnClone = screen.Clone(topLeft, widthHeight);
    Console.WriteLine(columnClone.width +":"+ columnClone.height);

    columnClone.Foreach(
    	(pos, value) => screen[WrapVec2(pos + topLeft + new Vec2i(pInstruction.value2,0))] = value
	);
}

Vec2i WrapVec2 (Vec2i pInput)
{
    pInput.X = WrapInt(pInput.X, screen.width);
    pInput.Y = WrapInt(pInput.Y, screen.height);
    return pInput;
}

//Make sure given input vectors remain on screen
int WrapInt (int pInput, int modulo)
{
    return ((pInput % modulo) + modulo) % modulo;
}
