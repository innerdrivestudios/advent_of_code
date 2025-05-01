//Solution for https://adventofcode.com/2021/day/13 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of coordinates and folding instructions

string[] myInput = File.ReadAllText(args[0])
    .ReplaceLineEndings(Environment.NewLine)
    .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

HashSet<Vec2i> dotCoordinates = myInput[0]
    .Split(Environment.NewLine)
    .Select(x => x.Split (","))
    .Select (x => new Vec2i(int.Parse(x[0]), int.Parse(x[1])))
    .ToHashSet();

List<string> foldingInstructions = myInput[1]
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .ToList();

(bool isXMirror, int mirrorValue) GetMirrorData (string pMirrorInput)
{
    int value = int.Parse(pMirrorInput.Substring(pMirrorInput.IndexOf ('=') + 1));

    return (pMirrorInput.StartsWith("fold along x"), value);
}

// ** Part 1: How many dots are visible after completing just the first fold instruction on your transparent paper?

HashSet<Vec2i> Fold (string pFoldingInstruction, HashSet<Vec2i> pDotCoordinates)
{
    var mirrorData = GetMirrorData(pFoldingInstruction);

    List<Vec2i> dotCoordinatesCopy = new List<Vec2i>(pDotCoordinates);
    foreach (Vec2i dot in dotCoordinatesCopy)
    {
        if (mirrorData.isXMirror)
        {
            if (dot.X < mirrorData.mirrorValue) continue;
            pDotCoordinates.Remove(dot);
            //get distance from dot to line and apply it twice to the original X
            Vec2i newDot = dot;
            newDot.X += 2 * (mirrorData.mirrorValue - dot.X);
            pDotCoordinates.Add(newDot);
        }
        else
        {
            if (dot.Y < mirrorData.mirrorValue) continue;
            pDotCoordinates.Remove(dot);
            //get distance from dot to line and apply it twice to the original Y
            Vec2i newDot = dot;
            newDot.Y += 2 * (mirrorData.mirrorValue - dot.Y);
            pDotCoordinates.Add(newDot);
        }
    }

    return pDotCoordinates; 
}

Console.WriteLine("Part 1:" + Fold(foldingInstructions[0], new(dotCoordinates)).Count);

// ** Part 2: What do the coordinates show?

foreach (string foldingInstruction in foldingInstructions)
{
    Fold(foldingInstruction, dotCoordinates);
}

int minX = dotCoordinates.Min (x => x.X); 
int minY = dotCoordinates.Min (x => x.Y);
int maxX = dotCoordinates.Max (x => x.X);
int maxY = dotCoordinates.Max (x => x.Y);

Console.WriteLine("Grid coordinates:" + minX +" "+ minY + " " + maxX + " " + maxY);
Grid<char> display = new Grid<char>(maxX+1, maxY+1);
display.Foreach(
    (pos, value) =>
    {
        display[pos] = dotCoordinates.Contains(pos)?'#':' ';
    }
);
display.Print("");