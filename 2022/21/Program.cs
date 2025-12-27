// Solution for https://adventofcode.com/2022/day/21 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of rules...

Dictionary<string, string> rules = new();

string[] myInput = File.ReadAllLines(args[0]);
foreach (string input in myInput)
{
    string[] rule = input.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    rules[rule[0]] = rule[1];
}

long Evaluate (string pNode)
{
    string rule = rules[pNode];

    if (long.TryParse(rule, out long result))
    {
        return result;
    }

    string[] ruleParts = rule.Split(" ");
    long left = Evaluate(ruleParts[0]);
    long right = Evaluate(ruleParts[2]);

    if (ruleParts[1] == "+") return left+right;
    else if (ruleParts[1] == "-") return left-right;
    else if (ruleParts[1] == "*") return left*right;
    else if (ruleParts[1] == "/") return left/right;

    throw new Exception("Invalid rule:" + rule);
}

Console.WriteLine("Part 1:" + Evaluate("root"));

// ** Part 2: My root has lrnp and ptnb

// Get the delta of applying humn 0 or 1 time

rules["humn"] = "0";
long lrnpHumn0 = Evaluate("lrnp");
long ptnbHumn0 = Evaluate("ptnb");

rules["humn"] = "1";
long lrnpHumn1 = Evaluate("lrnp");
long ptnbHumn1 = Evaluate("ptnb");

long deltaLrnp = lrnpHumn1 - lrnpHumn0;
long deltaPtnb = ptnbHumn1 - ptnbHumn0;

// Based on this we can see only the delta Lrnp is influenced...
Console.WriteLine("Deltas: " + deltaLrnp + "  " + deltaPtnb);

// Using that info we'll zone in on the exact value for humn
// (Testing showed we couldn't get there in one go, probably something to do with rounding errors)

long totalDelta = 0;
long deltaToCross = (ptnbHumn0 - lrnpHumn0) / deltaLrnp;

while (deltaToCross != 0)
{
    totalDelta += deltaToCross;
    rules["humn"] = "" + (totalDelta);
    lrnpHumn0 = Evaluate("lrnp");
    ptnbHumn0 = Evaluate("ptnb");
    deltaToCross = (ptnbHumn0 - lrnpHumn0) / deltaLrnp;
}

Console.WriteLine("Part 2: " + totalDelta);