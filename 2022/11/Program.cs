// Solution for https://adventofcode.com/2022/day/11 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of monkey information to process

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] monkeyInput = myInput.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.TrimEntries);

Monkey[] monkeys = monkeyInput.Select (x => new Monkey(x, 3)).ToArray();

// ** Part 1: Count the total number of times each monkey inspects items over 20 rounds

for (int i = 0; i < 20; i++)
{
    foreach (var monkey in monkeys) monkey.Inspect();
}

foreach (var monkey in monkeys)
{
    Console.WriteLine("Monkey " + monkey.id + " inspected items "+monkey.ItemsInspected + " times.");
}

long[] monkeyBusiness = monkeys.Select (x => x.ItemsInspected).OrderByDescending(x => x).Take(2).ToArray();
Console.WriteLine("Part 1 - Level of monkey business:"+ monkeyBusiness[0] * monkeyBusiness[1]);

// ** Part 2: Same but over 10000 rounds.
// The issue here is that due to the old * old operation in some of the Monkeys
// the result numbers overflow a (u)long. Initially this had me stumped for a bit
// I tried using doubles/floats/Decimals/BigIntegers etc.
// BigIntegers might work in theory but the numbers become so big, that so many 
// bits are required that the application slows down to a crawl.

// After sleeping on it for a night I realized the most important thing is to make sure
// that whatever we do with the worry levels, we need to maintain the property that
// it cannot change the outcome of the division test.

// In order to do that, creating monkeys updates a globalModulo value, which is
// updated by multiplying the previous global modulo by the current monkeys
// division test. (Since they are all prime numbers this will work without further tinkering).
// Using this value we can make sure any inspection results don't overflow.

// However since creating monkeys for a second time would modify this value a second
// time we need to reset it back to 1 between runs...
// (and yes, modifying global value is not the best approach)

Monkey.globalModulo = 1;
monkeys = monkeyInput.Select(x => new Monkey(x, 1)).ToArray();
Console.WriteLine(Monkey.globalModulo);

for (int i = 0; i < 10000; i++)
{
    //Console.WriteLine();
    //Console.WriteLine("Round "+(i+1));
    foreach (var monkey in monkeys)
    {
        monkey.Inspect();
        //Console.WriteLine("Monkey " + monkey.id + " inspected items " + monkey.ItemsInspected + " times");
    }
}

foreach (var monkey in monkeys)
{
    Console.WriteLine("Monkey " + monkey.id + " inspected items " + monkey.ItemsInspected + " times.");
}

monkeyBusiness = monkeys.Select(x => x.ItemsInspected).OrderByDescending(x => x).Take(2).ToArray();
Console.WriteLine("Part 2 - Level of monkey business:" + monkeyBusiness[0] * monkeyBusiness[1]);

