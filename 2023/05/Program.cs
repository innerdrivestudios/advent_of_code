//Solution for https://adventofcode.com/2023/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of values and remapping instructions to map these values to other values

// Each line within a map contains three numbers:
// the destination range start, the source range start, and the range length.
// e.g. 50 98 2 indicates source = 98, dest = 50, range = 2
// (yes, the swapping of source and dest is confusing)
// source = 98, dest = 50, range = 2 means:
// 98-99 (2 values) maps to 50-51 (two values)

// So our first task as usual is parsing the input again.
// The input is divided into a list of input values and blocks of remapping instructions.
// The remapping instructions have nice names, but nobody cares, these are basically just stages / levels
// in the remapping process...

// An input value goes through the first block providing an output value,
// this output value goes into the next block... etc

// How do we set this up? Let's first load the whole file as usual...

using System.Diagnostics;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Then split the blocks into separate entities

string[] myInputBlocks = myInput
    .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Then get the seeds....

long[] seeds = ParseSeeds(myInputBlocks[0]);

long[] ParseSeeds(string pSeedInput)
{
    //Input looks like "seeds: 1636419363 608824189 3409451394 227471750 12950548 91466703 ...."
    //So we'll cut of seeds: and then convert the rest:

    return pSeedInput
        .Substring(7)
        .Split(" ", StringSplitOptions.RemoveEmptyEntries)
        .Select(long.Parse)
        .ToArray();
}

// And the different remap blocks... 
// Initially I thought/tried to use a dictionary to easily map values from one range to another 
// also because of the misdirection of the table in the puzzle description;),
// but the ranges are so big that that is not going to work. 

// Instead we'll basically parse the input into lists of lists of (long src, long dst, long rng) tuples
// and we'll evaluate them on the spot for each input number... however to prevent it from becoming 
// completely unreadable, I'll offload this to a separate class ...

List<Remapper> remappers = new ();

for (int i = 1; i < myInputBlocks.Length; i++)
{
   remappers.Add(new Remapper(myInputBlocks[i]));
}

// Now that we have all the remap blocks, we are going to run all of our numbers through all of the remap blocks...
// Doesn't matter whether you run all numbers through each mapper, or every remapper over each number

long[] finalNumbers = seeds.Select (x => RemapNumber(x)).ToArray();

long RemapNumber (long pInput)
{
    for (int i = 0; i < remappers.Count; i++)
    {
        pInput = remappers[i].Remap(pInput);
    }
    return pInput;
}

// And now get the minimum:
Console.WriteLine("Part 1 - Lowest location: " + finalNumbers.Min());

// ** Part 2 - The soil numbers are actually ranges ... (of course they are ;))

Stopwatch stopwatch = new Stopwatch();
long lowest;

/**

// Naive initial approach... go through ALL input numbers... find the lowest output...
// Treating the seeds input as ranges...

stopwatch.Restart();
lowest = long.MaxValue;

for (int i = 0; i < seeds.Length - 1; i += 2)
{
	long startRange = seeds[i];
	long endRange = seeds [i + 1];

	for (long offset = 0; offset < endRange; offset++)
	{
		lowest = Math.Min(lowest, RemapNumber(startRange + offset));
	}
}

Console.WriteLine("Part 2 - Lowest overall: " + lowest);
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms");

// This takes about 1200 seconds -> 20 minutes on my system

/**

// So, unfortunately the naive approach takes very very very long :),
// so we'll have to find a better solution...
// We could try parallelization (is that a word?):

lowest = long.MaxValue;
stopwatch.Restart();

object lockObj = new object();

Parallel.For(0, seeds.Length / 2, i =>
{
    long startRange = seeds[i * 2];
    long endRange = seeds[i * 2 + 1];

    long localLowest = long.MaxValue; // Each thread gets its own lowest

    for (long offset = 0; offset < endRange; offset++)
    {
        localLowest = Math.Min(localLowest, RemapNumber(startRange + offset));
    }

    // Safely update the global lowest value
    lock (lockObj)
    {
        lowest = Math.Min(lowest, localLowest);
    }
});

Console.WriteLine("Part 2 - Lowest overall: " + lowest);
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms");

// This takes about 4.5 minutes on my system... 
// so that is already 5 times as fast, but still very slow ...
// And it isn't really a 'smart solution

/**/

// So what is a better solution?

// Basically all we need to know is what ranges the input ranges map to.
// For example, given the input range 1-10, 
// Remap Stage 1 might remap this to 101-110 (note the range is never scaled)
// Remap Stage 2 might remap 101-110 to 51-60 (note the range does not only go up)
// etc

// HOWEVER, the difficulty is (besides having several ranges and several stages)
// that ONE input range not necessarily maps to ONE output range.
// For example, given the input range 1-10, based on the remappers:
// 1-5 could map to 101-105
// 6-10 could map to 201-205

// That said, if we DO find these output ranges... 
// we can easily put them through the next stage using the same process...
// And just get the start values of all ranges and take their minimum, 
// since the ranges only go up ...

// So theoretically we could process all input ranges in one go, 
// do a bunch of complicated range math and map them to a bunch of output ranges in one go...
// But then I'd have to think really hard and probably make a lot of mistakes...

// So instead... we going to take all input ranges one at a time and get all the resulting output ranges...
// Collect all of them and then repeat for the next remap process... 
// So the process will look like this:

/**/

stopwatch.Restart();

// Set up to swappable lists that we can reuse from remap stage to stage

List<(long start, long range)> inValueRanges = new();
List<(long start, long range)> outValueRanges = new();

// Fill the initial in ranges based on the given puzzle input

for (int i = 0; i < seeds.Length; i+=2) {
    inValueRanges.Add((seeds[i], seeds[i+1]));
}

// Go through every remap stage, mapping each inrange to some outranges,
// while swapping out to in and repeating the process

foreach (Remapper remapper in remappers)
{
    foreach (var inValue in inValueRanges)
    {
        outValueRanges.AddRange(remapper.Remap(inValue));
    }

    // Swap the in list with the out list and clear the out list for the next run
    List<(long start, long range)> tmp = inValueRanges;
    inValueRanges = outValueRanges;
    outValueRanges = tmp;
    outValueRanges.Clear();
}

// Now that we have all output ranges after 7 or 8 remapping stages, we just pick the minimum values

Console.WriteLine("Part 2 - Lowest overall: " + inValueRanges.Min (x => x.start));
Console.WriteLine("Computed in " + stopwatch.ElapsedMilliseconds + " ms");

// On my system this executes in 1ms.
// Am I fucking proud of this? Yes I am.
// Does anyone give a shit? Probably not ;).
// Yay for me anyway xD
