// Solution for https://adventofcode.com/2022/day/14 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using LineSegment = (Vec2<int> start, Vec2<int> end);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// Your input: a list of line segments:

string[] myInput = File.ReadAllLines(args[0]);

List<LineSegment> lines = new List<LineSegment>();

int gridWidth = 0;
int gridHeight = 0;

foreach (var line in myInput)
{
    //Get all points...
    List<Vec2i> points =
        line
            .Split([",", "->"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Chunk(2)
            .Select (x => new Vec2i(int.Parse(x[0]), int.Parse(x[1])))
            .ToList();

    gridWidth = int.Max (gridWidth, points.Max(point => point.X));
    gridHeight = int.Max (gridHeight, points.Max(point => point.Y));

    //Now convert it to segments:
    for (int i = 0; i < points.Count - 1; i++)
    {
        lines.Add((points[i], points[i+1]));
    }
}

// Create the grid and fill it using 500 as the center point:
// (Note that we don't actually need a grid, we could also keep track of everything uses HashSets,
// but I found this approach the easiest)

Grid<char> cave = new Grid<char>(gridWidth+1, gridHeight+1);
cave.Foreach((pos, value) => cave[pos] = '.');
    
// Draw the lines

foreach (var line in lines)
{
    DrawLine (line, cave);
}

void DrawLine (LineSegment pLine, Grid<char> pCave)
{
    Vec2i delta = (pLine.end - pLine.start).Sign();
    if (delta.Magnitude() != 1) throw new Exception("Line segment error:" + pLine);
    Vec2i start = pLine.start;

    while (true) 
    {
        pCave[start] = '#';
        if (start == pLine.end) break;
        start += delta;
    } 
}

// ** Part 1: Simulate particles until the particle count doesn't change any more...

long particleCount = 0;
Vec2i spawnPosition = new Vec2i(500, 0);

Vec2i currentParticle = spawnPosition;

while (true)
{
    ParticleState particleState = Simulate(ref currentParticle, cave);

    if (particleState == ParticleState.Static)
    {
        particleCount++;
        currentParticle = spawnPosition;
    }
    else if (particleState == ParticleState.Removed)
    {
        break;
    }
}

ParticleState Simulate (ref Vec2i pParticle, Grid<char> pCave)
{
    // A unit of sand always falls down one step if possible.
    // If the tile immediately below is blocked (by rock or sand),
    // the unit of sand attempts to instead move diagonally one step down and to the left.
    // If that tile is blocked, the unit of sand attempts to instead move diagonally one step down and to the right.

    Vec2i down = pParticle + new Vec2i (0, 1);
    Vec2i leftdown = pParticle + new Vec2i (-1, 1);
    Vec2i rightdown = pParticle + new Vec2i (1, 1);

    Vec2i[] options = [down, leftdown, rightdown];

    foreach (Vec2i option in options)
    {
        if (!pCave.IsInside(option) || pCave[option] == '.')
        {
            pParticle = option;
            return pCave.IsInside(option) ? ParticleState.Moved : ParticleState.Removed;
        }
    }

    pCave[pParticle] = 'O';

    return ParticleState.Static;
}

Console.WriteLine("Part 1:" + particleCount);

// ** Part 2: Similar but different (see puzzle description)

int floorPosition = gridHeight + 2;
cave = new Grid<char>(1000, floorPosition + 1);
cave.Foreach((pos, value) => cave[pos] = pos.Y == floorPosition ? '#' : '.');

// Draw the lines
foreach (var line in lines)
{
    DrawLine(line, cave);
}

particleCount = 0;
currentParticle = spawnPosition;

while (true)
{
    if (cave[spawnPosition] != '.') break;

    ParticleState particleState = Simulate(ref currentParticle, cave);

    if (particleState == ParticleState.Static)
    {
        particleCount++;
        currentParticle = spawnPosition;
    }
}

Console.WriteLine("Part 2:" + particleCount);
