//Solution for https://adventofcode.com/2017/day/17 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input will be used by going to Debug/Debug Properties
// and specifying the value as a command line argument, e.g. "348" 
// This value will be passed to the built-in args[0] variable

// ** Your input: a number representing the amount of skips you have to do before inserting a new number

int skips = int.Parse(args[0]);
Console.WriteLine("Your input: " + skips);

// For part 1, just program it as advertised...

LinkedList<int> list = new LinkedList<int>();
list.AddLast(0);

LinkedListNode<int> node = list.First;

void Insert (int pValue, int pSkips)
{
    int reducedSkips = pSkips % list.Count;

    while (reducedSkips > 0)
    {
        if (node.Next != null) node = node.Next;
        else node = list.First;

        reducedSkips--;
    }
    node = list.AddAfter(node, pValue);
}

for (int i = 1; i <= 2017; i++)
{
	Insert(i, skips);
}

Console.WriteLine("Part 1: " + node.Next.Value);

// ** For part 2, let's do it a little bit smarter and realize we don't actually need a list in 
// this case...

// Keep track of where the index is and what the last index was
int last = 0;
int index = 0;

// Start at 1 since there is already a '0' to start with, i indicates "amount of elements" in the 
// imaginary list

for (int i = 1; i <= 50000000; i++)
{
    // Every step we skip "skips" elements and "add" a new one (hence the plus one)  
	index += skips + 1;
    // Then we wrap around the list
	index %= i;

    // If our current element jumps back to the first index, store i as the element we'll "add"
	if (index == 0 && last != i)
	{
		last = i;
	}
}

Console.WriteLine("Part 2: " + last);
