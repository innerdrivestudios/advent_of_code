// Solution for https://adventofcode.com/2023/day/15 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: stuff that needs to be hashed...

string[] strings = File.ReadAllText(args[0]).ReplaceLineEndings("").Split(',', StringSplitOptions.RemoveEmptyEntries);

int Hash (string pInput)
{
    int currentValue = 0;

    foreach (char c in pInput)
    {
        currentValue += c;
        currentValue *= 17;
        currentValue %= 256;
    }

    return currentValue;
}

// ** Test some stuff:
Console.WriteLine("Testing:");
Console.WriteLine("rn=1 => " + Hash("rn=1"));
Console.WriteLine("cm-  => " + Hash("cm-"));
Console.WriteLine("qp=3 => " + Hash("qp=3"));
Console.WriteLine("cm=2 => " + Hash("cm=2"));
Console.WriteLine("qp-  => " + Hash("qp-"));
Console.WriteLine("pc=4 => " + Hash("pc=4"));
Console.WriteLine("ot=9 => " + Hash("ot=9"));
Console.WriteLine("ab=5 => " + Hash("ab=5"));
Console.WriteLine("pc-  => " + Hash("pc-"));
Console.WriteLine("pc=6 => " + Hash("pc=6"));
Console.WriteLine("ot=7 => " + Hash("ot=7"));
Console.WriteLine("rn => " + Hash("rn"));
    
// ** Part 1:
int total = 0;
total += strings.Sum(x => Hash(x));
Console.WriteLine("Part 1: " + total);

// ** Part 2:

// Generate boxes with index 0 - 255
Box[] boxes = Enumerable.Range(0,256).Select (x => new Box(x)).ToArray();

// Handle each items = or - as advertised...
foreach (string s in strings)
{
    if (s.Contains("=")) HandleEquals(s);
    else HandleDash(s);
}

void HandleEquals(string s)
{
    int equalsIndex = s.IndexOf("=");
    string label = s.Substring(0, s.IndexOf("="));
    int boxIndex = Hash(label);
    boxes[boxIndex].Add(label, int.Parse(s.Substring(equalsIndex+1)));
}

void HandleDash(string s)
{
    string label = s.Substring(0, s.IndexOf("-"));
    int boxIndex = Hash(label);
    boxes[boxIndex].Remove(label);
}

Console.WriteLine("Part 2: " + boxes.Sum (x => x.FocalStrength()));