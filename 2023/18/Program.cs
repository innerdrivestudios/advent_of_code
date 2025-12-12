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

Vec2i currentPoint = new Vec2i(0,0);
Vec2i minPoint = new Vec2i (int.MaxValue, int.MaxValue);
Vec2i maxPoint = new Vec2i (int.MinValue, int.MinValue);

HashSet<Vec2i> plottedCoords = new () { currentPoint };
Dictionary<char, Vec2i> directionMap = new ()
{
    {'U', new (0,-1) }, {'D', new (0,1)  }, {'L', new (-1,0) }, {'R', new (1,0)  }, 
};

char[] directionChars = ['R', 'D', 'L', 'U'];

void PlotCoords (Instruction pInstruction)
{
//    string distanceString = pInstruction.hexColor.Substring(2, 5);
//    char direction = directionChars[pInstruction.hexColor[7]-'0'];
//    pInstruction.distance = Convert.ToInt32(distanceString,16);
//    pInstruction.direction = direction;

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
Vec2i delta = (maxPoint - minPoint) + new Vec2i(1,1);
int surfaceArea = delta.X * delta.Y;

// Run the floodfill...

Queue<Vec2i> queue = new Queue<Vec2i>();
queue.Enqueue(minPoint);
HashSet<Vec2i> visited = new (surfaceArea) { minPoint };
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

Console.WriteLine("Part 1:" + (surfaceArea - visited.Count));

// Part 2: For part 2, apparently, the initial instructions were wrong, YET AGAIN.
// And our approach for part 1, WHOEfully inadequate, YET AGAIN :).

// So, we'll need a different approach. Instead of the initial bounding box floodfill approach,
// we'll use a modified shoelace algorithm. The basic idea is that we can view the whole
// trench that is being drawn as a polygon for which we can calculate the area:
//
//  y
//  |    .______.
//  |    |      |
//  |    |_.    |___.
//  |      |        |
//  |      |    ____|
//  |      |____|
//  |
//  |
//  |___________________ x
//
// If we need to calculate the area for this we can start at any point
// and any time we move to the right we ADD the line width * the line height
// and any time we move to the left we SUBTRACT the line width * the line height
//
// E.g. starting in the top left for the first line:
//
//  y    v
//  |  > *______. <
//  |    |......|
//  |    |_.....|___.
//  |    ..|.....   |
//  |    ..|....____|
//  |    ..|____|
//  |    ........
//  |    ........
//  |___________________ x
//
// We start at *, see the line is 8 wide and at y 8 (or 7 it is a little hard to see this way :)),
// so we add 64. This is basically the area of the square formed by that line and its sides
// dropped all the way to Y = 0.
//
// Now of course we are adding/calculating way too much, since the polygon doesn't
// reach all the way to the 'bottom', but bear with me for a second.
//
// Ignoring this significant detail for now, we move on to the next bit indicated by ^:
//
//  y    
//  |    .______.
//  |    |      |
//  |    |_    >*___.<
//  |      |    ^...|
//  |      |    ____|
//  |      |____|....
//  |           .....
//  |           .....
//  |___________________ x
//
// This next bit is 5 wide and 6 high, so we add 30, again way too much...
// But now the interesting bit starts, when we move left at the next corner:
// (Note that we are ignoring all UPS and DOWNS.)
//
//  y    
//  |    .______.
//  |    |      |
//  |    |_     .___.
//  |      |        |
//  |      |    ____* <
//  |      |____|....
//  |           .....
//  |           .....
//  |___________________ x
//
// Since we are moving left with a width of 5 and height of 4, we subtract 20.
//
// Similar for the next corner:
//
//  y    
//  |    .______.
//  |    |      |
//  |    |_     .___.
//  |      |        |
//  |      |    ____.  
//  |      |____*<   
//  |      ......    
//  |      ......    
//  |___________________ x
//
// And the next:
//
//  y    
//  |    .______.
//  |    |      |
//  |    |_*    .___.
//  |    ..|        |
//  |    ..|    ____.  
//  |    ..|____|    
//  |    ...         
//  |    ...         
//  |___________________ x
//
// Applying this principle to the actually given test data:
//
// #######
// #######
// #######
// ..#####
// ..#####
// #######
// #####..
// #######
// .######
// .######
//
// We see the field is 7 x 10.
// Following the same principle we get:
// 7*10 - 2*4 + 2*3 - 6*0 - 1*2 + 2*5 - 2*7 = 
// 70 - 8 + 6 - 0 - 2 + 10 - 14 = 62
//
// NICE!

// Ok, time to put this into practice and see what other issues we'll encounter.
// One issue we can see already is that even though the instructions say things like:
// R 6 and D 5, these 6 and 5 etc do not match the actual length of the lines
// (e.g. the first R6 results in a line that is 7 long, but this difference doesn't always
// happen, it depends on whether the instructions are causing an inner turn or outer turn.
//
// In other words, seeing the cubes have size 1,1(,1) we need to know the coordinates of
// the OUTER lines...
//
// How do we do that?
//
// (No clue so far:))
//
// Ok, after some thought I have some ideas.
// And I also saw online that my ideas are not the best ones, but anyway, 
// I'll follow my own ideas first and then see if the online suggestion is better or not!

// My own idea:
// * I'll first calculate the polygon as provided by the instructions given.
// * Then, realizing we are dealing with 1x1 cubes and our current polygon is based
//   on the CENTER of those cubes, I'll calculate the OUTER polygon around those cubes
// * Using the OUTER polygon I'll be ready to perform the process above

// Step 1. Calculate the polygon based on the instructions


