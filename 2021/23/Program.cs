//Solution for https://adventofcode.com/2021/day/23 (Ctrl+Click in VS to follow link)

using BurrowState = System.Collections.Generic.Dictionary<Vec2<int>, int>;  // Positions to types where A = 0, B = 1, etc
using Room = Vec2<int>[];                                                   // A room holds two space, 0 = Room A etc
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Parse the input ...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

Grid<char> burrow = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1: Calculate the minimum cost to get everyone back to their own home.
// (See the link for the full description).
//
// To calculate this we have to keep several things in mind:
// - there is a concept of hallway and rooms
// - there are amphipods that have starting rooms and destination rooms
// - there are rules for "travelling"...
// - amphipods have different travel costs...
// - there is not a lot of path planning going on,
//   the cost of every path is simply the manhattan distance from start-end * the amphipod energy cost
//   but our path might be blocked by other amphipods, but it seems we don't need to go full A* or Dijkstra on this.

// First thing to figure out is:
// - which tiles designate valid hallway tiles
// - which tiles are part of which room
// - where the amphipods currently are and where they can go

// Notes:
// - theoretically we can go from a room directly to a room,
//   but cost-wise there should be no difference between doing that and "pausing" in the hallway

HashSet<Vec2i> hallwayTiles = new ();       //to know where we can go into the hallway
BurrowState amphipods = new ();             //during parsing we'll convert char A,B,C,D to 0,1,2,3,
                                            //mapping amphipods positions to types
List<Vec2i> roomTiles = new ();             //we'll need to sort these later, hence the list...

burrow.Foreach(
    (pos, value) =>
    {
        if (value == '.') 
        { 
            hallwayTiles.Add(pos); 
        }
        else if (char.IsAsciiLetterUpper(value)) 
        { 
            roomTiles.Add(pos);
            amphipods[pos] = value - 'A';
        }
    }
);

// Not all hallway tiles are valid stopping positions for our amphipods,
// remove all spaces above the rooms if they are in there...
foreach (Vec2i tile in roomTiles)
{
    hallwayTiles.Remove (tile - new Vec2i(0, 1));
}

// To be able to deal with arbitrarily long rooms we'll calculate the min and max room y
int minRoomY = roomTiles.Min(x => x.Y);
int maxRoomY = roomTiles.Max(x => x.Y);

// We also need to know where room 'A', 'B' etc is...
// And we are also ALWAYS filling this room up from the bottom...
// So we'll sort on +X and -Y
roomTiles.Sort((a, b) => (a.X - b.X) * burrow.width + (b.Y - a.Y));

// Now we'll store these in a 2d array of room index to locations
Room[] rooms = roomTiles.Chunk(maxRoomY-minRoomY+1).ToArray();

// Now for the search we'll need to know which amphipods can move and where they can move ...
// Which amphipods can move? 
// - All the top amphipods of a 'wrong' room
// - All the hallway amphipods that can move into their 'own' room
// In other words, we'll also need to figure out whether an amphipod IS in a room or not...

// We are in a room if we are not in the hallway
bool IsInRoom (Vec2i pPos)
{
    return pPos.Y != 1;
}

// IsCorrectInRoom returns whether the amphipod in this position is in the correct room
// and in the correct position in that room (otherwise it will still need to move...)
bool IsCorrectInRoom (BurrowState pBurrowState, Vec2i pPos)
{
    int type = pBurrowState[pPos];

    // type 0 should end up in room x=3, type 1 -> room x=5, etc
    // So if the X doesn't match the type we are false...
    if (pPos.X != 3 + 2 * type) return false;

    // Otherwise everything below us should match our type...
    for (int y = pPos.Y+1; y <= maxRoomY; y++)
    {
        Vec2i positionToCheck = new Vec2i(pPos.X, y);
        if (pBurrowState[positionToCheck] != type) return false;
    }
    return true;
}


// Returns whether an amphipod of the given type can enter its designated room...
bool CanEnterRoom(BurrowState pBurrowState, int pType, out Vec2i pFinalRoomPosition)
{
    pFinalRoomPosition = default;
    Room room = rooms[pType];

    for (int i = 0; i < room.Length; i++)
    {
        // If we pass this test, this will be the slot where we end up
        pFinalRoomPosition = room[i];

        // If this position is taken, its need to be taken by one of our own kind...
        if (pBurrowState.TryGetValue(room[i], out int type))
        {
            if (pType != type) return false;
        }
        else // if free take it, since everything before is our own kind (or we are the first)
        {
            return true;
        }
    }

    Console.WriteLine("We'll never get here");
    return true;
}

// Assuming we can move, this means we are either moving from the HALLWAY into a ROOM.
// Or from a ROOM into the HALLWAY. 
// This is always in an L shape (rotated/flipped):
// - from ROOM to HALLWAY - UP first, then SIDEWAYS
// - from HALLWAY to ROOM - SIDEWAYS first, then DOWN

int CostToReach (BurrowState pBurrowState, Vec2i pStart, Vec2i pEnd)
{
    Vec2i current = pStart;

    //always use the room x and the hallway y as an inBetween
    Vec2i inBetween = new Vec2i(IsInRoom(pStart)?pStart.X:pEnd.X, IsInRoom(pEnd)?pStart.Y:pEnd.Y);

    Vec2i startToInBetween = inBetween - pStart;
    startToInBetween = startToInBetween.Sign();

    while (current != inBetween)
    {
        Vec2i nextPosition = current + startToInBetween;
        if (pBurrowState.ContainsKey(nextPosition)) return -1;
        current = nextPosition;
    }

    Vec2i inBetweenToEnd = pEnd - inBetween;
    inBetweenToEnd = inBetweenToEnd.Sign();

    while (current != pEnd)
    {
        Vec2i nextPosition = current + inBetweenToEnd;
        if (pBurrowState.ContainsKey(nextPosition)) return -1;
        current = nextPosition;
    }

    return (int) ((pEnd - pStart).ManhattanDistance() * Math.Pow(10, pBurrowState[pStart]));
}

long CalculateSolutionCosts()
{
    PriorityQueue<BurrowState, long> searchSpace = new();
    Dictionary<string, long> costs = new();

    searchSpace.Enqueue(amphipods, 0);
    costs[GetStateIdentifier(amphipods)] = 0;

    while (searchSpace.Count > 0)
    {
        long currentCost = 0;
        searchSpace.TryPeek(out _, out currentCost);
        BurrowState currentState = searchSpace.Dequeue();

        if (IsFinalState(currentState)) return currentCost;

        foreach (var amphipod in currentState)
        {
            // If I'm already done, leave me be...
            if (IsCorrectInRoom(currentState, amphipod.Key)) continue;

            if (IsInRoom(amphipod.Key)) { 
                //if we are in a room, we can move into the hallway...
                foreach (Vec2i hallwayPosition in hallwayTiles)
                {
                    //skip occupied positions...
                    if (amphipod.Key == hallwayPosition) continue;

                    ExtendSearchSpace(currentState, amphipod.Key, hallwayPosition, searchSpace, costs);
                }
            }
            else
            {
                // if we are in the hallway, we will only move if to our own room if there is a spot free:
                if (CanEnterRoom(currentState, amphipod.Value, out Vec2i roomPosition))
                {
                    ExtendSearchSpace(currentState, amphipod.Key, roomPosition, searchSpace, costs);
                }
            }
        }
    }

    return -1;
}

void ExtendSearchSpace (BurrowState pCurrentState, Vec2i pStart, Vec2i pEnd, PriorityQueue<BurrowState, long> pQueue, Dictionary<string, long> pCostTable)
{
    //Checks IF we can reach that position and if so what the cost would be...
    int costToReach = CostToReach(pCurrentState, pStart, pEnd);
    if (costToReach == -1) return;

    // Create the new state with the moved amphipod
    BurrowState newState = new BurrowState(pCurrentState);
    newState.Remove(pStart);
    newState.Add(pEnd, pCurrentState[pStart]);

    // Check if we already had this state (costly)...
    string currentStateId = GetStateIdentifier(pCurrentState);
    string newStateId = GetStateIdentifier(newState);

    long newCost = pCostTable[currentStateId] + costToReach;

    if (pCostTable.ContainsKey(newStateId) && newCost >= pCostTable[newStateId]) return;

    pCostTable[newStateId] = newCost;
    pQueue.Enqueue(newState, newCost);
}

bool IsFinalState (BurrowState pState)
{
    //Is every amphipod in the correct room?

    foreach (var amphipod in pState)
    {
        if (amphipod.Key.Y == 1) return false;  // in the hallway
        if (amphipod.Key.X != 3 + 2 * amphipod.Value) return false; // in the wrong room
    }

    return true;
}

string GetStateIdentifier (BurrowState pState)
{
    //To identify a state we need to know WHEN we are in the same state.
    //If we order our amphipods by type first and then by X and then by Y
    //we get a good unique key which doesn't distinguish different amphipods of the same type:
    //(ie A1 in (1,1) and A2 in (2,2) will give the same key as the other way around)
    return string.Concat(pState.OrderBy(x => x.Value * 1000 + x.Key.X * 100 + x.Key.Y));
}

Console.WriteLine("Part 1: " + CalculateSolutionCosts());

// Part 2 IS Part 1, we just need to modify the input...

