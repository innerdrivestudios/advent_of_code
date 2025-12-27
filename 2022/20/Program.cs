// Solution for https://adventofcode.com/2022/day/20 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of numbers...

List<long> numbers = File.ReadAllLines(args[0]).Select(long.Parse).ToList();

int Wrap(long pIndex, int pWrap)
{
    return (int)((pIndex % pWrap) + pWrap) % pWrap;
}

// ** Part 1: Shuffle!

long GetGroveCoordinates (long pMultiplier, int pRepeatCount)
{
    Dictionary<long, long> count = new();
    List<(long, long)> numbersWithIds = new();

    for (int i = 0; i < numbers.Count; i++)
    {
        long number = numbers[i] * pMultiplier;
        long c = count.GetValueOrDefault(number, 0);
        long newCount = c + 1;
        count[number] = newCount;
        numbersWithIds.Add((c, number));
    }

    List<(long, long)> toShuffle = new(numbersWithIds);

    for (int j = 0; j < pRepeatCount; j++)
    {
        for (int i = 0; i < numbersWithIds.Count; i++)
        {
            var theKey = numbersWithIds[i];
            int index = toShuffle.IndexOf(theKey);
            toShuffle.RemoveAt(index);
            int newIndex = Wrap(index + theKey.Item2, toShuffle.Count);
            toShuffle.Insert(newIndex, theKey);
        }
    }

    int index0 = toShuffle.IndexOf((0, 0));
    int index1 = Wrap(index0 + 1000, numbersWithIds.Count);
    int index2 = Wrap(index0 + 2000, numbersWithIds.Count);
    int index3 = Wrap(index0 + 3000, numbersWithIds.Count);

    return toShuffle[index1].Item2 + toShuffle[index2].Item2 + toShuffle[index3].Item2;
}

Console.WriteLine("Part 1: " + GetGroveCoordinates(1,1));
Console.WriteLine("Part 2: " + GetGroveCoordinates(811589153,10));


