//Solution for https://adventofcode.com/2015/day/23 (Ctrl+Click in VS to follow link)

string myInput = "jio a, +22\r\ninc a\r\ntpl a\r\ntpl a\r\ntpl a\r\ninc a\r\ntpl a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\njmp +19\r\ntpl a\r\ntpl a\r\ntpl a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ninc a\r\ntpl a\r\ninc a\r\ntpl a\r\ntpl a\r\njio a, +8\r\ninc b\r\njie a, +4\r\ntpl a\r\ninc a\r\njmp +2\r\nhlf a\r\njmp -7\r\n";
string[] instructions = myInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

ulong[] registersPart1 = [0,0];
RunProgram(registersPart1, instructions);
Console.WriteLine("Part 1: Value of register B after executing the program:" + registersPart1[1]);

ulong[] registersPart2 = [1, 0];
RunProgram(registersPart2, instructions);
Console.WriteLine("Part 2: Value of register B after executing the program:" + registersPart2[1]);

void RunProgram(ulong[] pRegisters, string[] pInstructions)
{
    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        string instruction = pInstructions[instructionPointer];
        int offset = ExecuteInstruction(instruction, pRegisters);
        instructionPointer += offset;
        //Console.WriteLine(instruction + " executed -> " + instructionPointer + " | "+string.Join(",", pRegisters));
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
