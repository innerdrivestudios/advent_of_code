// Solution for https://adventofcode.com/2022/day/23 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using MovementTest = (Vec2<int>[] tests, Vec2<int> result);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

//** Your input: a map describing  bunch of elves

// Parse the input

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> map = new Grid<char>(myInput, Environment.NewLine);

// Gather the elves

HashSet<Vec2i> elves = new HashSet<Vec2i>();

map.Foreach((pos, value) =>
    {
        if (value == '#') elves.Add(pos);
    }
);

// Setup an elf count helper

int GetAdjacentElfCount(Vec2i pElf, Vec2i[] pDirections)
{
    return pDirections.Count(x => elves.Contains(pElf + x));
}

// All directions
Vec2i[] directions = [new(0, -1), new(0, 1), new(-1, 0), new(1, 0), new (-1,-1), new (-1,1), new (1,-1), new (1,1)];

// Just the movement test directions + final result
List<MovementTest> movementTests = [
    ([new (0,-1), new (-1,-1), new (1,-1)], new Vec2i(0,-1)),   //Move north test
    ([new (0,1), new (-1,1), new (1,1)], new Vec2i(0,1)),       //Move south test
    ([new (-1,0), new (-1,-1), new (-1,1)], new Vec2i(-1,0)),   //Move west test
    ([new (1,0), new (1,-1), new (1,1)], new Vec2i(1,0)),       //Move east test
];

Dictionary<Vec2i, Vec2i> proposals = new();
Dictionary<Vec2i, int> proposalCount = new();

int roundCount = 0;

void RunProposals (int pMaxRoundCount)
{
    do
    {
        proposals.Clear();
        proposalCount.Clear();

        // Set up all proposals

        foreach (Vec2i elf in elves)
        {
            if (GetAdjacentElfCount(elf, directions) == 0) continue;

            // By default our proposal is to not move...
            Vec2i newElfPosition = elf;

            // Unless one of our tests succeeds
            foreach (MovementTest test in movementTests)
            {
                if (GetAdjacentElfCount(elf, test.tests) == 0)
                {
                    newElfPosition = elf + test.result;
                    break;
                }
            }

            // Update the proposals and proposal count...
            proposals[elf] = newElfPosition;
            proposalCount[newElfPosition] = proposalCount.GetValueOrDefault(newElfPosition, 0) + 1;
        }

        // Execute all proposals

        HashSet<Vec2i> elvesClone = new(elves);

        foreach (Vec2i elf in elvesClone)
        {
            // IF other elfs want to follow my proposal, skip us...
            if (!proposals.ContainsKey(elf) || proposalCount[proposals[elf]] > 1) continue;
            elves.Remove(elf);
            elves.Add(proposals[elf]);
        }

        // Update the movement tests
        movementTests.Add(movementTests[0]);
        movementTests.RemoveAt(0);

        roundCount++;
    }
    while (proposals.Count > 0 && roundCount < pMaxRoundCount);   
}

// Run up to and including round 10
RunProposals(10);
Vec2i min = Vec2i.Min(elves);
Vec2i max = Vec2i.Max(elves);
Vec2i delta = max - min;
long area = (delta.X + 1) * (delta.Y + 1);

Console.WriteLine("Part 1: " + (area - elves.Count));

// Let run until finished...
RunProposals(int.MaxValue);
Console.WriteLine("Part 2: " + roundCount);





