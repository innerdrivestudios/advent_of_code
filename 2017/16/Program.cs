//Solution for https://adventofcode.com/2017/day/16 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list a dance moves for programs

using System.Text.RegularExpressions;

string myInput = File.ReadAllText(args[0]);

List<char> programs = "abcdefghijklmnop".Select(x => x).ToList();
string[] instructions = myInput.ReplaceLineEndings("").Split(",", StringSplitOptions.RemoveEmptyEntries);

Regex spinParser = new Regex(@"s(\d+)");
Regex swapIndexParser = new Regex(@"x(\d+)/(\d+)");
Regex swapNameParser = new Regex(@"p(.)/(.)");

void Dance()
{
    foreach (string instruction in instructions)
    {
        switch (instruction[0])
        {
            case 's': Spin(instruction); break;
            case 'x': SwapIndex(instruction); break;
            case 'p': SwapName(instruction); break;
        }
    }
}

void Spin(string pInstruction)
{
    //Console.WriteLine("spin");
    Match match = spinParser.Match(pInstruction);
    if (match.Success)
    {
        int spinSize = int.Parse(match.Groups[1].Value);
        IEnumerable<char> range = programs.GetRange(programs.Count - spinSize, spinSize);
        programs.RemoveRange(programs.Count - spinSize, spinSize);  
        programs.InsertRange(0, range);
    }
    else throw new Exception("Could not parse spin instruction");
}

void SwapName(string pInstruction)
{
    //Console.WriteLine("swapname");
    Match match = swapNameParser.Match(pInstruction);
    if (match.Success)
    {
        int indexA = programs.IndexOf(match.Groups[1].Value[0]);
        int indexB = programs.IndexOf(match.Groups[2].Value[0]);
        char tmp = programs[indexA];
        programs[indexA] = programs[indexB];
        programs[indexB] = tmp;
    }
    else throw new Exception("Could not parse spin instruction");
}


void SwapIndex(string pInstruction)
{
    //Console.WriteLine("swapindex");
    Match match = swapIndexParser.Match(pInstruction);
    if (match.Success)
    {
        int indexA = int.Parse(match.Groups[1].Value);
        int indexB = int.Parse(match.Groups[2].Value);
        char tmp = programs[indexA];
        programs[indexA] = programs[indexB];
        programs[indexB] = tmp;
    }
    else throw new Exception("Could not parse spin instruction");
}

Dance();
Console.WriteLine("Part 1:" + string.Concat(programs));

// ** Part 2: Perform the dance a billion times...

// Ok well maybe not a billion, let's look for a cycle to speed things up :)

programs = "abcdefghijklmnop".Select(x => x).ToList();
HashSet<string> visited = new ();

int iterations = 1000000000;
int originalIterations = iterations;

for (int i = 0; i < iterations;i++)
{
    Dance();

    //Look for a cycle
    if (iterations == originalIterations && !visited.Add(string.Concat(programs)))
    {
        iterations = i + originalIterations % i;
    }
}

Console.WriteLine("Part 2:" + string.Concat(programs));

