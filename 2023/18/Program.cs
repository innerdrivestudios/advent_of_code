// Solution for https://adventofcode.com/2023/day/18 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Instruction = (char direction, int distance, string hexColor);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of digging directions...

List<Instruction> instructions = File.ReadAllText(args[0]).Trim()
	.ReplaceLineEndings().Split(Environment.NewLine)               //Get all separate lines
    .Select (x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
    .Select (x => new Instruction(x[0][0], int.Parse(x[1]), x[2]))
	.ToList();

Console.WriteLine();

// ** Part 1: For part 1 we'll use a similar approach as for 2022-18:
// - Plot all points as directed
// - Create a "bounding box" around the points and flood fill the bounding box
// - Count every element that is not flood filled in the bounding box

Dictionary<char, Vec2i> directionMap = new ()
{
    {'U', new (0,1) }, {'D', new (0,-1)  }, {'L', new (-1,0) }, {'R', new (1,0)  }, 
};

char[] directionChars = ['R', 'D', 'L', 'U'];

{   // Scoped to avoid conflicts in part 2

    Vec2i currentPoint = new Vec2i(0, 0);
    Vec2i minPoint = new Vec2i(int.MaxValue, int.MaxValue);
    Vec2i maxPoint = new Vec2i(int.MinValue, int.MinValue);

    HashSet<Vec2i> plottedCoords = new() { currentPoint };

    void PlotCoords(Instruction pInstruction)
    {
        for (int i = 0; i < pInstruction.distance; i++)
        {
            currentPoint += directionMap[pInstruction.direction];

            minPoint.Min(currentPoint);
            maxPoint.Max(currentPoint);

            plottedCoords.Add(currentPoint);
        }
    }

    // Plot all points
    foreach (Instruction instruction in instructions) PlotCoords(instruction);

    // Adjust min max
    minPoint -= new Vec2i(1, 1);
    maxPoint += new Vec2i(1, 1);

    // Get basic surface area (note that ranges are inclusive, hence the +1,1)
    Vec2i delta = (maxPoint - minPoint) + new Vec2i(1, 1);
    int surfaceArea = delta.X * delta.Y;

    // Run the floodfill...

    long GetVisitedCount()
    {

        Queue<Vec2i> queue = new Queue<Vec2i>();
        queue.Enqueue(minPoint);
        HashSet<Vec2i> visited = new(surfaceArea) { minPoint };
        Vec2i[] directions = directionMap.Values.ToArray();


        while (queue.Count > 0)
        {
            Vec2i current = queue.Dequeue();

            foreach (Vec2i direction in directions)
            {
                Vec2i newPoint = current + direction;


                if (
                    newPoint.X < minPoint.X || newPoint.X > maxPoint.X ||
                    newPoint.Y < minPoint.Y || newPoint.Y > maxPoint.Y
                    ) continue;

                if (visited.Contains(newPoint)) continue;
                if (plottedCoords.Contains(newPoint)) continue;

                queue.Enqueue(newPoint);
                visited.Add(newPoint);
            }
        }

        return visited.Count;
    }

    Console.WriteLine("Part 1: " + (surfaceArea - GetVisitedCount()));

}

// Part 2: For part 2, apparently, the initial instructions were wrong, YET AGAIN.
// And our approach for part 1, WHOEfully inadequate, YET AGAIN :).

// I struggled long and hard with this, trying to deal with all edge cases, boundaries etc,
// realizing I had to use a basic approach of calculating the area of a polygon by adding
// areas as lines went right and subtracting areas as lines went left, but this went wrong
// for several reasons:
// 1) we don't know the winding of our polygon
// 2) polygon lines are 1 unit thick, and a delta of x can depending on whether we 
//    have a left-right turn, left-left turn or right-right turn be x, x+1 or x-1
//    A problem which I could not easily solve without a ton of hacking and difficulties.
//
// Reading up on the interwebs I ran into the solution (which I wasn't able to come up with myself)
// using a combination of the shoelace formula and pick's theorem:

// First get all points...

List<Vec2i> points =  new List<Vec2i>();
Vec2i current = new Vec2i(0, 0);

foreach(Instruction i in instructions)
{
    Instruction instruction = i;

    bool useAlternateInterpretation = true;

    if (useAlternateInterpretation) 
    { 
		string distanceString = instruction.hexColor.Substring(2, 5);
		instruction.distance = Convert.ToInt32(distanceString,16);
		instruction.direction = directionChars[instruction.hexColor[7] - '0'];

		//Console.WriteLine(instruction.direction + instruction.distance);
	}

	Vec2i direction = directionMap[instruction.direction];
	Vec2i newPoint = current + direction * instruction.distance;
	points.Add(newPoint);
	current = newPoint;
}

// Now apply the formula's

long twiceArea = 0;
long boundary = 0;

for (int i = 0; i < points.Count; i++)
{
	var a = points[i];
	var b = points[(i + 1)%points.Count];

	twiceArea += (long)a.X * b.Y - (long)b.X * a.Y;
	boundary += Math.Abs((long)b.X - a.X) + Math.Abs((long)b.Y - a.Y);
}

twiceArea = Math.Abs(twiceArea);

long result = (twiceArea + boundary) / 2 + 1;
Console.WriteLine("Part 2: " + result);

// Magic ... and to be continued...










