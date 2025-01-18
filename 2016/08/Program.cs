//Solution for https://adventofcode.com/2016/day/8 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
//instructions will be rect, row or column (see below)
//values will be width, height | row, amount | column, amount
using Instruction = (string instruction, int value1, int value2);
using System.Text.RegularExpressions;

string myInput = "rect 1x1\r\nrotate row y=0 by 6\r\nrect 1x1\r\nrotate row y=0 by 3\r\nrect 1x1\r\nrotate row y=0 by 5\r\nrect 1x1\r\nrotate row y=0 by 4\r\nrect 2x1\r\nrotate row y=0 by 5\r\nrect 2x1\r\nrotate row y=0 by 2\r\nrect 1x1\r\nrotate row y=0 by 5\r\nrect 4x1\r\nrotate row y=0 by 2\r\nrect 1x1\r\nrotate row y=0 by 3\r\nrect 1x1\r\nrotate row y=0 by 3\r\nrect 1x1\r\nrotate row y=0 by 2\r\nrect 1x1\r\nrotate row y=0 by 6\r\nrect 4x1\r\nrotate row y=0 by 4\r\nrotate column x=0 by 1\r\nrect 3x1\r\nrotate row y=0 by 6\r\nrotate column x=0 by 1\r\nrect 4x1\r\nrotate column x=10 by 1\r\nrotate row y=2 by 16\r\nrotate row y=0 by 8\r\nrotate column x=5 by 1\r\nrotate column x=0 by 1\r\nrect 7x1\r\nrotate column x=37 by 1\r\nrotate column x=21 by 2\r\nrotate column x=15 by 1\r\nrotate column x=11 by 2\r\nrotate row y=2 by 39\r\nrotate row y=0 by 36\r\nrotate column x=33 by 2\r\nrotate column x=32 by 1\r\nrotate column x=28 by 2\r\nrotate column x=27 by 1\r\nrotate column x=25 by 1\r\nrotate column x=22 by 1\r\nrotate column x=21 by 2\r\nrotate column x=20 by 3\r\nrotate column x=18 by 1\r\nrotate column x=15 by 2\r\nrotate column x=12 by 1\r\nrotate column x=10 by 1\r\nrotate column x=6 by 2\r\nrotate column x=5 by 1\r\nrotate column x=2 by 1\r\nrotate column x=0 by 1\r\nrect 35x1\r\nrotate column x=45 by 1\r\nrotate row y=1 by 28\r\nrotate column x=38 by 2\r\nrotate column x=33 by 1\r\nrotate column x=28 by 1\r\nrotate column x=23 by 1\r\nrotate column x=18 by 1\r\nrotate column x=13 by 2\r\nrotate column x=8 by 1\r\nrotate column x=3 by 1\r\nrotate row y=3 by 2\r\nrotate row y=2 by 2\r\nrotate row y=1 by 5\r\nrotate row y=0 by 1\r\nrect 1x5\r\nrotate column x=43 by 1\r\nrotate column x=31 by 1\r\nrotate row y=4 by 35\r\nrotate row y=3 by 20\r\nrotate row y=1 by 27\r\nrotate row y=0 by 20\r\nrotate column x=17 by 1\r\nrotate column x=15 by 1\r\nrotate column x=12 by 1\r\nrotate column x=11 by 2\r\nrotate column x=10 by 1\r\nrotate column x=8 by 1\r\nrotate column x=7 by 1\r\nrotate column x=5 by 1\r\nrotate column x=3 by 2\r\nrotate column x=2 by 1\r\nrotate column x=0 by 1\r\nrect 19x1\r\nrotate column x=20 by 3\r\nrotate column x=14 by 1\r\nrotate column x=9 by 1\r\nrotate row y=4 by 15\r\nrotate row y=3 by 13\r\nrotate row y=2 by 15\r\nrotate row y=1 by 18\r\nrotate row y=0 by 15\r\nrotate column x=13 by 1\r\nrotate column x=12 by 1\r\nrotate column x=11 by 3\r\nrotate column x=10 by 1\r\nrotate column x=8 by 1\r\nrotate column x=7 by 1\r\nrotate column x=6 by 1\r\nrotate column x=5 by 1\r\nrotate column x=3 by 2\r\nrotate column x=2 by 1\r\nrotate column x=1 by 1\r\nrotate column x=0 by 1\r\nrect 14x1\r\nrotate row y=3 by 47\r\nrotate column x=19 by 3\r\nrotate column x=9 by 3\r\nrotate column x=4 by 3\r\nrotate row y=5 by 5\r\nrotate row y=4 by 5\r\nrotate row y=3 by 8\r\nrotate row y=1 by 5\r\nrotate column x=3 by 2\r\nrotate column x=2 by 3\r\nrotate column x=1 by 2\r\nrotate column x=0 by 2\r\nrect 4x2\r\nrotate column x=35 by 5\r\nrotate column x=20 by 3\r\nrotate column x=10 by 5\r\nrotate column x=3 by 2\r\nrotate row y=5 by 20\r\nrotate row y=3 by 30\r\nrotate row y=2 by 45\r\nrotate row y=1 by 30\r\nrotate column x=48 by 5\r\nrotate column x=47 by 5\r\nrotate column x=46 by 3\r\nrotate column x=45 by 4\r\nrotate column x=43 by 5\r\nrotate column x=42 by 5\r\nrotate column x=41 by 5\r\nrotate column x=38 by 1\r\nrotate column x=37 by 5\r\nrotate column x=36 by 5\r\nrotate column x=35 by 1\r\nrotate column x=33 by 1\r\nrotate column x=32 by 5\r\nrotate column x=31 by 5\r\nrotate column x=28 by 5\r\nrotate column x=27 by 5\r\nrotate column x=26 by 5\r\nrotate column x=17 by 5\r\nrotate column x=16 by 5\r\nrotate column x=15 by 4\r\nrotate column x=13 by 1\r\nrotate column x=12 by 5\r\nrotate column x=11 by 5\r\nrotate column x=10 by 1\r\nrotate column x=8 by 1\r\nrotate column x=2 by 5\r\nrotate column x=1 by 5\r\n";

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

    string[] instructionsAsString = pInstructions.Split("\r\n",StringSplitOptions.RemoveEmptyEntries);

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
