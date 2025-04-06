// Solution for https://adventofcode.com/2023/day/10 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid of pipes!

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Step 1 as usual is parsing the input into a usable format.
// Since we'll need to model grid tile access points, 
// the easiest way to do that is by using bit flags:
// Note: the byte data type would be enough in theory,
// but apparently the | & ^ ops are int operators, 
// which would result in a ton of annoying cast operations...

int NORTH = 1;
int EAST = 2;
int SOUTH = 4;
int WEST = 8;

// This way we can easily model all of our pieces in the grid:

Dictionary<char, int> pieceMap = new()
{
    { '|', NORTH | SOUTH }, //  | is a vertical pipe connecting north and south.
    { '-', EAST  | WEST },  //  - is a horizontal pipe connecting east and west.
    { 'L', NORTH | EAST },  //  L is a 90-degree bend connecting north and east.
    { 'J', NORTH | WEST },  //  J is a 90-degree bend connecting north and west.
    { '7', SOUTH | WEST },  //  7 is a 90-degree bend connecting south and west.
    { 'F', SOUTH | EAST },  //  F is a 90-degree bend connecting south and east.
    { '.', 0 },             //  . is ground; there is no pipe in this tile.
    { 'S', -1 },            //  S is the starting position of the animal -> special case
};

// And using those pieces, parse the grid of pipes:

Vec2i startPosition = new Vec2i();
Grid<int> pipeGrid = new Grid<int>(myInput, Environment.NewLine, null, ConversionHandler);

int ConversionHandler (Vec2<int> pPosition, string pValue)
{
    if (pValue == "S") startPosition = pPosition;
    //Map the first char of the grid value to its int flag
    return pieceMap[pValue[0]];
}

Console.WriteLine(startPosition);

// Now to actually iterate over the grid pieces in the correct directions, we'll need some more info, such as:
// 1. What kind of block the starting position holds
// 2. How we can follow a direction from the starting block all the way around until we are at the starting block again
//
// For this I'm going to define some more data such as a direction map

Dictionary<int, (Vec2i direction, int opposite)> directionMap = new()
{
    {  NORTH, (new (0,-1), SOUTH) },
    {  SOUTH, (new (0,1), NORTH) },
    {  EAST, (new (1,0), WEST) },
    {  WEST, (new (-1,0), EAST) }
};

// To determine our start piece, we'll iterate over every direction, look up the value there
// and if its opposite value is set, our start position will need to match that...
// For example, if the grid cell NORTH of the start position has the value SOUTH,
// then start will need to have the value NORTH.
// If we are lucky, after completing this process, start will have only 2 bits sets...

int startValue = 0;
foreach (var direction in directionMap)
{
    if (!pipeGrid.IsInside(startPosition + direction.Value.direction)) continue;

    int cellContents = pipeGrid[startPosition + direction.Value.direction];

    if ((cellContents & direction.Value.opposite) > 0)
    {
        startValue |= direction.Key;
    }
}

Console.WriteLine("Start value: " + startValue);

// We'll verify the amount of bits as well

int GetBitCount (int pValue)
{
    int bitCount = 0;
    while (pValue>0)
    {
        if ((pValue & 1) > 0) bitCount++;
        pValue >>= 1;
    }
    return bitCount;
}

Console.WriteLine("Start value valid? " + (GetBitCount(startValue) == 2));
if (GetBitCount(startValue) != 2)
{
    Console.WriteLine("Cannot determine starting piece, exiting");
    return;
}

// Now that we have establised the starting piece and we know it is valid,
// we select any of its directions as a starting direction...

pipeGrid[startPosition] = startValue;

int currentDirection = 0;
foreach (var direction in directionMap)
{
    if ((startValue & direction.Key) > 0)
    {
        currentDirection = direction.Key; break;
    }
}

Console.WriteLine("Starting direction: " + currentDirection);

// ** Part 1: Find how many steps we need to take to get the furthest away from the starting point.

// Now that we know the start position, its value and one of its directions, we can start looping ...
// And we loop until our next step is the startPosition again...

Vec2i currentPosition = startPosition;
int stepsTaken = 0;

// We'll need this for part 2...
HashSet<Vec2i> visited = new() {  currentPosition };

do
{
    //How do we step?
    //We first take our current direction to deduct the step direction:
    Vec2i stepDirection = directionMap[currentDirection].direction;
    currentPosition += stepDirection;
    visited.Add(currentPosition);

    //Now in the next position, we need to decide where to go...
    //But first: a little sanity check: every tile can only have two directions:
    int cellValue = pipeGrid[currentPosition];
    if (GetBitCount(cellValue) != 2) throw new Exception("Invalid tile encountered");

    //Now we have the cell value and we know where we came from (e.g. WEST)
    //In other words cellValue has to contain EAST.
    //So if we clear EAST from the cellValue we are left with the next direction 
    //since every cell value describes two directions:
    currentDirection = cellValue ^ directionMap[currentDirection].opposite;
    stepsTaken++;
} 
while (currentPosition != startPosition);

Console.WriteLine("Part 1: Steps taken = " + stepsTaken + " (half equals "+(stepsTaken/2)+")");

// ** Part 2: Find the enclosed grid cell count.

// The "easiest" way to find out if any grid cell is inside or outside is to
// look at a cell and first check if it is part of the boundary, if so, it is not inside.
// If it is not part of the boundary we step from the grid cell to any edge, 
// (in this case I've chosen north but any direction would do) and count how many
// boundaries we cross. If the amount of boundaries is even the cell is outside of 
// the loop, if the amount of boundaries is odd, we are inside.
//
// How we count the boundaries is pretty tricky though!:

bool IsPointInsideLoop (Vec2i pTestPosition)
{
    //If the start is part of the pipe, we are not inside the area enclosed by the pipe
    if (visited.Contains(pTestPosition)) return false;

    //Now start counting boundaries
    int boundaryCount = 0;

    //While the cell we are testing is inside the grid, we check...
    while (pipeGrid.IsInside (pTestPosition))
    {
        //if we hit part of a pipe/boundary...
        if (visited.Contains(pTestPosition))
        {
			//however what kind of boundaries and did we cross it?
			//we only cross a boundary if we are moving up and the boundary goes from
			//left to right. However, part of the boundary might also be vertical,
			//compare these cases:
			
            //   1      2       3
			//   ┌      ┌   
			//   │      │       ─
			//   ┘      └

            //Case 3 is clear we cross the boundary.
            //Case 1 is also clear, we cross the boundary
            //Case 2 is tricky, we walk the boundary but don't cross it
            //Also case 1 and 2 are tricky in itself because we need to follow the pipe upwards
            //as long as it goes and treat all pieces together as one boundary:

            //We gather all bit flag in this value, if we find both a west and east exit
            //we've cross a boundary
			int bitFlags = 0;

            do
            {
                bitFlags |= pipeGrid[pTestPosition];
                pTestPosition += directionMap[NORTH].direction;
            }
            while (
                pipeGrid.IsInside(pTestPosition) &&         //we are still on the field
                visited.Contains(pTestPosition) &&          //this position is a pipe
                (pipeGrid[pTestPosition] & SOUTH) > 0       //and it is connected to the previous part
            );

            //Did we cross the boundary?
            if ((bitFlags & WEST) > 0 && (bitFlags & EAST) > 0) boundaryCount++;
		}
        else // if we haven't hit a boundary just skip the cell
        {
            pTestPosition += directionMap[NORTH].direction;
        }
    }

    //if we encountered an odd number of boundaries we are inside the loop
    return boundaryCount % 2 == 1;
}

/** // For debugging:

Grid<char> debugGrid = new Grid<char>(pipeGrid.width, pipeGrid.height);

Dictionary<int, char> valueToCharMap = new() {
    { NORTH | SOUTH,    '│'},
    { NORTH | WEST,     '┘'},
    { NORTH | EAST,     '└'},
    { EAST | SOUTH ,    '┌'},
    { EAST | WEST,      '─'},
    { SOUTH | WEST,     '┐'}
};

pipeGrid.Foreach(
    (position, value) =>
    {
        if (visited.Contains(position))
        {
            debugGrid[position] = valueToCharMap[value];
        }
        else
        {
            debugGrid[position] = IsInside(position) ? 'I' : 'O';
		}
    }
);

debugGrid.Print();

/**/

int insideCellCount = 0;
pipeGrid.Foreach(
    (position, value) =>
    {
        if (visited.Contains(position)) return;

        if (IsPointInsideLoop(position))
        {
            insideCellCount++;
        }
    }
);

Console.WriteLine("Part 2: Inside cell count:" + insideCellCount);
