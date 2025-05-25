//Solution for https://adventofcode.com/2024/day/14 (Ctrl+Click in VS to follow link)

using System.Text.RegularExpressions;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of robot positions and velocities

string myInput = File.ReadAllText(args[0]);

Regex robotMatcher = new Regex(@"p=(-?\d+),(-?\d+) v=(-?\d+),(-?\d+)", RegexOptions.Multiline);

MatchCollection matches  = robotMatcher.Matches(myInput);

List<Robot> robots = new List<Robot>();

foreach (Match match in matches)
{
	Robot robot = new Robot();
	robot.position = new Vec2i(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
	robot.velocity = new Vec2i(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value));

	robots.Add(robot);
}

// ** Part 1: Simulate all robots in a limited space and calculate how many are in each quadrant, 
// then multiply those values together

// Define the space

int width = 101;
int height = 103;

// Define some helper methods

void Simulate(Robot robot)
{
    Vec2i newPosition = robot.position;
    newPosition += robot.velocity;
    newPosition.X = (newPosition.X + width) % width;
    newPosition.Y = (newPosition.Y + height) % height;
    robot.position = newPosition;
}

// Now simulate all robots a 100 frames

for(int i = 0; i < 100; i++) robots.ForEach(Simulate);

// Now count quadrants, keeping in mind that robots may overlap ...
Dictionary<Vec2i, long> scores = new Dictionary<Vec2i, long>();

foreach (Robot robot in robots)
{
    Vec2i position = robot.position;

	// In the center? Skip it...
    if (position.X == width / 2 || position.Y == height / 2) continue;

	// Take half of the width or height and add 1 so we correctly quantize the position into quadrants
	Vec2i quadrant = new Vec2i(position.X / ((width / 2) + 1), position.Y / ((height / 2) + 1));
    
    scores.TryAdd(quadrant, 0);
    scores[quadrant]++;
}

Console.WriteLine("Part 1:" + scores.Values.Aggregate((x,y) => x * y));

// ** Part 2: What is the fewest number of seconds that must elapse for the robots to display the Easter egg?
// We keep simulating until no robots overlap, hoping that is the egg we are looking for ...

HashSet<Vec2i> positions = new HashSet<Vec2i>();
bool overlappingRobotFound = false;
//where did we leave off?
int iterations = 100; 

while (true)
{
	//Simulate all robots, keep track of any overlapping ones
	positions.Clear();
	overlappingRobotFound = false;
	foreach (Robot robot in robots)
	{
		Simulate(robot);
		overlappingRobotFound = overlappingRobotFound || !positions.Add(robot.position);
    }

	iterations++;

	if (!overlappingRobotFound) {
		Draw();
		Console.WriteLine("Part 2:"+iterations);
		break;
	}
}

void Draw()
{
	for (int y = 0; y < height; y++)
	{
		for (int x = 0; x < width; x++)
		{
			if (positions.Contains(new Vec2i(x, y)))
			{
				Console.Write("#");
			}
			else
			{
				Console.Write(".");
			}
		}
        Console.WriteLine("");
    }
}

