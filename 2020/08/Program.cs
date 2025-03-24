// Solution for https://adventofcode.com/2020/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a long list of instructions

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// Parse all the instructions
(string, int)[] instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select (x => x.Split(" "))
    .Select (y => (y[0], int.Parse(y[1])))
    .ToArray ();

// Write some helper methods to execute the program and return the value + whether the program completed normally

(int value, bool result) RunProgram ()
{
    int acc = 0;
    int instructionPointer = 0;
    HashSet<int> executedInstructions = new HashSet<int>() { instructionPointer };

    while (instructionPointer < instructions.Length)
    {
        instructionPointer += ProcessInstruction(instructions[instructionPointer], ref acc);

        if (!executedInstructions.Add(instructionPointer))
        {
            return (acc, false);
        }
    }

    return (acc, true);
}

int ProcessInstruction((string instruction, int value) pInstruction, ref int pAccumulator)
{
    if (pInstruction.instruction == "acc")
    {
        pAccumulator += pInstruction.value;
        return 1;
    }
    else if (pInstruction.instruction == "jmp") 
    {
        return pInstruction.value;
    }
    else if (pInstruction.instruction == "nop")
    {
        return 1;
    }

    throw new Exception("Unexpected error occurred");
}

Console.WriteLine("Part 1: Acc = " + RunProgram().value);

// ** Part 2: Simple brute force approach

int FindAccumulatorValueAfterFixingTheBrokenInstruction ()
{
    int instructionToChange = 0;

    while (instructionToChange < instructions.Length)
    {
        if (instructions[instructionToChange].Item1 == "acc")
        {
            instructionToChange++;
            continue;
        }

        //swap
        if (instructions[instructionToChange].Item1 == "jmp") instructions[instructionToChange].Item1 = "nop";
        else instructions[instructionToChange].Item1 = "jmp";

        //run
        var programResult = RunProgram();

        //swap back
        if (instructions[instructionToChange].Item1 == "jmp") instructions[instructionToChange].Item1 = "nop";
        else instructions[instructionToChange].Item1 = "jmp";

        if (programResult.result)
        {
            return programResult.value;
        }

        instructionToChange++;
    }

    return -1;
}

Console.WriteLine("Part 2: Acc = " + FindAccumulatorValueAfterFixingTheBrokenInstruction());
