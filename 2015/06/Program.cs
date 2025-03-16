//Solution for https://adventofcode.com/2015/day/6 (Ctrl+Click in VS to follow link)

using System.Drawing;
using System.Text.RegularExpressions;
//define Instruction typedef => instruction = "toggle|turn on|turn off" + bounds (and yes, enums would be better)
using Instruction = (string instruction, System.Drawing.Rectangle bounds);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of instructions to change the state of certain lights in a 2D grid of lights
// e.g. turn on 489,959 through 759,964
//      turn off 489,959 through 759,964
//      toggle 489,959 through 759,964

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Step 1: parse the given instruction list of instructions into an easier to handle format

Regex instructionParser = new Regex(@"(toggle|turn on|turn off) (\d+),(\d+) through (\d+),(\d+)\r\n");
MatchCollection matches = instructionParser.Matches(myInput);
List<Instruction> instructions = new List<Instruction>();

foreach (Match match in matches)
{
    int left = int.Parse(match.Groups[2].Value);        //first number (\+d)        => left
    int top = int.Parse(match.Groups[3].Value);         //second number (\+d)       => top
    int right = int.Parse(match.Groups[4].Value);       //etc                       => right
    int bottom = int.Parse(match.Groups[5].Value);      //etc                       => bottom

    Instruction instruction = new Instruction(
        match.Groups[1].Value, 					                        //toggle|turn on|turn off
        new Rectangle(left, top, right - left + 1, bottom - top + 1)    //+1 because ranges are inclusive
    );

    instructions.Add(instruction);
}

// NOW we can run the challenges ;)

// ** Part 1: interpret all instructions as is and count the resulting number of lights that are on

int size = 1000;
Grid<int> lights = new Grid<int>(size, size);

foreach (Instruction instruction in instructions)
{
    switch (instruction.instruction)
    {
        case "turn on":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] = 1);

            break;
        case "turn off":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] = 0);

            break;
        case "toggle":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] = 1 - value);

            break;
        default:
            Console.WriteLine("Op not found");
            break;
    }
}

int totalOn = 0;
lights.Foreach( (pos, value) => totalOn += value );

Console.WriteLine("Part 1 - Total lights on:" + totalOn);

// ** Part 2: interpret all instructions as changes in brightness

//Reset lights
lights.Foreach((pos, value) => lights[pos] = 0);

foreach (Instruction instruction in instructions)
{
    switch (instruction.instruction)
    {
        case "turn on":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] += 1);                                          //brightness + 1
            break;
        case "turn off":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] = Math.Max(0, lights[pos] - 1));             //brightness - 1 with a min of 0
            break;
        case "toggle":
            lights.ForeachRegion(instruction.bounds, (pos, value) => lights[pos] += 2);                                          //brightness + 2
            break;
        default:
            Console.WriteLine("Op not found");
            break;
    }
}

int totalBrightness = 0;
lights.Foreach((pos, value) => totalBrightness += value);

Console.WriteLine("Part 2 - Total brightness:" + totalBrightness);
