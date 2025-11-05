// Solution for https://adventofcode.com/2019/day/20 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a dungeon with portals

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// Turn the input into a grid

Grid<char> dungeon = new Grid<char>(myInput, Environment.NewLine);

// Locate potential portals, note all the characters for portals are added in
// top to bottom, left to right order

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

// Process potential portals, searching only down and right

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

int GetShortestPathCost ()
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


Console.WriteLine("Part 1: " + GetShortestPathCost());
