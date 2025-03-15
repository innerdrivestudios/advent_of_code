// Solution for https://adventofcode.com/2018/day/3 (Ctrl+Click in VS to follow link)

using Vec2i = Vec2<int>;
using Plot = (int id, Vec2<int> position, Vec2<int> size);
using System.Text.RegularExpressions;

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of plots:
// Each plot has the following structure:
// #id @ x,y: widthXheight

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1 - How many units of plots are overlapping with other plots?
// Note: if a unit is claimed by 3 plots, it still only counts as 1 overlapping unit

// Approach -
// * create a grid of ints, initialized to 0
// * process each plot, increasing the int, if the int overwritten == 1 increase the overlap counter

// Set up the grid

Grid<int> fabric = new Grid<int>(1000, 1000);
fabric.Foreach((position, value) => fabric[position] = 0);

// Parse the plots

Regex plotParser = new Regex(@"#(\d+) @ (\d+),(\d+): (\d+)x(\d+)");
MatchCollection matchCollection = plotParser.Matches(myInput);

List<Plot> plots = new List<Plot>();

foreach (Match match in matchCollection)
{
    Plot plot = new Plot();
    plot.id =       int.Parse(match.Groups[1].Value);
    plot.position = new Vec2i (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
    plot.size =     new Vec2i (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value));
    plots.Add(plot);
}

// Map and count the plots

int doublyAllocated = 0;                                    

foreach (Plot plot in plots)
{
    fabric.ForeachRegion(
        plot.position,
        plot.size,
        (position, value) =>
        {
            if (fabric[position] == 1) doublyAllocated++;
            fabric[position] += 1;
        }
    );
}

Console.WriteLine("Part 1 - Doubly allocated: " + doublyAllocated);

// ** Part 2 - Check which plots are ok without any overlap, basically all plots for which the overlap count is 0

List<int> plotsWithoutOverlap = new List<int>();            

foreach (Plot plot in plots)
{
    int overlapCountForThisPlot = 0;
    fabric.ForeachRegion(
        plot.position,
        plot.size,
        (position, value) =>
        {
            if (fabric[position] > 1) overlapCountForThisPlot++;
        }
    );
    if (overlapCountForThisPlot == 0) plotsWithoutOverlap.Add(plot.id);
}

Console.WriteLine("Part 2 - Plots without overlap: " + string.Join (",", plotsWithoutOverlap));