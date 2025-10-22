//Solution for https://adventofcode.com/2018/day/22 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Vec3i = Vec3<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: some values that we need to calculate tile types down below...

// ** Step 1: Parse the input 

string[] myInput = File.ReadAllLines(args[0]);

long depth = 0;
Vec2i target = new();

foreach (var line in myInput)
{
    string input = line.Trim();
    if (input.StartsWith("depth: ")) depth = long.Parse(input.Replace("depth: ", ""));
    else if (input.StartsWith("target: "))
    {
        string[] coords = input.Replace("target: ", "").Split(',', StringSplitOptions.TrimEntries);
        target = new Vec2i(int.Parse(coords[0]), int.Parse(coords[1]));
    }
}

// ** Part 1: Calculate geo indices, erosion levels and type

int[,] erosionLevels = new int[target.X+1, target.Y+1];

for (int y  = 0; y <= target.Y; y++)
{
    for (int x = 0; x <= target.X; x++)
    {
        long value = 0;

        if ((x == 0 && y == 0) || (x == target.X && y == target.Y)) value = 0;
        else if (y == 0) value = x * 16807 ;
        else if (x == 0) value = y * 48271;
        else value = erosionLevels[x-1,y] * erosionLevels[x,y-1];

        erosionLevels[x, y] = (int)((value + depth) % 20183);
    }
}

int totalRisk = 0;

for (int y = 0; y <= target.Y; y++)
{
    for (int x = 0; x <= target.X; x++)
    {
        int modErosion = erosionLevels[x, y] % 3;

        totalRisk += modErosion;
    }
}

Console.WriteLine("Part 1: " + totalRisk);

// ** Part 2: Given the puzzle description what is the fewest number of minutes required to reach the target?

// First of all, we'll need to change the way we store stuff since we don't know where we might walk...

// So instead of a grid, we'll use an erosion level map
Dictionary<Vec2i, int> erosionLevelMap = new();

// With a helper method to actually get the erosion type:

int GetErosionLevel (Vec2i pPosition)
{
    //Either the value we are looking for is already in the map or we need to calculate it...
    if (!erosionLevelMap.ContainsKey(pPosition))
    {
        erosionLevelMap[pPosition] = CalculateErosionLevel(pPosition);
    }

    return erosionLevelMap[pPosition];
}

int CalculateErosionLevel(Vec2i pPosition)
{
    long value = 0;

    int x = pPosition.X;
    int y = pPosition.Y;

    if ((x == 0 && y == 0) || (x == target.X && y == target.Y)) value = 0;
    else if (y == 0) value = x * 16807;
    else if (x == 0) value = y * 48271;
    else value = GetErosionLevel(new Vec2i(x - 1, y)) * GetErosionLevel(new Vec2i(x, y - 1));

    int regionType = (int)((value + depth) % 20183);
    return regionType;
}

// Now we need to complete the search...

// We have 3 region types and matching gear (0 no gear, 1 climbing gear, 2 torch gear),
// Note that: climbing and torch gear is not allowed.
// 0 -> rocky       --> 1 or 2
// 1 -> wet         --> 0 or 1
// 2 -> narrow      --> 0 or 2

// I'll encode this in a list:
List<List<int>> validGearPerRegionType =
    [
        [1,2],
        [0,1],
        [0,2]
    ];

// Starting state is (0,0) with 2 equipped.
// Moving takes 1 minute, switching gear, doesn't matter which switch takes 7 minutes
// To store both position and gear, I'll use a Vec3i

Vec3i start = new Vec3i(0, 0, 2);
PriorityQueue<Vec3i, int> queue = new();
Dictionary<Vec3i, int> costs = new ();
queue.Enqueue(start, 0);
costs[start] = 0;

Vec3i[] directions = [ new (-1,0,0), new (1,0,0), new (0,-1,0), new (0,1,0) ];

while (queue.Count > 0)
{
    Vec3i current = queue.Dequeue();
    int cost = costs[current];

    if (current.X == target.X && current.Y == target.Y)
    {
        //if we are not wearing the torch once we are at the target, equip it 
        //if (current.Z != 2) cost += 7;
        Console.WriteLine("Part 2: " + cost);
        break;
    }

    foreach (Vec3i direction in directions)
    {
        Vec3i next = current + direction;
        if (next.X < 0 || next.Y < 0) continue;

        // If the position is valid... let's see what gear we need to get there...

        List<int> currentlyValidGear = validGearPerRegionType[GetErosionLevel(new Vec2i(current.X, current.Y)) % 3];
        List<int> nextValidGear = validGearPerRegionType[GetErosionLevel(new Vec2i(next.X, next.Y)) % 3];

        for (int nextGearTypeIndex = 0; nextGearTypeIndex < nextValidGear.Count; nextGearTypeIndex++)
        {
            int nextGearType = nextValidGear[nextGearTypeIndex];

            //We need to be able to traverse our current region and the next region
            if (!currentlyValidGear.Contains(nextGearType)) continue;

            if (next.X == target.X && next.Y == target.Y && nextGearType != 2) continue;
            
            next.Z = nextGearType;

            int newCost = cost + ((current.Z == next.Z) ? 1 : 8);

            // Coming in from different directions might alter the total cost
            // So only if we find a same or more expensive path, we skip it
            if (costs.ContainsKey(next) && newCost >= costs[next])
            {
                continue;
            }

            queue.Enqueue(next, newCost);
            costs[next] = newCost;
        }
    }
}
