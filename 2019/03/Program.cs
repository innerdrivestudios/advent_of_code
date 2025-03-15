// Solution for https://adventofcode.com/2019/day/3 (Ctrl+Click in VS to follow link)
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of move instructions, U)p D)own R)ight L)eft and a distance
// This input consists of two sets of these instructions, separated by \r\n for two different wires

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - Simulate the movement indicated by the instructions above and find
// "What is the Manhattan distance from the central port to the closest intersection?"

// Split all the instructions into separate sets of string[]
string[][] wires = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(x => x.Split(",", StringSplitOptions.RemoveEmptyEntries))
    .ToArray();

// Setup an array of directions, so we can easily move, lookuptable order matches directions order
string directionLookupTable = "RDLU";
Directions<Vec2i> directions = new Directions<Vec2i>([new Vec2i(1,0), new Vec2i(0,1), new Vec2i(-1, 0), new Vec2i(0, -1)]);

// Starting position and direction
Vec2i position = new Vec2i(0, 0);

// Keep track of all wire positions (alternatively we could also use line equations)
// For part 1 a hashset is enough, for part 2 a dictionary is required, so hence the dictionary
Dictionary<Vec2i, int> wire1PositionsToSteps = new();
int stepCount = 0;

foreach (string wire in wires[0])
{
    directions.index = directionLookupTable.IndexOf(wire[0]);
    int moves = int.Parse(wire.Substring(1));

    //instead of doing this in one step, split it into discrete steps
    for (int i = 0; i < moves;i++)
    {
        position += directions.Current();
        //Sneaky, you need to increase this every step
        stepCount++;    
        //But only write it, if it wasn't written before
        if (!wire1PositionsToSteps.ContainsKey(position)) wire1PositionsToSteps[position] = stepCount;
    }
}

//Now loop through all positions again, seeing which are hit by the second set of wires
Dictionary<Vec2i, int> wire2PositionsToSteps = new();

HashSet<Vec2i> intersections = new HashSet<Vec2i>();
position = new Vec2i(0, 0);
stepCount = 0;

foreach (string wire in wires[1])
{
    directions.index = directionLookupTable.IndexOf(wire[0]);
    int moves = int.Parse(wire.Substring(1));

    //instead of doing this in one step, split it into discrete steps
    for (int i = 0; i < moves; i++)
    {
        position += directions.Current();
        //Sneaky, you need to increase this every step
        stepCount++;
        //But only write it, if it wasn't written before
        if (!wire2PositionsToSteps.ContainsKey(position)) wire2PositionsToSteps[position] = stepCount;

        //And lastly record the intersections ...
        if (wire1PositionsToSteps.ContainsKey(position)) intersections.Add(position);
    }
}

Console.WriteLine("Part 1 - The closest intersection is: " + intersections.Min(x => x.ManhattanDistance()));
Console.WriteLine("Part 2 - The closest intersection is: " + intersections.Min(x => wire1PositionsToSteps[x] + wire2PositionsToSteps[x]));
