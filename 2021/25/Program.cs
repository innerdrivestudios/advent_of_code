//Solution for https://adventofcode.com/2021/day/25 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Parse the input...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> sea = new Grid<char>(myInput,Environment.NewLine);

int wrapAroundX = sea.width;
int wrapAroundY = sea.height;

Vec2i east = new Vec2i(1, 0);
Vec2i south = new Vec2i(0, 1);

HashSet<Vec2i> eastHerd = new();
HashSet<Vec2i> southHerd = new();

// Map each seaCucumber position to it's direction
sea.Foreach(
    (pos, value) =>
    {
        if (value != '.') (value == '>' ? eastHerd : southHerd).Add(pos); //seaCucumbers[pos] = directions[value];
    }
);

// Define the herds and their directions and move them while we can ...

bool moved = false;
int movedCount = 0;
(HashSet<Vec2i>, Vec2i)[] herds = [(eastHerd, east), (southHerd, south)];

do
{
    moved = false;

    for (int i = 0; i <  herds.Length; i++)
    {
 		Dictionary<Vec2i, Vec2i> currentToNew = new();

		foreach (Vec2i currentPosition in herds[i].Item1)
		{
			Vec2i newPosition = currentPosition + herds[i].Item2;
			newPosition.X %= wrapAroundX;
			newPosition.Y %= wrapAroundY;

			bool canMove = !(herds[0].Item1.Contains(newPosition) || herds[1].Item1.Contains(newPosition));
			moved |= canMove;
			currentToNew[currentPosition] = canMove ? newPosition : currentPosition;
		}

        herds[i].Item1 = currentToNew.Values.ToHashSet();
	}

    movedCount++;

} while (moved);

Console.WriteLine("Part 1: " + movedCount);


