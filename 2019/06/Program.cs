// Solution for https://adventofcode.com/2019/day/6 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of planet pairs indicating what is orbiting what

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Parse all the planet pairs

(string planetA, string planetB)[] planetPairs = myInput
	.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
	.Select (x => x.Split (")", StringSplitOptions.RemoveEmptyEntries))
	.Select (x => (x[0], x[1]))
	.ToArray ();

// Now set up a inverse dependency map (parent-child map from root planet to dependent planet)

Dictionary<string, HashSet<string>> planetDependencies = new();

foreach (var planetPair in planetPairs)
{
	if (!planetDependencies.TryGetValue(planetPair.planetA, out HashSet<string> result))
	{
		planetDependencies[planetPair.planetA] = result = new HashSet<string> ();
	}

	result.Add (planetPair.planetB);
}

// ** Part 1: Count all direct and indirect orbits...
// This is basically nothing more than adding up the depths of every node...

int GetCumulativeDepthCount (string pRootNode, int pDepth = 0)
{
	if (planetDependencies.TryGetValue(pRootNode, out HashSet<string> children))
	{
		int cumulativeDepthCount = pDepth;

		foreach (var child in children)
		{
			cumulativeDepthCount += GetCumulativeDepthCount(child, pDepth + 1);
		}
		
		return cumulativeDepthCount;
	}

	return pDepth;
}

Console.WriteLine("Part 1:" + GetCumulativeDepthCount("COM"));

// ** Part 2: How many steps does it take to get YOU next to SAN?
// For this we'll need to know the path to YOU, the path to SAN and how much of the path is shared.

// pNode - the node we are looking for
// pParent - the parent node we start looking at
// pPath  - the path so far
List<string> GetPath (string pNode, string pParent = "COM", List<string> pPath = null)
{
	// Update the path history...
	
	pPath = pPath ?? new();
	pPath.Add(pParent);

	// Search our children...

	if (planetDependencies.TryGetValue(pParent, out HashSet<string> children))
	{
		// Do we contain the child? DONE!
		if (children.Contains(pNode)) return pPath;

		// Otherwise continue the child search... making sure you make a copy of the list
		// since we don't want to corrupt the results by moving the list up and down the call hierarchy...
		foreach (var child in children)
		{
			List<string> path = GetPath(pNode, child, new (pPath));
			if (path != null) return path;
		}
	}

	// Nothing found? Return null...
	return null;
}

List<string> santaPath = GetPath("SAN");
List<string> yourPath = GetPath("YOU");

//Now we have both paths, cut off the shared bits (just skipping would be faster, but anyway)
while (santaPath[0] == yourPath[0])
{
	santaPath.RemoveAt(0);
	yourPath.RemoveAt(0);
}

Console.WriteLine("Part 2 - Path from you to santa:" +(santaPath.Count + yourPath.Count));

