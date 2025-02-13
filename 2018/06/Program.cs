// Solution for https://adventofcode.com/2018/day/6 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of number pairs

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Vec2i[] inputCoords = myInput
	.Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)		//Seperate lines
	.Select (x => x.Split(", ", StringSplitOptions.RemoveEmptyEntries))		//string[] of numbers
	.Select (y => new Vec2i(int.Parse(y[0]), int.Parse(y[1])))				//Vec2i
	.ToArray ();                                                            //Vec2i[]

// ** Part 1 - For a grid whose size is based on these numbers,
//			   fill the grid based on distances from the given coordinates.
//			   When tied for distance, nobody gets the grid.
//			   Find the largest area which is not touching the side of the grid.

// How can we approach this efficiently?
// - first off we'll need a "grid" correctly sized...
// - second we'll need a mechanism to efficiently calculate the distance from every point to every point

// Brute force approach:
// - for every grid cell, count the distance to every other point given
// - if there is 1 closest point, mark it the point owner as the grid cell owner
//	 if there are 2 closest points, mark the grid cell as '.'
//   if you find another point that is closer, reset the closest point count
// - if a grid location is on the border score it with -1
// - award a point for each assigned grid location that hasn't a score of -1

// The nice thing about this puzzle:
// - it looks like you need a real grid, but the trick is that you don't :)
// - you only need to administrate data as if you where dealing with an actual grid

// First find the grid dimensions...

Vec2i gridDimensions = new Vec2i();

foreach (Vec2i coord in inputCoords)
{
	gridDimensions.X = Math.Max (gridDimensions.X, coord.X);
	gridDimensions.Y = Math.Max (gridDimensions.Y, coord.Y);
}

// Set up the count administration with an initial value of 0

int[] cellCount = new int[inputCoords.Length];

// Now go through all grid cells ...

for (int x = 0; x <= gridDimensions.X; x++)
{
	for (int y = 0; y <= gridDimensions.Y; y++)
	{
		Vec2i gridLocation = new Vec2i(x, y);

		// Count the distance from each inputCoords to this gridLocation
		// to see which inputcoord(s) win(s) ...

		int distanceCount = 0;
		int closestDistance = int.MaxValue;
		int closestIndex = -1;

		for (int i = 0; i < inputCoords.Length; i++)
		{
			int distance = (inputCoords[i] - gridLocation).ManhattanDistance();

			if (distance <= closestDistance)
			{
				//if the distance is the same as the current closest distance, we inc the count of items having this distance
				if (distance == closestDistance) distanceCount++;
				//otherwise this "first" closestDistance counts as one
				else distanceCount = 1;
				closestDistance = distance;
				closestIndex = i;
			}
		}

		// If only one won and we are not ignored the cellCount for this index process this coord
		if (distanceCount == 1 && cellCount[closestIndex] != -1)
		{
			// If the winning cell is on the edge of the grid, mark this coord as ignored...
			if (gridLocation.X == 0 || gridLocation.Y == 0 || gridLocation.X == gridDimensions.X && gridLocation.Y == gridDimensions.Y)
			{
				cellCount[closestIndex] = -1;
			}
			else //we expand our area count for this index...
			{
				cellCount[closestIndex]++;
			}
		}
		// else: multiple winners are not counted...
	}
}

Console.WriteLine("Part 1 - Largest area that isn't infinite:" + cellCount.Max());

// ** Part 2 : What is the size of the region containing all locations which have a total distance to all given coordinates of less than 10000?

int regionSize = 0;

for (int x = 0; x <= gridDimensions.X; x++)
{
    for (int y = 0; y <= gridDimensions.Y; y++)
    {
        Vec2i gridLocation = new Vec2i(x, y);

        // Count the distance from each inputCoords to this gridLocation...

        int distanceTotal = 0;

        for (int i = 0; i < inputCoords.Length; i++)
        {
            distanceTotal += (inputCoords[i] - gridLocation).ManhattanDistance();
        }

		// Less than 10000? NICE :)

		if (distanceTotal < 10000) regionSize++;
    }
}

Console.WriteLine("Part 2 - Region size:" + regionSize);