// Solution for https://adventofcode.com/2019/day/17 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 15,
// which reuses the IntCode computer from day 13,
// which reuses the IntCode computer from day 11,
// which reuses the IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9
// https://adventofcode.com/2019/day/11
// https://adventofcode.com/2019/day/13
// https://adventofcode.com/2019/day/15

VacuumRobot vacuumRobot = new VacuumRobot();
IntCodeComputer robotController = new IntCodeComputer(myInput, vacuumRobot);
robotController.Run();

Grid<char> scaffold = new Grid<char>(vacuumRobot.GetOutput(), "\n");
scaffold.Print("");

// ** Part 1: We could use a follow the intersections approach, but the area is fairly 
// small, so let's try a scanline approach first.

Vec2i[] crossDirections = [new(1, 0), new(0, 1), new(-1, 0), new(0, -1)];

bool IsIntersection (Vec2i pPoint)
{
	Vec2i left = pPoint + crossDirections[0];
	Vec2i right = pPoint + crossDirections[1];
	Vec2i up = pPoint + crossDirections[2];
	Vec2i down = pPoint + crossDirections[3];

	return
		scaffold[pPoint] == '#' &&
		scaffold.IsInside(left) && scaffold[left] == '#' &&
		scaffold.IsInside(right) && scaffold[right] == '#' &&
		scaffold.IsInside(up) && scaffold[up] == '#' &&
		scaffold.IsInside(down) && scaffold[down] == '#';
}

long intersectionScore = 0;
scaffold.Foreach(
	(pos, value) => 
	{
		if (IsIntersection(pos)) intersectionScore += (pos.X * pos.Y);
    }
);

Console.WriteLine("Part 1: " + intersectionScore);

// ** Part 2: Holy shit :).

// Before programming the robot, let's see if we can figure out the overall instructions
// in terms of the lowest level commands: L R and amount of units...

// So let's create a "pathfinder"...

// First figure out the starting point...

Vec2i start = new();
string directionChars = ">v<^";
Directions<Vec2i> directions = new(crossDirections);

scaffold.Foreach(
	(pos, value) =>
	{
		if (directionChars.Contains(value))
		{
			start = pos;
			directions.index = directionChars.IndexOf(value);
		}
	}
);

Console.WriteLine("Starting at " + start);
Console.WriteLine("In direction:"+directions.Current());

//Now we need to search for a path...
//How do we do that? 
//We move forward if we can, otherwise we look left or right to where we need to go...
//This is a bit of fishy debug code, to get the actual route to display...

/*
int count = 0;

while (true)
{

	//try forward
	Vec2i newPos = start + directions.Current();
	if (scaffold.IsInside(newPos) && scaffold[newPos] == '#')
	{
		start = newPos;
		count++;
		continue;
    }

	//try left
	directions.index -= 1;
	newPos = start + directions.Current();
	if (scaffold.IsInside(newPos) && scaffold[newPos] == '#')
	{
		start = newPos;
		if (count > 0) Console.Write(count+",");
		Console.Write("L,");
		count = 1;
		continue;
	}

	//try right
	directions.index += 2;
	newPos = start + directions.Current();
	if (scaffold.IsInside(newPos) && scaffold[newPos] == '#')
	{
		start = newPos;
        if (count > 0) Console.Write(count + ",");
		Console.Write("R,");
		count = 1;
        continue;
	}

	//scaffold[start] = 'X';
	break;
}

Console.Write(count);
*/

// The instructions printed by the code above are (in my case):
// L,6,R,12,L,6,L,8,L,8,L,6,R,12,L,6,L,8,L,8,L,6,R,12,R,8,L,8,L,4,L,4,L,6,
// L,6,R,12,R,8,L,8,L,6,R,12,L,6,L,8,L,8,L,4,L,4,L,6,L,6,R,12,R,8,L,8,L,4,
// L,4,L,6,L,6,R,12,L,6,L,8,L,8

// If we try to split this into 3 repeating patterns A, B, C MANUALLY (for now), we get:
// L,6,R,12,L,6,L,8,L,8,
// L,6,R,12,L,6,L,8,L,8,
// L,6,R,12,R,8,L,8,
// L,4,L,4,L,6,
// L,6,R,12,R,8,L,8,
// L,6,R,12,L,6,L,8,L,8,
// L,4,L,4,L,6,
// L,6,R,12,R,8,L,8,
// L,4,L,4,L,6,
// L,6,R,12,L,6,L,8,L,8,
//
// When defining patterns:
// A = L,6,R,12,L,6,L,8,L,8,
// B = L,6,R,12,R,8,L,8,
// C = L,4,L,4,L,6,
//
// This becomes A,A,B,C,B,A,C,B,C,A (see the VacuumRobot for the rest)

robotController.OverwriteMemory(0, 2);
vacuumRobot.Reset();
robotController.Run();
Console.WriteLine("Part 2:" + vacuumRobot.GetOutput());

// Possible improvement, figure out all of the patterns automatically through code, skipped for now...

