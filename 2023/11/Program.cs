// Solution for https://adventofcode.com/2023/day/11 (Ctrl+Click in VS to follow link)

using System.Runtime.InteropServices;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a galaxy map

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Step 1 as usual is parsing the input into a usable format.
Grid<char> galaxy = new Grid<char>(myInput, Environment.NewLine);

// Now there are two ways about solving the first part...
// We could actually expand the galaxy, or we could just register 
// where the empty rows and columns are so we can adjust our calculations
// based on that data

// Going with the 2nd approach for now...
// But we'll store this in a weird format, by actually storing the additional
// size a row or column has...
List<int> emptyRows = Enumerable.Range(0, galaxy.height).Select (x => 1).ToList();
List<int> emptyColumns = Enumerable.Range(0, galaxy.width).Select(x => 1).ToList();
List<Vec2i> galaxies = new();

galaxy.Foreach(
	(pos, value) =>
	{
		//we are only looking at # values
		if (value != '#') return;

		//if we found a # there is a galaxy there and additional distance = 0
		galaxies.Add(pos);
		emptyColumns[pos.X] = 0;
		emptyRows[pos.Y] = 0;
	}
);

long GatherDistances (long pEmptyCost)
{
	// Now gather all distances
	long totalDistance = 0;

	for (int i = 0; i < galaxies.Count - 1; i++)
	{
		for (int j = i + 1; j < galaxies.Count; j++)
		{
			//base distance...
			totalDistance += (galaxies[j] - galaxies[i]).ManhattanDistance();

			//now collect additional distance incurred by empty rows and columns		
			int minX = int.Min(galaxies[i].X, galaxies[j].X);
			int maxX = int.Max(galaxies[i].X, galaxies[j].X);
			int minY = int.Min(galaxies[i].Y, galaxies[j].Y);
			int maxY = int.Max(galaxies[i].Y, galaxies[j].Y);

			Span<int> addRows = CollectionsMarshal.AsSpan(emptyRows).Slice(minY, maxY - minY);
			Span<int> addColumns = CollectionsMarshal.AsSpan(emptyColumns).Slice(minX, maxX - minX);

			foreach (int row in addRows) totalDistance += row * (pEmptyCost-1);
			foreach (int col in addColumns) totalDistance += col * (pEmptyCost - 1);

		}
	}

	return totalDistance;	
}

Console.WriteLine("Part 1 " + GatherDistances(2));

// ** Part 2 - Thank god for choosing the right approach ;)

Console.WriteLine("Part 2 " + GatherDistances(1000000));

