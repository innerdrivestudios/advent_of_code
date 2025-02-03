//Solution for https://adventofcode.com/2016/day/17 (Ctrl+Click in VS to follow link)

using System.Security.Cryptography;
using System.Text;
using Vec2i = Vec2<int>;

// ** Your input: part of an MD5 key

// In visual studio you can modify the char sequence used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

string myInput = args[0];

// ** The challenge: path finding in a 4 x 4 grid where we need to go from (0,0) to (3,3)
//    The kicker: whether a cell is reachable from another cell is determined by inspecting
//    the result of a hexadecimal MD5 hash.
//    The MD5 hash is constructed by using our puzzle input and the road we've travelled so far:
//    U)p, D)own, L)eft, R)ight
//    Given the hexadecimal hash, its first 4 characters specify the state of the doors in our cell:
//      [0] describes up, [1] down, [2] left, [3] right
//    If the character b, c, d, e, f means the door is open, any other char means it is closed.

// Let's first define the start and end position...

Vec2i startPosition = new Vec2i(0, 0);
Vec2i endPosition = new Vec2i(3, 3);

// Followed by some definitions we'll need for navigating... note that in the direction array MATTERS
// It matches this "[0] describes up, [1] down, [2] left, [3] right"

//X = > & Y = v

string directionChars = "UDLR";
Vec2i[] directions = { new Vec2i(0, -1), new Vec2i(0, 1), new Vec2i(-1, 0), new Vec2i(1, 0) };

// Add a method to test whether a position is valid at all:

bool IsValid (Vec2i pPosition)
{
    return  pPosition.X >= 0 && pPosition.X <= 3 &&
            pPosition.Y >= 0 && pPosition.Y <= 3;
}

// The interesting thing is that since our MD5 input changes based on the path we take,
// we need to keep track of some kind of history per node we travel,
// we could either do this recursively (keeping track of the history on the stack),
// or by adding a node class which keeps tracks of position and the path so far...

// Let's try it recursively... :)

string GetShortestPath (string pPuzzleInput, Vec2i pCurrentPosition, Vec2i pGoal, string pPathSoFar = "")
{
    //if we are where we need to go, we are done :)
    if (pCurrentPosition == pGoal) return pPathSoFar;

    //if not, let's ask all of our children

    string shortestPath = null;

    //steered by the hexString of our lowercase MD5 hash 
    string hexString =
        Convert.ToHexString(
            MD5.HashData(
                Encoding.ASCII.GetBytes(pPuzzleInput + pPathSoFar)
                )
        ).ToLower();

    //step through all the possible directions
    for (int i = 0; i < directions.Length; i++)
    {
        Vec2i newPosition = pCurrentPosition + directions[i];

        //but only process the valid ones (inside the grid)
        if (!IsValid(newPosition)) continue;

        //So for direction i we look at the i-th char in the hexstring, if this is in bcdef, the door is open
        if ("bcdef".Contains(hexString[i]))
        {
            //if the door is open continue searching from our newPosition to the goal, and update the path so far
            //using the ith character from our directionChars string
            string pathFound = GetShortestPath(pPuzzleInput, newPosition, pGoal, pPathSoFar + directionChars[i]);
            if (pathFound != null && (shortestPath == null || pathFound.Length < shortestPath.Length)) shortestPath = pathFound;
        }
    }

    return shortestPath;
}

Console.WriteLine("Part 1 - Shortest path: " + GetShortestPath(myInput, startPosition, endPosition));

// ** Part 2: Now get the length of the longest path ...

string GetLongestPath(string pPuzzleInput, Vec2i pCurrentPosition, Vec2i pGoal, string pPathSoFar = "")
{
    //if we are where we need to go, we are done :)
    if (pCurrentPosition == pGoal) return pPathSoFar;

    //if not, let's ask all of our children

    string longestPath = null;

    //steered by the hexString of our lowercase MD5 hash 
    string hexString =
        Convert.ToHexString(
            MD5.HashData(
                Encoding.ASCII.GetBytes(pPuzzleInput + pPathSoFar)
                )
        ).ToLower();

    //step through all the possible directions
    for (int i = 0; i < directions.Length; i++)
    {
        Vec2i newPosition = pCurrentPosition + directions[i];

        //but only process the valid ones (inside the grid)
        if (!IsValid(newPosition)) continue;

        //So for direction i we look at the i-th char in the hexstring, if this is in bcdef, the door is open
        if ("bcdef".Contains(hexString[i]))
        {
            //if the door is open continue searching from our newPosition to the goal, and update the path so far
            //using the ith character from our directionChars string
            string pathFound = GetLongestPath(pPuzzleInput, newPosition, pGoal, pPathSoFar + directionChars[i]);
            if (pathFound != null && (longestPath == null || pathFound.Length > longestPath.Length)) longestPath = pathFound;
        }
    }

    return longestPath;
}

Console.WriteLine("Part 2 - Longest path length: " + GetLongestPath(myInput, startPosition, endPosition).Length);
