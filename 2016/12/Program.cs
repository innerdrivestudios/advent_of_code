//Solution for https://adventofcode.com/2016/day/12 (Ctrl+Click in VS to follow link)

// Your input: a sequence of instructions

string myInput = "cpy 1 a\r\ncpy 1 b\r\ncpy 26 d\r\njnz c 2\r\njnz 1 5\r\ncpy 7 c\r\ninc d\r\ndec c\r\njnz c -2\r\ncpy a c\r\ninc a\r\ndec b\r\njnz b -2\r\ncpy c b\r\ndec d\r\njnz d -6\r\ncpy 16 c\r\ncpy 12 d\r\ninc a\r\ndec d\r\njnz d -2\r\ndec c\r\njnz c -5\r\n";
string[] instructions = myInput.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

// Part 1 - Execute the code with four registers starting at zero

int[] registersPart1 = [0,0,0,0];
RunProgram(registersPart1, instructions);
Console.WriteLine("Part 1: Value of register A after executing the program:" + registersPart1[0]);

// Part 2 - Execute the code with four registers starting at zero, except c which starts at 1

int[] registersPart2 = [0,0,1,0];
RunProgram(registersPart2, instructions);
Console.WriteLine("Part 2: Value of register A after executing the program:" + registersPart2[0]);

void RunProgram(int[] pRegisters, string[] pInstructions)
{
    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        string instruction = pInstructions[instructionPointer];
        int offset = ExecuteInstruction(instruction, pRegisters);
        instructionPointer += offset;
    }
}

int ExecuteInstruction(string pInstruction, int[] pRegisters)
{
    string[] instructionParts = pInstruction.Split(" ", StringSplitOptions.RemoveEmptyEntries);
    string _operator = instructionParts[0];

    //cpy x y copies x (either an integer or the value of a register) into register y.
    //inc x increases the value of register x by one.
    //dec x decreases the value of register x by one.
    //jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.

    switch (_operator)
    {
		case "cpy": return ExecuteCpyInstruction(instructionParts, pRegisters); 
		case "inc": return ExecuteAddInstruction(instructionParts, pRegisters, 1); 
		case "dec": return ExecuteAddInstruction(instructionParts, pRegisters, -1); 
		case "jnz": return ExecuteJnzInstruction(instructionParts, pRegisters); 

        default:
            throw new InvalidOperationException("Unknown operator " + _operator);
    }
}

//cpy x y copies x (either an integer or the value of a register) into register y.
int ExecuteCpyInstruction(string[] pInstruction, int[] pRegisters)
{
	int xValue = GetOperandValue(pInstruction[1], pRegisters);

	//Assign it to register y
	pRegisters[pInstruction[2][0] - 'a'] = xValue;
    return 1;
}

//inc x increases the value of register x by one.
//dec x decreases the value of register x by one.
int ExecuteAddInstruction(string[] pInstruction, int[] pRegisters, int v)
{
	pRegisters[pInstruction[1][0] - 'a'] += v;
    return 1;
}

//jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.
int ExecuteJnzInstruction(string[] pInstruction, int[] pRegisters)
{
    int xValue = GetOperandValue(pInstruction[1], pRegisters);
	int yValue = int.Parse(pInstruction[2]);

    return xValue == 0 ? 1 : yValue;   
}

int GetOperandValue (string pOperand, int[] pRegisters)
{
	//Is x a value? 
	if (!int.TryParse(pOperand, out int value))
	{
		//If not, interpret it as register value (take the first char of that string and subtract the a char)
		value = pRegisters[pOperand[0] - 'a'];
	}

    return value;
}