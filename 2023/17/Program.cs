// Solution for https://adventofcode.com/2023/day/17 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using PathNode = (Vec2<int> position, Vec2<int> direction, int stepsInThisDirection);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a map that uses traffic patterns, ambient temperature,
// and hundreds of other parameters to calculate exactly how much heat loss
// can be expected for a crucible entering any particular city block.

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

Grid<int> lavaField = new Grid<int>(myInput, Environment.NewLine);

// ** Part 1:

PathNode startA = (new Vec2i(0, 0), new Vec2i(1,0), 1);
PathNode startB = (new Vec2i(0, 0), new Vec2i(0,1), 1);
Vec2i end = new Vec2i(lavaField.width-1, lavaField.height-1);

LavaFieldAdapterPart1 lfa = new LavaFieldAdapterPart1(lavaField, [startA, startB], end);
var pathResult = Dijkstra.Search<PathNode>(lfa);

Console.WriteLine("Part 1: " + pathResult?.totalCost);

// ** Part 2:

LavaFieldAdapterPart2 lfa2 = new LavaFieldAdapterPart2(lavaField, [startA, startB], end);
pathResult = Dijkstra.Search<PathNode>(lfa2);
Console.WriteLine("Part 2: " + pathResult?.totalCost);
