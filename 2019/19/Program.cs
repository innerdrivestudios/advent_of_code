// Solution for https://adventofcode.com/2019/day/17 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 17,
// which reuses the IntCode computer from day 15,
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
// https://adventofcode.com/2019/day/17

DroneInfoProvider droneInfoProvider = new DroneInfoProvider();
IntCodeComputer robotController = new IntCodeComputer(myInput, droneInfoProvider);

// ** Part 1: How many points are affected by the tractor beam in the 50x50 area closest to the emitter?
// (For each of X and Y, this will be 0 through 49.)

int totalInBeam = 0;

for (int x = 0; x < 50; x++)
{
	for (int y = 0; y < 50; y++)
	{
        droneInfoProvider.SetCoordinateToTest(new Vec2i(x, y));
        robotController.Run();
		totalInBeam += droneInfoProvider.beingPulled ? 1 : 0;
    }
}

Console.WriteLine("Part 1:" + totalInBeam);

// ** Part 2: What is the XY coord of the top left corner where we can fit a 100x100 square into the beam?

// This is pretty simple, we'll reuse a variation of what we did above to count the amount of diagonal positions
// within the beam at a certain distance from the origin:

int GetDiagonalLengthWithinBeam (int pDistance)
{
    int totalInBeam = 0;

    List<Vec2i> points = new List<Vec2i>();

    for (int x = 0; x <= pDistance; x++)
    {
        Vec2i point = new Vec2i(x, pDistance - x);
        droneInfoProvider.SetCoordinateToTest(point);
        robotController.Run();
        totalInBeam += droneInfoProvider.beingPulled ? 1 : 0;
    }

    return totalInBeam;
}

// Unoptimized and fairly lazy...

List<Vec2i> GetPointsWithinBeam(int pDistance)
{
    List<Vec2i> points = new List<Vec2i>();

    for (int x = 0; x <= pDistance; x++)
    {
        Vec2i point = new Vec2i(x, pDistance - x);
        droneInfoProvider.SetCoordinateToTest(point);
        robotController.Run();
        if (droneInfoProvider.beingPulled) points.Add(point);
    }

    return points;
}

// Run a loop to find a diagonal of length a hundred...

// First let's make a good estimate, since we know the beam diagonal size will grow linearly with the distance:
int lengthAtFiveHundred = GetDiagonalLengthWithinBeam(500);
int startingDistance = 100 / lengthAtFiveHundred * 500;
int distanceFromOrigin = startingDistance;

while (true) 
{
    int length = GetDiagonalLengthWithinBeam(distanceFromOrigin);

    if (length == 100) break;

    // Using a slightly faster way to converge on the answer we are looking for
    // This is not scientifically correct, would be better to use a sequence of 1,2,4,8,16 etc
    // and then sink back down again (like a binary search), but this is quicker to program ;)
    // Depending on the beam in question you can divide by more than 2 if needed
    distanceFromOrigin += (100 - length)/2 + 1;
}

List<Vec2i> pointsWithinBeam = GetPointsWithinBeam(distanceFromOrigin);

// Get start and end of diagonal
Vec2i first = pointsWithinBeam.First();
Vec2i last = pointsWithinBeam.Last();

// Calculate the top left using the first and last point
Vec2i topLeft = new Vec2i(first.X, last.Y);

// Get the requested answer
Console.WriteLine("Part 2:" + (topLeft.X * 10000 + topLeft.Y));