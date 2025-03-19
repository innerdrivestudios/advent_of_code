//Solution for https://adventofcode.com/2024/day/6 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a map where a guard patrols ....

// Load the file
string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Set up the direction string and matching directions array...
string directionString = "^>v<";
Directions<Vec2i> directions = new Directions<Vec2i>(
    [
        new Vec2i(0,-1), new Vec2i(1,0), new Vec2i(0,1), new Vec2i(-1,0)
    ]
);

// Set up the grid
Vec2i startPosition = new Vec2i();
Grid<char> area = new Grid<char> (myInput, Environment.NewLine, null, ProcessGridCells);
int startDirection = 0;

// Process each grid element to either return the contents OR if the grid cell is actually a 
// representation of a guard position and its direction, to replace it with a . (but store the pos/dir)
char ProcessGridCells (Vec2i pPosition, string pInput)
{
    if (directionString.Contains(pInput)) {
        startPosition = pPosition;
        startDirection = directionString.IndexOf(pInput);
        return '.';
    }
    else
    {
        return pInput[0];
    }
}

//area.Print();

// ** Part 1: Get the number of distinct positions we enter when moving towards the exit

int GetDistinctMoveCountToExit()
{
    Vec2i currentPosition = startPosition;
    directions.index = startDirection;

    //Create a visited list of positions
    HashSet<Vec2i> positions = new HashSet<Vec2i>();
    positions.Add(currentPosition);

    //Add keep a hashset to detect cycles (part 2)
    HashSet<(Vec2i, int)> cycleDetection = new HashSet<(Vec2i, int)>();
    cycleDetection.Add((currentPosition, directions.index));

    while (true)
    {
        Vec2i newPosition = currentPosition + directions.Current();
        if (!area.IsInside(newPosition)) { break; }

        if (area[newPosition] == '#')
        {
            directions.Next();
        }
        else
        {
            currentPosition = newPosition;
            positions.Add(newPosition);

            //If we are running in circles return -1
            if (!cycleDetection.Add((currentPosition, directions.index)))
            {
                return -1;
            }
        }
    }

    //If we can escape, return the amount of distinct positions
    return positions.Count;
}

Console.WriteLine("Part 1 Step count to exit: " + GetDistinctMoveCountToExit());

// ** Part 2 : Detect on how many positions we can put an obstacle so that the guard keeps running in cycles

int CountCycleInducingObstacles()
{
    int index = 0;
    int cycleInducingObstacleCount = 0;

    while (index < area.width * area.height)
    {
        Vec2i positionToTest = new Vec2i(index % area.width, index / area.width);
        index++;

        if (positionToTest == startPosition || area[positionToTest] == '#') continue;

        //Set it
        area[positionToTest] = '#';
        //Test it
        if (GetDistinctMoveCountToExit() < 0) cycleInducingObstacleCount++;
        //Undo it
        area[positionToTest] = '.';
    }

    return cycleInducingObstacleCount;
}

Console.WriteLine("Part 2 Cycle inducing obstacle count: " + CountCycleInducingObstacles());