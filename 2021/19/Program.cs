//Solution for https://adventofcode.com/2021/day/19 (Ctrl+Click in VS to follow link)

using Vec3i = Vec3<int>;
using AxisSystem = (Vec3<int> xBasisVector, Vec3<int> yBasisVector, Vec3<int> zBasisVector);

// For part 1 we need to transform the coordinates to all different possible axis systems.
// The puzzle explanation is a bit vague but what it basically comes down to is that
// given a global reference frame, e.g. East, Up, and North, we can take an axis system X, Y, Z
// and map each of these axis to the reference frame in either positive or negative direction...
// e.g. X to East, Y to Up, Z to North (also often called forward).
// OR we can map X to -East etc...
//
// Couple of things worth noting, ANY axis can only be mapped to an global reference ONCE.
// E.g. if we map X to East, we cannot map Y to -East since that would be a collapse of our axis system.
//
// That leaves us with 6 choices for the X axis, 4 for the Y axis and 2 for the Z axis.
// BUT... if we would accept/implement/etc all of these possible axis system outcomes, 
// we would get 48 different axis combinations, of which half would be reflections of the other.
// E.g. X=EAST, Y=UP, Z=NORTH would be a mirror of X=-EAST, Y=UP, Z=NORTH.
//
// We are not interested in reflected axis systems, since the puzzle description excluded those.
// Meaning we are left with 24 variations of our XYZ axis system, that are all rotated variations of
// X=EAST, Y=UP, Z=FORWARD
//
// The question of course is, how can we easily define/generate these axis systems and how can we use them
// to "process" the given coordinates to see if they match any other coordinates.
//
// The basic answer to "how can we store these axis systems and process our coordinates" is = use matrices.
// A full explanation is out of scope here, but really quickly:
// 1) An x,y,z coordinate basically means: give me the point x steps along the x axis, y steps along the y axis, etc.
// 2) Those x, y, z axis are normally (1,0,0), (0,1,0), etc but can also be different (orthogonal) axis
// 3) Calculating the end point can then easily be done by calculating (dot (coord, x-axisvector), dot (coord, y-axisvector), dot (coord, z-axisvector))
//      which is basically the same as multiplying the coord with a 3x3 matrix made from the x, y, z axis vector.
//
// In other words, we need to define 24 matrices, or 24 combinations of 3 axis vectors, either by hand or by code.
// Let's do it half, half... first I'll define the possible basis vectors in this scenario:

Vec3i[] basisVectors = [new(-1, 0, 0), new(1, 0, 0), new(0, -1, 0), new(0, 1, 0), new(0, 0, -1), new(0, 0, 1)];

// Now we generate all combinations of our axis systems... 
// To make sure we don't get any reflected systems in there,
// we only pick the first 2 axis and create the 3rd using the cross product,
// which will always result in a right-handed coordinate system.
// (we don't really care whether the coordinate system is left or right handed
// as long as it is either always the one or the other)

List<AxisSystem> possibleOrientations = new List<AxisSystem>();

for (int x = 0; x < basisVectors.Length; x++)
{
    Vec3i xBasicVector = basisVectors[x];

    for (int y = 0; y < basisVectors.Length; y++)
    {
        if (x / 2 == y / 2) continue;
        Vec3i yBasicVector = basisVectors[y];
        Vec3i zBasicVector = Vec3i.Cross(xBasicVector, yBasicVector);

        possibleOrientations.Add(new AxisSystem(xBasicVector, yBasicVector, zBasicVector));
    }
}

// Then we'll also need a method to transform a coordinate from whatever system it is in to another...

Vec3i Transform (Vec3i pInput, AxisSystem pAxisSystem)
{
    return pInput.X * pAxisSystem.xBasisVector + pInput.Y * pAxisSystem.yBasisVector + pInput.Z * pAxisSystem.zBasisVector;
}

// Ok, now it is time to parse all the input and see if we can match the coordinates for scanner 1, 2, 3 etc
// to the coordinates of scanner 0, after optionally transforming them or not...

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

HashSet<Vec3i>[] scannerBlocks = myInput
    //Get the major string blocks, that start with "Scanner .. " etc
    .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    //With each block
    .Select(x => x
                //Get the separate lines...
                .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                //..skip the "Scanner..." heading
                .Skip(1)
                //Split all other lines on , and convert the result to an array of ints...
                .Select(y =>
                    y.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                     .Select(int.Parse)
                     .ToArray()
                )
                //So that we can turn each line into a vector...
                .Select(x => new Vec3i(x[0], x[1], x[2]))
                .ToHashSet()
    )
    .ToArray();


// Input two coordinate sets...
// pScannerBlockA is our reference frame.
// pScannerBlockB is a set OF coordinates that MIGHT match with enough coordinates in pScannerBlockB
// IF we rotate and offset it correctly

bool FindTransformedOverlap (HashSet<Vec3i> pScannerBlockA, HashSet<Vec3i> pScannerBlockB, out Vec3i pSystemDelta, out AxisSystem pTransform)
{
    // To find any overlap, we go over every possible transformation
    foreach (AxisSystem axisSystem in possibleOrientations)
    {
        //Transform system b relative to its own origin
        HashSet<Vec3i> transformedBlockBCoordinates = pScannerBlockB.Select(x => Transform(x, axisSystem)).ToHashSet();

        // Now we need to shift block B to all possible options trying to find a match...
        // What are possible options? 
        // ONE option is to REALLY try to match the scanner cubes over a distance of -2000 to 2000 on each axis,
        // since that is the maximum distance two scanners can be apart...
        // In theory this would work, but it is really REALLY slow...
        // OR...
        // We could say, IF these coordinates overlap, there must be a POINT A in system 1 and a POINT B in system 2,
        // that WHEN aligned, make enough of the other points match...
        // 
        foreach (Vec3i refPointA in pScannerBlockA)
        {
            foreach (Vec3i refPointB in transformedBlockBCoordinates)
            {
                HashSet<Vec3i> offsetBlockACoordinates = pScannerBlockA.Select(x => x - refPointA).ToHashSet();
                HashSet<Vec3i> offsetBlockBCoordinates = transformedBlockBCoordinates.Select(x => x - refPointB).ToHashSet();
                
                int overlappingCount = offsetBlockACoordinates.Count(offsetBlockBCoordinates.Contains);
                if (overlappingCount >= 12)
                {
                    //We found a match!
                    //Now we needed to move system A over refPointA and system B over refPointB distance to make them align,
                    //so B relative to A is A minus B
                    //Console.WriteLine("Difference is " + (refPointA - refPointB));
                    pSystemDelta = refPointA - refPointB;
                    pTransform = axisSystem;    
                    return true;
                }
            }
        }
    }

    pSystemDelta = default;
    pTransform = default;
    return false;
}

// Create a list to hold the scanner positions, where we only know the actual positions for scanner 0 at the moment...
HashSet<Vec3i> beacons = scannerBlocks[0].ToHashSet();

// And a whole list of blocks we still need to merge into this set...
List<HashSet<Vec3i>> scannerBlocksToMerge = scannerBlocks.Skip(1).ToList();

List<Vec3i> scannerPositions = new ();
scannerPositions.Add(new Vec3i(0, 0, 0));

while (scannerBlocksToMerge.Count > 0)
{
    // Go through the blocks and try to merge them into the reference frame of scanner 0...
    for (int i = scannerBlocksToMerge.Count - 1; i >= 0; i--)
    {
        // If successful, take the transformed and move coordinates which are always relative to the whole system now
        if (FindTransformedOverlap(beacons, scannerBlocksToMerge[i], out Vec3i delta, out AxisSystem transform))
        {
            beacons.UnionWith(scannerBlocksToMerge[i].Select(x => Transform(x, transform) + delta));
            scannerPositions.Add(delta);
            scannerBlocksToMerge.RemoveAt(i);
        }
    }
    Console.WriteLine(scannerBlocksToMerge.Count + " scanners left to merge...");
}

Console.WriteLine("Part 1: " + beacons.Count);

// ** Part 2 : Find the largest distance apart:

long maxDistance = 0;

for (int i = 0; i < scannerPositions.Count - 1; i++)
{
    for (int j = 0; j < scannerPositions.Count; j++)
    {
        maxDistance = long.Max (maxDistance, (scannerPositions[j] - scannerPositions[i]).ManhattanDistance());
    }
}

Console.WriteLine("Part 2: " + maxDistance);

