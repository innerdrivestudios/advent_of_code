// Solution for https://adventofcode.com/2023/day/20 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch module specifications...

string[] myInput = File.ReadAllLines(args[0]);

// ** For this puzzle I was tempted to go down the
// oh-lets-try-to-cram-everything-into-locally-defined-tuples-and-dictionaries approach,
// but expecting a debugging nightmare, I went with a full blown OO approach instead, 
// where a single BroadcasterModule is the facade/entry to the rest of the system.

BroadcasterModule broadcaster = null;

foreach (string moduleSpecification in myInput)
{
    AbstractModule newModule = null;
    if (moduleSpecification.StartsWith("broadcaster")) newModule = broadcaster = new BroadcasterModule(moduleSpecification);
    else if (moduleSpecification.StartsWith("%")) newModule = new FlipFlopModule(moduleSpecification.Substring(1));
    else if (moduleSpecification.StartsWith("&")) newModule = new Conjunction(moduleSpecification.Substring(1));
}

broadcaster.Initialize(null);

Console.WriteLine("Part 1: " + broadcaster.Part1());
Console.WriteLine("Part 2: " + broadcaster.Part2());

