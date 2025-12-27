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
            //Console.Clear();
           // map.Print("");
           // Console.ReadKey();
        }

        //Console.WriteLine(distance);
        //Console.ReadKey();
    }
    else
    {
       // Console.WriteLine(instructions[instructionPointer]);
        directions.index += instructions[instructionPointer++] == 'L' ? -1 : 1;
      //  Console.WriteLine("New direction: " + directions.Current());
      //  Console.ReadKey();
    }
}

Console.WriteLine("Part 1: " + (1000 * (position.Y+1) + 4 * (position.X+1) + directions.index));