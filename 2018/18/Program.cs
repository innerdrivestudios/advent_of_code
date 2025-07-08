//Solution for https://adventofcode.com/2018/day/18 (Ctrl+Click in VS to follow link)

using System.Text;
using System.Xml;
using Vec2i = Vec2<int>;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a lumber collection area...

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

// ** Processing the input:

Grid<char> lumberArea = new Grid<char>(myInput, Environment.NewLine);

// ** Part 1 : What will the total resource value of the lumber collection area be after 10 minutes?

// First let's define some helper methods and variables

Vec2i[] neighbours = [
        new (-1,-1),    new (0,-1),     new (1,-1),
        new (-1,0),                     new (1,0),
        new (-1,1),     new (0,1),      new (1,1)
    ];

(int trees, int open, int lumberyards) CountNeighbours (Grid<char> pGrid, Vec2i pPosition)
{
    (int trees, int open, int lumberyards) result = (0, 0, 0);

    foreach (var offset in neighbours)
    {
        Vec2i position = pPosition + offset;

        if (!pGrid.IsInside (position)) continue;

        if (pGrid[position] == '.') result.open++;
        else if (pGrid[position] == '#') result.lumberyards++;
        else if (pGrid[position] == '|') result.trees++;
    }

    return result;
}

// Now we'll do the modified game of life

Grid<char> RunGameOfLife (Grid<char> pInput, Grid<char> pOutput) {

    pInput.Foreach(

        (position, value) =>
        {
            var neighbourCounts = CountNeighbours (pInput, position);

            if (value == '.')
            {
                pOutput[position] = neighbourCounts.trees >= 3 ? '|' : '.';
            }
            else if (value == '|')
            {
                pOutput[position] = neighbourCounts.lumberyards >= 3 ? '#' : '|';
            }
            else if (value == '#')
            {
                pOutput[position] = (neighbourCounts.lumberyards >= 1 && neighbourCounts.trees >= 1) ? '#' : '.';
            }
        }

    );

    return pOutput;
}

// Now run the game of life...

Grid<char> input = lumberArea;
Grid<char> output = new Grid<char>(lumberArea.width, lumberArea.height);

for (int i = 0; i < 10; i++)
{
    Grid<char> tmp = input;
    input = RunGameOfLife(input, output);
    output = tmp;

    //Console.Clear();
    //Console.WriteLine(i);
    //output.Print("");
    //Console.ReadKey();
}

// And count the results...

(int trees, int open, int lumberyards) totalCounts = (0, 0, 0);

// Important: INPUT carries the last result!
input.Foreach(
    (pos, value) =>
    {

        if (value == '.') totalCounts.open++;
        else if (value == '|') totalCounts.trees++;
        else if (value == '#') totalCounts.lumberyards++;
    }
);

Console.WriteLine("Part 1:" + (totalCounts.trees * totalCounts.lumberyards));

// ** Part 2: What is the state after 1000000000 minutes?

// Let's see if we can detect some kind of pattern with a brute force string hash

int iteration = 0;
HashSet<string> visited = new();

string GetStringHash (Grid<char> pInput)
{
    StringBuilder sb = new StringBuilder();
    pInput.Foreach((pos,value) => sb.Append(value));
    return sb.ToString();
}

while (true)
{
    Grid<char> tmp = input;
    input = RunGameOfLife(input, output);
    output = tmp;

    iteration++;    

    //input carries the last result...
    if (!visited.Add (GetStringHash(input)))
    {
        break;
    }
}

Console.WriteLine("Part 2: ..." );
Console.WriteLine("Pattern detected after: " + iteration + " iterations...");

int iterationsToComplete = 1000000000 - 10 /* Part 1 */ - iteration;

// How many can we remove since they'd repeat anyway?
iterationsToComplete -= ((iterationsToComplete / iteration) * iteration);

// Complete the remaining iterations:

for (int i = 0; i < iterationsToComplete; i++)
{
    Grid<char> tmp = input;
    input = RunGameOfLife(input, output);
    output = tmp;
}

totalCounts = (0, 0, 0);

input.Foreach(
    (pos, value) =>
    {

        if (value == '.') totalCounts.open++;
        else if (value == '|') totalCounts.trees++;
        else if (value == '#') totalCounts.lumberyards++;
    }
);

Console.WriteLine("Part 2:" + (totalCounts.trees * totalCounts.lumberyards));



