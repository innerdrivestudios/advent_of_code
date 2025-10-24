//Solution for https://adventofcode.com/2018/day/23 (Ctrl+Click in VS to follow link)

using Vec3l = Vec3<long>;
using Nanobot = (Vec3<long> position, long range);
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of nanobots

string[] myInput = File.ReadAllLines(args[0]);
Regex nanobotParser = new Regex(@"pos=<(-?\d+),(-?\d+),(-?\d+)>, r=(-?\d+)");

List<Nanobot> nanobots = new();

foreach (var input in myInput)
{
	Match match = nanobotParser.Match(input);
	if (match.Success)
	{
		nanobots.Add(
			new Nanobot (
				new Vec3l(
					long.Parse(match.Groups[1].Value), 
					long.Parse(match.Groups[2].Value), 
					long.Parse(match.Groups[3].Value)
				),
				long.Parse(match.Groups[4].Value)
			)
		);
	}
	else Console.Write("X");
}

Console.WriteLine(nanobots.Count + " nanobots parsed.");

nanobots.Sort ((a, b) => Math.Sign(b.range - a.range));

Nanobot biggestRangeBot = nanobots[0];

Console.WriteLine(
	"Part 1:" + 
	nanobots.Count(
		x => 
		(x.position - biggestRangeBot.position).ManhattanDistance() < biggestRangeBot.range
	)
);

// ** Part 2: What position is in the range of the most nanobots?

// I thought about/tried different things:

// 1. Bruteforcing it ->
//		only thought about this, the range of space makes it clear this is not an option right away
// 2. Count the overlap for each Radar ->
//		wrong idea since the fact that 1 radar overlaps with 2 radars, doesn't mean the 3 of them overlap
//		at a single point in space
// 3. Assume the solution has to lie in a point where 2 spheres touch ->
//		works for the test data but not for my real data
// 4. Zoom out ->
//		What if we make the universe smaller, so multiple points end up being one point...
//		And we look for the overlap then, but realizing we are simplifying things, zoom out
//      a bit and repeat until we are all zoomed in again? 

(Vec3l position, int count) ZoomOut (Vec3l pMin, Vec3l pMax, long pZoomFactor)
{
    pMin /= pZoomFactor;
	pMax /= pZoomFactor;

    //Console.WriteLine(pMin);
    //Console.WriteLine(pMax);
    //Console.WriteLine(pMax-pMin);

    Vec3l bestCoordinate = new Vec3l();
    int highest = 0;

    for (long x = pMin.X; x <= pMax.X; x++)
	{
		for (long y = pMin.Y; y <= pMax.Y; y++)
		{
			for (long z = pMin.Z; z <= pMax.Z; z++)
			{
				//We want to check the "original coordinate"
				Vec3l coordinateToCheck = new Vec3l(x, y, z) * pZoomFactor;
				//But squint/zoom out a bit where math is concerned...
                int overlapCount = GetNanobotsInRangeCount(coordinateToCheck, pZoomFactor);
				if (overlapCount >= highest)
				{
					if (overlapCount > highest)
					{
                        bestCoordinate = coordinateToCheck;
                        highest = overlapCount;
                    }
					else // turns out this is not really needed, even though I feel it is more correct?
					{
						if (highest > 0 && overlapCount == highest && coordinateToCheck.ManhattanDistance() < bestCoordinate.ManhattanDistance())
						{
							bestCoordinate = coordinateToCheck;
						}
					}

                }
			}
		}
	}

	return (bestCoordinate, highest);
}

// Modified get nano bots in range that takes zooming out into account...

int GetNanobotsInRangeCount(Vec3l pPosition, double pZoomFactor)
{
    return nanobots.Count(
        x =>
		//Be lenient on both the required distance							      //And the required range...
        Math.Floor((x.position - pPosition).ManhattanDistance() / pZoomFactor) <= Math.Ceiling(x.range / pZoomFactor)
    );
}

int initialZoomFactor = 10000000;
Vec3l min = Vec3l.Min(nanobots.Select(x => x.position));
Vec3l max = Vec3l.Max(nanobots.Select(x => x.position));
var result = ZoomOut(min, max, initialZoomFactor);

Console.WriteLine(min);
Console.WriteLine(max);
Console.WriteLine(result + " " + initialZoomFactor + " " + result.position.ManhattanDistance());
Console.WriteLine();

while (initialZoomFactor != 1)
{
	// now that we have a new center point, we'll calculate a new min and max
	initialZoomFactor /= 2;
	min = result.position - 2*new Vec3l(initialZoomFactor, initialZoomFactor, initialZoomFactor);
	max = result.position + 2*new Vec3l(initialZoomFactor, initialZoomFactor, initialZoomFactor);

    result = ZoomOut(min, max, initialZoomFactor);

    Console.WriteLine(min);
    Console.WriteLine(max);
    Console.WriteLine($"Part 2 at zoom {initialZoomFactor}:" + result + " "+ result.position.ManhattanDistance());
    Console.WriteLine();
}

Console.WriteLine("Part 2:" + result.position.ManhattanDistance());

// Side note: using this solution I feel there is still a chance with a different data set
// that focusing in on a single peak would ignore the true peak... but tbh I'm not entirely sure...