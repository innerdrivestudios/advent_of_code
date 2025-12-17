// Solution for https://adventofcode.com/2020/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying a command line argument, e.g. 32415.
// This value will be passed to the built-in args[0] variable

string myInput = args[0];

// ** Part 1: Implementing a straightforward, ugly and slow solution just to see if I got the logic correct:

List<int> cups = myInput.ToCharArray().Select (x => x - '0').ToList();

int cupCount = cups.Count;
int currentCupIndex = 0;

void MovePart1()
{
    int element1 = cups[(currentCupIndex + 1) % cupCount];
    int element2 = cups[(currentCupIndex + 2) % cupCount];
    int element3 = cups[(currentCupIndex + 3) % cupCount];

    //Console.WriteLine(string.Join(" ", cups));
    //Console.WriteLine("Picked up " + element1 + " " + element2 + " " + element3);

    cups.RemoveAt(currentCupIndex+1);
    cups.RemoveAt(currentCupIndex+1);
    cups.RemoveAt(currentCupIndex+1);

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
    cups.Insert(insertIndex+1, element1);
    cups.Insert(insertIndex+2, element2);
    cups.Insert(insertIndex+3, element3);

    currentCupIndex = (currentCupIndex+1 % cupCount);
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
    MovePart1();
}

while (cups[0] != 1)
{
    cups.Add(cups[0]);
    cups.RemoveAt(0);
}

Console.WriteLine("Final:" + string.Concat(cups.Skip(1)));

// ** Part 2: THe amount of numbers should equal a million and we need to repeat it 10.000.000 times.
// I'm gonna go on a limb here and assume this is a data structure problem and not one of those $&@#$)(*@#&)$-find-the-&@(#$-repeating-$#@$-pattern puzzles.

// Approach:
// - make a doubly linked list with all the numbers
// - create an array where the index points to the node with the value

cups = myInput.ToCharArray().Select(x => x - '0').ToList();
Node[] lookupTable = new Node[cups.Count+1+1000000];

Node current = null;
Node last = null;
Node first = null;
int maxValue = 0;

for (int i = 0; i < cups.Count; i++)
{
    int value = cups[i];
    current = new Node() { value = value };
    lookupTable[value] = current;

    if (first == null) first = current;

    if (last != null)
    {
        last.next = current;
        current.prev = last;
    }

    last = current;
    maxValue = int.Max(maxValue, value);
}

for (int i = 10; i <= 1000000; i++)
{
    int value = i;
    current = new Node() { value = value };
    lookupTable[value] = current;

    if (first == null) first = current;

    if (last != null)
    {
        last.next = current;
        current.prev = last;
    }

    last = current;
    maxValue = int.Max(maxValue, value);
}


last.next = first;
first.prev = last;

Node currentNode = first;

void MovePart2()
{
    Node iterator = currentNode;

    Node element1Node = currentNode.next;
    Node element2Node = currentNode.next.next;
    Node element3Node = currentNode.next.next.next;

    int element1 = element1Node.value;
    int element2 = element2Node.value;
    int element3 = element3Node.value;

    Node after = currentNode.next.next.next.next;

    // Remove these three nodes ...
    iterator.next = after;
    after.prev = iterator;

    // todo below this line...

    int maxLabel = maxValue;
    while (element1 == maxLabel || element2 == maxLabel || element3 == maxLabel)
    {
        maxLabel--;
    }

    int destinationCupLabel = currentNode.value - 1;
    if (destinationCupLabel < 1) destinationCupLabel = maxValue;

    while (element1 == destinationCupLabel || element2 == destinationCupLabel || element3 == destinationCupLabel)
    {
        destinationCupLabel--;
        if (destinationCupLabel < 1) destinationCupLabel = maxLabel;
    }

    //Console.WriteLine("Destination:" + destinationCupLabel);
    //Console.ReadKey();

    Node destinationCup = lookupTable[destinationCupLabel];

    Node next = destinationCup.next;

    destinationCup.next = element1Node;
    element1Node.prev = destinationCup;
    element3Node.next = next;
    next.prev = element3Node;

    currentNode = currentNode.next;
}


for (int i = 0; i < 10000000; i++)
{
    MovePart2();
}

while (currentNode.value != 1)
{
    currentNode = currentNode.next;
}

currentNode = currentNode.next;

long value1 = currentNode.value;
currentNode = currentNode.next;
value1 *= currentNode.value;
Console.WriteLine(value1);




