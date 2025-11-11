// Solution for https://adventofcode.com/2019/day/20 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a dungeon with portals

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// Turn the input into a grid

Grid<char> dungeon = new Grid<char>(myInput, Environment.NewLine);

// Locate potential portal by looking at UPPER CASE characters:
// Note all the characters for portals are added in top to bottom, left to right order

List<Vec2i> potentialPortals = new();
dungeon.Foreach(
	(pos, value) =>
	{
		if (char.IsUpper(value))
		{
			//Console.WriteLine(value);
			potentialPortals.Add(pos);
		}
		
	}
);

// Process potential portals, searching only down and right,
// since due to the way we added the potential portals we added them in top to bottom, left to right order

Vec2i[] directions = [new(1, 0), new(0, 1)];
Dictionary<string, List<Vec2i>> portalCoordinates = new();

while (potentialPortals.Count > 0)
{
	Vec2i current = potentialPortals[0];
	potentialPortals.RemoveAt(0);

	// Portals are isolated in a way that we can easily figure out if
	// the portal is going right or down
	Vec2i portalEndRight = current + directions[0];
	Vec2i portalEndDown = current + directions[1];

	string portalId = "";
	Vec2i portalCoordinate = new Vec2i();

	// based on that we remove the end point (either right or down)

	if (potentialPortals.Contains(portalEndRight))
	{
		portalId = new string([dungeon[current], dungeon[portalEndRight]]);
		//Console.WriteLine("Located:" + portalId);
		potentialPortals.Remove(portalEndRight);

		// If the portal chars are found, we still need to find the actual portal position
		Vec2i testPosition = current - directions[0];
		if (dungeon.IsInside(testPosition) && dungeon[testPosition] == '.') portalCoordinate = testPosition;
		else portalCoordinate = portalEndRight + directions[0];
	}
	else if (potentialPortals.Contains(portalEndDown))
	{
		portalId = new string ([dungeon[current], dungeon[portalEndDown]]);
		//Console.WriteLine("Located:" + portalId);
		potentialPortals.Remove(portalEndDown);

		// If the portal chars are found, we still need to find the actual portal position
		Vec2i testPosition = current - directions[1];
		if (dungeon.IsInside (testPosition) && dungeon[testPosition] == '.') portalCoordinate = testPosition;
		else portalCoordinate = portalEndDown + directions[1];
	}

	if (!portalCoordinates.ContainsKey(portalId)) portalCoordinates[portalId] = new();

	portalCoordinates[portalId].Add(portalCoordinate);
}

// Almost done setting up, now we need to connect the portal positions to eachother:
// In other words, we don't really need to know WHICH portal is WHICH

Dictionary<Vec2i, Vec2i> portalLinks = new();

foreach (var portalKeyValue in portalCoordinates)
{
	if (portalKeyValue.Value.Count == 2)
	{
		portalLinks[portalKeyValue.Value[0]] = portalKeyValue.Value[1];
		portalLinks[portalKeyValue.Value[1]] = portalKeyValue.Value[0];
	}
}

// Get the start and the end position:
Vec2i start = portalCoordinates["AA"][0];
Vec2i end = portalCoordinates["ZZ"][0];

// End run the basic dijkstra search...

int GetShortestPathCostPart1 ()
{
	Queue<Vec2i> queue = new Queue<Vec2i>();
	queue.Enqueue(start);

	Dictionary<Vec2i, int> costs = new Dictionary<Vec2i, int>();
	costs[start] = 0;

	Vec2i[] directions = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];

	while (queue.Count > 0)
	{
		Vec2i current = queue.Dequeue();
		if (current == end) return costs[end];

		// First check the normal directions
		foreach (var direction in directions)
		{
			Vec2i newPosition = current + direction;
			if (dungeon[newPosition] != '.') continue;

			//Already visited
			if (costs.ContainsKey(newPosition)) continue;

			queue.Enqueue(newPosition);
			costs[newPosition] = costs[current] + 1;
		}

		// Now check whether there is a portal we can jump to:
		if (portalLinks.ContainsKey(current))
		{
			Vec2i newPosition = portalLinks[current];

			//Already visited
			if (costs.ContainsKey(newPosition)) continue;

			queue.Enqueue(newPosition);
			costs[newPosition] = costs[current] + 1;
		}
	}

	return -1;
}

Console.WriteLine("Part 1: " + GetShortestPathCostPart1());

// ** Part 2: We have to CO(DE) deeper :) Brilliant :).

// So we'll need to know which portals are on the outside and which portals are on the inside...

// Well, actually, we only need to know if a portal is on the inside, because if it is not,
// it is on the outside!

HashSet<Vec2i> insidePortals = new();

// We can check this by looking at the distance from the center between two portals;
// the one closest will be an inner portal, the furthest an outer portal

Vec2i center = new Vec2i(dungeon.width / 2, dungeon.height / 2);

foreach (var portal in portalCoordinates)
{
	// If the portal doesn't have 1 entry and 1 exit skip
	if (portal.Value.Count != 2) continue;

	// Now figure out which portal is closest to the center...
	Vec2i coord1 = portal.Value[0];
	Vec2i coord2 = portal.Value[1];

	if ((coord1 - center).MaxAbsCoord() < (coord2 - center).MaxAbsCoord())
	{
		insidePortals.Add (coord1);
	}
	else
	{
		insidePortals.Add (coord2);
	}
}

// With this info we can rewrite our path finding mechanism


int GetShortestPathCostPart2()
{
	// Search elements are now 
	Queue<(Vec2i position, int level)> queue = new ();

	(Vec2i position, int level) startNode = (start, 0);

    queue.Enqueue(startNode);
	
	// Set up cost table 
	Dictionary<(Vec2i, int), int> costs = new ();
	costs[startNode] = 0;

	Vec2i[] directions = [new(-1, 0), new(0, -1), new(1, 0), new(0, 1)];

	while (queue.Count > 0)
	{
		(Vec2i, int) current = queue.Dequeue();
		if (current == (end, 0)) return costs[current];

		Vec2i currentPosition = current.Item1;
		int currentLevel = current.Item2;
		int currentCost = costs[current];
		int newCost = currentCost + 1;

		// First check the normal directions
		foreach (var direction in directions)
		{
			Vec2i newPosition = currentPosition + direction;
			if (dungeon[newPosition] != '.') continue;

			//Already visited
			if (costs.ContainsKey((newPosition, currentLevel))) continue;

			costs[(newPosition, currentLevel)] = newCost;
			queue.Enqueue((newPosition, currentLevel));
		}

		// Now check whether there is a portal we can jump to:
		if (portalLinks.ContainsKey(currentPosition))
		{
			if (currentLevel == 0 && !insidePortals.Contains(currentPosition)) continue;

			Vec2i newPosition = portalLinks[currentPosition];
			int newLevel = currentLevel + (insidePortals.Contains(currentPosition) ? 1 : -1);

			var newNode = (newPosition, newLevel);

			//Already visited
			if (costs.ContainsKey(newNode)) continue;

			costs[newNode] = newCost;
			queue.Enqueue(newNode);
		}
	}

	return -1;
}

Console.WriteLine("Part 2: " + GetShortestPathCostPart2());

// Build a cache table from all outside portals to all outside portals going 1 up (-1)
// Build a cache table from all inside to all inside portals stay 