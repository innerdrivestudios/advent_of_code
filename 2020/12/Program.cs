// Solution for https://adventofcode.com/2020/day/12 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Instruction = (char instruction, int distance);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of steering instructions

string[] myInput = File.ReadAllLines(args[0]);
List<Instruction> instructions = myInput
	.Select(x => (x[0], int.Parse(x.Substring(1))))
	.ToList();

//Set up directions (starting east) plus starting position
Directions<Vec2i> directions = new Directions<Vec2i>([new(1, 0), new(0, -1), new(-1, 0), new(0, 1)]);
directions.index = 0;
//Set up direction string that matches directions above
string directionString = "ESWN";
Vec2i currentPosition = new Vec2i(0, 0);

//Follow the instructions as provided in the puzzle...

foreach (var instruction in instructions)
{
	int direction = directionString.IndexOf(instruction.instruction);
	if (direction > -1)
	{
		currentPosition += directions.Get(direction) * instruction.distance;
	}
    else
    {
		switch (instruction.instruction)
		{
			case 'L': 
				directions.index -= instruction.distance/90; 
				break;
			case 'R': 
				directions.index += instruction.distance / 90;
				break;
			case 'F': 
				currentPosition += directions.Current() * instruction.distance; 
				break;
		}     
    }
}

Console.WriteLine("Part 1: " + currentPosition.ManhattanDistance());

// ** Part 2: Reinterpret all instructions differently :)
// (See puzzle description)

currentPosition = new Vec2i(0, 0);
Vec2i waypoint = new Vec2i(10, 1);

//Define rotation helper method:
Vec2i Rotate (Vec2i pInput, int pRotations)
{
	Vec2i tmp = pInput;
	//how many times do we have to rotate?
	for (int i = 0; i < int.Abs(pRotations); i++)
	{
		//Rotate a vector by 90 degrees is either -Y,X or Y,-X
		//depending on the direction of rotation
		tmp = new Vec2i(tmp.Y, -tmp.X) * int.Sign(pRotations);
	}
	return tmp;
}

// Follow the instructions again, but now differentiate between
// which vector you apply the operations to

foreach (var instruction in instructions)
{
	int direction = directionString.IndexOf(instruction.instruction);
	if (direction > -1)
	{
		waypoint += directions.Get(direction) * instruction.distance;
	}
	else
	{
		switch (instruction.instruction)
		{
			case 'L':
				waypoint = Rotate(waypoint, -instruction.distance / 90);
				break;
			case 'R':
				waypoint = Rotate(waypoint, instruction.distance / 90);
				break;
			case 'F':
				currentPosition += waypoint * instruction.distance;
				break;
		}
	}
}

Console.WriteLine("Part 2: " + currentPosition.ManhattanDistance());
