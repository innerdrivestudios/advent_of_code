// Solution for https://adventofcode.com/2020/day/20 (Ctrl+Click in VS to follow link)

using System.Drawing;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: an array of strings describing tile data (an id + newline separated hash tags)

string[] allPieceData = File.ReadAllText(args[0])
	.ReplaceLineEndings()
	.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

// Parse all tiles, this will also:
// - parse the id of the WHOLE tile
// - generate and store id's for all of the tile's sides, flipped and not flipped (called forward and backward)

Console.WriteLine("Parsing tiles...");
List<Tile> tiles = allPieceData.Select (x => new Tile(x)).ToList();

// Just some assertions and experiments...
Console.WriteLine("Tile count:" + tiles.Count);
Console.WriteLine("Tiles with 8 side id's:" + tiles.Count (x => x.sideIds.Count == 8));
Console.WriteLine("All tiles have 8 side id's? " + !tiles.Any(x => x.sideIds.Count != 8));
Console.WriteLine();

// In other words: we have x * x tiles, all tile's have 8 different id's identifying their sides

// Now count how many times each side id's exists...
Dictionary<int, int> sideIdToSideIdCount = new();

foreach (Tile tile in tiles)
{
	foreach (int id in tile.sideIds)
	{
		sideIdToSideIdCount[id] = sideIdToSideIdCount.GetValueOrDefault(id) + 1;
	}
}

Console.WriteLine("Different side id's stored:" + sideIdToSideIdCount.Keys.Count);
Console.WriteLine("All side id's have max two matches:" + (sideIdToSideIdCount.Where(x => x.Value <= 2).Count() == sideIdToSideIdCount.Count));

// In other words all side id's occur max 2 times, so each side is either NOT matched or UNIQUELY matched

// Now filter out all side id's that are unmatched, e.g. their occurrence count is 1: these are edge + corner pieces 
List<int> unmatchedIds = sideIdToSideIdCount.Where (x => x.Value == 1).Select (x => x.Key).ToList();

// Next up, we basically have 3 different types of pieces:
// 1) pieces matched on all sides (non edge or corner pieces)
// 2) pieces matched on 3 sides (non corner edge pieces)
// 3) pieces matched on 2 sides (corner pieces)

// In other words:
// - type 1 pieces should not share any side ids with our unmatchedIds	=> 0
// - type 2 pieces will share some ids with our unmatchedIds			=> 2 (1 per unmatched side * 2 due to the flipping)
// - type 3 pieces will share the most ids with our unmatchedIds		=> 4 (1 per unmatched side * 2 ...)

// For proof
int maxMatches = tiles.Max(x => x.sideIds.Intersect(unmatchedIds).Count());
Console.WriteLine("Share count for corner pieces:" + maxMatches);

// So filter these 4 corner pieces out of the whole tile set, multiply their tile id's and print them
HashSet<Tile> cornerPieces = tiles.Where(x => x.sideIds.Intersect(unmatchedIds).Count() == 4).ToHashSet();
long result = cornerPieces.Aggregate(1L, (x, y) => x * y.tileID);
Console.WriteLine("Part 1: " + result);

// ** Part 2: Let's first try and reconstruct the puzzle starting at a given piece...
// this should be fairly easy, since every id has at most one match...
// We'll start with a single puzzle piece, rotate it so the left and top are unmatched
// and then fill out the rest of the puzzle...
// Note: I am interpreting all grids as coordinate systems with x to the right and y is down
// meaning positive rotation of the grids is clockwise
// (positive rotation in a 2d coordinate system is defined as a rotation from x to y)

int puzzleDimensions = (int)Math.Sqrt(tiles.Count);
Console.WriteLine("Puzzle dimensions: " + puzzleDimensions);
Tile[,] puzzle = new Tile[puzzleDimensions, puzzleDimensions];  

void CompletePuzzle (Tile pStartingTile)
{
	OrientTopLeftStartingPiece (pStartingTile);
	puzzle[0, 0] = pStartingTile;

	// With the top left intact and knowing that only one side id matches
	for (int y = 0; y < puzzleDimensions; y++)
	{
		for (int x = 0; x < puzzleDimensions; x++)
		{
			if (x == 0 && y == 0) continue;
			if (y == 0) puzzle[x, y] = FindCorrectlyOrientedRightPiece(puzzle[x - 1, y]);
			else puzzle[x, y] = FindCorrectlyOrientedBottomPiece(puzzle[x, y - 1]);
		}
	}
}

void OrientTopLeftStartingPiece (Tile pTile)
{
	// Rotate so the top and left side are unmatched
	while (true)
	{
		var topSideIds = pTile.GatherTopSideIds();
		var leftSideIds = pTile.GatherLeftSideIds();
		if (unmatchedIds.Contains(topSideIds.forward) && unmatchedIds.Contains(leftSideIds.forward)) break;
		pTile.tileData.Rotate(1);
	}
}

Tile FindCorrectlyOrientedRightPiece (Tile tile)
{
	// Get the id of the right side
	int idToMatch = tile.GatherRightSideIds().forward;

	// Gather the tiles that match with our right side and are not us...
	List<Tile> matchingTiles = tiles.Where(x => x.sideIds.Contains(idToMatch) && x.tileID != tile.tileID).ToList();

	if (matchingTiles.Count != 1) Console.WriteLine("Unexpected Issue");
	Tile matchingTile = matchingTiles[0];

	// Rotate this tile until its left side matches our right side
	while (true)
	{
		var leftSides = matchingTile.GatherLeftSideIds();
		if (leftSides.forward == idToMatch || leftSides.backward == idToMatch) break;
		matchingTile.tileData.Rotate(1);
	}

	// If the backward side matched (the flipped side), flip our piece
	if (matchingTile.GatherLeftSideIds().backward == idToMatch) matchingTile.tileData.FlipVertical();

	return matchingTile;
}

// Same as above but now to match a piece above us
Tile FindCorrectlyOrientedBottomPiece (Tile tile)
{
	int idToMatch = tile.GatherBottomSideIds().forward;

	//Gather the tiles that match with our right side and are not us...
	List<Tile> matchingTiles = tiles.Where(x => x.sideIds.Contains(idToMatch) && x.tileID != tile.tileID).ToList();

	if (matchingTiles.Count != 1) Console.WriteLine("Unexpected Issue");
	Tile matchingTile = matchingTiles[0];

	while (true)
	{
		var topSides = matchingTile.GatherTopSideIds();
		if (topSides.forward == idToMatch || topSides.backward == idToMatch) break;
		matchingTile.tileData.Rotate(1);
	}

	if (matchingTile.GatherTopSideIds().backward == idToMatch) matchingTile.tileData.FlipHorizontal();

	return matchingTile;
}

// Just try out different piece indices, flipped horizontally/vertically or not, until the seamonster count is not zero.
// We might need to flip the tile horizontally before passing it in.
List<Tile> cornerPiecesAsList = cornerPieces.ToList();
int pieceIndex = 1;
//cornerPiecesAsList[pieceIndex].tileData.FlipHorizontal();
CompletePuzzle(cornerPiecesAsList[pieceIndex]);

// Now that we have the complete puzzle, we'll transform it into a joined grid...
Grid<char> newPuzzle = new Grid<char> (puzzleDimensions * 8, puzzleDimensions * 8);

for (int x = 0; x < puzzleDimensions; x++)
{
	for (int y = 0; y < puzzleDimensions; y++)
	{
		puzzle[x, y].tileData.ForeachRegion(
			new Rectangle(1, 1, 8, 8), 
			(pos, value) => { newPuzzle[pos.X-1 + x*8, pos.Y-1+y*8] = value; }
		);
	}
}

// newPuzzle.Print("");

// Now we need to count sea monsters :)

// First transform the sea monster data in a set of relative coords in a hashset
string seaMonsterData = "                  # \r\n#    ##    ##    ###\r\n #  #  #  #  #  #   ";
Grid<char> seaMonsterParser = new Grid<char>(seaMonsterData, "\r\n");
HashSet<Vec2i> seaMonsterCoords = new HashSet<Vec2i>();
seaMonsterParser.Foreach((pos, value) => { if (value == '#') seaMonsterCoords.Add(pos); });

// Define a helper method to detect a sea monster at position x,y
bool IsSeaMonsterPresent (Vec2i pTopLeftPosition)
{
	foreach (Vec2i pos in seaMonsterCoords)
	{
		if (newPuzzle[pos + pTopLeftPosition] != '#') return false;
	}
	return true;
}

// Now test and count sea monsters
int seaMonsterCount = 0;

for (int x = 0; x <= newPuzzle.width - seaMonsterParser.width; x++)
{
	for (int y = 0; y <= newPuzzle.height - seaMonsterParser.height; y++)
	{
		if (IsSeaMonsterPresent(new Vec2i(x,y))) seaMonsterCount++;
	}
}

if (seaMonsterCount == 0) return;

int hashTagCount = 0;
newPuzzle.Foreach((pos, value) => { if (value == '#') hashTagCount++; });
hashTagCount -= seaMonsterCount * seaMonsterCoords.Count;

Console.WriteLine("Part 2: " + hashTagCount);