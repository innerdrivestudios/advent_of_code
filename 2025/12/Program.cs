// Solution for https://adventofcode.com/2025/day/12 (Ctrl+Click in VS to follow link)

using PuzzleRequirement = (int width, int height, int[] pieceCounts);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: ....

string[] myInput = File.ReadAllLines(args[0]);

List<Grid<char>> puzzlePieces = new();
List<PuzzleRequirement> puzzleRequirements = new();

int inputLineIndex = 0;

while (inputLineIndex < myInput.Length)
{
    string inputLine = myInput[inputLineIndex];

    if (string.IsNullOrWhiteSpace(inputLine))
    {
        inputLineIndex++;
        continue;
    }

    if (inputLine.Contains("x")) // parse puzzle requirement
    {
        string[] requirementParts = inputLine
            .Split(['x', ':', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        PuzzleRequirement requirement = new PuzzleRequirement();
        requirement.width = int.Parse(requirementParts[0]);
        requirement.height = int.Parse(requirementParts[1]);
        requirement.pieceCounts = requirementParts.Skip(2).Select (int.Parse).ToArray();
        
        puzzleRequirements.Add(requirement);

        inputLineIndex++;
    }
    else // parse puzzle piece
    {
        inputLineIndex++;
        string gridData = "";
        gridData += myInput[inputLineIndex++]+Environment.NewLine;
        gridData += myInput[inputLineIndex++]+Environment.NewLine;
        gridData += myInput[inputLineIndex++]+Environment.NewLine;
        puzzlePieces.Add(new Grid<char>(gridData, Environment.NewLine));
    }
}

int[] hashesPerPiece = new int[puzzlePieces.Count];
for (int i = 0; i < hashesPerPiece.Length; i++)
{
    int hashCount = 0;
    puzzlePieces[i].Foreach((pos, value) => { hashCount += value == '#' ? 1 : 0; });
    hashesPerPiece[i] = hashCount;
}

List<PuzzleRequirement> always = new();
List<PuzzleRequirement> never = new();
List<PuzzleRequirement> maybe = new();

foreach (PuzzleRequirement pr in puzzleRequirements)
{
    int columns = pr.width / 3;
    int rows = pr.height / 3;
    int pieceCount = columns * rows;

    if (pr.pieceCounts.Sum() <= pieceCount)
    {
        always.Add(pr);
    }
    else
    {
        int blocksRequired = 0;
        for (int i = 0; i < pr.pieceCounts.Length; i++)
        {
            blocksRequired += hashesPerPiece[i] * pr.pieceCounts[i];
        }

        if (blocksRequired > pr.width * pr.height) never.Add(pr);
        else maybe.Add(pr);
    }
}

if (maybe.Count == 0) Console.WriteLine("Part 1: " + always.Count);
else Console.WriteLine("More research needed.");
