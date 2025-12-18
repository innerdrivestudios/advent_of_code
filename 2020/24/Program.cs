// Solution for https://adventofcode.com/2020/day/24 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying a command line argument, e.g. 32415.
// This currentValue will be passed to the built-in args[0] variable

string[] myInput = File.ReadAllLines(args[0]);

// ** Part 1:

Dictionary<Vec2i, bool> tileMap = new();

// Possible matches:
// e, se, sw, w, nw, and ne, ordered by precedence:
// se, sw, nw, and ne, w, e

string[] tokens = ["se", "sw", "nw", "ne", "w", "e"];
Vec2i[] directions = [new(1, -1), new(0, -1), new(-1, 1), new(0, 1), new(-1, 0), new(1, 0)];


Vec2i ProcessInstructions(string pInstruction)
{
    Vec2i endPosition = new Vec2i(0, 0);

    int readPointer = 0;

    while (readPointer < pInstruction.Length)
    {
        for (int i = 0; i < tokens.Length; i++)
        {
            if (StartsWith(readPointer, pInstruction, tokens[i]))
            {
                readPointer += tokens[i].Length;
                endPosition += directions[i];
            }
        }
    }

    return endPosition;
}

bool StartsWith (int pIndex, string pString, string pMatch)
{
    if (pIndex + pMatch.Length > pString.Length) return false;

    for (int i = 0; i < pMatch.Length; i++)
    {
        if (pString[i + pIndex] != pMatch[i]) return false;
    }

    return true;
}

foreach (string line in myInput)
{
    Vec2i endPosition = ProcessInstructions(line);

    tileMap[endPosition] = !tileMap.GetValueOrDefault(endPosition, true);
}

Console.WriteLine("Part 1: " + tileMap.Count (x => !x.Value));

// ** Part 2: 

void PassTheDay (Dictionary<Vec2i, bool> pInput, Dictionary<Vec2i, bool> pOutput)
{
    pOutput.Clear();

    // Get all black tiles since white tiles aren't really defined at the start, 
    // only if a tile is toggled twice
    List<Vec2i> blackTiles = pInput.Where(x => !x.Value).Select(x => x.Key).ToList();

    foreach (Vec2i blackTile in blackTiles)
    {
        int neighborCount = GetBlackNeighbors(blackTile, pInput);
        pOutput[blackTile] = (neighborCount == 0 || neighborCount > 2);

        //Since white tiles require black tiles next to it, we can 'iterate' all white tiles, 
        //by visiting our black neighbors
        foreach (Vec2i direction in directions)
        {
            Vec2i neighbor = blackTile + direction;
            //If a neighbor is not white, skip it
            if (!pInput.GetValueOrDefault(neighbor, true)) continue;
            neighborCount = GetBlackNeighbors(neighbor, pInput);
            if (neighborCount == 2) pOutput[neighbor] = false;
        }
    }
}

int GetBlackNeighbors (Vec2i pPosition, Dictionary<Vec2i, bool> pInput)
{
    return directions.Count (x => !pInput.GetValueOrDefault (pPosition + x, true));
}

Dictionary<Vec2i, bool> newDay = new();
for (int i = 0; i < 100; i++)
{
    PassTheDay(tileMap, newDay);
    var tmp = tileMap;
    tileMap = newDay;
    newDay = tmp;
}

Console.WriteLine("Part 2: " + tileMap.Count(x => !x.Value));