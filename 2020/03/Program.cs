// Solution for https://adventofcode.com/2020/day/3 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with open spaces (.) and trees (#)

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

//True is walkable, false is not
Grid<bool> treeGrid = new Grid<bool>(myInput, Environment.NewLine, null, (position, value) => value == ".");

// Part 1 -

// You start on the open square (.) in the top-left corner
// and need to reach the bottom (below the bottom-most row on your map).

// Starting at the top-left corner of your map and following a slope of right 3 and down 1,
// how many trees would you encounter?

// Plot twist: the grid wraps around from left to right, so if you fall off the map on the right you appear on the left
// At least, that is how the forest is structured.

long TreeCount (Vec2i pSlope)
{
    Vec2i position = new(0, 0);
    int treeCount = 0;

    while (treeGrid.IsInside(position))
    {
        if (!treeGrid[position]) treeCount++;
        position += pSlope;
        position.X %= treeGrid.width;
    }

    return treeCount;
}

Console.WriteLine("Part 1 - Trees encountered: " + TreeCount(new (3,1)));

// Part 2 - The same but now for:

// Right 1, down 1.
// Right 3, down 1. 
// Right 5, down 1.
// Right 7, down 1.
// Right 1, down 2.

long totalTreesMultiplied = 1;
var slopes = new Vec2i[] { new Vec2i(1, 1), new Vec2i(3, 1), new Vec2i(5, 1), new Vec2i(7, 1), new Vec2i(1, 2) };

foreach (Vec2i slope in slopes) {
    totalTreesMultiplied *= TreeCount(slope);
}

Console.WriteLine("Part 2 - Trees encountered: " + totalTreesMultiplied);







