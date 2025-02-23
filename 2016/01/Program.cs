// Solution for https://adventofcode.com/2016/day/1 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of turn and move instructions, L/R rotates -90/90 and the int moves in the current direction

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

string[] instructions = myInput.Split(", ", StringSplitOptions.RemoveEmptyEntries);

//** Part 1: Calculate how many blocks away the easter bunny head quarters are...

// Setup a global array of directions, so we can easily turn left and right
Directions<Vec2i> directions = new Directions<Vec2i>([new Vec2i(1,0), new Vec2i(0,1), new Vec2i(-1, 0), new Vec2i(0, -1)]);

int Part1(string[] pInstructions)
{
    //Starting position and direction
    Vec2i position = new Vec2i(0, 0);
    directions.index = 1;

    foreach (string instruction in pInstructions)
    {
        //R9 -> [0] = R
        int rotation = instruction[0] == 'R' ? 1 : -1;
        directions.index += rotation;

        //R9 -> Substring 1 = 9
        int moves = int.Parse(instruction.Substring(1));
		position += directions.Current() * moves;
    }

    return position.ManhattanDistance();
}

Console.WriteLine("Part 1 - The Easter Bunny Headquarters distance: " + Part1(instructions));

// ** Part 2: Figure out the location of the first position entered twice

int Part2(string[] pInstructions)
{
    //Starting position and direction
    Vec2i position = new Vec2i(0, 0);
    directions.index = 1;

    //Define a hashset to keep track of prior positions
    HashSet<Vec2i> visited = new HashSet<Vec2i>();

    foreach (string instruction in pInstructions)
    {
        int rotation = instruction[0] == 'R' ? 1 : -1;
        directions.index += rotation;

        int moves = int.Parse(instruction.Substring(1));

        //instead of doing this in one step, split it into discrete steps
        for (int i = 0; i < moves;i++)
        {
            //position += directions.Current() * moves;
            position += directions.Current();
            if (!visited.Add(position)) return position.ManhattanDistance();
        }
    }

    return -1;
}

Console.WriteLine("Part 2 - First duplicate position distance: " + Part2(instructions));

Console.ReadKey();

