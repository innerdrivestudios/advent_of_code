// Solution for https://adventofcode.com/2025/day/4 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> area = new Grid<char>(myInput, Environment.NewLine);

// Helper methods:

Vec2i[] directions = [ new(-1,-1), new(0,-1), new(1,-1), new(-1,0), new(1,0), new(-1,1), new(0,1), new(1,1)];

int CountNeighbors (Vec2i pPosition)
{
    int neighbors = 0;

    foreach (var direction in directions)
    {
        Vec2i neighborPosition = pPosition + direction;
        neighbors += area.IsInside(neighborPosition) && area[neighborPosition] == '@' ? 1 : 0;
    }

    return neighbors;
}

// ** Part 1:

int accessibleRolls = 0;

area.Foreach(
    (pos, value) =>
    {
        accessibleRolls += value == '@' && (CountNeighbors(pos) < 4) ? 1 : 0;
    }
);

Console.WriteLine("Part 1: " + accessibleRolls);

// ** Part 2:

int GetTotalRemovableRollsCount ()
{
    int rollsRemovedCountTotal = 0;

    while (true)
    {
        int rollsRemoveCountLocal = 0;

        area.Foreach(
            (pos, value) =>
            {
                bool canRemove = value == '@' && (CountNeighbors(pos) < 4);
                if (canRemove) { 
                    rollsRemoveCountLocal++;
                    area[pos] = '.';
                }
            }
        );

        rollsRemovedCountTotal += rollsRemoveCountLocal;
        if (rollsRemoveCountLocal == 0) break;
    }

    return rollsRemovedCountTotal;
}

Console.WriteLine("Part 2: " + GetTotalRemovableRollsCount());