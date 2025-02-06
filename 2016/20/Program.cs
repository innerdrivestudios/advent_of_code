//Solution for https://adventofcode.com/2016/day/20 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of ranges of invalid ip addresses

using System.Formats.Asn1;

string myInput = File.ReadAllText(args[0]);

//myInput = "5-8\r\n0-2\r\n4-7";

myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// ** Part 1: Find the first valid IP address

// Let's parse the input first
List<(uint start, uint end)> ranges =
    myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)               // Get the separate lines
    .Select(
        x => x.Split("-", StringSplitOptions.RemoveEmptyEntries)        // Get the split numbers...
    )
    .Select(y => (uint.Parse(y[0]), uint.Parse(y[1])))                    // As int tuples ...
    .ToList();

// Now we want to collapse / join overlapping ranges ...
// Fastest way to do that is to first sort on the start of the range...

ranges.Sort ((x, y) => x.start.CompareTo(y.start));

// And then actually collapse the ranges ...

for (int i = 0; i < ranges.Count - 1; i++)
{
    for (int j = i + 1; j < ranges.Count;)
    {
        //i start is always <= j start, so if i end goes past j start, or ends where j starts... there is an overlap...
        if (ranges[i].end >= ranges[j].start-1)
        {
            // Note the MAX ! That is the tricky part in this setup!
            ranges[i] = (ranges[i].start, Math.Max (ranges[i].end, ranges[j].end));
            ranges.RemoveAt(j);
        }
        else
        {
            j++;
        }
    }
}

Console.WriteLine("Part 1 - First valid IP:" + (ranges[0].end + 1));

// ** Part 2 - How many IPs are allowed by the blacklist?

// This one is easy, now that we have all the ranges, just subtract them from uint.MaxValue ...
// BUT one tricky thing :), the ranges are INCLUSIVE, so 0-uint.MaxValue is actually equal to uint.MaxValue PLUS 1 different ip addresses!

uint allowed = uint.MaxValue;

foreach (var range in ranges)
{
    //In addition, these ranges are also inclusive, e.g 1 - 4 includes 4 elements, so we need to do:
    allowed -= (range.end - range.start + 1);
}

Console.WriteLine("Part 2 - Allowed IPs:" + (allowed + 1));