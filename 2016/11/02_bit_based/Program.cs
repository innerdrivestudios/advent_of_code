//Solution for https://adventofcode.com/2016/day/11 (Ctrl+Click in VS to follow link)

using System.Diagnostics;

/*

This is a bit based version of puzzle 2016/11 to test how much we can improve the performance 
compared to a string based version. Make sure you understand the string based version this puzzle first.

The idea is that we represent every set of chip and generator with a 2 bits sequence:
00 -> no chip and no generator
01 -> yes chip, no generator
10 -> no chip, yes generator
11 -> both

Of course, we could also swap this setup, doesn't matter.

In the test input we have:

// F4   .  .  .  .  
// F3   .  .  LG . 
// F2 ^ HG .  .  .
// F1 E .  HM .  LM

In other words, if we need 2 bits to represent HG/HM and 2 bits to represent LG/LM => 4 bits

With respect to the REAL input for the puzzle, I got:
Part 1: THULIUM, PLUTONIUM, STRONTIUM, PROMETHIUM, RUTHENIUM => 5 * 2 = 10 bits
Part 2: Add 2 additional elements -> 10 + 2*2 = 14 bits

So with this setup we'll need to be able to do the same things as before:
- Set up the initial state
- Generate additional states
- Test states 
- Convert states to unique keys to mark them visited 
- Etc...

Nice challenge :)

*/

// Starting with the test input again, this is how we'd set it up for example:

// F4   .  .  .  .      => 0000 => 0
// F3   .  .  LG .      => 0010 => 2
// F2 ^ HG .  .  .      => 1000 => 8
// F1 E .  HM .  LM     => 0101 => 5

// How do we know 0010 is LG and 0001 is LM?
// We don't, BUT by anding/orring/shifting bits we CAN test if both 10 and 01 are set in a specific bit pair.

// But... we're not there yet, let's first convert the above into code:
RTGFacility testFacility = new RTGFacility([5,8,2,0]);

// Note that the rest of the code in this file remained mostly the same,
// most of the nitty gritty details are wrapped in the internals of the RTGFacility class
Console.WriteLine("Test input - Moves required:" + RunDijkstra(testFacility));

/*
And for my input which was originally:
TG  TM  PG  .   SG  .   .   .   .   .   =>  1110100000 = 928
.   .   .   PM  .   SM  .   .   .   .   =>  0001010000 = 80
.   .   .   .   .   .   pG  pM  RG  RM  =>  0000001111 = 15
.   .   .   .   .   .   .   .   .   .   =>  0000000000 = 0
We get:
*/

RTGFacility part1Facility = new RTGFacility([928,80,15,0]);

Console.WriteLine("Part 1 - Moves required:" + RunDijkstra(part1Facility));

/*
Then for part 2 we add another four items to floor 0, while the rest remains the same...
easiest way is by just adding 15 (which is 1111 <- 4 items) and shifting that 10 bits
*/

RTGFacility part2Facility = new RTGFacility([928 + (15 << 10), 80, 15, 0]);
Console.WriteLine("Part 2 - Moves required:" + RunDijkstra(part2Facility));

int RunDijkstra (RTGFacility pInputFacility)
{
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    
    Console.WriteLine("Dijkstra started...");

    PriorityQueue<RTGFacility, int> todoList = new();
    todoList.Enqueue(pInputFacility, 0);

    HashSet<long> visited = new HashSet<long>();

    while (todoList.Count > 0)
    {
        RTGFacility current = todoList.Dequeue();
        visited.Add(current.GetUniqueIDAsLong());

        if (current.IsSolution())
        {
            Console.WriteLine("Dijkstra completed in ..." + (stopwatch.ElapsedMilliseconds/1000f) +" seconds...");
            return current.movesMade;
        }

        List<Move> possibleMoves = current.GetPossibleMoves();

        for (int i = 0; i < possibleMoves.Count; i++)
        {
            RTGFacility newState = current.MakeMove(possibleMoves[i]);
            if (!visited.Contains(newState.GetUniqueIDAsLong()))
            {
                //GetRating is basically A*, it combines the current cost and estimate
                todoList.Enqueue(newState, newState.GetRating());
            }
        }
    }

    return -1;
}

