//Solution for https://adventofcode.com/2016/day/1 (Ctrl+Click in VS to follow link)
using Vec2i = Vec2<int>;

//Your input: a list of turn and move instructions, L/R rotates -90/90 and the int moves in the current direction
string myInput = "R2, L5, L4, L5, R4, R1, L4, R5, R3, R1, L1, L1, R4, L4, L1, R4, L4, R4, L3, R5, R4, R1, R3, L1, L1, R1, L2, R5, L4, L3, R1, L2, L2, R192, L3, R5, R48, R5, L2, R76, R4, R2, R1, L1, L5, L1, R185, L5, L1, R5, L4, R1, R3, L4, L3, R1, L5, R4, L4, R4, R5, L3, L1, L2, L4, L3, L4, R2, R2, L3, L5, R2, R5, L1, R1, L3, L5, L3, R4, L4, R3, L1, R5, L3, R2, R4, R2, L1, R3, L1, L3, L5, R4, R5, R2, R2, L5, L3, L1, L1, L5, L2, L3, R3, R3, L3, L4, L5, R2, L1, R1, R3, R4, L2, R1, L1, R3, R3, L4, L2, R5, R5, L1, R4, L5, L5, R1, L5, R4, R2, L1, L4, R1, L1, L1, L5, R3, R4, L2, R1, R2, R1, R1, R3, L5, R1, R4\r\n";

//Your task: figure out where you end up after following the instructions and checking the first space entered twice...

//Split all the instructions into separate strings first
string[] instructions = myInput.Replace("\r\n", "").Split(", ", StringSplitOptions.RemoveEmptyEntries);

//Setup an array of directions, so we can easily turn left and right
Directions<Vec2i> directions = new Directions<Vec2i>([new Vec2i(1,0), new Vec2i(0,1), new Vec2i(-1, 0), new Vec2i(0, -1)]);

Console.WriteLine("Part 1 - The Easter Bunny Headquarters distance: " + Part1(instructions));
Console.WriteLine("Part 2 - First duplicate position distance: " + Part2(instructions));

Console.ReadKey();

//Part 1 = Calculating how many blocks away the easter bunny head quarters are...
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

//Part 2 = Figuring out the location of the first position entered twice
int Part2(string[] pInstructions)
{
    //Starting position and direction
    Vec2i position = new Vec2i(0, 0);
    directions.index = 1;

    //Added a hashset to keep track of prior position
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


