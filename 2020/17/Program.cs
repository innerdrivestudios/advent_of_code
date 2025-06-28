// Solution for https://adventofcode.com/2020/day/17 (Ctrl+Click in VS to follow link)

using Vec4i = Vec4<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: 

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();
Grid<char> flatRegion = new Grid<char>(myInput, Environment.NewLine);

// ** Since this might be a region of unending space and we have no idea where this is
// going, we'll use a HashSet<> to store active flags...

// Due to part 2 where we'll have to take things into 4D, we define all cubes in 4D already, 
// zero-ing out the W part for now (and since we are provided with an initial 2D slice, we
// also zero out the Z part for the initial active cubes).

HashSet<Vec4i> activeCubes = new();

flatRegion.Foreach(
	(pos, value) =>
	{
		if (value == '#') activeCubes.Add(new Vec4i(pos.X, pos.Y, 0, 0));
	}
);

// We also need to know our neighboring cubes, we'll pre-generate the offsets,
// again zero-ing out the W part, and making sure the center is ignored.
List<Vec4i> neighborOffsets3D = new();

// Generate all offsets ...
for (int x = -1; x <= 1; x++)
	for (int y = -1; y <= 1; y++)
		for (int z = -1; z <= 1; z++)
			neighborOffsets3D.Add(new Vec4i(x, y, z, 0));

// Ignore the center ...
neighborOffsets3D.Remove(new Vec4i(0, 0, 0,0));

HashSet<Vec4i> DoCycle(HashSet<Vec4i> pInput, List<Vec4i> pNeighborSet)
{
	HashSet<Vec4i> newStates = new();
	HashSet<Vec4i> visited = new();

	//Iterate over all active cubes...
	foreach (Vec4i pos in pInput)
	{
		visited.Add(pos);
		// "If a cube is active and exactly 2 or 3 of its neighbors are also active,
		// the cube remains active. Otherwise, the cube becomes inactive."
		int activeNeighbors = CountActiveNeighbors(pInput, pNeighborSet, pos);

		if (activeNeighbors == 2 || activeNeighbors == 3)
		{
			newStates.Add(pos);
		}

		//"If a cube is inactive but exactly 3 of its neighbors are active,
		// the cube becomes active. Otherwise, the cube remains inactive."

		// Now we can't test EVERYTHING, but we do know that we need to run this
		// test for each neighbor of pos that is not active...
		
		foreach(Vec4i neighbor in pNeighborSet)
		{
			Vec4i neighborPosition = pos + neighbor;
			if (visited.Contains(neighborPosition)) continue;	//Don't redo work
			if (pInput.Contains(neighborPosition)) continue;    //Only count inactive

			visited.Add(neighborPosition);
			int activeNeighbors2 = CountActiveNeighbors(pInput, pNeighborSet, neighborPosition);
			if (activeNeighbors2 == 3) newStates.Add(neighborPosition);
		}
	}

	return newStates;
}

int CountActiveNeighbors (HashSet<Vec4i> pActiveSet, List<Vec4i> pNeighbourSet, Vec4i pPosition, int pMax = 3)
{
	int activeNeighbors = 0;
	foreach (Vec4i pos in pNeighbourSet)
	{
		activeNeighbors += (pActiveSet.Contains(pPosition + pos)) ? 1 : 0;
		if (activeNeighbors > pMax) return activeNeighbors;
	}
	return activeNeighbors;
}

HashSet<Vec4i> activeCubes3D = new HashSet<Vec4i>(activeCubes);

for (int i = 0; i < 6; i++) {
	activeCubes3D = DoCycle(activeCubes3D, neighborOffsets3D);
}

Console.WriteLine("Part 1:" + activeCubes3D.Count);

// ** Part 2: Now take it into 4 dimensions...

List<Vec4i> neighborOffsets4D = new();

for (int x = -1; x <= 1; x++)
	for (int y = -1; y <= 1; y++)
		for (int z = -1; z <= 1; z++)
			for (int w = -1; w <= 1; w++)
			neighborOffsets4D.Add(new Vec4i(x, y, z, w));

neighborOffsets4D.Remove(new Vec4i(0, 0, 0, 0));

HashSet<Vec4i> activeCubes4D = new HashSet<Vec4i>(activeCubes);

for (int i = 0; i < 6; i++)
{
	activeCubes4D = DoCycle(activeCubes4D, neighborOffsets4D);
}

Console.WriteLine("Part 2:" + activeCubes4D.Count);
