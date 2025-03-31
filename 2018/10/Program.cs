//Solution for https://adventofcode.com/2018/day/10 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of positions and velocities

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Let's parse the input first...

List<Particle> particles = new List<Particle>();
Regex parser = new Regex(@"position=<\s*(-?\d+),\s*(-?\d+)> velocity=<\s*(-?\d+),\s*(-?\d+)>");

MatchCollection matches = parser.Matches(myInput);
foreach (Match match in matches)
{
	particles.Add(
		new Particle ()
		{
			position = new Vec2i(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
			velocity = new Vec2i(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
		}
	);
}

// ** Part 1: Simulate the particles until some text appears...
// How long is that? My assumption is that it is somewhere around the point
// where the bounding box around the particles is the smallest...

int minX, minY, maxX, maxY = 0;
int minWidth = int.MaxValue;
int minHeight = int.MaxValue;	

bool Simulate (bool pForward = true)
{
	minX = minY = int.MaxValue;
	maxX = maxY = int.MinValue;

	for (int i = 0; i < particles.Count; i++)
	{
		Particle p = particles[i];
		p.Simulate(pForward);

		minX = Math.Min(minX, p.position.X);
		maxX = Math.Max(maxX, p.position.X);

		minY = Math.Min(minY, p.position.Y);
		maxY = Math.Max(maxY, p.position.Y);
	}

	int newWidth = maxX - minX;
	int newHeight = maxY - minY;

	bool boundingBoxSizeDecreased = newWidth < minWidth && newHeight < minHeight;

	minWidth = Math.Min(minWidth, newWidth);
	minHeight = Math.Min(minHeight, newHeight);

	return boundingBoxSizeDecreased;
}

int seconds = 0;
while (Simulate(true)) seconds++;

//Now go one step back
Simulate(false);

Vec2i topLeft = new Vec2i(minX, minY);

Grid<char> display = new Grid<char>(minWidth+1,minHeight+1);
display.Foreach((pos, c) => display[pos] = ' ');

foreach (Particle p in particles)
{
	display[p.position-topLeft] = '#';
}

Console.WriteLine("Part 1 - Characters:");
display.Print();

// ** Part 2:
Console.WriteLine("Part 2 - In " + seconds + " seconds");