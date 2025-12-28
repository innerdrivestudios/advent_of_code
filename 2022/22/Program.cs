// Solution for https://adventofcode.com/2022/day/22 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//** Your input: a map and directions...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

string[] inputParts = myInput.Split(Environment.NewLine + Environment.NewLine);

//Prep the grid data since it has uneven length etc
string[] gridData = inputParts[0].Split(Environment.NewLine);   
int maxWidth = gridData.Max (x => x.Length);
for (int i = 0; i < gridData.Length;i++)
{
    gridData[i] = gridData[i].PadRight(maxWidth, ' ');
}

Grid<char> map = new Grid<char>(string.Join(Environment.NewLine, gridData), Environment.NewLine);

// Clean the instructions and set up the instructions pointer
string instructions = inputParts[1].Trim();
int instructionPointer = 0;

// Set up the directions and the starting position
Directions<Vec2i> directions = new Directions<Vec2i>([new (1,0), new(0,1), new(-1, 0), new (0,-1)]);
directions.index = 0;
Vec2i position = new Vec2i(gridData[0].IndexOf('.'), 0);

// Now process all instructions...

while (instructionPointer < instructions.Length)
{
    if (char.IsDigit(instructions[instructionPointer]))
    {
        int distance = 0;
        while (instructionPointer < instructions.Length && char.IsDigit(instructions[instructionPointer]))
        {
            distance = distance * 10 + instructions[instructionPointer] - '0';
            instructionPointer++;
        }

        for (int i = 0; i < distance; i++)
        {
            Vec2i nextPos = position + directions.Current();

            //Do we need to wrap around?
            if (!map.IsInside(nextPos) || map[nextPos] == ' ')
            {
                // If so search backwards to where we need to wrap...
                Vec2i oppositeDirection = -directions.Current();
                nextPos += oppositeDirection;
                while (map.IsInside(nextPos) && map[nextPos] != ' ')
                {
                    nextPos += oppositeDirection;
                }
                //Undo the last step
                nextPos += directions.Current();
            }

            if (map[nextPos] == '#') break; else position = nextPos;
            //map[nextPos] = 'X';
        }
    }
    else
    {
        directions.index += instructions[instructionPointer++] == 'L' ? -1 : 1;
    }
}

//map.Print("");
//Console.ReadKey();

Console.WriteLine("Part 1: " + (1000 * (position.Y+1) + 4 * (position.X+1) + directions.index));

// ** Part 2: 
// So for part 2 we need to wrap the given data around the side of a cube.
// This is probably doable for any given input,
// i.e. there is probably a solution that will work no matter which input you provide,
// given the input is "correct".
// However, to save myself a lot of work and headaches, I'll build a solution specific to my puzzle.
//
// The basic idea is to find out, which sides should be connected (see the provided word doc).
// In the provided word document, this is for example side 4 and side 5.
// All the other combinations (easiest to see if you print it out, cut it out and fold it) are:

// 4    <-->    5
// 3    <-->    6
// 1    <-->    10
// 2    <-->    9
// 7    <-->    8
// 11   <-->    14

// 12   <-->    13

// 7 pairs!
//
// For each pair, we can step OUT on one side and IN on the other side, changing direction as we do so.
// So I'll make a map from JUST-OUTSIDE positions on the OUT side, to JUST-INSIDE positions on the IN side plus the new direction.
//
// For example everything along the outside of side 4 = (100,50) to (149,50) needs to be mapped to the inside of side 5 (99,50) to (99,99),
// with an entry direction of 2 (-1,0). Since there is overlap in the outside of 4 and 5 we'll also need to include the outgoing direction in the map.
//
// This gives us in shorthand: 

// 4 --> 5 = (100,50) + 50 * (1,0) in direction (0,1) maps to (99,50) + 50 * (0,1) in direction (-1,0)

// We need to do this for all sides and convert this into a lookup table:

Dictionary<(Vec2i, Vec2i), (Vec2i, Vec2i)> teleportationMap = new();

// With a helper method to fill it:

void FillTeleportationMap (Vec2i pStartA, Vec2i pSideDirectionA, Vec2i pExitDirection, Vec2i pStartB, Vec2i pSideDirectionB, Vec2i pEntryDirection, int pCount)
{
    for (int i = 0; i < pCount; i++)
    {
        teleportationMap[(pStartA, pExitDirection)] = (pStartB, pEntryDirection);
        //map[pStartA] = 'A';
        //map[pStartB] = 'B';
        pStartA += pSideDirectionA;
        pStartB += pSideDirectionB;
    }
}

// 4 --> 5 = (100,50) + 50 * (1,0) in direction (0,1) maps to (99,50) + 50 * (0,1) in direction (-1,0)
FillTeleportationMap(new (100, 50), new(1, 0), new(0, 1), new(99, 50), new(0, 1), new(-1, 0), 50);

// 5 --> 4 = (100,50) + 50 * (0,1) in direction (1,0) maps to (100,49) + 50 * (1,0) in direction (0,-1)
FillTeleportationMap(new (100, 50), new(0, 1), new(1, 0), new(100, 49), new(1, 0), new(0, -1), 50);

// 6 --> 3 = (100,100) + 50 * (0,1) in direction (1,0) maps to (149,49) + 50 * (0,-1) in direction (-1,0)
FillTeleportationMap(new (100, 100), new (0, 1), new (1, 0), new (149, 49), new (0, -1), new (-1, 0), 50);

// 3 --> 6 = (150,0)   + 50 * (0,1) in direction (1,0) maps to (99,149) + 50 * (0,-1) in direction (-1,0)
FillTeleportationMap(new(150, 0), new(0, 1), new(1, 0), new(99, 149), new(0, -1), new(-1, 0), 50);

// 1 --> 10 = (50,-1) + 50 * (1,0) in direction (0,-1) maps to (0,150) + 50 * (0,1) in direction (1,0)
FillTeleportationMap(new(50, -1), new(1, 0), new(0, -1), new(0, 150), new(0, 1), new(1, 0), 50);

// 10 --> 1 = (-1,150) + 50 * (0,1) in direction (-1,0) maps to (50,0) + 50 * (1,0) in direction (0,1)
FillTeleportationMap(new(-1, 150), new(0, 1), new(-1, 0), new(50, 0), new(1, 0), new(0, 1), 50);

// 2 --> 9  = (100,-1) + 50 * (1,0) in direction (0,-1) maps to (0,199) + 50 * (1,0) in direction (0,-1)
FillTeleportationMap(new(100, -1), new(1, 0), new(0, -1), new(0, 199), new(1, 0), new(0, -1), 50);

// 9 --> 2  = (0,200) + 50 * (1,0) in direction (0,1) maps to (100,0) + 50 * (1,0) in direction (0,1)
FillTeleportationMap(new(0, 200), new(1, 0), new(0, 1), new(100, 0), new(1, 0), new(0, 1), 50);

// 7 --> 8 =  (50,150) + 50 * (1,0) in direction (0,1) maps to (49,150) + 50 * (0,1) in direction (-1,0)
FillTeleportationMap(new(50, 150), new(1, 0), new(0, 1), new(49, 150), new(0, 1), new(-1, 0), 50);

// 8 --> 7 =  (50,150) + 50 * (0,1) in direction (1,0) maps to (50,149) + 50 * (1,0) in direction (0,-1)
FillTeleportationMap(new(50, 150), new(0, 1), new(1, 0), new(50, 149), new(1, 0), new(0, -1), 50);

// 11 -> 14 = (-1,100) + 50 * (0,1) in direction (-1,0) maps to (50,49) + 50 * (0,-1) in direction (1,0)
FillTeleportationMap(new(-1, 100), new(0, 1), new(-1, 0), new(50, 49), new(0, -1), new(1, 0), 50);

// 14 -> 11 = (49,0) + 50 (0,1) in direction (-1,0) maps to (0,149) + 50 * (0,-1) in direction (1,0)
FillTeleportationMap(new(49, 0), new(0, 1), new(-1, 0), new(0, 149), new(0, -1), new(1, 0), 50);

// 13 -> 12 = (49,50) + 50 * (0,1) in direction (-1,0) maps to (0,100) + 50 * (1,0) in direction (0,1)
FillTeleportationMap(new(49, 50), new(0, 1), new(-1, 0), new(0, 100), new(1, 0), new(0, 1), 50);

// 12 -> 13 = (0,99) + 50 * (1,0) in direction (0,-1) maps to (50,50) + 50 * (0,1) in direction (1,0)
FillTeleportationMap(new(0, 99), new(1, 0), new(0, -1), new(50, 50), new(0, 1), new(1, 0), 50);

// Reset the directions and the starting position
instructionPointer = 0;
directions.index = 0;
position = new Vec2i(gridData[0].IndexOf('.'), 0);

while (instructionPointer < instructions.Length)
{
    if (char.IsDigit(instructions[instructionPointer]))
    {
        int distance = 0;
        while (instructionPointer < instructions.Length && char.IsDigit(instructions[instructionPointer]))
        {
            distance = distance * 10 + instructions[instructionPointer] - '0';
            instructionPointer++;
        }

        for (int i = 0; i < distance; i++)
        {
            // We start by moving where we think we need to go...
            Vec2i nextPos = position + directions.Current();

            // IF we stepped outside, we .....
            var lookupKey = (nextPos, directions.Current());
            if (teleportationMap.ContainsKey(lookupKey))
            {
                // Check where we would go...
                var nextStep = teleportationMap[(nextPos, directions.Current())];
                // IF we are block we stop and DON'T rotate (<-- nasty :))
                if (map[nextStep.Item1] == '#') break;

                // Else accept the 'teleportation'
                nextPos = nextStep.Item1;
                directions.index = directions.directions.IndexOf(nextStep.Item2);
            }

            if (map[nextPos] == '#') break; else position = nextPos;
        }
    }
    else
    {
        directions.index += instructions[instructionPointer++] == 'L' ? -1 : 1;
    }
}

Console.WriteLine("Part 2: " + (1000 * (position.Y + 1) + 4 * (position.X + 1) + directions.index));

