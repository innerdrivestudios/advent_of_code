// Solution for https://adventofcode.com/2019/day/18 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using System.Diagnostics;

// SearchNodePart1 describes:
// - at which key we are at (a-z),
// - which keys we have collected (a - z converted to 1 << 0-25 and merged as a bitset),
// - what it has cost us this far in terms of steps to get to this point

using SearchNodePart1 = (char currentLocation, int currentKeys, int currentCost);

// EdgeData describes:
// - which doors we encounter along the way from key a to b (the graph nodes)
// - which keys we encounter along the way from key a to b
// - the cost of travelling from key a to b

using EdgeData = (int doorsEncountered, int keysEncountered, int cost);

// More documentation on these aliases below

// For Part 2 we added this one:

// SearchNodePart2 describes:
// - at which key each of the 4 robots is (a-z),
// - which keys we have collected (a - z converted to 1 << 0-25 and merged as a bitset),
// - what it has cost us this far in terms of steps to get to this point

using SearchNodePart2 = (char[] currentLocations, int currentKeys, int currentCost);


// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a dungeon with keys...

// First, parse the input into a grid

Stopwatch sw = Stopwatch.StartNew();

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> dungeon = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1: What is the shortest path that allows you to collect all keys?

// This is basically an extended Travelling Salesman Problem, with:
// - a large amount of keys (too large to generate all route permutations and brute force it)
// - additional constraints on which keys we can collect since they might be behind locked doors

// To answer this, we'll first need to find out where:
// - the entrance is
// - all the keys are (stored in a map from char to Vec2i):
// 
// Note that we don't need to know where the doors are at this point, 
// we'll encounter them as we move from key to key automatically.

Vec2i entrance = new Vec2i();
Dictionary<char, Vec2i> keys = new();

dungeon.Foreach(
	(position, value) =>
	{
		if (value == '@')
		{
			entrance = position;
		}
		else if (char.IsAsciiLetterLower(value))
		{
			keys[value] = position;
		}
	}
);

// Now we know where the entrance is and where all keys are.
// We'll use this data to calculate and cache the path length from each key to each key,
// but we also need to *start* somewhere, so we'll also calculate and store the
// path length from the starting node @ to all keys.

// Why do we need this? 
//
// To solve the problem we need to figure out our route:
// - is it a-b-c-etc?
// - is it a-b-d-etc?
// In other words, we need the cost of going from x to y over and over again
// not really caring about the actual path, so that why we pre-cache those costs,
// to avoid re and re calculating them.

// At the same time: AS we are travelling from x to y, we will both encounter
// DOORS and OTHER KEYS, where:
// - the DOORS indicate the keys we need to collect first to even get there
// - the OTHER keys indicate the bonus keys we would pick up along the way

// There are different ways to store this set of required keys and encountered keys
// for example a list (slow) or a hashset (better), but in this case, since the
// amount of keys is limited we can use an even faster approach: a bit set.
// So basically every key a-z is converted into a number 0-25
// which is converted into a bit using 1 << (0-25)
// which can then be combined with other bits.

int GetBit(char pChar) => 1 << (char.ToLower(pChar) - 'a');

// With that, we can define a basic BFS search that keeps track of the cost as well:

EdgeData CalculateEdgeData(Vec2i pFrom, Vec2i pTo)
{
	// So we have a queue with points we need to explore, 
	// and the edge data up to the point (doorsEncounted, keysEncountered, cost)
    Queue<(Vec2i position, EdgeData edgeData)> queue = new();

	// Set up our edgeData (zero doors encountered, zero keys encountered, zero cost)
	EdgeData startData = new EdgeData(0, 0, 0);

	// If we start out at a key, bag it...
    if (char.IsAsciiLetterLower(dungeon[pFrom])) startData.keysEncountered |= GetBit(dungeon[pFrom]);

    // Now we start searching over our grid from start key position to end key position
    queue.Enqueue((pFrom, startData));
    
	HashSet<Vec2i> visited = new() { pFrom };

	Vec2i[] directions = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];

    while (queue.Count > 0)
    {
        var currentNode = queue.Dequeue();
        if (currentNode.position == pTo) return currentNode.edgeData;

        foreach (var direction in directions)
        {
            var nextPosition = currentNode.position + direction;

            if (!dungeon.IsInside(nextPosition) || dungeon[nextPosition] == '#') continue;
            if (visited.Contains(nextPosition)) continue;

			// Clone the edge data for the next node, so we can update it...
			EdgeData nextEdgeData = currentNode.edgeData;
			nextEdgeData.cost += 1;

			// Are we encountering another key?
            if (char.IsAsciiLetterLower(dungeon[nextPosition]))
            {
				nextEdgeData.keysEncountered |= GetBit(dungeon[nextPosition]);
            }
			// Or door?
			else if (char.IsAsciiLetterUpper(dungeon[nextPosition]))
			{
                nextEdgeData.doorsEncountered |= GetBit(dungeon[nextPosition]);
            }

			queue.Enqueue((nextPosition, nextEdgeData));
            visited.Add(nextPosition);
        }
    }

    return new EdgeData(-1, -1, -1);
}

// Now we'll build a map from any @/key char to the others...
// (even though we won't be using ALL of this data...)

EdgedGraph<char, EdgeData> CreateGraph (Dictionary<char, Vec2i> pCharsToPositionMap)
{
    List<char> charList = pCharsToPositionMap.Keys.ToList();

    // And build a map of all key to key costs...
    EdgedGraph<char, EdgeData> keyGraph = new();

    // We could do this based on positions only,
    // but that makes things even harder to understand later

    for (int i = 0; i < charList.Count - 1; i++)
    {
        for (int j = i + 1; j < charList.Count; j++)
        {
            char charA = charList[i];
            char charB = charList[j];
            EdgeData edgeData = CalculateEdgeData(pCharsToPositionMap[charA], pCharsToPositionMap[charB]);

            // If there is a path...
            if (edgeData.keysEncountered != -1)
            {
                keyGraph.AddEdge(charA, charB, edgeData);
            }
        }

    }

    return keyGraph;
}

// Clone the dictionary with keys and add '@'
Dictionary<char, Vec2i> allChars = new(keys) { ['@'] = entrance };
EdgedGraph<char, EdgeData> keyGraph = CreateGraph (allChars);

// With our cache table filled, we can look for the final piece of the puzzle (part 1 :))

int GetOptimalPathPart1()
{
    //Start out at '@' with zero keys collected (bitflag is 0) and a cost of 0
    SearchNodePart1 start = new SearchNodePart1('@', 0, 0);

    PriorityQueue<SearchNodePart1, float> priorityQueue = new();
    priorityQueue.Enqueue(start, 0);

    Dictionary<string, int> visited = new();

    // What will our keys collected bitset look like if we collected all keys?
    int allKeysMask = (int)(Math.Pow(2, keys.Count) - 1);

    while (priorityQueue.Count > 0)
    {
        //Did we find all keys?
        SearchNodePart1 current = priorityQueue.Dequeue();
        if (current.currentKeys == allKeysMask) return current.currentCost;

        // If not, get the current key...
        int currentKey = (current.currentLocation - 'a');

        // And iterate over all keys, that...
        for (int i = 0; i < keys.Count; i++)
        {
            //... are not us...
            if (i == currentKey) continue;

            //... are not already in our possession:
            int nextKeyBitFlag = 1 << i;
            if ((current.currentKeys & nextKeyBitFlag) == 1) continue;

            // Are reachable with our current set of collected keys:
            // Where would we go?
            char targetKeyChar = (char)('a' + i);
            EdgeData edgeData = keyGraph.GetEdgeData(current.currentLocation, targetKeyChar);
            if ((current.currentKeys & edgeData.doorsEncountered) != edgeData.doorsEncountered) continue;

            // If we get here, we match all requirements to collect this new key, 
            // so update our keyset we would have after moving here:
            int newKeySet = current.currentKeys | edgeData.keysEncountered;
            // the cost...
            int newCost = current.currentCost + edgeData.cost;

            // Make sure we don't keep redoing things...
            string key = "" + targetKeyChar + " " + newKeySet;

            if ((visited.ContainsKey(key) && visited[key] > newCost) || !visited.ContainsKey(key))
            {
                SearchNodePart1 newNode = new SearchNodePart1(targetKeyChar, newKeySet, newCost);
                priorityQueue.Enqueue(newNode, newNode.currentCost);
                visited[key] = newNode.currentCost;
            }
        }
    }
    return -1;
}

int shortestPathCost = GetOptimalPathPart1();
Console.WriteLine("Part 1: " + shortestPathCost);
Console.WriteLine("Calculated in:" + sw.ElapsedMilliseconds);

// ** Part 2: But what if we put some extra walls in and have four robots moving around?

// Let's first update our dungeon according to specs:

sw.Restart();
dungeon.ForeachRegion(entrance - new Vec2i(1,1), new Vec2i(3, 3), (pos, value) => dungeon[pos] = '#');

// And define the new entrances:
Vec2i[] entrances = [entrance - new Vec2i(1, 1), entrance + new Vec2i(1, 1), entrance - new Vec2i(-1, 1), entrance + new Vec2i(-1, 1)];

// Now we need to rebuild all data structures that we built before
// But we can't use @ four times ;), so we'll just use 1,2,3,4 one for each robot

allChars = new(keys);

for (int i = 0; i < entrances.Length; i++)
{
    char entranceChar = (char)('1' + i);
    dungeon[entrances[i]] = entranceChar;
    allChars[entranceChar] = entrances[i];
}

keyGraph = CreateGraph(allChars);

// Other than that we don't need to define a lot of extra info, we just have to rewrite our search algorithm,
// so that instead of 1 searcher, we have a list of searchers, with a shared collection of keys.
// Probably also a bit of optimization so that a robot doesn't keep trying to reach keys the robot can never reach:

int GetReachableSet (char pRobot)
{
    int reachableSet = 0;

    for (int i = 0; i < keys.Count; i++)
    {
        reachableSet |= keyGraph.HasEdgeData(pRobot, (char)('a' + i)) ? 1 << i : 0;
    }

    return reachableSet;
}

/**
// For debugging, show which robot can reach with key:
Console.WriteLine("zyxwvutsrqponmlkjihgfedcba");

for (int i = 0; i < 4; i++)
{
    Console.WriteLine(Convert.ToString (GetReachableSet((char)('1' + i)), 2).PadLeft(26) + "  <== Robot " + (i+1) + " can reach");
}

/**/
//dungeon.Print("");

int GetOptimalPathPart2()
{
    // What do we need to keep track of for this search?
    // We need to keep track of where 4 robots are.
    // Start out at 1,2,3,4 with zero keys collected (bitflag is 0) and a cost of 0

    SearchNodePart2 start = new(new char[] { '1', '2', '3', '4' }, 0, 0);

    // Set up the queue
    PriorityQueue<SearchNodePart2, float> priorityQueue = new();
    priorityQueue.Enqueue(start, 0);
    
    // And the visited list
    Dictionary<string, int> visited = new();

    // Also calculate which robot can reach which keys, so we don't do dumb searching...
    int[] reachableKeySets = new int[entrances.Length];
    
    for (int i = 0; i < 4; i++)
    {
        reachableKeySets[i] = GetReachableSet((char)('1' + i));
    }

    // What will our keys collected bitset look like if we collected all keys?
    int allKeysMask = (int)(Math.Pow(2, keys.Count) - 1);

    while (priorityQueue.Count > 0)
    {
        //Did we find all keys?
        SearchNodePart2 current = priorityQueue.Dequeue();
        if (current.currentKeys == allKeysMask) return current.currentCost;

        // If not, check all options, for all robots...
        for (int robot = 0; robot < 4; robot++)
        {
            int currentKey = (current.currentLocations[robot] - 'a');

            // And iterate over all keys, that...
            for (int i = 0; i < keys.Count; i++)
            {
                //... are not where the robot is...
                if (i == currentKey) continue;

                //... are not already in possession of any of the robots:
                int nextKeyBitFlag = 1 << i;
                if ((current.currentKeys & nextKeyBitFlag) == 1) continue;

                //... can be reached by this robot
                if ((reachableKeySets[robot] & nextKeyBitFlag) == 0) continue;

                // Are reachable with our current set of collected keys:
                // Where would we go?
                char targetKeyChar = (char)('a' + i);
                EdgeData edgeData = keyGraph.GetEdgeData(current.currentLocations[robot], targetKeyChar);
                if ((current.currentKeys & edgeData.doorsEncountered) != edgeData.doorsEncountered) continue;

                // If we get here, we match all requirements to collect this new key, 
                // so update our keyset we would have after moving here:
                int newKeySet = current.currentKeys | edgeData.keysEncountered;
                // the cost...
                int newCost = current.currentCost + edgeData.cost;

                // Make sure we don't keep redoing things...

                char[] newRobotStates = (char[])current.currentLocations.Clone();
                newRobotStates[robot] = targetKeyChar;

                string key = "" + string.Join ("",targetKeyChar) + " " + newKeySet;

                if ((visited.ContainsKey(key) && visited[key] > newCost) || !visited.ContainsKey(key))
                {
                    SearchNodePart2 newNode = new SearchNodePart2 (newRobotStates, newKeySet, newCost);
                    priorityQueue.Enqueue(newNode, newNode.currentCost);
                    visited[key] = newNode.currentCost;
                }
            }
        }
    }
    return -1;
}

Console.WriteLine("Part 2: " + GetOptimalPathPart2());
Console.WriteLine("Calculated in:" + sw.ElapsedMilliseconds);
