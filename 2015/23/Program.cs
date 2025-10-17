// Solution for https://adventofcode.com/2015/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a kind of assembly program 

string[] myInput = File.ReadAllLines(args[0]);

// Defining some helper methods to execute these instructions...

void RunProgram(ulong[] pRegisters, string[] pInstructions)
{
    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        string instruction = pInstructions[instructionPointer];
        int offset = ExecuteInstruction(instruction, pRegisters);
        instructionPointer += offset;
    }
}

int ExecuteInstruction(string pInstruction, ulong[] pRegisters)
{
    string[] instructionParts = pInstruction.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    string _operator = instructionParts[0];
    int registerIndex = instructionParts[1][0] - 'a';

    switch (_operator)
    {
        case "hlf": pRegisters[registerIndex] = pRegisters[registerIndex] / 2; break;
        case "tpl": pRegisters[registerIndex] = pRegisters[registerIndex] * 3; break;
        case "inc": pRegisters[registerIndex] = pRegisters[registerIndex] + 1; break;
        case "jie": if (pRegisters[registerIndex] % 2 == 0) return int.Parse(instructionParts[2]); break;
        case "jio": if (pRegisters[registerIndex] == 1)     return int.Parse(instructionParts[2]); break;
        case "jmp": return int.Parse(instructionParts[1]);

        default:
            throw new InvalidOperationException("Unknown operator " + _operator);
    }

    //default increase of the instructions pointer
    return 1;
}

// ** Part 1:

ulong[] registersPart1 = [0, 0];
RunProgram(registersPart1, myInput);
Console.WriteLine("Part 1: Value of register B after executing the program:" + registersPart1[1]);

// ** Part 2:

ulong[] registersPart2 = [1, 0];
RunProgram(registersPart2, myInput);
Console.WriteLine("Part 2: Value of register B after executing the program:" + registersPart2[1]);
