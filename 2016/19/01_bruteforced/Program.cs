//Solution for https://adventofcode.com/2016/day/19 (Ctrl+Click in VS to follow link)

using Elf = int;

// In visual studio you can modify the input used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

// ** Your input: A number of elves

int myInput = int.Parse(args[0]);

// ** Part 1 : Figure out which elf ends up with all the presents if elves keep stealing the presents of the next elf.
//
// Note:
// - the amount of presents are actually a decoy...
// - nobody cares about the amount of presents you end up with, since it is equal to the amount of elves

// Helper method to skip elves (ended up only using the pSkip = 1 version ...)

LinkedListNode<Elf> SkipNodes(LinkedListNode<Elf> pCurrentElf, int pSkip = 1)
{
    LinkedListNode<Elf> currentElf = pCurrentElf;
    LinkedList<Elf> list = pCurrentElf.List;

    for (int i = 0; i < pSkip; i++)
    {
        //First get the next elf, looping around in the circle
        currentElf = currentElf.Next;
        if (currentElf == null) currentElf = list.First;
    }

    return currentElf;
}

// Code blocks for part 1

{
    // First initialize all elves, each elf is simply an int 
    LinkedList<Elf> elves = new LinkedList<Elf>();

    for (int i = 0; i < myInput; i++)
    {
        elves.AddLast(i + 1);
    }

    // Now keep stealing gifts until there is one elf left

    LinkedListNode<Elf> currentElf = elves.First;

    while (elves.Count != 1)
    {
        //First get the next elf, looping around in the circle
        LinkedListNode<Elf> nextElf = SkipNodes(currentElf, 1);

        //Steal the next elf's presents: note that we don't actually have to do this...
        //(imaginary stealing going on :))
        //And remove that next elf using this O(1) (!!!) operation
        elves.Remove(nextElf);

        //Now move to the next elf, again, looping around to the start
        currentElf = SkipNodes(currentElf, 1);
    }

    Console.WriteLine("Part 1 - Who has all the presents? Elf: " + elves.First.Value);
}

// ** Part 2 - Similar but now instead of the next elf, we need to get the opposite elf and steal their presents
// We could continue to use a linkedlist, but cannot index a linkedlist directly, so searching for an elf will be O(n)
// If we use a normal list we can index directly (way easier), but removing an item is O(n)...
// However all O(n) are not equal, removing an element means shifting all elements... let's just try to use a normal list first

{
    List<Elf> elves = new List<Elf>();

    for (int i = 0; i < myInput; i++)
    {
        elves.Add(i + 1);
    }

    //just for debugging...
    int granularity = 10000;
    int elvesToGo = elves.Count / granularity;

    // Now keep stealing gifts from opposite elf until there is one elf left

    int elfIndex = 0;

    Console.WriteLine("Brute force solving part 2 ....");

    while (elves.Count != 1)
    {
        // To get the opposite elf we need to skip half of the remaining elves ourselves excluded

        Elf currentElf = elves[elfIndex];

        int nextIndex = (elfIndex + (elves.Count / 2)) % elves.Count;
        Elf nextElf = elves[nextIndex];

        //Console.WriteLine(currentElf + " steals from " + nextElf + " " + string.Join(",", elves));
        elves.RemoveAt(nextIndex);

        //This is a sneaky one, if we are removing an elf before or on us, we need to reduce the current elf index!
        if (nextIndex <= elfIndex) elfIndex--;

        //Then we increase it again...
        elfIndex++;
        if (elfIndex >= elves.Count) elfIndex = 0;

        if (elves.Count/granularity != elvesToGo)
        {
            Console.WriteLine((elves.Count/granularity)*granularity + " elves to eliminate before we know the answer ...");
            elvesToGo = elves.Count/granularity;
        }
    }

    Console.WriteLine("Part 2 - Who has all the presents? Elf:" + elves[0]);
}

// The question of course is, how can we improve this??
// And the answer is, we should be able to... but how?
// Check version 2!
