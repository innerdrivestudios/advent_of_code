// Solution for https://adventofcode.com/2017/day/22 (Ctrl+Click in VS to follow link)

using System.Runtime.InteropServices;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with cells that are on or off

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> grid = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1: Given your actual map, after 10000 bursts of activity,
// how many bursts cause a node to become infected?
// (Do not count nodes that begin infected.)

// Let's start by converting all cells to a hashmap:
HashSet<Vec2i> cells = new HashSet<Vec2i>();
grid.Foreach((pos, value) => { if (value == '#') cells.Add(pos); });

// Get the center:
Vec2i virusPosition = new Vec2i(grid.width/2, grid.height/2);
Console.WriteLine("Start position:" + virusPosition);

// Implement the described movement behaviour:
Directions<Vec2i> directions = new Directions<Vec2i>([new (-1,0), new(0,-1), new (1,0), new(0,1)]);

// We start upwards
directions.index = 1;

int Burst_Part1()
{
    if (cells.Contains(virusPosition)) {
        directions.index++;
        cells.Remove(virusPosition);
        virusPosition += directions.Current();
        return 0;
    }
    else
    {
        directions.index--;
        cells.Add(virusPosition);
        virusPosition += directions.Current();
        return 1;
    }
}

long infectedNodes = 0;
for (int i = 0; i < 10000; i++)
{
    infectedNodes += Burst_Part1();
}

Console.WriteLine("Part 1:" + infectedNodes);

// ** Part 2: Same idea, different rules:

//We'll keep it simple and store some different states:
//Not present? -> clean
const int CLEAN = 0;
const int WEAKENED = 1;
const int INFECTED = 2;
const int FLAGGED = 3;

Dictionary<Vec2i, int> cellStates = new ();
grid.Foreach((pos, value) => { if (value == '#') cellStates[pos] = INFECTED; });
virusPosition = new Vec2i(grid.width / 2, grid.height / 2);
directions.index = 1;

int Burst_Part2()
{
    int state = CLEAN;
    cellStates.TryGetValue(virusPosition, out state);

    switch (state)
    {
        case CLEAN:
            directions.index--;
            cellStates[virusPosition] = WEAKENED;
            virusPosition += directions.Current();
            return 0;
        case WEAKENED:
            cellStates[virusPosition] = INFECTED;
            virusPosition += directions.Current();
            return 1;
        case INFECTED:
            directions.index++;
            cellStates[virusPosition] = FLAGGED;
            virusPosition += directions.Current();
            return 0;
        case FLAGGED:
            directions.index += 2;
            cellStates.Remove(virusPosition);
            virusPosition += directions.Current();
            return 0;
        
        default: return 0;
    }
}

infectedNodes = 0;
for (int i = 0; i < 10000000; i++)
{
    infectedNodes += Burst_Part2();
}

Console.WriteLine("Part 2:" + infectedNodes);

