// Solution for https://adventofcode.com/2019/day/15 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program lines that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

// This puzzle reuses the existing IntCode computer from day 13,
// which reuses the IntCode computer from day 11,
// which reuses the IntCode computer from day 9,
// which reuses the IntCode computer from day 5,
// which reuses the existing IntCode computer from day 2 :)

// Previous IntCode computers:
// https://adventofcode.com/2019/day/2
// https://adventofcode.com/2019/day/5
// https://adventofcode.com/2019/day/9
// https://adventofcode.com/2019/day/11
// https://adventofcode.com/2019/day/13

//IntCodeComputer droidController = new IntCodeComputer(myInput, new ConsoleIO());

// Let the droid explore its surroundings...
RepairDroid droid = new RepairDroid();
IntCodeComputer droidController = new IntCodeComputer(myInput, droid);
droidController.Run();

// Get all important info from the droid
HashSet<Vec2i> visited = droid.GetVisited();
HashSet<Vec2i> walls = droid.GetWalls();
Vec2i start = droid.GetStart();
Vec2i oxygen = droid.GetOxygen();

// Create a grid with it
int maxX = visited.Max(pos => pos.X);
int maxY = visited.Max(pos => pos.Y);

Grid<char> area = new Grid<char>(maxX+1, maxY+1);
area.Foreach((pos, value) => area[pos] = '#');

foreach (Vec2i pos in visited)
{
    area[pos] = walls.Contains(pos) ? '#' : '.';
}

//** Part 1: Search the grid:
GraphGridAdapter gga = new GraphGridAdapter(area, ['.']);
List<Vec2i> path = SearchUtils.BFSSearch(gga, start, oxygen);
Console.WriteLine("Part 1:" + path.Count);

//area.Print("");

//** Part 2: Get the flow map and print the maximum:

var flowMap = SearchUtils.FlowMap(gga, oxygen);
Console.WriteLine("Part 2:" + flowMap.Values.Max());
Console.WriteLine();