//Solution for https://adventofcode.com/2018/day/17 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Line = (Vec2<int> start, Vec2<int> end);
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

string[] myInput = File.ReadAllLines(args[0]);

// ** Your input: a list of lines... specified like:
// x=500, y=823..830
// y=1424, x=368..382

// ** Processing the input:

// - Read all the lines, convert it to start(x,y) to end (x,y)
// - Get min max
// - Setup a grid with all these lines...

Regex xLineParser = new Regex(@"x=(\d+), y=(\d+)..(\d+)", RegexOptions.Compiled);
Regex yLineParser = new Regex(@"y=(\d+), x=(\d+)..(\d+)", RegexOptions.Compiled);

List<Line> lines = new List<Line>();

foreach (string line in myInput)
{
	Match match;

	if ((match = xLineParser.Match(line)).Success)
	{

		lines.Add(
			(
				new Vec2i(
					int.Parse(match.Groups[1].Value),
					int.Parse(match.Groups[2].Value)
				),
				new Vec2i(
					int.Parse(match.Groups[1].Value),
					int.Parse(match.Groups[3].Value)
				)
			) 
		);

	}
	else if ((match = yLineParser.Match(line)).Success)
	{
		lines.Add(
			(
				new Vec2i(
					int.Parse(match.Groups[2].Value),
					int.Parse(match.Groups[1].Value)
				),
				new Vec2i(
					int.Parse(match.Groups[3].Value),
					int.Parse(match.Groups[1].Value)
				)
			)
		);
	}
	else throw new Exception("Parse error in line: " + line);
}

int xMin = lines.Min(point => point.start.X);
int yMin = lines.Min(point => point.start.Y);

int xMax = lines.Max(point => point.end.X);
int yMax = lines.Max(point => point.end.Y);

Console.WriteLine("Grid minmax: " + new Vec2i(xMin, yMin) + " to " + new Vec2i(xMax, yMax));

// Gonna move everything a bit to the left so it is easier to see
int border = 10;
Vec2i offset = new Vec2i(-xMin + border, 0);
int width = (xMax - xMin) + 1 + 2 * border;

Grid<char> sandBox = new Grid<char>(width, yMax + 1);
sandBox.Foreach((pos, value) => sandBox[pos] = '.');

foreach (Line line in lines)
{
	PlotLine(line, sandBox, offset);
}

void PlotLine (Line pLine, Grid<char> pSandbox, Vec2i pOffset)
{
	Vec2i delta = (pLine.end - pLine.start);
	int steps = delta.ManhattanDistance();
	Vec2i stepSize = delta.Sign();
	Vec2i start = pLine.start + pOffset;

	for(int i = 0; i <= steps; i++)
	{
		pSandbox[start + stepSize * i] = '#';
	}
}

// ** Part 1: Pour water until nothing changes anymore...

// 2nd try :) -> Model this as a simply DFS that unwinds based on places to go or running out of the screen.

// Visited provides us with the following things:
// - no value -> not visited, if we hit any square already visited while going forward, we skip it
// - 0 go down
// - 1 go left
// - 2 go right
// - 3 done unwind
Dictionary<Vec2i, int> visited = new();

// It feels like this can be optimized or done more elegantly, maybe in the holiday ;)

bool debug = false;

void RunDFS (Vec2i pStart)
{
	Stack<Vec2i> todo = new();
	todo.Push(pStart);
	visited[pStart] = -1;
						   //0		  //1		  //2			//3 is unwind  //4 is unwind and max out new current
	Vec2i[] directions = [ new (0,1), new (-1,0), new (1,0) ];

	while (todo.Count > 0)
	{
		//Get the current stack node, don't pop it yet, since we don't know if we are done or not...
		Vec2i current = todo.Peek();

		//Increase its value (default - 1) to get the direction we are travelling in
		visited[current]++;
		//Not really needed, this is for debugging only
		sandBox[current] = ("" + visited[current])[0];

		int directionIndex = visited[current];

		// If we have tested all directions for this node...
		if (directionIndex > 2)
		{
			//... we are done with this node ...
			todo.Pop();

			// However... as we move back to the previous node we need to test a couple of things... (if there are nodes left...)
			if (todo.Count == 0) continue;

			// If our current direction index > 3 it means our current node was a water stream going down (magic number :))
			if (directionIndex > 3)
			{
				// Check if the new current node is above it, if so, mark that as a water stream as well
				Vec2i delta = current - todo.Peek();
				if (delta.Y != 0) visited[todo.Peek()] = 3;
			}
			else
			{
				// If not, check if the current new node was going down, while the current (last node) was a 3
                if (visited.GetValueOrDefault(todo.Peek(), -1) == 0 && visited.GetValueOrDefault(current, -1) == 3)
                {
					// If that is the case we need to know whether to back up as a water stream,
					// or go left and right again. That depends on whether the current node has left and right boundaries...
					if (!BoundariesFound(current)) visited[todo.Peek()] = 3;
                }
            }
        }
		else
		{
			// If we weren't done testing, get the new direction we want to go
			Vec2i newPos = current + directions[directionIndex];

			// If that is outside of the sandbox, mark the node as outside
			if (!sandBox.IsInside(newPos))
			{
				visited[current] = 3;
			}
			// If it is inside but not visited yet 
			else if (!visited.ContainsKey(newPos))
			{
				// And the cell is empty... move there
				if (sandBox[newPos] != '#')
				{
					todo.Push(newPos);
					visited[newPos] = -1;
				}
			}
			// If the new pos is inside the grid, and visited and completely done, below us AND this the row below us is an overflowing edge, 
			else if (visited[newPos] == 3 && (newPos - current).Y != 0 && !BoundariesFound(newPos))
			{
				// ... mark the current node as a water stream...
				visited[current] = 3;
				// Don't pop it since, the node above us depends on knowing we are a water stream...
            }
        }

		if (debug) { 
			Console.ReadKey();
			Console.Clear();
			sandBox.Print();
		}

    }
}

// Call this method with a start value of 3 (eg the visited map should map pStart to 3)
bool BoundariesFound (Vec2i pStart)
{
    Vec2i scanPoint = pStart;

	//We scan left until our point is no longer a 3... 
    while (visited.GetValueOrDefault(scanPoint, -1) == 3) scanPoint -= new Vec2i(1, 0);
	
	//At that point we should have encountered a boundary (which will not be in the map)
	//So if it IS in the map, it is not 3 and not a boundary
    if (visited.ContainsKey(scanPoint)) return false;
    
    scanPoint = pStart;
    while (visited.GetValueOrDefault(scanPoint, -1) == 3) scanPoint += new Vec2i(1, 0);
    if (visited.ContainsKey(scanPoint)) return false;

	//If we found boundaries in both directions... return true
	return true;

}

Vec2i faucetLocation = new Vec2i(500, 0);
faucetLocation += offset;

debug = false;
RunDFS(faucetLocation);

HashSet<Vec2i> wet = visited.Where(x => x.Key.Y >= yMin && x.Key.Y <= yMax).Select(x => x.Key).ToHashSet();
Console.WriteLine("Part 1: " + wet.Count);

/*
FileStream outFile = new FileStream("d:\\test.txt", FileMode.Create);
TextWriter textWriter = new StreamWriter(outFile);
Console.SetOut(textWriter);

sandBox.Print("");
*/

// ** Part 2: Gonna do this one through elimination.... 
// Our map is full of 3 and 4 values (3 being water in the bucket perhaps overflowing, 4 being streaming water)
// E.g. like this:

// 433333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#333333333####333333#
// 4#333333333#..#333333#
// 4#333333333####333333#
// 4#3333333333333333333#
// 4#3333333333333333333#
// 4#####################
// 
// In other words we can take all 4 values, scan left and right, removing any 3 values we encounter and count the left overs...

HashSet<Vec2i> fourValues = visited.Where(x => x.Value == 4).Select(x => x.Key).ToHashSet();

foreach (Vec2i fourValue in fourValues)
{
    sandBox[fourValue] = '.';
	wet.Remove(fourValue);
	
	Vec2i scanPoint = fourValue - new Vec2i(1, 0);

    while (visited.GetValueOrDefault(scanPoint, -1) == 3)
	{
		wet.Remove(scanPoint);
		sandBox[scanPoint] = '.';
		scanPoint -= new Vec2i(1, 0);
	}

    scanPoint = fourValue + new Vec2i(1, 0);

    while (visited.GetValueOrDefault(scanPoint, -1) == 3)
    {
        wet.Remove(scanPoint);
        sandBox[scanPoint] = '.';
        scanPoint += new Vec2i(1, 0);
    }
}

Console.WriteLine("Part 2: " + wet.Count);

/*
FileStream outFile = new FileStream("d:\\test2.txt", FileMode.Create);
TextWriter textWriter = new StreamWriter(outFile);
Console.SetOut(textWriter);

sandBox.Print("");
textWriter.Flush();
textWriter.Close();
*/

