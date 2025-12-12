// Solution for https://adventofcode.com/2025/day/9 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

List<Vec2i> points = myInput
    .Split([Environment.NewLine, ","], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Select(int.Parse)
    .Chunk(2)
    .Select(x => new Vec2i(x[0], x[1]))
    .ToList();

// ** Part 1: Generate all pairs...

long maxRectSize = 0;

for (int i = 0; i < points.Count - 1; i++)
{
    for (int j = i + 1; j < points.Count; j++)
    {
        Vec2i delta = points[j] - points[i];
        long newArea = (long.Abs(delta.X)+1) * (long.Abs(delta.Y)+1);
        if (newArea > maxRectSize) maxRectSize = newArea;
    }
}

Console.WriteLine("Part 1: " + maxRectSize);

//** Part 2:

/* 
// First attempt, works but very slow...
// We can't create a grid, it would be too big
// But we can store all points on the lines of the polygon (which is pretty cool tbh)

HashSet<Vec2i> linePoints = new();

void FillLine (Vec2i pStart, Vec2i pEnd)
{
    Vec2i delta = pEnd - pStart;
    int steps = delta.ManhattanDistance();
    delta = delta.Sign();

    Vec2i current = pStart;

    for (int i = 0; i < steps ; i++)
    {
        linePoints.Add(current);
        current += delta;
    }
}

for (int i = 0; i < points.Count; i++)
{
    FillLine(points[i], points[(i + 1) % points.Count]);
}

// Slow!

bool RectangleHasInsidePoints(Vec2i first, Vec2i second)
{
    int minX = int.Min(first.X, second.X);
    int maxX = int.Max(first.X, second.X);
    int minY = int.Min(first.Y, second.Y);
    int maxY = int.Max(first.Y, second.Y);

    foreach (Vec2i point in linePoints)
    {
        if (point.X > minX && point.X < maxX && point.Y > minY && point.Y < maxY) return true;
    }

    return false;
}

*/

// Can check all rectangles that contain no other points

maxRectSize = 0;

for (int i = 0; i < points.Count; i++)
{
    for (int j = i+1; j < points.Count; j++)
    {
        Vec2i first = points[i];
        Vec2i second = points[j];
        Vec2i delta = second - first;

        long newArea = (long.Abs(delta.X) + 1) * (long.Abs(delta.Y) + 1);

        if (newArea > maxRectSize)
        {
            if (!RectangleHasInsidePointsOptimized(first, second))
            {
                Console.WriteLine("Considering " + maxRectSize);
                maxRectSize = newArea;
            }
        }
    }
}


bool RectangleHasInsidePointsOptimized(Vec2i first, Vec2i second)
{
    int minX = int.Min(first.X, second.X);
    int maxX = int.Max(first.X, second.X);
    int minY = int.Min(first.Y, second.Y);
    int maxY = int.Max(first.Y, second.Y);

    for (int i = 0; i < points.Count; i++)
    {
        Vec2i pointA = points[i];
        Vec2i pointB = points[(i+1) % points.Count];

        int pointMinX = int.Min(pointA.X, pointB.X);
        int pointMaxX = int.Max(pointA.X, pointB.X);
        int pointMinY = int.Min(pointA.Y, pointB.Y);
        int pointMaxY = int.Max(pointA.Y, pointB.Y);

        if (pointMinX < maxX && pointMaxX > minX && pointMinY < maxY && pointMaxY > minY) return true;
    }

    return false;
}

Console.WriteLine("Part 2: " + maxRectSize);

