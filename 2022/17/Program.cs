// Solution for https://adventofcode.com/2022/day/17 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// Your input: a list of left and right instructions for pieces that are falling down...

string movementInstructions = File.ReadAllText(args[0]).ReplaceLineEndings("");
int currentInstruction = -1;

char GetNextInstruction ()
{
	currentInstruction++;
	currentInstruction %= movementInstructions.Length;
	return movementInstructions[currentInstruction];
}

// Now let's define the pieces and a "get next piece mechanism"

List<Rock> rockTypes = [
	new Rock([new (0,0), new(1, 0), new(2, 0), new(3, 0)]),
	new Rock([new (1,0), new(0, 1), new(1, 1), new(2, 1), new (1,2)]),
	new Rock([new (0,0), new(1, 0), new(2, 0), new(2, 1), new (2,2)]),
	new Rock([new (0,0), new(0, 1), new(0, 2), new(0, 3)]),
	new Rock([new (0,0), new(1, 0), new(0, 1), new(1, 1)])
];

int rockTypeIndex = -1;

Rock GetNextRock ()
{
	rockTypeIndex++;
	rockTypeIndex %= rockTypes.Count;
	return rockTypes[rockTypeIndex].Clone();
}

// Now let's create some helper methods for spawning rocks and printing
// what we have so far... so we can test that mechanism in isolation

// Start with some artificial boundaries for drawing,
// and modify them as needed when we add or move a piece...

int minX = 0;
int maxX = 6;
int minY = 0;
int maxY = 20;

HashSet<Vec2i> rockPieces = new();		//Keep track of all the rock pieces that are locked down
Rock currentRock = null;				//Keep track of the rock currently still moving
long rocksSpawned = 0;					//The number of rocks spawned so far...
int highestPiecesPoint = -1;			//The Y of the highest rock *piece* locked down so far

// Define some helper methods....

Rock SpawnRock ()
{
	Rock rock = GetNextRock();

	// 2 from the left, 3 empty spaces between highest and new pieces bottom (eg. last height plus 4 gives 3 empty spaces)
	rock.SetPosition (new Vec2i(2, highestPiecesPoint+4));

	//This is only required for debug drawing so we know from where to start
	foreach (var rockPiece in rock) { 
        maxY = int.Max(maxY, rockPiece.Y);
    }

	rocksSpawned++;

    return rock;
}

// For debug drwaing ...
void DrawCave ()
{
	Console.Clear();
	for (int y = maxY; y >= minY; y--)
	{
		for (int x = minX; x <= maxX; x++)
		{
			Vec2i point = new Vec2i(x, y);
			if (rockPieces.Contains(point))
			{
                Console.Write('#');
            }
			else if (currentRock != null && currentRock.Contains (point))
			{
                Console.Write('@');
            }
			else
			{
                Console.Write('.');
            }

		}
		Console.WriteLine();
	}
}

// Moving a piece...

bool MovePiece(Rock pRock, int pDx, int pDy)
{
    //First catch the obvious move block...
    if (pRock.position.X + pDx < 0) return false;
    if (pRock.position.X + pRock.width - 1 + pDx > maxX) return false;
    if (pRock.position.Y + pDy < 0) return false;

    //then try to move the actual block
    currentRock.Move(new Vec2i(pDx, pDy));

    bool couldMove = true;

    foreach (var rockPiece in currentRock)
    {
        if (rockPieces.Contains(rockPiece))
        {
            couldMove = false; break;
        }
    }

    //If we couldn't move without creating overlap, undo the move
    if (!couldMove) currentRock.Move(new Vec2i(-pDx, -pDy));

    return couldMove;
}

// Now run the actual part 1

long GetHighestPoint (long pRockCount)
{
    // Clear the system
    currentInstruction = -1;
    rockTypeIndex = -1;
    rockPieces.Clear();
    rocksSpawned = 0;
    currentRock = null;
    highestPiecesPoint = -1;

    while (true)
    {
        if (currentRock == null) currentRock = SpawnRock();

        //we alternate between moving a rock left and right...
        char nextInstruction = GetNextInstruction();

        if (nextInstruction == '<')
        {
            //try to move but ok if it fails
            MovePiece(currentRock, -1, 0);
        }
        else if (nextInstruction == '>')
        {
            //try to move but ok if it fails
            MovePiece(currentRock, 1, 0);
        }

        bool couldMove = MovePiece(currentRock, 0, -1);

        if (!couldMove)
        {
            //try to move but if it fails, we need to lock this piece in and spawn a new piece...
            foreach (var rockPiece in currentRock)
            {
                rockPieces.Add(rockPiece);
                highestPiecesPoint = int.Max(highestPiecesPoint, rockPiece.Y);
            }

            currentRock = null;

            if (rocksSpawned == pRockCount)
            {
                return highestPiecesPoint + 1;
            }
        }
    }
}

Console.WriteLine("Part 1:" + GetHighestPoint(2022));

// ** Part 2 is going to be a bit messy... since I didn't refactor this into a bunch of reusable methods, there 
// is some code duplication going on ;).

// The main thing here is to see if we can detect a pattern based on the movement instructions and rocks we have...
// E.g. if rolling through all the movement instructions and rocks will result in the same height difference and 
// amount of rocks spawned every time... this is a little bit trial and error, since even if this works, 
// I can't really prove WHY it works ... (except for the fact that the input has been constructed to work that way ...)

// SO step one is to try and find a pattern using a modified version of the previous method...

(long repeatedHeight, long repeatedPieces) FindPattern (int pRequiredPatternRepetitionCount)
{
    // Clear the system like we did previously
    currentInstruction = -1;
    rockTypeIndex = -1;
    rockPieces.Clear();
    rocksSpawned = 0;
    currentRock = null;
    highestPiecesPoint = -1;

    // Set up some variables to track
    long lastRockCount = 0;
    long lastHeight = 0;
    long lastRockCountDelta = 0;
    long lastHeightDelta = 0;
    int repetitionCount = 0;
    bool patternFound = false;

    while (true)
    {
        if (currentRock == null) currentRock = SpawnRock();

        //we alternate between moving a rock left and right...
        char nextInstruction = GetNextInstruction();

        if (nextInstruction == '<')
        {
            //try to move but ok if it fails
            MovePiece(currentRock, -1, 0);
        }
        else if (nextInstruction == '>')
        {
            //try to move but ok if it fails
            MovePiece(currentRock, 1, 0);
        }

        //Every time the instruction set started over, check if a pattern can be detected...
        if (currentInstruction == 0)
        {
            long heightDelta = (highestPiecesPoint - lastHeight);
            long rockCountDelta = rocksSpawned - lastRockCount;

            if (heightDelta == lastHeightDelta && rockCountDelta == lastRockCountDelta) repetitionCount++;

            lastHeightDelta = heightDelta;
            lastRockCountDelta = rockCountDelta;
            lastHeight = highestPiecesPoint;
            lastRockCount = rocksSpawned;

            if (repetitionCount >= pRequiredPatternRepetitionCount) patternFound = true;
        }

        bool couldMove = MovePiece(currentRock, 0, -1);

        if (!couldMove)
        {
            //try to move but if it fails, we need to lock this piece in and spawn a new piece...
            foreach (var rockPiece in currentRock)
            {
                rockPieces.Add(rockPiece);
                highestPiecesPoint = int.Max(highestPiecesPoint, rockPiece.Y);
            }

            currentRock = null;

            if (patternFound)
            {
                return (lastHeightDelta, lastRockCountDelta);
            }
        }

    }
}

var pattern = FindPattern(5);
Console.WriteLine("Pattern detected:" + pattern);

// Ok now we've got the pattern, we know after how many blocks everything repeats and the involved height...
// Now we're gonna check how many times this fits in to the requested amount of 1000000000000 blocks...

long patternFits = 1000000000000 / pattern.repeatedPieces;
long patternFitHeight = patternFits * pattern.repeatedHeight;

// But we also need to calculate the height of the remaining rocks that didn't exactly fit the pattern:

long rocksToStillCheck = 1000000000000 - (patternFits * pattern.repeatedPieces);
long addedHeight = GetHighestPoint(rocksToStillCheck);

long total = patternFitHeight + addedHeight;

Console.WriteLine("Part 2: " + total);

//DrawCave();

//Console.WriteLine((highestPiecesPoint + 1) + (999999998990 / 1745) * 2752);
//(1000000000000 - 999999998990)
//if (rocksSpawned == 1000000000000 - (1000000000000/1745)*1745)
