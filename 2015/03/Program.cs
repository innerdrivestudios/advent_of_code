//Solution for https://adventofcode.com/2015/day/3 (Ctrl+Click in VS to follow link)

//Setup some easy typedefs
using Vec2Int = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of directions for Santa or Robots to follow like ^<><v etc

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: Calculate how many different houses are visited by Santa

// First setup a map of direction chars to direction vectors
Dictionary<char, Vec2Int> directions = new Dictionary<char, Vec2Int>()
{
	{ '>' , new Vec2Int(1, 0)  },
	{ '^' , new Vec2Int(0, 1)  },
	{ '<' , new Vec2Int(-1, 0) },
	{ 'v' , new Vec2Int(0, -1) },
};

// Then setup a hashset of visited houses
HashSet<Vec2Int> distinctHousesVisited = new HashSet<Vec2Int>();

Vec2Int santaLocation = new Vec2Int(0, 0);
distinctHousesVisited.Add(santaLocation);

//get direction vector from the directions map using the direction char,
//add it to santa and add the result to the hashset
foreach (char directionChar in myInput) {
    distinctHousesVisited.Add(santaLocation += directions[directionChar]);
}

Console.WriteLine("Part 1 (Distinct house count for Santa):"+ distinctHousesVisited.Count);

// Part 2: Calculate how many different houses are visited by Santa and the helper robot
// if they alternate following instructions from the list

// Reset what we had
distinctHousesVisited.Clear();
santaLocation = new Vec2Int(0, 0);

// Setup some new variables
Vec2Int roboLocation = new Vec2Int(0, 0);
distinctHousesVisited.Add(santaLocation);
distinctHousesVisited.Add(roboLocation);

// Run through all the steps again but now alternating between Santa and Robosanta

int step = 0;
    
foreach (char directionChar in myInput)
{
    if (step++ % 2 == 0)  distinctHousesVisited.Add(santaLocation += directions[directionChar]);
    else                  distinctHousesVisited.Add(roboLocation  += directions[directionChar]);
}

Console.WriteLine("Part 2 (Distinct house count for Santa+Robot):"+ distinctHousesVisited.Count);