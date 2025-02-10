//Solution for https://adventofcode.com/2022/day/5 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of stacks of chars and move instructions to move a char from stack to stack

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] inputParts = myInput.Split(Environment.NewLine + Environment.NewLine);

// Convert the stack into better usable datastructures
string stackDefinitionBlock = inputParts[0];
string[] stackLines = stackDefinitionBlock.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

string moveInstructionBlock = inputParts[1];
string[] moveInstructions = moveInstructionBlock.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Every tower uses 3 chars ([A]) with a space between it, in other words
// int stringLength = towerCount * 3 + (towerCount - 1) * 1;
// int stringLength = towerCount * 4 - 1
// stringLength + 1 = towerCount * 4
// towerCount = (stringLength + 1) / 4;

int towerCount = (stackLines[0].Length + 1) / 4;
Console.WriteLine($"Parsing {towerCount} towers...");

// We define the towers as a list of stacks of chars.

List<Stack<char>> stacks = GetStacks();

// But since all instructions are 1 based, we'll add a dummy empty one at index 0 + 1 for each tower

List<Stack<char>> GetStacks() {

    List<Stack<char>> stacks = new();

    for (int i  = 0; i < towerCount + 1;  i++) stacks.Add(new Stack<char>());

    // Now fill all the stack, bottom up, skipping the tower index line ...

    for (int i = stackLines.Length - 2; i >= 0; i--)
    {
        for (int j = 0; j < towerCount; j++)
        {
            Stack<char> stack = stacks[j+1];

            char c = stackLines[i][1 + 4 * j];
            if (c != ' ') stack.Push(c);
        }
    }

    return stacks;
}

// Now let's define some simple methods, to parse a move instruction and execute it

// "move 1 from 3 to 9"
Regex moveInstructionParser = new Regex(@"move (\d+) from (\d+) to (\d+)");

void ProcessMoveInstruction (string pInstruction)
{
    Match match = moveInstructionParser.Match(pInstruction);

    int amount = int.Parse(match.Groups[1].Value);
    int from = int.Parse(match.Groups[2].Value);
    int to = int.Parse(match.Groups[3].Value);

    for (int i = 0;i < amount; i++) Move(from, to);
}

void Move (int pFrom, int pTo)
{
    if (stacks[pFrom].Count > 0)
    {
        stacks[pTo].Push(stacks[pFrom].Pop());
    }
    else
    {
        throw new Exception("Stack empty?");
    }
}

// Process all the move instructions....

foreach (string instruction in moveInstructions)
{
    ProcessMoveInstruction (instruction);
}

// Now gather the top of each stack:

string GetTopStackResult(List<Stack<char>> pStacks)
{
    string result = "";
    for (int stackIndex = 1; stackIndex < pStacks.Count; stackIndex++)
    {
        result += pStacks[stackIndex].Peek();
    }

    return result;
}

Console.WriteLine("Part 1 - Stack tops:" + GetTopStackResult(stacks));

// ** Part 2 - 1 change: when you move multiple crates, you move all of them at once, retaining their order.

// Approach: since it is only such a small set of chars, we'll do this in the simplest way possible...
// Every move instruction moves everything to stack 0 first (our dummy empty stack from part 1) and then to the final stack

// Reset our stacks

stacks = GetStacks();

// Update our Move instruction

void ProcessMoveInstruction_New (string pInstruction)
{
    Match match = moveInstructionParser.Match(pInstruction);

    int amount = int.Parse(match.Groups[1].Value);
    int from = int.Parse(match.Groups[2].Value);
    int to = int.Parse(match.Groups[3].Value);

    // Updated part..., bit lazy but it works :)
    for (int i = 0; i < amount; i++) Move(from, 0);
    for (int i = 0; i < amount; i++) Move(0, to);
}

// Process all the updated move instructions....

foreach (string instruction in moveInstructions)
{
    ProcessMoveInstruction_New (instruction);
}

Console.WriteLine("Part 2 - Stack tops:" + GetTopStackResult(stacks));