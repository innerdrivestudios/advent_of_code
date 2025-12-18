// Solution for https://adventofcode.com/2020/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying a command line argument, e.g. 32415.
// This currentValue will be passed to the built-in args[0] variable

string myInput = args[0];

// ** Part 1: Implementing a straightforward, ugly and slow solution just to see if I got the logic correct:
{

    List<int> cups = myInput.ToCharArray().Select(x => x - '0').ToList();

    int cupCount = cups.Count;
    int currentCupIndex = 0;

    void Move()
    {
        int element1 = cups[(currentCupIndex + 1) % cupCount];
        int element2 = cups[(currentCupIndex + 2) % cupCount];
        int element3 = cups[(currentCupIndex + 3) % cupCount];

        //Console.WriteLine(string.Join(" ", cups));
        //Console.WriteLine("Picked up " + element1 + " " + element2 + " " + element3);

        cups.RemoveAt(currentCupIndex + 1);
        cups.RemoveAt(currentCupIndex + 1);
        cups.RemoveAt(currentCupIndex + 1);

        int destinationCupLabel = cups[currentCupIndex] - 1;
        if (destinationCupLabel < 1) destinationCupLabel = 9;
        int maxLabel = 9;

        while (element1 == maxLabel || element2 == maxLabel || element3 == maxLabel)
        {
            maxLabel--;
        }

        while (element1 == destinationCupLabel || element2 == destinationCupLabel || element3 == destinationCupLabel)
        {
            destinationCupLabel--;
            if (destinationCupLabel < 1) destinationCupLabel = maxLabel;
        }

        //Console.WriteLine("Destination:" + destinationCupLabel);
        //Console.ReadKey();

        int insertIndex = cups.IndexOf(destinationCupLabel);
        cups.Insert(insertIndex + 1, element1);
        cups.Insert(insertIndex + 2, element2);
        cups.Insert(insertIndex + 3, element3);

        currentCupIndex = (currentCupIndex + 1 % cupCount);
        //Console.WriteLine("Current:" + cups[currentCupIndex]);

        //Offset this thing so we don't go out of bounds...
        while (currentCupIndex > 0)
        {
            cups.Add(cups[0]);
            cups.RemoveAt(0);
            currentCupIndex--;
        }
    }

    for (int i = 0; i < 100; i++)
    {
        Move();
    }

    while (cups[0] != 1)
    {
        cups.Add(cups[0]);
        cups.RemoveAt(0);
    }

    Console.WriteLine("Part 1: " + string.Concat(cups.Skip(1)));
}

// ** Part 2: THe amount of numbers should equal a million and we need to repeat it 10.000.000 times.
// I'm gonna go on a limb here and assume this is a data structure problem and not one of those $&@#$)(*@#&)$-find-the-&@(#$-repeating-$#@$-pattern puzzles.

// Approach:
// - make a (doubly?) linked list with all the numbers
// - create an array where the index points to the node with the currentValue

{
    List<int> cups = myInput.ToCharArray().Select(x => x - '0').ToList();
    
    int maxValue = 1000000;
    Node[] lookupTable = new Node[maxValue + 1];

    Node current = null;
    Node last = null;
    Node first = null;

    for (int i = 0; i <= maxValue; i++)
    {
        if (i == 9) continue;

        int value = i < cups.Count ? cups[i] : i;
        current = new Node() { value = value };
        lookupTable[value] = current;

        if (first == null) first = current;
        if (last != null) last.next = current;

        last = current;
    }

    // Close the loop
    last.next = first;

    Node currentNode = first;

    void Move()
    {
        Node iterator = currentNode;

        Node e1 = currentNode.next;
        Node e2 = currentNode.next.next;
        Node e3 = currentNode.next.next.next;

        Node after = currentNode.next.next.next.next;

        // Remove these three nodes ...
        iterator.next = after;

        int destinationCupLabel = currentNode.value - 1;
        if (destinationCupLabel < 1) destinationCupLabel = maxValue;

        while (e1.value == destinationCupLabel || e2.value == destinationCupLabel || e3.value == destinationCupLabel)
        {
            destinationCupLabel--;
            if (destinationCupLabel < 1) destinationCupLabel = maxValue;
        }

        Node destinationCup = lookupTable[destinationCupLabel];
        e3.next = destinationCup.next;                          //link our third element to whatever comes after the destination cup
        destinationCup.next = e1;                               //and link whatever comes after the dest cup to our 1st element

        currentNode = currentNode.next;
    }

    for (int i = 0; i < 10000000; i++)
    {
        Move();
    }

    currentNode = lookupTable[1].next;
    Console.WriteLine("Part 2: " + ((long)lookupTable[1].next.value * lookupTable[1].next.next.value));

}

// Ok, this works and pretty fast... but this made me wonder whether we could remove the linked list completely
// and solely rely on the unique integer values to act as pointers to the next value using my lookup table...
// haven't tried that yet though ...

