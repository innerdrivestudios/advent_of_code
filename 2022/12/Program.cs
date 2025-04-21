// Solution for https://adventofcode.com/2022/day/12 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a heightmap

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Vec2i start = new();
Vec2i end = new();

Grid<int> heightmap = new Grid<int>(myInput, Environment.NewLine, null, ConvertInputCharToInt);

int ConvertInputCharToInt (Vec2i pPosition, string pInput)
{
	//each input element is a char a-z,
	//a is the lowest (0), z the highest
	//S = a, E = z
	if (pInput == "S")
	{
		start = pPosition;
		pInput = "a";
	} 
	else if (pInput == "E")
	{
		end = pPosition;
		pInput = "z";
	}

	return pInput[0]-'a';
}

Console.WriteLine("Start:" + start + " End:" + end);

HeightmapGridGraphAdapter heightGraph = new(heightmap);

List<Vec2i> path = SearchUtils.BFSSearch(heightGraph, start, end);
Console.WriteLine("Part 1: "+path.Count);

// ** Part 2: Which a to end gives the shortest path?
// The best way to do this would be to actually inverse the search
// from end to start value and build a flowmap, but it turns out
// brute forcing is quick enough, so I'll leave it at this for now:

// First get all a location's:

List<Vec2i> aLocations = new();
heightmap.Foreach(
	(position, value) =>
	{
		if (value == 0) aLocations.Add(position);
	}
);

int shortestPath = int.MaxValue;

foreach (Vec2i aLocation in aLocations)
{
	path = SearchUtils.BFSSearch(heightGraph, aLocation, end);
	if (path != null && path.Count < shortestPath) shortestPath = path.Count;	
}
	
Console.WriteLine("Part 2: "+shortestPath);
