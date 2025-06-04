//Solution for https://adventofcode.com/2021/day/15 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid of cave exploration risk levels

Grid<int> cave = new Grid<int>(
	File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine), 
	Environment.NewLine
);

Vec2i start = new Vec2i(0, 0);
Vec2i end = new Vec2i(cave.width-1, cave.height-1);

// ** Part 1:

var searchResults = Dijkstra.Search(new DijkstraGridAdapterPart1(cave), start, end);
Console.WriteLine("Part 1:" + searchResults.costs[searchResults.path.Last()]);

// ** Part 2:

// Showing two ways to do this:

// 1: Actually just generate the bigger cave...

Grid<int> biggerCave = new Grid<int>(cave.width * 5, cave.height * 5);

biggerCave.Foreach(
	(pos, value) => {
        //to detect if this position is within the grid, we can't use grid.IsInside anymore ...
        //we'll need to define our own new IsInside ...
        int gridWidth = cave.width;
        int gridHeight = cave.height;

        int xBlock = pos.X / gridWidth;
        int yBlock = pos.Y / gridHeight;

        //Values need to be positive, but in the first 5 blocks (block index 0,1,2,3,4)
        if (pos.X >= 0 && pos.Y >= 0 && xBlock < 5 && yBlock < 5)
        {
            //The tricky bit is in the risk level which goes 1,2,3,4,5,6,7,8,9,1,2,3,4,5,6,7,8,9 etc
            //So we need to do some offset math hacks with 1+ and -1 % 9
            int riskLevel = 1 + (cave[pos.X % gridWidth, pos.Y % gridHeight] + xBlock + yBlock - 1) % 9;
            biggerCave[pos] = riskLevel;
        }

    }
);

end = new Vec2i(biggerCave.width - 1, biggerCave.height - 1);
searchResults = Dijkstra.Search(new DijkstraGridAdapterPart2(cave), start, end);
Console.WriteLine("Part 2:" + searchResults.costs[end]);

// Alternatively we don't generate a bigger map, but encode this in our grid adapter:

end = new Vec2i(cave.width * 5 - 1, cave.height * 5 - 1);
searchResults = Dijkstra.Search(new DijkstraGridAdapterPart2(cave), start, end);
Console.WriteLine("Part 2:" + searchResults.costs[end]);
