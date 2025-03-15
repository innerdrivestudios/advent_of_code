// Solution for https://adventofcode.com/2021/day/2 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of steering instructions

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

(string instruction, long distance)[] steeringInstructions =
	myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)		// Get all the separate lines
	.Select(x => x.Split(" "))												// Split those on " " so we have arrays of instruction and distance
	.Select(x => (x[0], long.Parse(x[1])))									// Convert these to (string, long)
	.ToArray();																// And return them as an array

// ** Part 1: Calculate the final distance travelled by the submarine in X an Y direction and multiply them

long GetFinalDistanceAndDepth ((string instruction, long distance)[] pSteeringInstructions)
{
	(long x, long y) finalPosition = (0, 0);

	foreach (var instruction in pSteeringInstructions)
	{
		switch (instruction.instruction)
		{
			case "forward": finalPosition.x += instruction.distance; break;
			case "up":		finalPosition.y -= instruction.distance; break;
			case "down":	finalPosition.y += instruction.distance; break;
		}
	}

	//We need to multiply instead of add for this challenge
	return finalPosition.x * finalPosition.y;
}

Console.WriteLine("Part 1 - Total distance travelled: " + GetFinalDistanceAndDepth(steeringInstructions));

// ** Part 2:
// In addition to horizontal position and depth,
// you'll also need to track a third value, aim, which also starts at 0.
// The commands also mean something entirely different than you first thought:

//    down X increases your aim by X units.
//    up X decreases your aim by X units.
//    forward X does two things:
//        It increases your horizontal position by X units.
//        It increases your depth by your aim multiplied by X.

long GetFinalDistanceDepthAndAim((string instruction, long distance)[] pSteeringInstructions)
{
	(long x, long y, long aim) finalPosition = (0, 0, 0);

	foreach (var instruction in pSteeringInstructions)
	{
		switch (instruction.instruction)
		{
			case "forward": 
				finalPosition.x += instruction.distance; 
				finalPosition.y += finalPosition.aim * instruction.distance; 
				break;
			case "up": 
				finalPosition.aim -= instruction.distance; 
				break;
			case "down": 
				finalPosition.aim += instruction.distance; 
				break;
		}
	}

	//We need to multiply instead of add for this challenge
	return finalPosition.x * finalPosition.y;
}

Console.WriteLine("Part 2 - Total distance travelled: " + GetFinalDistanceDepthAndAim(steeringInstructions));