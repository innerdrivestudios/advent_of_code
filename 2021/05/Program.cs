//Solution for https://adventofcode.com/2021/day/5 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of line definitions like this:
// 88,177-> 566,655
// 346,264-> 872,264
// 409,631-> 506,534
// 300,216-> 300,507
// etc

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings();

List<(Vec2i, Vec2i)> lineSegments = myInput
    .Split (Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)     //Get the separate lines
    .Select (
        x => 
         x.Replace("-> ", ",")                                              //Transform each line into a,b,c,d
         .Split (",", StringSplitOptions.RemoveEmptyEntries)                //Transform into string[]
         .Select (int.Parse)                                                //Transform into int[]
         .ToArray ()
    )
    .Select (x => (new Vec2i(x[0], x[1]), new Vec2i(x[2], x[3])))           //Transform in list of (vec2i, vec2i)
    .ToList ();

// ** Part 1: Considering only the horizontal and vertical lines,
// at how many points do at least two lines overlap?

// Two simple ways to do this:
// - Plot each line into a grid, increasing the count of each grid cell as the line goes through it
// - Do a line vs line collision for each line pair

// Hard to know at this point what will be the best approach considering what part 2 might be
// (e.g. => "include non horizontal/vertical lines as well" or "at least 3 lines overlap"

// Let's just roll with the grid approach for now

// Looking at the input, there are no negative lines and no line has coords higher than 999

Grid<int> grid = new(1000, 1000);

// Now get only horizontal and vertical lines

List<(Vec2i, Vec2i)> straightLines = lineSegments.Where (a => a.Item1.X == a.Item2.X || a.Item1.Y == a.Item2.Y).ToList ();

// Plot each straight line into the grid
foreach (var line in straightLines)
{
    PlotLine(line, grid);
}

// Define PlotLine is a way that works for all lines that take at least 1 grid size steps at a time (straight or 45 degrees).
// This is an assumption, since non 45 degree lines would cause rounding errors, which might make the puzzle non deterministic
void PlotLine((Vec2i start, Vec2i end) pLine,  Grid<int> grid)
{
    //Total delta is distance from a to b (which can be zero)
    Vec2i delta = pLine.end - pLine.start;
    int steps = Math.Max (Math.Abs(delta.X), Math.Abs(delta.Y));

    //(1,0), (0,1), (1,-1) etc ...
    Vec2i deltaPerStep = delta / steps;

    //Zero distance lines, still need to take at least one step...
    for (int i = 0; i <= steps; i++)
    {
        grid[pLine.start + deltaPerStep * i]++;
    }
}

// Now just count every cell > 1
int overlapCount = 0;
grid.Foreach((pos, value) => overlapCount += value > 1 ? 1 : 0);

Console.WriteLine("Part 1 - Overlap count: " + overlapCount);

// ** Part 2 - Exactly the same thing, but now for all lines,
// which now also includes exactly diagonal lines (45 degrees phew, we got lucky ;))

// Reset the grid
grid.Foreach((pos, value) => grid[pos] = 0);

// Plot EACH line into the grid
foreach (var line in lineSegments)
{
    PlotLine(line, grid);
}

overlapCount = 0;
grid.Foreach((pos, value) => overlapCount += value > 1 ? 1 : 0);

Console.WriteLine("Part 2 - Overlap count: " + overlapCount);

// Easy peasy lemon squeezy :)






