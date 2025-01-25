// Solution for https://adventofcode.com/2021/day/2 (Ctrl+Click in VS to follow link)

// Your input: a bunch of steering instructions

string myInput = "forward 5\r\ndown 8\r\ndown 6\r\ndown 7\r\ndown 8\r\nforward 7\r\ndown 3\r\nup 6\r\nforward 6\r\ndown 2\r\nforward 5\r\ndown 6\r\nup 3\r\ndown 4\r\nforward 4\r\ndown 6\r\ndown 1\r\nup 5\r\nforward 5\r\ndown 1\r\ndown 7\r\nup 2\r\ndown 7\r\nforward 1\r\nforward 6\r\ndown 1\r\nup 1\r\nup 4\r\nforward 3\r\nforward 6\r\nforward 1\r\nforward 4\r\nup 3\r\nforward 1\r\nforward 4\r\ndown 9\r\nforward 4\r\nforward 8\r\nup 8\r\nforward 5\r\nup 4\r\nup 3\r\ndown 8\r\nforward 5\r\ndown 4\r\nforward 1\r\nforward 7\r\ndown 1\r\nforward 8\r\ndown 4\r\nforward 2\r\nforward 7\r\nforward 9\r\nup 4\r\ndown 3\r\nforward 7\r\nforward 6\r\ndown 8\r\nforward 2\r\nforward 5\r\nforward 4\r\ndown 6\r\nforward 6\r\nup 5\r\ndown 3\r\ndown 6\r\ndown 5\r\ndown 7\r\ndown 8\r\nup 5\r\ndown 5\r\nforward 5\r\nforward 4\r\nup 3\r\ndown 7\r\ndown 3\r\nforward 4\r\ndown 2\r\nforward 4\r\nforward 3\r\nforward 4\r\nforward 9\r\nforward 6\r\nforward 8\r\nup 8\r\ndown 8\r\nup 5\r\ndown 4\r\ndown 8\r\nup 7\r\nup 8\r\ndown 6\r\ndown 3\r\nforward 2\r\nforward 7\r\nup 1\r\nup 2\r\nforward 2\r\ndown 7\r\ndown 1\r\nup 9\r\nforward 6\r\nforward 4\r\ndown 2\r\nup 6\r\ndown 2\r\ndown 1\r\ndown 3\r\nup 6\r\ndown 1\r\ndown 8\r\nforward 7\r\nup 8\r\nforward 5\r\nforward 8\r\ndown 8\r\nforward 6\r\nforward 8\r\ndown 3\r\ndown 4\r\ndown 6\r\nup 2\r\nforward 6\r\nup 9\r\nforward 4\r\nforward 8\r\nup 4\r\ndown 8\r\nforward 8\r\ndown 8\r\ndown 4\r\ndown 5\r\nforward 7\r\ndown 6\r\ndown 6\r\nup 2\r\nup 1\r\nforward 7\r\nforward 8\r\nforward 4\r\nforward 9\r\ndown 7\r\nforward 4\r\nup 5\r\ndown 3\r\nup 4\r\ndown 9\r\ndown 2\r\ndown 8\r\nforward 3\r\nforward 5\r\nforward 7\r\nforward 9\r\nforward 5\r\nforward 8\r\nforward 6\r\nforward 4\r\nforward 6\r\nforward 7\r\nforward 2\r\ndown 1\r\ndown 8\r\ndown 4\r\ndown 5\r\ndown 6\r\nup 3\r\nup 2\r\nforward 4\r\ndown 4\r\nforward 7\r\nup 6\r\nup 9\r\ndown 1\r\ndown 3\r\ndown 1\r\nup 3\r\nup 1\r\ndown 2\r\nup 5\r\nforward 1\r\ndown 7\r\nforward 9\r\ndown 4\r\nup 4\r\ndown 6\r\ndown 3\r\nforward 4\r\nup 6\r\nup 4\r\nforward 1\r\nup 7\r\ndown 1\r\ndown 7\r\ndown 7\r\nforward 9\r\ndown 3\r\ndown 3\r\nforward 6\r\ndown 2\r\nforward 7\r\nup 4\r\nup 8\r\ndown 8\r\nforward 7\r\nforward 6\r\ndown 7\r\nforward 5\r\nup 6\r\nup 6\r\ndown 9\r\nup 6\r\nup 2\r\nforward 9\r\nforward 1\r\nup 5\r\nup 3\r\ndown 9\r\nup 8\r\ndown 7\r\nup 7\r\nforward 5\r\ndown 7\r\ndown 4\r\nforward 2\r\nforward 3\r\nforward 5\r\ndown 1\r\nup 6\r\ndown 6\r\nup 6\r\ndown 8\r\ndown 3\r\ndown 4\r\nforward 9\r\ndown 3\r\nforward 3\r\nup 1\r\ndown 2\r\nforward 8\r\ndown 7\r\nup 9\r\nforward 1\r\ndown 3\r\nforward 1\r\nforward 8\r\ndown 3\r\nforward 8\r\nforward 6\r\ndown 1\r\ndown 9\r\nforward 2\r\ndown 1\r\ndown 6\r\nup 1\r\nup 7\r\ndown 9\r\nforward 6\r\nforward 5\r\nforward 2\r\nup 6\r\ndown 6\r\nforward 6\r\nup 3\r\ndown 7\r\ndown 8\r\nforward 5\r\ndown 7\r\nforward 8\r\ndown 8\r\nforward 4\r\ndown 6\r\nforward 4\r\ndown 7\r\nup 5\r\ndown 5\r\ndown 5\r\ndown 4\r\ndown 3\r\nforward 8\r\nforward 1\r\ndown 8\r\ndown 2\r\nforward 3\r\nforward 7\r\nforward 3\r\ndown 5\r\ndown 6\r\ndown 8\r\ndown 6\r\nforward 9\r\nforward 4\r\nforward 8\r\ndown 5\r\ndown 7\r\nforward 4\r\nup 5\r\ndown 8\r\nup 6\r\nup 7\r\ndown 6\r\ndown 8\r\nforward 3\r\nup 6\r\nforward 7\r\ndown 4\r\nup 1\r\nup 8\r\nforward 3\r\ndown 6\r\ndown 1\r\nforward 7\r\ndown 1\r\ndown 9\r\nforward 6\r\ndown 4\r\nforward 3\r\nforward 1\r\ndown 5\r\ndown 9\r\ndown 9\r\ndown 5\r\ndown 8\r\ndown 7\r\nforward 1\r\nforward 5\r\ndown 2\r\nforward 2\r\nforward 1\r\ndown 8\r\nforward 6\r\ndown 3\r\nforward 4\r\nup 2\r\nup 8\r\nforward 7\r\nforward 4\r\ndown 8\r\nup 6\r\nforward 3\r\nup 1\r\nup 2\r\nforward 5\r\nforward 9\r\ndown 5\r\nforward 2\r\nforward 5\r\nup 6\r\ndown 1\r\ndown 1\r\ndown 6\r\nforward 6\r\ndown 7\r\nforward 5\r\nforward 8\r\ndown 7\r\ndown 5\r\nforward 9\r\nforward 1\r\nup 6\r\ndown 7\r\nforward 1\r\nforward 4\r\ndown 5\r\ndown 6\r\nup 3\r\nup 8\r\nup 5\r\ndown 8\r\ndown 8\r\ndown 6\r\ndown 2\r\ndown 3\r\ndown 9\r\nforward 8\r\nforward 7\r\nforward 7\r\nup 5\r\ndown 5\r\nforward 9\r\nup 8\r\nup 5\r\nforward 1\r\ndown 9\r\ndown 9\r\nforward 9\r\nforward 4\r\nforward 6\r\nup 9\r\nup 5\r\nup 3\r\ndown 9\r\nup 7\r\nup 1\r\ndown 3\r\ndown 9\r\ndown 7\r\nforward 6\r\ndown 7\r\nforward 7\r\nforward 8\r\ndown 2\r\nforward 5\r\nup 1\r\ndown 6\r\nup 9\r\nforward 5\r\nup 9\r\ndown 2\r\ndown 3\r\nforward 5\r\ndown 9\r\nforward 9\r\nforward 2\r\nforward 8\r\ndown 1\r\nforward 8\r\nup 1\r\nforward 3\r\nup 1\r\ndown 1\r\nforward 9\r\ndown 2\r\nforward 2\r\nup 1\r\nup 8\r\ndown 2\r\ndown 7\r\ndown 5\r\nup 2\r\nup 6\r\ndown 9\r\ndown 7\r\ndown 7\r\nup 6\r\nup 8\r\ndown 7\r\nforward 5\r\ndown 4\r\ndown 5\r\nup 8\r\nup 6\r\ndown 6\r\nforward 6\r\nup 6\r\ndown 1\r\ndown 1\r\ndown 1\r\nforward 1\r\ndown 8\r\ndown 4\r\ndown 5\r\ndown 2\r\ndown 5\r\nup 8\r\nup 8\r\ndown 3\r\ndown 6\r\ndown 1\r\nforward 6\r\nforward 5\r\nforward 1\r\ndown 3\r\ndown 4\r\nup 9\r\ndown 3\r\nup 8\r\nforward 5\r\ndown 5\r\nforward 2\r\ndown 8\r\ndown 2\r\nup 1\r\nforward 7\r\nup 8\r\nforward 7\r\ndown 3\r\ndown 1\r\ndown 3\r\nforward 4\r\ndown 5\r\ndown 8\r\nforward 8\r\nforward 3\r\nforward 7\r\ndown 7\r\nforward 4\r\ndown 1\r\nforward 3\r\nup 2\r\ndown 7\r\ndown 1\r\nforward 4\r\nforward 7\r\ndown 3\r\ndown 1\r\nforward 4\r\ndown 3\r\nforward 2\r\nup 9\r\ndown 5\r\ndown 9\r\nforward 5\r\nup 5\r\ndown 3\r\nup 6\r\nup 8\r\ndown 7\r\ndown 3\r\ndown 9\r\nforward 6\r\nforward 8\r\nforward 3\r\ndown 6\r\nup 8\r\nforward 8\r\nforward 9\r\ndown 4\r\ndown 1\r\nforward 2\r\ndown 2\r\nup 2\r\ndown 5\r\ndown 1\r\ndown 3\r\nforward 4\r\ndown 3\r\nup 8\r\nup 6\r\nup 5\r\ndown 4\r\nforward 3\r\nup 6\r\nforward 6\r\nforward 2\r\ndown 8\r\ndown 5\r\nforward 3\r\nup 1\r\nforward 5\r\nforward 9\r\nforward 5\r\ndown 5\r\nforward 3\r\nforward 6\r\nforward 5\r\nforward 3\r\ndown 1\r\ndown 1\r\ndown 1\r\ndown 9\r\nforward 8\r\nforward 2\r\nforward 4\r\nforward 8\r\ndown 1\r\nup 8\r\ndown 1\r\ndown 6\r\ndown 5\r\nup 8\r\ndown 4\r\nforward 8\r\nforward 6\r\ndown 6\r\nforward 2\r\nforward 7\r\nforward 2\r\nup 7\r\nforward 4\r\nup 1\r\nup 8\r\ndown 3\r\ndown 2\r\ndown 3\r\nup 7\r\ndown 9\r\nup 5\r\ndown 1\r\ndown 3\r\nup 5\r\ndown 6\r\nup 9\r\ndown 4\r\ndown 7\r\ndown 6\r\ndown 4\r\nforward 5\r\nforward 6\r\ndown 8\r\nforward 3\r\nforward 8\r\nup 5\r\nup 6\r\nup 8\r\nforward 8\r\nforward 1\r\ndown 6\r\nforward 3\r\nforward 3\r\nforward 6\r\ndown 3\r\ndown 2\r\nforward 5\r\ndown 5\r\nforward 6\r\ndown 3\r\ndown 9\r\ndown 8\r\ndown 6\r\ndown 6\r\nforward 1\r\nup 5\r\ndown 9\r\nforward 3\r\nforward 3\r\ndown 2\r\nforward 8\r\nforward 3\r\nforward 2\r\nforward 5\r\ndown 4\r\ndown 1\r\nup 2\r\ndown 1\r\ndown 1\r\nforward 5\r\ndown 7\r\nup 7\r\ndown 9\r\ndown 8\r\ndown 6\r\nforward 3\r\nforward 5\r\ndown 3\r\ndown 6\r\nup 3\r\nup 2\r\nup 8\r\ndown 3\r\nup 3\r\ndown 6\r\nforward 7\r\nforward 4\r\nup 5\r\nforward 1\r\nup 3\r\nforward 8\r\ndown 2\r\ndown 5\r\ndown 2\r\nforward 4\r\nforward 4\r\ndown 4\r\nup 8\r\ndown 1\r\nup 2\r\nforward 2\r\nforward 9\r\nforward 4\r\ndown 3\r\ndown 7\r\nforward 1\r\ndown 2\r\nforward 8\r\ndown 8\r\nforward 3\r\ndown 7\r\nforward 9\r\nforward 6\r\nup 1\r\nforward 3\r\nup 2\r\nup 3\r\nforward 6\r\ndown 8\r\nup 9\r\ndown 2\r\ndown 9\r\ndown 6\r\ndown 4\r\nforward 5\r\nforward 3\r\nup 7\r\nforward 7\r\nup 7\r\nup 6\r\ndown 7\r\ndown 2\r\nup 7\r\ndown 5\r\nup 9\r\nforward 3\r\nup 6\r\nup 6\r\nup 6\r\nup 1\r\nforward 5\r\nforward 5\r\ndown 8\r\nforward 6\r\nforward 7\r\ndown 3\r\ndown 4\r\ndown 2\r\ndown 4\r\ndown 1\r\nforward 7\r\ndown 7\r\ndown 5\r\nforward 8\r\nup 6\r\nup 8\r\nforward 8\r\nforward 2\r\nforward 4\r\ndown 6\r\ndown 4\r\ndown 2\r\ndown 3\r\nforward 8\r\nforward 6\r\ndown 3\r\nforward 7\r\nforward 4\r\nup 8\r\ndown 9\r\nforward 5\r\nup 5\r\nup 5\r\nup 7\r\nforward 3\r\nup 1\r\ndown 2\r\nforward 5\r\nforward 5\r\nup 1\r\nforward 4\r\ndown 6\r\nup 5\r\nup 3\r\nforward 9\r\ndown 9\r\ndown 6\r\ndown 1\r\ndown 2\r\ndown 4\r\ndown 7\r\nforward 3\r\nup 5\r\nforward 2\r\ndown 3\r\nforward 7\r\nup 8\r\nup 3\r\nforward 6\r\nup 7\r\nup 1\r\nup 2\r\ndown 5\r\nforward 5\r\ndown 3\r\ndown 5\r\ndown 6\r\nup 1\r\ndown 2\r\nup 1\r\nforward 3\r\ndown 3\r\ndown 4\r\ndown 6\r\ndown 1\r\ndown 3\r\nforward 9\r\nforward 1\r\ndown 1\r\nup 3\r\nforward 4\r\nforward 7\r\nforward 4\r\ndown 2\r\nforward 6\r\nforward 2\r\nforward 7\r\ndown 9\r\nforward 8\r\nforward 3\r\nup 8\r\ndown 9\r\nup 8\r\nforward 5\r\nforward 9\r\ndown 4\r\nforward 1\r\nup 9\r\nforward 2\r\ndown 6\r\nup 3\r\nforward 1\r\nforward 3\r\nforward 8\r\ndown 7\r\ndown 3\r\ndown 5\r\ndown 2\r\ndown 2\r\nforward 4\r\nforward 1\r\ndown 2\r\nup 8\r\ndown 2\r\nforward 3\r\ndown 2\r\ndown 6\r\ndown 1\r\nup 1\r\ndown 7\r\ndown 3\r\nforward 3\r\nforward 1\r\nforward 9\r\ndown 9\r\ndown 2\r\nup 1\r\nforward 9\r\nup 2\r\ndown 2\r\nforward 3\r\ndown 4\r\nforward 9\r\nforward 5\r\nup 5\r\nforward 2\r\nup 3\r\nforward 8\r\ndown 3\r\nforward 5\r\nforward 5\r\ndown 8\r\nup 9\r\nforward 7\r\nup 2\r\nup 2\r\nup 1\r\nup 7\r\ndown 8\r\nforward 9\r\nforward 9\r\nup 6\r\ndown 5\r\nforward 7\r\ndown 9\r\ndown 8\r\ndown 5\r\ndown 3\r\ndown 2\r\nforward 6\r\ndown 7\r\nforward 3\r\nup 5\r\nforward 1\r\nup 7\r\nforward 3\r\ndown 5\r\ndown 9\r\ndown 8\r\nforward 2\r\nup 4\r\nforward 7\r\nforward 5\r\nforward 8\r\nforward 7\r\nup 7\r\nforward 4\r\nup 7\r\ndown 9\r\nforward 1\r\nforward 3\r\ndown 3\r\nforward 4\r\ndown 3\r\nforward 3\r\ndown 5\r\ndown 1\r\nforward 6\r\ndown 4\r\ndown 3\r\ndown 2\r\nup 1\r\ndown 1\r\ndown 6\r\ndown 6\r\nforward 9\r\ndown 5\r\nforward 1\r\nup 4\r\nforward 7\r\ndown 8\r\nforward 1\r\nforward 9\r\nforward 7\r\ndown 1\r\ndown 3\r\nup 2\r\ndown 5\r\nup 6\r\nforward 2\r\nup 2\r\ndown 7\r\ndown 9\r\nforward 3\r\nup 5\r\nup 7\r\ndown 4\r\nforward 6\r\ndown 8\r\nforward 7\r\nup 1\r\nup 4\r\nforward 4\r\ndown 9\r\nforward 9\r\nforward 9\r\ndown 3\r\nforward 5\r\nforward 1\r\ndown 3\r\ndown 8\r\nforward 7\r\ndown 4\r\nforward 3\r\ndown 3\r\nforward 8\r\nforward 2\r\nforward 6\r\nup 9\r\nforward 2\r\ndown 9\r\nforward 2\r\ndown 1\r\nforward 9\r\nup 1\r\nup 4\r\nup 1\r\ndown 1\r\nforward 4\r\nup 9\r\nup 8\r\ndown 1\r\ndown 3\r\ndown 2\r\nforward 9\r\ndown 7\r\ndown 4\r\nforward 2\r\nup 9\r\ndown 7\r\ndown 1\r\ndown 9\r\nforward 2\r\ndown 2\r\nforward 9\r\ndown 5\r\nup 1\r\ndown 3\r\nup 6\r\ndown 4\r\nforward 8\r\ndown 2\r\ndown 2\r\ndown 9\r\nforward 9\r\nforward 2\r\ndown 1\r\nforward 6\r\ndown 2\r\nup 4\r\ndown 8\r\nup 4\r\ndown 6\r\ndown 2\r\nforward 7\r\ndown 3\r\nup 3\r\nforward 1\r\nup 4\r\nforward 5\r\ndown 7\r\ndown 8\r\nforward 7\r\nforward 3\r\ndown 5\r\nup 6\r\ndown 7\r\ndown 1\r\nup 7\r\ndown 1\r\nforward 6\r\nforward 3\r\nforward 3\r\nforward 7\r\n";

(string instruction, long distance)[] steeringInstructions =
	myInput
	.Split("\r\n", StringSplitOptions.RemoveEmptyEntries)		// Get all the separate lines
	.Select(x => x.Split(" "))									// Split those on " " so we have arrays of instruction and distance
	.Select(x => (x[0], long.Parse(x[1])))						// Convert these to (string, long)
	.ToArray();													// And return them as an array

// Part 1: Calculate the final distance travelled by the submarine in X an Y direction and multiply them

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

// Part 2:
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