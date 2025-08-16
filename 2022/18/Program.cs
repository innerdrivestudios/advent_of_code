// Solution for https://adventofcode.com/2022/day/18 (Ctrl+Click in VS to follow link)

using Vec3i = Vec3<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of 3d coordinates...

var coordinates = File.ReadAllText(args[0]).Trim()
    .ReplaceLineEndings().Split (Environment.NewLine)               //Get all separate lines
    .Select (
        x => x
            .Split (",", StringSplitOptions.RemoveEmptyEntries)
            .Select (int.Parse).ToArray()
    )                                                               //Convert each line into an int[]
    .Select (x => new Vec3i(x[0], x[1], x[2]))                    //Convert each int[] into a Vec3
    .ToList ();

// ** Part 1: Calculate the surface area that is exposed...

// For this we'll simply move all cubes into a Dictionary mapping their position to a count of 
// 6 sides. After that we'll loop over all cubes subtracting 1 for each of their neighbours.
// The requested answer is the total that is left after this process.

Dictionary<Vec3i, int> cubes = coordinates.ToDictionary(x => x, x => 6);

Vec3i[] directions = [ new (1,0,0), new (-1,0,0), new (0,1,0), new (0,-1,0), new (0,0,1), new (0,0,-1)];

void ProcessCubePart1 (Vec3i pCube)
{
    int startValue = cubes[pCube];
    
    foreach (Vec3i direction in directions)
    {
        if (cubes.ContainsKey(pCube + direction))
        {
            startValue--;
        }
    }

    cubes[pCube] = startValue;
}

cubes.Keys.ToList ().ForEach (x => ProcessCubePart1 (x));

Console.WriteLine("Part 1:" + cubes.Sum (x => x.Value));

// ** Part 2: Whooops, we also included interior surfaces, but they should be deducted as well.
// In other words, for all cubes, we need to find how many interior spaces they are bordering,
// and deduct those from the total surface area as well...

// How we'll do this is by:
// - creating a 3d dimensional cube. one bigger on each side then the 
// min max coords of our little cubes (an encompassing space basically).
// - running a flood fill on this 3d space starting at the top left corner of the space,
//   filling all empty space encountered
// - removing all known cubes from whatever is left by the flood fill
// 
// Whatever is left is our internal surface.

Vec3i min = new Vec3i (int.MaxValue, int.MaxValue, int.MaxValue);
Vec3i max = new Vec3i (int.MinValue, int.MinValue, int.MinValue);

foreach (Vec3i cube in coordinates)
{
    min.X = int.Min(min.X, cube.X);
    min.Y = int.Min(min.Y, cube.Y);
    min.Z = int.Min(min.Z, cube.Z);
    max.X = int.Max(max.X, cube.X);
    max.Y = int.Max(max.Y, cube.Y);
    max.Z = int.Max(max.Z, cube.Z);
}

// Offset both min and max to create an encompassing 3D space...
min += new Vec3i(-1, -1, -1);
max += new Vec3i(1, 1, 1);

// Start with adding all the cubes we already have...
HashSet<Vec3i> cubicSpace  = coordinates.ToHashSet();

// Then run a flood fill starting at min,
// filling up everything outside the given coordinates in the cubic space...

Queue<Vec3i> queue = new();
queue.Enqueue(min);
cubicSpace.Add(min);

while (queue.Count > 0)
{
    Vec3i current = queue.Dequeue();

    foreach (Vec3i direction in directions)
    {
        Vec3i neighbor = current + direction;

        if (cubicSpace.Contains(neighbor)) continue;
        
        if (neighbor.X < min.X || neighbor.Y < min.Y || neighbor.Z < min.Z) continue;
        if (neighbor.X > max.X || neighbor.Y > max.Y || neighbor.Z > max.Z) continue;

        queue.Enqueue(neighbor);
        cubicSpace.Add(neighbor);
    }
}

// Now we process all cubes again, removing any sides that are NOT in our cubic space 
// since any cube not in cubic space denotes the inside surface area

void ProcessCubePart2(Vec3i pCube)
{
	int startValue = cubes[pCube];

	foreach (Vec3i direction in directions)
	{
		if (!cubicSpace.Contains(pCube + direction))
		{
			startValue--;
		}
	}

	cubes[pCube] = startValue;
}

cubes.Keys.ToList().ForEach(x => ProcessCubePart2(x));

Console.WriteLine("Part 2:" + cubes.Sum(x => x.Value));
