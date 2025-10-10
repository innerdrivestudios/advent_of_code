//Solution for https://adventofcode.com/2016/day/19 (Ctrl+Click in VS to follow link)

// In visual studio you can modify the input used by going to
// Debug/Debug Properties and changing the command line arguments.
// This value given will be passed to the built-in args[0] variable.

// ** Your input: A number of elves

int myInput = int.Parse(args[0]);

// ** Part 1 : Figure out which elf ends up with all the presents if elves keep stealing the presents of the next elf.
//
// Here is where it gets extremely awesome, watch this video first:
// https://www.youtube.com/watch?v=uCsD3ZGzMgE
//
// Then check the solutions below (only part 1, still need to do part 2!):

Queue<char> myInputAsBinary = new(Convert.ToString(myInput, 2));
myInputAsBinary.Enqueue(myInputAsBinary.Dequeue());
Console.WriteLine(
    "Part 1 - Who has all the presents ? Elf: " + 
    Convert.ToInt32(string.Concat(myInputAsBinary), 2)
);

// Now for part 2 ... we also need to check if we can find some kind of pattern just like in the video for part 1
// Set the following value to true to print the list and see if you can find the pattern yourself ...

/*
if (false)
{
    for (int i = 1; i <= 100; i++)
    {
        Console.WriteLine("Amount of elves => " + i + " ==> survivor is : " + BruteForcedPart2.GetSurvivingElf(i));
    }
}
*/

// After inspecting this table we see a couple of things...
// The sequence starts with 1, then 1 with next odd number 3
// Then 1,2,3, followed by the next 3 odds numbers... ending with 9
// Then 1-9, followed by 9 odd numbers ending with 27
// Then 1-27, followed by 27 odd numbers etc...

// Next thing to note is that, 1, 3, 9, 27, 81 all have 1, 3, 9, 27, 81 as their answer, so something with a power of 3

// Let's say we have x elves... how do we find the lower and up power of 3 bound?

int amountOfElves = myInput;

double log = Math.Log(amountOfElves, 3);
int lowerBound = (int)Math.Floor(log);
int upperBound = (int)Math.Ceiling(log);

int lowerBoundValue = (int)Math.Pow(3, lowerBound);
int upperBoundValue = (int)Math.Pow(3, upperBound);

//Console.WriteLine(amountOfElves + " has " + lowerBoundValue + " and " + upperBoundValue + " as bounds.");

// Now in the first half of these bounds, e.g. for 40 elves, we would get [27 .. (27+81)/2 .. 81]
// the matching elf / number increase by one, in the 2nd half by two at time ... according to this logic ...

int middle = (lowerBoundValue + upperBoundValue) / 2;
//Console.WriteLine("Middle is " + middle);

if (amountOfElves <= middle)
{
    Console.WriteLine("Part 2 - Surviving elf is: " + (amountOfElves - lowerBoundValue));
}
else
{
    Console.WriteLine("Part 2 - Surviving elf is: " + (lowerBoundValue + (amountOfElves - middle) * 2));
}


