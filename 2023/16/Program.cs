// Solution for https://adventofcode.com/2023/day/16 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a grid with mirrors!

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

Grid<char> mirrorField = new Grid<char>(myInput, Environment.NewLine);
//mirrorField.Print("");

Directions<Vec2i> directions = new ([new(1,0), new (0,1), new (-1,0), new (0,-1)]);

// What are our options?
// . -> don't do anything
// | -> if (index % 2 != 0) don't do anything
//      if (index % 2 == 0) create two new beams in directions 1 & 3
// - -> if (index % 2 == 0) don't do anything
//      if (index % 2 != 0) create two new beams in directions 0 & 2
// / -> if (index % 2 == 0) index-- else index++
// \ -> if (index % 2 == 0) index++ else index--

long GetEnergizedTileCount (Beam pStartingBeam)
{
	// First beam enters in the top left corner, heading to the right...
	List<Beam> beams = [pStartingBeam];
	HashSet<Vec2i> energizedTiles = new HashSet<Vec2i>() { beams[0].position };
	HashSet<(Vec2i, int)> visited = new HashSet<(Vec2i, int)>();

	while (beams.Count > 0)
	{
		for (int i = beams.Count - 1; i >= 0; i--)
		{
			Beam beam = beams[i];

			if (!mirrorField.IsInside(beam.position))
			{
				beams.RemoveAt(i);
			}
			else
			{
				energizedTiles.Add(beam.position);

				if (!visited.Add((beam.position, beam.direction)))
				{
					beams.RemoveAt(i);
					continue;
				}

				char content = mirrorField[beam.position];

				// . -> don't do anything
				//if (content == '.') continue;

				// directions = new ([new(1,0), new (0,1), new (-1,0), new (0,-1)]);
				// | -> if (index % 2 != 0) don't do anything
				//      if (index % 2 == 0) create two new beams in directions 1 & 3
				if (content == '|')
				{
					if (beam.direction % 2 == 0)
					{
						beams.RemoveAt(i);
						Beam a = new Beam(beam.position, 1);
						Beam b = new Beam(beam.position, 3);
						beams.Add(a);
						beams.Add(b);
					}
				}

				// - -> if (index % 2 == 0) don't do anything
				//      if (index % 2 != 0) create two new beams in directions 0 & 2
				else if (content == '-')
				{
					if (beam.direction % 2 != 0)
					{
						beams.RemoveAt(i);
						Beam a = new Beam(beam.position, 0);
						Beam b = new Beam(beam.position, 2);
						beams.Add(a);
						beams.Add(b);
					}
				}

				// / -> if (index % 2 == 0) index-- else index++
				else if (content == '/')
				{
					if (beam.direction % 2 == 0) beam.direction--; else beam.direction++;
					beam.direction = ((beam.direction % 4) + 4) % 4;
				}

				// \ -> if (index % 2 == 0) index++ else index--

				else if (content == '\\')
				{
					if (beam.direction % 2 != 0) beam.direction--; else beam.direction++;
					beam.direction = ((beam.direction % 4) + 4) % 4;
				}

				beam.position += directions.Get(beam.direction);
			}
		}
		//Console.WriteLine("Beam count:" + beams.Count);
	}
	return energizedTiles.Count;
}

Console.WriteLine("Part 1: "+GetEnergizedTileCount(new Beam (new(0,0), 0)));

// ** Part 2: Find the highest possible energized tile count based on the starting position
// Let's try brute force first !

long highestEnergizedTileCount = 0;

for (int x = 0; x < mirrorField.width;x++)
{
	highestEnergizedTileCount = long.Max(
			highestEnergizedTileCount,
			GetEnergizedTileCount(new Beam (new Vec2i(x, 0), 1))
		);

	highestEnergizedTileCount = long.Max(
			highestEnergizedTileCount,
			GetEnergizedTileCount(new Beam(new Vec2i(x, mirrorField.height-1), 3))
		);
}

for (int y = 0; y < mirrorField.height; y++)
{
	highestEnergizedTileCount = long.Max(
			highestEnergizedTileCount,
			GetEnergizedTileCount(new Beam(new Vec2i(0, y), 0))
		);

	highestEnergizedTileCount = long.Max(
			highestEnergizedTileCount,
			GetEnergizedTileCount(new Beam(new Vec2i (mirrorField.width - 1, y), 2))
		);
}

// Apparently brute forcing was good enough :)
Console.WriteLine("Part 2: "+highestEnergizedTileCount);