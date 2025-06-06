// Solution for https://adventofcode.com/2022/day/15 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;
using Vec2i = Vec2<int>;
using Sensor = (Vec2<int> position, Vec2<int> closestBeacon);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// Your input: a list of sensor descriptions including an indication of where the closest beacon is (according to manhattan distance)

string sensorDescriptions = File.ReadAllText(args[0]);

Regex sensorParser = new Regex(@"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
MatchCollection sensorMatches = sensorParser.Matches(sensorDescriptions);

List<Sensor> sensorList = new List<Sensor>();
foreach (Match match in sensorMatches)
{
    sensorList.Add(
        new Sensor(
            new Vec2i(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
            new Vec2i(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
        )
    );
}

// ** Part 1: In the row where y=2000000, how many positions cannot contain a beacon?

HashSet<Vec2i> GatherNonAvailableBeaconPositionsAtYIs (Sensor pSensor, int pY, bool debug = false)
{
    //Based on the sensor position and the closest beacon, we first get the delta between them
    Vec2i beaconDelta = pSensor.position - pSensor.closestBeacon;
    int beaconDistance = beaconDelta.ManhattanDistance();

    //How much of the manhatten distance is taken up by the y distance from the center?
    int unitsTaken = int.Abs(pY - pSensor.position.Y);
    int unitsLeft = beaconDistance - unitsTaken;
    int xStart = pSensor.position.X - unitsLeft;
    int xEnd = pSensor.position.X + unitsLeft;

    HashSet<Vec2i> result = new HashSet<Vec2i>();

    for(int x = xStart; x <= xEnd; x++)
    {
        result.Add(new Vec2i(x, pY));
        if (debug) Console.WriteLine(new Vec2i(x, pY));
    }

    return result;
}

// Gather the all the positions where there is no beacon...
HashSet<Vec2i> nonBeaconPositions = new HashSet<Vec2i>();
foreach (Sensor sensor in sensorList)
{
    nonBeaconPositions.UnionWith(GatherNonAvailableBeaconPositionsAtYIs(sensor, 2000000));
}

// Make sure the actual beacon positions aren't in there
foreach (Sensor sensor in sensorList)
{
    nonBeaconPositions.Remove (sensor.closestBeacon);
}

Console.WriteLine("Part 1:" + nonBeaconPositions.Count);

// ** Part 2 : Find the only possible position for the distress beacon. What is its tuning frequency?

// Your handheld device indicates that the distress signal is coming from a beacon nearby.
// The distress beacon is not detected by any sensor, but the distress beacon must have x and y coordinates each
// no lower than 0 and no larger than 4000000.

// To isolate the distress beacon's signal, you need to determine its tuning frequency,
// which can be found by multiplying its x coordinate by 4000000 and then adding its y coordinate.

// Unfortunately, the current implementation is way too slow, so we'll need to find a faster way.

// Let's first rewrite our GatherNonAvailableBeaconPositionsAtYIs into GetSensorRange, where x is start and y is end (inclusive):

Vec2i? GetSensorRange(Sensor pSensor, int pY)
{
    //Based on the sensor position and the closest beacon, we first get the delta between them
    Vec2i beaconDelta = pSensor.position - pSensor.closestBeacon;
    int beaconDistance = beaconDelta.ManhattanDistance();

    //How much of the manhatten distance is taken up by the y distance from the center?
    int unitsTaken = int.Abs(pY - pSensor.position.Y);
    int unitsLeft = beaconDistance - unitsTaken;
    if (unitsLeft < 0) return null;

    int xStart = pSensor.position.X - unitsLeft;
    int xEnd = pSensor.position.X + unitsLeft;

    return new Vec2i(xStart, xEnd);
}

// SO now we know the x range a single sensor has at position pY.
// But we are not done yet, we need to combine all ranges into a list of ranges, merging ranges together where possible:

List<Vec2i> GetSensorRanges(int pY)
{
    List<Vec2i> ranges = new List<Vec2i>();

    for (int i = 0; i < sensorList.Count; i++)
    {
        Vec2i? sensorRange = GetSensorRange(sensorList[i], pY);

        if (sensorRange == null) continue;
        ranges.Add(sensorRange.Value);
    }

    // Optimized version added later...
    return MergeRanges(ranges);

    // Not the fastest algorithm, but fast enough, should try and optimize this to make it faster

    bool condensed = true;
    while (condensed)
    {
        condensed = false;
        for (int i = 0; i < ranges.Count - 1; i++)
        {
            Vec2i rangeA = ranges[i];
            for (int j = i+1; j < ranges.Count;)
            {
                Vec2i rangeB = ranges[j];

                if (rangeA.Y >= rangeB.X && rangeA.X <= rangeB.Y)
                {
                    rangeA.X = int.Min(rangeA.X, rangeB.X);
                    rangeA.Y = int.Max(rangeA.Y, rangeB.Y);
                    ranges[i] = rangeA;
                    ranges.RemoveAt(j);
                    condensed = true;
                    break;
                }
                else
                {
                    j++;
                }
            }
        }
    }

    return ranges;
}

List<Vec2i> MergeRanges(List<Vec2i> ranges)
{
    if (ranges.Count == 0) return new();

    ranges.Sort((a,b) => a.X - b.X);
    
    List<Vec2i> merged = new();

    Vec2i current = ranges[0];

    for (int i = 1; i < ranges.Count; i++)
    {
        Vec2i next = ranges[i];

        // Check for overlap or adjacency (mergeable), we see 1-10 and 11-20 as mergeable
        if (current.Y >= next.X - 1)
        {
            current.Y = int.Max(current.Y, next.Y);
        }
        else
        {
            merged.Add(current);
            current = next;
        }
    }

    merged.Add(current);
    return merged;
}

for (int y = 0; y <= 4000000; y++)
{
    var r = GetSensorRanges(y);
    
    // Theoretically the answer could also be in a 1 range list starting at 1 or ending at 4000000-1
    // but this was more logical (the gap is between 2 ranges)
    // we can have more than 2 ranges since that would mean two spaces left over (otherwise the ranges would have been merged)

    if (r.Count == 2)
    {
        //Since the ranges are already sorted, we can assume this situation, trying to detect the single gap
        if (r[0].Y == r[1].X - 2)
        {
            Vec2i result = new Vec2i(r[0].Y + 1, y);
            long tuningFrequency = result.X * 4000000L + result.Y;
            Console.WriteLine("Part 2:" + tuningFrequency);
            break;
        }
    }
}