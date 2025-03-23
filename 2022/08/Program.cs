// Solution for https://adventofcode.com/2022/day/8 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a forest heightmap

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Set up some helper values

HashSet<Vec2i> treesWeCanSee = new HashSet<Vec2i>();
Grid<int> treeGrid = new Grid<int>(myInput, Environment.NewLine);

// Scan all rows and columns to find out how many trees we can see
for (int x = 0; x < treeGrid.width; x++)
{
    GatherTreesInALine(new Vec2i(x, 0), new Vec2i(0, 1));
    GatherTreesInALine(new Vec2i(x, treeGrid.height-1), new Vec2i(0, -1));
}

for (int y = 0; y < treeGrid.height; y++)
{
    GatherTreesInALine(new Vec2i(0, y), new Vec2i(1, 0));
    GatherTreesInALine(new Vec2i(treeGrid.width - 1,y), new Vec2i(-1, 0));
}

void GatherTreesInALine (Vec2i pStartPosition, Vec2i pDirection)
{
    // We always see the first tree
    Vec2i scanPosition = pStartPosition;
    treesWeCanSee.Add(scanPosition);
    int heighest = treeGrid[scanPosition];
    
    // Early exit, doesn't get higher than this
    if (heighest == 9) return; 

    // Then starting from the next position,
    scanPosition += pDirection;

    while (treeGrid.IsInside(scanPosition))
    {
        // keep looking as long as we encounter higher trees
        int delta = treeGrid[scanPosition] - heighest;
        
        if (delta > 0)
        {
            treesWeCanSee.Add(scanPosition);
            heighest = Math.Max (treeGrid[scanPosition], heighest);
        }
        
        if (heighest == 9)  break; //early exit, doesn't get higher than this

        scanPosition += pDirection;
    }
}

Console.WriteLine("Part 1 - Trees we can see count:" + treesWeCanSee.Count);

// ** Part 2: Now we need to get the viewing distance from every single tree in every direction
// Then calculate it's scenic score and get the tree with the highest scenic score

int GetViewingDistance(Vec2i pStartPosition, Vec2i pDirection)
{
    // We start at the tree next to us
    Vec2i scanPosition = pStartPosition + pDirection;

    // If there is any
    if (!treeGrid.IsInside(scanPosition)) return 0;

    // We record the starting tree height
    int startingTreeHeight = treeGrid[pStartPosition];
    int viewingDistance = 0;

    while (treeGrid.IsInside(scanPosition))
    {
        // Then for any tree next to us we increase the viewing distance
        viewingDistance++;

        // But if its height is bigger or equal to our own, we're done
        if (treeGrid[scanPosition] >= startingTreeHeight) break;

        scanPosition += pDirection;
    }

    return viewingDistance;
}

int GetScenicScore (Vec2i pStartPosition)
{
    return
        GetViewingDistance(pStartPosition, new Vec2i(0, -1)) *
        GetViewingDistance(pStartPosition, new Vec2i(-1, 0)) *
        GetViewingDistance(pStartPosition, new Vec2i(1, 0)) *
        GetViewingDistance(pStartPosition, new Vec2i(0, 1));
}

int highestScenicScore = int.MinValue;
treeGrid.Foreach(
    (position, value) =>
    {
        highestScenicScore = Math.Max(highestScenicScore, GetScenicScore(position));
    }
);

Console.WriteLine("Part 2 - Highest Scenic Score:" + highestScenicScore);