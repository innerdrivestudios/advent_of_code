// Solution for https://adventofcode.com/2022/day/10 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of statements to process

Queue<string> myInput = new Queue<string>(File.ReadAllLines(args[0]));

//Initial cycles and values:
int cycle = 0;
int x = 1;

List<int> cycleValues = new List<int>() { x };

void RunProgram()
{
    while (myInput.Count > 0)
    {
        string instruction = myInput.Dequeue();

        int waitCount = instruction.StartsWith("noop") ? 1 : 2;
        
        while (waitCount > 0) { 
            cycle++; 
            waitCount--;
            cycleValues.Add(x);
        }

        if (instruction.StartsWith("addx"))
        {
            x += int.Parse(instruction.Split(' ')[1]);
        }
    }
}

RunProgram();

List<int> markers = [20, 60, 100, 140, 180, 220];
Console.WriteLine("Part 1: " +markers.Sum (x => x * cycleValues[x]));

// ** Part 2: Render the CRT screen. The CRT screen works line a scan line, 
// the screen size is 40 wide and 6 high, so it takes 240 cycles to draw...

// Each pixel coordinate is given by the cycle index:
// x = (cycleIndex-1) % width
// y = (cycleIndex-1) / width
// Easier if we remove the first element of the cycle values though:
cycleValues.RemoveAt(0);

// In order for a pixel to be on, any of the 3-pixel wide sprite pixels
// need to overlap with the scanline x position.
// The sprite center position is given cycle-x value:

Console.WriteLine("Part 2:");

Grid<char> crt = new Grid<char>(40, 6);

for (int i = 0; i < cycleValues.Count; i++)
{
    int posX = i % 40;
    int posY = i / 40;
    
    int spritePosition = cycleValues[i];
    crt[posX, posY] = (posX >= spritePosition - 1 && posX <= spritePosition + 1) ? '#' : ' ';
}

crt.Print();

