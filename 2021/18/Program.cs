//Solution for https://adventofcode.com/2021/day/18 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: snailfish :)

string[] myInput = File.ReadAllLines(args[0]);

// ** Define some helper methods...

// Define a simple helper method to avoid having to declare an int ref param all the time

Node ParseSnailFishHelper (string pInput)
{
    int index = 0;
    return ParseSnailFishExpression(pInput, ref index);
}

// Recursively parse a snail fish expression, building a tree of snailfish 'nodes'
// The startindex is recursively updated as parsing progresses...
Node ParseSnailFishExpression(string pInput, ref int pStartIndex)
{
    // We have [,] nodes and value nodes (e.g. 100)
    Node node = new Node();

    if (char.IsDigit(pInput[pStartIndex]))
    {
        node.value = ParseDigit(pInput, ref pStartIndex);
    }
    else
    {
        pStartIndex++;                                                                  // skip [
        node.leftNode = ParseSnailFishExpression(pInput, ref pStartIndex);              // parse left child content
        pStartIndex++;                                                                  // skip ,
        node.rightNode = ParseSnailFishExpression(pInput, ref pStartIndex);             // parse right child content
        pStartIndex++;                                                                  // skip ]

        node.leftNode.parent = node;                                                    // don't forget to assign parents...
        node.rightNode.parent = node;
    }

    return node;
}

// Simple helper method to parse digits with an offset into a string

int ParseDigit (string pInput, ref int pStartIndex)
{
    int result = 0;
    while (char.IsDigit(pInput[pStartIndex]))
    {
        result = result * 10 + (pInput[pStartIndex]-'0');
        pStartIndex++;
    }
    return result;
}

// ** Part 1:

Node node = null;

foreach (string input in myInput)
{
    Node nextNode = ParseSnailFishHelper(input);
    node = node == null ? nextNode : node.AddNode(nextNode);
}

Console.WriteLine("Part 1: " + node.Magnitude());

// ** Part 2: 

long max = 0;

for (int i = 0; i < myInput.Length; i++)
{
    for (int j = 0; j < myInput.Length; j++)
    {
        if (i == j) continue;

        //note that node a and b are changed currently by the addnode operation
        Node a = ParseSnailFishHelper(myInput[i]);
        Node b = ParseSnailFishHelper(myInput[j]);

        max = long.Max(max, a.AddNode(b).Magnitude());
    }
}

Console.WriteLine("Part 2: " + max);