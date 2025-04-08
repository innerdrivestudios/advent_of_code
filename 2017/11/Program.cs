//Solution for https://adventofcode.com/2017/day/11 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of steps on a hex based grid

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings("");
string[] steps = myInput.Split(",").ToArray();

// ** Part 1: Determine the fewest number of steps required to reach the position indicated by all the instructions...

// To solve this problem I interpreted the hexagon grid as a skewed axis system (see picture in this folder).
// This way we can map all directions:

Dictionary<string, Vec2i> directionMap = new()
{
    {"n",   new Vec2i(0,1) },
    {"ne",  new Vec2i(1,1) },
    {"se",  new Vec2i(1,0) },
    {"s",   new Vec2i(0,-1) },
    {"sw",  new Vec2i(-1,-1) },
    {"nw",  new Vec2i(-1,0) }
};

Vec2i position = new Vec2i();
int maxSteps = 0;               
foreach (var step in steps)
{
    position += directionMap[step];
    maxSteps = int.Max(maxSteps, int.Max(int.Abs(position.X), int.Abs(position.Y)));
}

// When we got the position we are not done, since due to the way the grid is constructed, e.g. 5,3 can be reached in 5 steps
// In other words, the max of x and y is what we are looking for:

Console.WriteLine("Part 1:"+int.Max(int.Abs(position.X), int.Abs(position.Y)));
Console.WriteLine("Part 2:"+maxSteps);