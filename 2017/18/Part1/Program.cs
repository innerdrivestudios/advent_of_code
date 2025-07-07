//Solution for https://adventofcode.com/2017/day/18 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of instructions for playing sounds...

string[] myInput = File.ReadAllLines(args[0]);
string[][] instructions = myInput.Select(x => x.Split(' ')).ToArray();

// ** Part 1: Figure out what the first recovered value is in the received register...

// Define the main program runner

long instructionPointer = 0;
long lastSoundPlayed = 0;
Dictionary<string, long> registers = new ();    

while (instructionPointer < instructions.Length)
{
    var instruction = instructions[instructionPointer];

    Console.WriteLine(string.Join (" ", instruction));
    //Console.ReadKey();

    switch (instruction[0])
    {
        case "snd": HandleSnd(instruction); break;
        case "set": HandleSet(instruction); break;
        case "add": HandleAdd(instruction); break;
        case "mul": HandleMul(instruction); break;
        case "mod": HandleMod(instruction); break;
        case "rcv": HandleRcv(instruction); break;
        case "jgz": HandleJGZ(instruction); break;
    }
}

// And all the helper methods ...

void HandleSnd(string[] instruction)
{
    long valueX = registers.GetValueOrDefault(instruction[1], 0);
    lastSoundPlayed = valueX;
    instructionPointer++;
}

void HandleSet(string[] instruction)
{
    long valueY = GetValue(instruction[2]);
    registers[instruction[1]] = valueY;
    instructionPointer++;
}

void HandleAdd(string[] instruction)
{
    long valueY = GetValue(instruction[2]);
    registers[instruction[1]] = registers.GetValueOrDefault(instruction[1],0) + valueY;
    instructionPointer++;
}

void HandleMul(string[] instruction)
{
    long valueY = GetValue(instruction[2]);
    registers[instruction[1]] = registers.GetValueOrDefault(instruction[1], 0) * valueY;
    instructionPointer++;
}

void HandleMod(string[] instruction)
{
    long valueY = GetValue(instruction[2]);
    registers[instruction[1]] = registers.GetValueOrDefault(instruction[1], 0) % valueY;
    instructionPointer++;
}

void HandleRcv(string[] instruction)
{
    long valueX = registers.GetValueOrDefault(instruction[1], 0);

    if (valueX != 0)
    {
        Console.WriteLine("Recovered " + lastSoundPlayed);
        //Stop the machine
        instructionPointer = instructions.Length;
    }

    instructionPointer++;
}

void HandleJGZ(string[] instruction)
{
    long valueX = registers.GetValueOrDefault(instruction[1], 0);
    long valueY = GetValue(instruction[2]);

    if (valueX > 0) instructionPointer += valueY;
    else instructionPointer++;
}

long GetValue (string pInput)
{
    if (long.TryParse(pInput, out var value)) return value;
    else return registers.GetValueOrDefault (pInput, 0);
}

// ** Part 2: Since part 2 requires a different approach to keep it clean I implemented that part in a separate project

