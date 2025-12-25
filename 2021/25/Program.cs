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

Dictionary<Vec2i, Vec2i> seaCucumbers = new();
Dictionary<char, Vec2i> directions = new()
{
    {'>', east },
    {'v', south }
};

// Map each seaCucumber position to it's direction
sea.Foreach(
    (pos, value) =>
    {
        if (value != '.') seaCucumbers[pos] = directions[value];
    }
);

bool moved = false;
int movedCount = 0;

do
{
    moved = false;

    List<Vec2i> eastFacingFreeToMove = new();
    foreach (var kv in seaCucumbers)
    {
        if (kv.Value == south) continue;

        Vec2i newPosition = kv.Key + east;
        newPosition.X %= wrapAroundX;

        if (!seaCucumbers.ContainsKey(newPosition)) eastFacingFreeToMove.Add(kv.Key);
    }
    
    foreach (Vec2i position in eastFacingFreeToMove)
    {
        seaCucumbers.Remove(position);
        Vec2i newPosition = position + east;
        newPosition.X %= wrapAroundX;
        seaCucumbers[newPosition] = east;
    }

    List<Vec2i> southFacingFreeToMove = new();

    foreach (var kv in seaCucumbers)
    {
        if (kv.Value == east) continue;

        Vec2i newPosition = kv.Key + south;
        newPosition.Y %= wrapAroundY;

        if (!seaCucumbers.ContainsKey(newPosition)) southFacingFreeToMove.Add(kv.Key);
    }

    foreach (Vec2i position in southFacingFreeToMove)
    {
        seaCucumbers.Remove(position);
        Vec2i newPosition = position + south;
        newPosition.Y %= wrapAroundY;
        seaCucumbers[newPosition] = south;
    }

    movedCount++;

    moved = eastFacingFreeToMove.Count > 0 || southFacingFreeToMove.Count > 0;

} while (moved);

Console.WriteLine("Part 1: "+movedCount);
