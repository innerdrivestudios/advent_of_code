//Solution for https://adventofcode.com/2024/day/15 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a ware house map with boxes and a robot and a list of directions for the robot to follow

string[] myInput = File.ReadAllText(args[0])
    .ReplaceLineEndings(Environment.NewLine)
    .Split (Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

string wareHouseMap = myInput[0];
string robotInstructions = myInput[1].ReplaceLineEndings("");

// ** Part 1: After the robot is finished moving, what is the sum of all boxes' GPS coordinates?

// Step 1. Initialize the warehouse and store the robot position

Vec2i robotPosition = new Vec2i();
Grid<char> wareHouse = new Grid<char>(wareHouseMap, Environment.NewLine, null, FindRobotPosition);

char FindRobotPosition(Vec2i pPosition, string pContent)
{
    if (pContent == "@") robotPosition = pPosition;
    return pContent[0];
}

// Step 2. Execute all instructions as given...

string directionChars = ">v<^";
Vec2i[] directions = { new Vec2i(1, 0), new Vec2i(0, 1), new Vec2i(-1, 0), new Vec2i(0, -1) };

int instructionIndex = 0;

while (instructionIndex < robotInstructions.Length)
{
    char instruction = robotInstructions[instructionIndex];
    Vec2i direction = directions[directionChars.IndexOf(instruction)];

    if (MovePart1(robotPosition, direction))
    {
        robotPosition += direction;
    }

    instructionIndex++;
}

// Check recursively whether the thing at position can move in the given direction
// What this basically means is that we need to check in the given direction until we 
// encounter a free spot or a wall.
// If we encounter a free spot, everything before that can move in the given direction,
// If we encounter a wall, not.

bool MovePart1(Vec2i pPosition, Vec2i pDirection)
{
    //Base cases:
    if (wareHouse[pPosition] == '#') return false;
    if (wareHouse[pPosition] == '.') return true;

    //So if the current spot we are checking is not a wall or empty space, 
    //we need to check if whatever is there can be moved out of the way...

    Vec2i newPosition = pPosition + pDirection;
    if (MovePart1(newPosition, pDirection))
    {
        UpdateGrid(wareHouse, pPosition, newPosition);
        return true;
    }
    else return false;  
}

void UpdateGrid(Grid<char> pGrid, Vec2i pOldPos, Vec2i pNewPos)
{
    pGrid[pNewPos] = pGrid[pOldPos];
    pGrid[pOldPos] = '.';
}

int gpsResult = 0;

wareHouse.Foreach(
    (pos, value) =>
    {
        if (value == 'O') gpsResult += (pos.X) + 100 * pos.Y;
    }

);

Console.WriteLine("Part 1:" + gpsResult);

// ** Part 2: New warehouse setup! 
// This warehouse's layout is surprisingly similar to the one you just helped.
// There is one key difference: everything except the robot is twice as wide!
// The robot's list of movements doesn't change.

// First reset the old ware house:
wareHouse = new Grid<char>(wareHouseMap, Environment.NewLine, null, FindRobotPosition);

// Initialize the new warehouse:
Grid<char> newWareHouse = new Grid<char> (wareHouse.width * 2, wareHouse.height);

newWareHouse.Foreach(
    (pos, value) =>
    {
        char originalContent = wareHouse[pos.X / 2, pos.Y];
        switch (originalContent)
        {
            case '@': originalContent = '.'; break;
            case 'O': originalContent = (pos.X % 2 == 0) ? '[' : ']'; break;
        }
        newWareHouse[pos] = originalContent;
    }
);

// Now update the robot position:
robotPosition.X *= 2;

// And put the robot back into the warehouse:

newWareHouse[robotPosition] = '@';

// NOW we do exactly the same thing as before, but we need to make a change in our checking...
// Since if, for example, one side of a box can be moved, we can't actually move it yet, 
// UNTIL we know that the whole RIGHT side can also be moved. 
// Which was not how it was set up to now, since in part 1 when we found our child could be moved, 
// we immediately moved ourselves as well. The issue here is not finding out whether both children
// can be moved, but rather whether our sibling call was able to move and complete!

instructionIndex = 0;

while (instructionIndex < robotInstructions.Length)
{
    char instruction = robotInstructions[instructionIndex];
    Vec2i direction = directions[directionChars.IndexOf(instruction)];

    if (CanMovePart2 (robotPosition, direction))
    {
        DoMovePart2 (robotPosition, direction);
        robotPosition += direction;
    }

    instructionIndex++;
}

// First check recursively if EVERYTHING *CAN* move
bool CanMovePart2(Vec2i pPosition, Vec2i pDirection)
{
    char currentContent = newWareHouse[pPosition];

    // Same as previous:
    if (currentContent == '#') return false;
    // Empty space can be 
    if (currentContent == '.') return true;

    // Here we deviate from the previous setup
    // If we are moving things horizontally, or it is the robot we are moving...
    // nothing has changed, except that we ONLY check, but don't move YET
    
    Vec2i newPosition = pPosition + pDirection;

    if (pDirection.X != 0 || currentContent == '@')
    {
        return CanMovePart2 (newPosition, pDirection);
    }
    else if (pDirection.Y != 0)
    {
        // If we are moving horizontally and we are moving a box,
        // we need to check if both the left and right side of the box can be moved
        if (currentContent == '[')
        {
            return CanMovePart2(newPosition, pDirection) && CanMovePart2(newPosition + new Vec2i(1, 0), pDirection);
        }
        else if (currentContent == ']')
        {
            return CanMovePart2(newPosition, pDirection) && CanMovePart2(newPosition + new Vec2i(-1, 0), pDirection);
        }
        else // We should not ever get here
        {
            throw new Exception("Something is wrong...");
        }
    }
    else // We should not ever get here
    {
        throw new Exception("Something is wrong...");
    }
}

// Do the actual moving!
void DoMovePart2(Vec2i pPosition, Vec2i pDirection)
{
    char currentContent = newWareHouse[pPosition];

    if (currentContent == '#') return;
    if (currentContent == '.') return;
    
    Vec2i newPosition = pPosition + pDirection;
    
    if (pDirection.X != 0 || currentContent == '@')
    {
        DoMovePart2(newPosition, pDirection);
        UpdateGrid(newWareHouse, pPosition, newPosition);
    }
    else if (pDirection.Y != 0)
    {
        if (currentContent == '[')
        {
            DoMovePart2 (newPosition, pDirection);
            DoMovePart2 (newPosition + new Vec2i(1, 0), pDirection);
            UpdateGrid (newWareHouse, pPosition, newPosition);
            UpdateGrid (newWareHouse, pPosition + new Vec2i(1, 0), newPosition + new Vec2i(1, 0));
        }
        else if (currentContent == ']')
        {
            DoMovePart2 (newPosition, pDirection);
            DoMovePart2 (newPosition + new Vec2i(-1, 0), pDirection);
            UpdateGrid (newWareHouse, pPosition, newPosition);
            UpdateGrid (newWareHouse, pPosition + new Vec2i(-1, 0), newPosition + new Vec2i(-1, 0));
        }
    }
}

gpsResult = 0;

// Check only [ this time!
newWareHouse.Foreach(
    (pos, value) =>
    {
        if (value == '[') gpsResult += (pos.X) + 100 * pos.Y;
    }

);

Console.WriteLine("Part 2:" + gpsResult);