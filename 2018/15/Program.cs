//Solution for https://adventofcode.com/2018/day/15 (Ctrl+Click in VS to follow link)

using System.Text;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// Declare the required variables:

Vec2i[] directions = [new(0,-1), new(-1,0), new(1,0), new(0,1)];	// Define some base directions, in reading order...
Grid<char> dungeon;													// The actual dungeon
List<NPC> NPCs = new();												// All of our NPCs
Dictionary<Vec2i, NPC> pos2Npc;                                     // Store all NPCs mapped on their position...

// Define a ton of helper methods we'll need:

// Read the dungeon definition and parses all NPC's from it, building the required data structures
void InitializeCombat ()
{
    dungeon = new Grid<char>(
        File.ReadAllText(args[0]).ReplaceLineEndings(Environment.NewLine),
        Environment.NewLine
    );

	NPCs.Clear();

	dungeon.Foreach(
		(pPosition, pValue) =>
		{
			if (pValue == 'G' || pValue == 'E')
			{
				NPCs.Add(
					new NPC(
						pValue == 'G' ? NPC.NPCType.Goblin : NPC.NPCType.Elf,
						pPosition
					)
				);
				dungeon[pPosition] = '.';
			}
		}
	);

	pos2Npc = NPCs.ToDictionary(x => x.position, x => x);
}

// Moves the NPC while keeping the cache intact 
void MoveNPC (NPC pNPC, Vec2i pNewPosition)
{
    pos2Npc.Remove(pNPC.position);
    pNPC.position = pNewPosition;
    pos2Npc[pNPC.position] = pNPC;
}

void PrintDungeon ()
{
	dungeon.Print("", Environment.NewLine,
		(pos, value) =>
		{
			if (pos2Npc.TryGetValue(pos, out var npc))
			{
				return npc.type == NPC.NPCType.Elf ? "E" : "G";
			}
			else return ""+dungeon[pos];
		}
	);
}

void Debug(bool pPause = true)
{
    Console.Clear();
    PrintDungeon();
    Console.WriteLine();

    if (pPause) Console.ReadKey();
	//Thread.Sleep(100);
}

// Define some methods to run the combat as prescribed...

int RunCombat ()
{ 
	InitializeCombat();
	int round = 0;

    //Combat proceeds in Rounds...
    while (RunRound())
	{
		round++;
		Debug(false);
	}

    //Console.WriteLine("Fought for " + round + " rounds");
	return round * NPCs.Sum(x => x.hitPoints);
}

bool RunRound()
{
	// Each round, each unit that is still alive takes a turn
	
	// The order in which units take their turns within a round
	// is the reading order of their starting positions in that round

	NPCs.Sort(SortNPCsByTheirPosition);

	foreach (NPC npc in NPCs)
	{
		if (npc.hitPoints > 0)
		{
			if (TakeTurn (npc) == TurnResult.NO_TARGETS_LEFT) return false;
			//Debug();
		}
	}

	// Update our data structures
	NPCs.RemoveAll(x => x.hitPoints <= 0);
	pos2Npc = NPCs.ToDictionary(x => x.position, x => x);

	return true;
}

int SortNPCsByTheirPosition(NPC pA, NPC pB)
{
	return SortPositions(pA.position, pB.position);
}

int SortPositions(Vec2i pA, Vec2i pB)
{
    if (pA.Y == pB.Y) return pA.X - pB.X;
    else return pA.Y - pB.Y;
}

TurnResult TakeTurn(NPC pNPC)
{
	// Each unit begins its turn by identifying all possible targets (enemy units).
	// In reading order...
	List<NPC> targets = FindTargets(pNPC);

	// If no targets remain, combat ends.
	if (targets.Count == 0) return TurnResult.NO_TARGETS_LEFT;

    // The unit might already be in range of a target:

    NPC adjacentTarget = GetAdjacentTarget(pNPC, targets);

    if (adjacentTarget == null)
    {
        List<Vec2i> openSquares = IdentifyOpenSquaresNextToTargets(targets);

		// If the unit is not already in range of a target,
		// and there are no open squares which are in range of a target,
		// the unit ends its turn.
		if (openSquares.Count == 0) return TurnResult.IDLE;

		// get paths to open squares etc and move...
		// if we weren't able to move return...
		if (!DoMove(pNPC, openSquares)) return TurnResult.IDLE;

		// Check if we have a target now...
		adjacentTarget = GetAdjacentTarget(pNPC, targets);

		// If not... return ...
		if (adjacentTarget == null) return TurnResult.MOVED;
    }
	
	// If we got this far and have an adjacent target... attack it...
	DoAttack(pNPC, adjacentTarget);
	return TurnResult.FOUGHT;
}

// Find all possible targets...
List<NPC> FindTargets (NPC pNPC)
{
	return NPCs.Where (x => x.type != pNPC.type && x.hitPoints > 0).ToList();
}

// The adjacent target with the fewest hit points is selected;
// in a tie, the adjacent target with the fewest hit points which is first in reading order is selected.
// (This happens automatically if the pNPCs list is in reading order)
NPC GetAdjacentTarget(NPC pNPC, List<NPC> pNPCs)
{
	List<NPC> adjacent = pNPCs
		.Where(x => (x.position - pNPC.position).ManhattanDistance() == 1)
		.OrderBy(x => x.hitPoints)
		.ToList();

	if (adjacent.Count == 0) return null;
	
	adjacent = adjacent.Where(x => x.hitPoints == adjacent[0].hitPoints).ToList();
	adjacent.Sort(SortNPCsByTheirPosition);

	return adjacent[0];
}

// The unit identifies all of the open squares (.) that are in range of each target;
// these are the squares which are adjacent (immediately up, down, left, or right)
// to any target and which aren't already occupied by a wall or another unit.

List<Vec2i> IdentifyOpenSquaresNextToTargets(List<NPC> targets)
{
	HashSet<Vec2i> result = new();

	foreach (NPC target in targets)
	{
		foreach (Vec2i direction in directions)
		{
			Vec2i positionToEvaluate = target.position + direction;
			if (
				dungeon[positionToEvaluate] == '.' && !Occupied(positionToEvaluate)
			) result.Add(positionToEvaluate);
		}
	}

	List<Vec2i> openPositions = result.ToList();
    openPositions.Sort(SortPositions);
    return openPositions;
}

// Returns whether it moved...
bool DoMove (NPC pNPC, List<Vec2i> pOpenSquares)
{
	// To move, the unit first considers the squares that are in range
	// and determines which of those squares it could reach in the fewest steps.

	// A step is a single movement to any adjacent(immediately up, down, left, or right) open(.) square.
	// Units cannot move into walls or other units.
	// The unit does this while considering the current positions of units and does not do any prediction about where units will be later.
	// If the unit cannot reach (find an open path to) any of the squares that are in range, it ends its turn.
	// If multiple squares are in range and tied for being reachable in the fewest steps, the square which is first in reading order is chosen.

	List<Vec2i> closestSteps = new();
	int shortestPathLength = int.MaxValue;

	foreach (Vec2i targetPosition in pOpenSquares)
	{
		List<Vec2i> path = BFSSearch(pNPC.position, targetPosition, shortestPathLength);

		if (path != null && path.Count <= shortestPathLength)
		{
			if (path.Count < shortestPathLength)
			{
				closestSteps.Clear();
				shortestPathLength = path.Count;
			}
			closestSteps.Add(path[0]);
		}
	}

	if (closestSteps.Count > 0)
	{
		closestSteps.Sort(SortPositions);
		MoveNPC(pNPC, closestSteps[0]);
		return true;
	}
	else
	{
		return false;
	}
}

List<Vec2i> BFSSearch (Vec2i pStart, Vec2i pEnd, int pMaxCost = int.MaxValue)
{
	// Can we ever attain a shorter path than given?
    if ((pEnd - pStart).ManhattanDistance() > pMaxCost) return null;

    Queue<Vec2i> todo = new();
    Dictionary<Vec2i, Vec2i> parentMap = new ();
	Dictionary<Vec2i, int> costMap = new ();

    todo.Enqueue(pStart);
	costMap[pStart] = 0;

    while (todo.Count > 0)
    {
        Vec2i current = todo.Dequeue();
        if (current.Equals(pEnd)) return GetPath(parentMap, current);
		
		int currentCost = costMap[current];
		if (currentCost > pMaxCost) return null;

        var neighbours = GetNeighbours(current);

        foreach (Vec2i neighbour in neighbours)
        {
            if (costMap.ContainsKey(neighbour)) continue;
            todo.Enqueue(neighbour);
			costMap[neighbour] = currentCost + 1;
            parentMap[neighbour] = current;
        }
    }

    return null;
}

List<Vec2i> GetNeighbours(Vec2i pPosition)
{
	List<Vec2i> neighbours = new();

	foreach (Vec2i direction in directions)
	{
		Vec2i neighbour = pPosition + direction;
		if (dungeon[neighbour] == '.' && !Occupied(neighbour)) neighbours.Add(neighbour);
	}

	return neighbours;
}

List<Vec2i> GetPath (Dictionary<Vec2i, Vec2i> pParentMap, Vec2i pEndNode)
{
    List<Vec2i> path = new();

    while (pParentMap.TryGetValue(pEndNode, out Vec2i parentNode))
    {
        path.Add(pEndNode);
        pEndNode = parentNode;
    }

    path.Reverse();

    return path;
}

bool Occupied (Vec2i pPosition)
{
	return pos2Npc.ContainsKey(pPosition) && pos2Npc[pPosition].hitPoints > 0;
}

void DoAttack (NPC pNPC, NPC pTarget)
{
	pTarget.hitPoints = int.Max(pTarget.hitPoints - pNPC.attackPower, 0);
}


// ** Part 1:
Console.WriteLine("Part 1 - Battle score: " + RunCombat());

// ** Part 2: 
// - Rewrite the combat mechanism to figure out, what the minimum health required is for the Elves to all survive...

// We'll run a fight, if the elves won, we'll return the combat score... if any elves died, we'll return -1
// (Brute force aproach...)

int RunModdedCombat(int pAttackPower)
{
    InitializeCombat();

	List<NPC> allElves = NPCs.Where( npc => npc.type == NPC.NPCType.Elf).ToList();
	allElves.ForEach(elf => elf.attackPower = pAttackPower);

    int round = 0;

    //Combat proceeds in Rounds...
    while (RunRound())
    {
        round++;
        //Debug(false);
    }

	if (allElves.Count (x => x.hitPoints > 0) == allElves.Count)
	{
	    //Console.WriteLine(pAttackPower + " => " + round + " => " + round * NPCs.Sum(x => x.hitPoints));
		return round * NPCs.Sum(x => x.hitPoints);
    }
	else
	{
		return -1;
	}
}

//Console.WriteLine();
//Console.WriteLine("Starting binary search...");
//Console.WriteLine("Establishing lower and upper boundaries...");

int lowerLimit = 4;
int upperLimit = lowerLimit;
int finalBattleScore = -1;

while ((finalBattleScore = RunModdedCombat(upperLimit)) == -1)
{
	lowerLimit = upperLimit;
	upperLimit *= 2;
}

//Console.WriteLine("Lower limit:" + lowerLimit);
//Console.WriteLine("Upper limit:" + upperLimit);

int BinarySearch (int pLowerLimit, int pUpperLimit)
{
	if (pLowerLimit == pUpperLimit) return pLowerLimit;

	int middle = (pLowerLimit + pUpperLimit) / 2;

    Console.WriteLine("Running battle with "+ middle + " attack power...");

	if (RunModdedCombat(middle) > -1) return BinarySearch(pLowerLimit, middle-1);
	else return BinarySearch(middle+1, pUpperLimit);
}

int winningPower = BinarySearch(lowerLimit, upperLimit);

Console.WriteLine("Part 2 The elves would win with an attack power of " + winningPower + " and a battle score of " + RunModdedCombat(winningPower) + " points.");
