// Solution for https://adventofcode.com/2021/day/11 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid of numbers representing 10 x 10 octopuses

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

Grid<int> energyLevels = new Grid<int>(myInput, Environment.NewLine);
Vec2i[] directions = [new(-1, -1), new(0, -1), new(1, -1), new(-1, 0), new(1, 0), new(-1, 1), new(0, 1), new(1, 1)];

int Step(Grid<int> pGrid)
{
    // First the energy level of each octopus increases by one...
    pGrid.Foreach((pos, value) => pGrid[pos]++);
    
    // Flash each octopus with a value of 9
    HashSet<Vec2i> alreadyFlashed = new HashSet<Vec2i>();

    pGrid.Foreach(
        (pos, value) =>
        {
            if (pGrid[pos] > 9) Flash(pGrid, pos, alreadyFlashed);
        }
    );

    // Reset all octopuses that flashed during this step
    foreach (Vec2i flasher in alreadyFlashed) pGrid[flasher] = 0;

    return alreadyFlashed.Count;
}

void Flash (Grid<int> pGrid, Vec2i pFlashPosition, HashSet<Vec2i> pAlreadyFlashed)
{
    if (pAlreadyFlashed.Contains(pFlashPosition)) return;
    pAlreadyFlashed.Add(pFlashPosition);

    foreach (Vec2i direction in directions)
    {
        Vec2i adjacentPosition = pFlashPosition + direction;
        if (pGrid.IsInside(adjacentPosition))
        {
            pGrid[adjacentPosition]++;
            if (pGrid[adjacentPosition] > 9) Flash(pGrid, adjacentPosition, pAlreadyFlashed);
        }
    }
}

// ** Part 1 & 2:

int steps = 0;
int totalFlashes = 0;

while (true)
{
    int flashesForThisStep = Step(energyLevels);

    totalFlashes += flashesForThisStep;

    steps++;

    if (steps == 100) Console.WriteLine("Part 1:" + totalFlashes);
    if (flashesForThisStep == energyLevels.totalElements)
    {
        Console.WriteLine("Part 2:" + steps);
        break;
    }
}

