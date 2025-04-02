//Solution for https://adventofcode.com/2019/day/10 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<char> grid = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1: Find the position from which the most asteroids are visible...

// Approach:
// - Enumerate all # positions
// - Clone it to a HashSet
// - Go through each item in the list,
//   Extrapolate it's position and remove the results that are 'hidden'
// - Return whatever is left and count its number of items

List<Vec2i> positionsList = new List<Vec2i>();
grid.Foreach((position, character) => { if (character == '#') positionsList.Add(position); });

// To extrapolate the position we'll use a helper method that will allow us
// to calculate the smallest integer step we can make.
//
// This is based on the common greatest divisor.
// E.g. if our step is 2,4, the smallest integer step is 1,2
//		if our step is 6,9, the smallest integer step is 2,3
//
// To calculate the GCD we'll use the Euclidean algorithm based on %:
int GCD(int a, int b)
{
	while (b != 0)
	{
		int temp = b;
		b = a % b;
		a = temp;
	}
	return a;

	// Why/how does this work?
	// The GCD of two numbers doesn't change if you replace the larger
	// number with the remainder of dividing the larger by the smaller, e.g.
	//
	// If d divides both a and b, then d also divides a % b:
	// E.g. if 3 divides both 9 and 6, then 3 also divides 9 % 6 = 3
	// since we can write 9 as 3x3 and 6 as 2x3, so we can basically subtract them
	// The other way around if you have 9 and 3
	// then whatever divides them can also divide 9+3
}

HashSet<Vec2i> GetVisiblePositions(Vec2i pStartingPosition)
{
	//Clone all the asteroid positions
	HashSet<Vec2i> positionsSet = new HashSet<Vec2i>(positionsList);
	//Don't count the current asteroid
	positionsSet.Remove(pStartingPosition);

	//Then for each other asteroid in the list, get its delta
	//and starting from THAT asteroid, take the smallest whole (integer) step
	//in the direction of that asteroid removing any asteroid we find
	//until we are outside of the grid
	foreach (Vec2i pos in positionsList)
	{
		//If we were already removed, skip us
		if (!positionsSet.Contains(pos)) continue;
	
		//get the delta between us and the next asteroid
		Vec2i delta = pos - pStartingPosition;
		Vec2i extrapolated = pStartingPosition + delta;

		//from there take the minimum whole step
		int gcd = GCD(Math.Abs(delta.X), Math.Abs(delta.Y));
		delta.X /= gcd;
		delta.Y /= gcd;
        extrapolated += delta;

		//And keep removing any asteroid encountered
		while (grid.IsInside(extrapolated))
		{
			positionsSet.Remove(extrapolated);
			extrapolated += delta;
		}
	}

	return positionsSet;
}

//Now using the methods above, find the maximum visible asteroids
//their count and the space station position
int maxAsteroidsVisible = int.MinValue;
HashSet<Vec2i> maxVisiblePositions = null;
Vec2i centerPosition = new Vec2i();

grid.Foreach(
	(position, character) =>
	{
		if (grid[position] == '#')
		{
			HashSet<Vec2i> visiblePositions = GetVisiblePositions(position);

			if (visiblePositions.Count > maxAsteroidsVisible)
			{
				maxAsteroidsVisible = visiblePositions.Count;
				maxVisiblePositions = visiblePositions;
				centerPosition = position;
			}
		}
	}
);

Console.WriteLine("Part 1:" + maxAsteroidsVisible);

// ** Part 2: What will be the coordinate of the 200th asteroid and thus our bet?
// We just deducted we can see 317 asteroids,
// in other words, we don't need to make more than one round, phew!

// The "only" thing we need to do is sort the asteroids correctly according to their
// angle to the station location. Some errors I made here initially is forgetting
// that 0 degrees in a normal axis system is to the right and the fact that up
// means NEGATIVE y's...
double OffsettedAngle (Vec2i pPosition)
{
	// By default in a coordinate system where X is to the right and Y is up
	// Atan2 (y,x) gives the angle offset from (1,0) which is zero, 
	// rotating CCW from X to Y.
	//
	// In this system up is -Y, which is basically our X axis then
	// (since up is zero degrees) meaning the original Y axis would point to the left
	// which which we don't want, since plus rotation needs to rotate clockwise to the right
	// so we also flip that, in other words Y = -X, X = -Y

	// Note all of this is in radians, converting to degrees for readability is optional

	return Math.Atan2(-pPosition.X, -pPosition.Y);
}

List<Vec2i> sortedPositions = maxVisiblePositions.OrderBy(x => OffsettedAngle(x - centerPosition)).ToList();
Vec2i position200 = sortedPositions[199];
Console.WriteLine("Part 2: " + (100 * position200.X + position200.Y));

/*
for (int i = 0; i < sortedPositions.Count; i++) 
{
	Vec2i position = sortedPositions[i];
    Console.WriteLine("["+i+"] " + position + " " + (position - centerPosition) +  "=> "+ OffsettedAngle(position-centerPosition));
}
*/
