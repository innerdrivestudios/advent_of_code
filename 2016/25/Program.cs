//Solution for https://adventofcode.com/2016/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of instructions
// Note this is a modification of the machine from day 12.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// First define the interpreter...

List<int> globalOut = new();

/*
See below...
void RunProgram(int[] pRegisters, string[][] pInstructions)
{
    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        int offset = ExecuteInstruction(pInstructions, instructionPointer, pRegisters);
        instructionPointer += offset;
    }
}
*/

int ExecuteInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters)
{
    string _operator = pInstructions[pInstructionPointer][0];

    //cpy x y copies x (either an integer or the value of a register) into register y.
    //inc x increases the value of register x by one.
    //dec x decreases the value of register x by one.
    //jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.
    //tgl x toggles the instruction x away (see specs for what that means...

    switch (_operator)
    {
		case "cpy": return ExecuteCpyInstruction(pInstructions, pInstructionPointer, pRegisters); 
		case "inc": return ExecuteAddInstruction(pInstructions, pInstructionPointer, pRegisters, 1); 
		case "dec": return ExecuteAddInstruction(pInstructions, pInstructionPointer, pRegisters, -1); 
		case "jnz": return ExecuteJnzInstruction(pInstructions, pInstructionPointer, pRegisters); 
		case "tgl": return ExecuteTglInstruction(pInstructions, pInstructionPointer, pRegisters); 
		case "out": return ExecuteOutInstruction(pInstructions, pInstructionPointer, pRegisters); 

        default:
            throw new InvalidOperationException("Unknown operator " + _operator);
    }
}

// cpy x y copies x (either an integer or the value of a register) into register y.
int ExecuteCpyInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];
	
    //Get the value of the 1st param, whether it is value or register
    int xValue = GetOperandValue(instruction[1], pRegisters);

    if (IsRegister(instruction[2]))
    {
        //Assign it to register y -> cpy 1 a, pInstruction[2] takes 3rd item, subtracts 'a'
        pRegisters[instruction[2][0] - 'a'] = xValue;
    }

    return 1;
}

// inc x increases the value of register x by one.
// dec x decreases the value of register x by one.
int ExecuteAddInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters, int pValue)
{
    string[] instruction = pInstructions[pInstructionPointer];

    if (IsRegister(instruction[1]))
    {
        pRegisters[instruction[1][0] - 'a'] += pValue;
    }

    return 1;
}

// jnz x y jumps to an instruction y away (positive means forward; negative means backward), but only if x is not zero.
int ExecuteJnzInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];
    int xValue = GetOperandValue(instruction[1], pRegisters);
	int yValue = GetOperandValue(instruction[2], pRegisters);

    return xValue == 0 ? 1 : yValue;   
}

int ExecuteTglInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];
    
    int toggleArgument = GetOperandValue(instruction[1], pRegisters);
    int toggleInstructionPointer = pInstructionPointer + toggleArgument;

    if (toggleInstructionPointer >= 0 && toggleInstructionPointer < pInstructions.Length)
    {
        string[] instructionToToggle = pInstructions[pInstructionPointer + toggleArgument];

        if (instructionToToggle.Length == 2)
        {
            instructionToToggle[0] = instructionToToggle[0] == "inc" ? "dec" : "inc";
        }
        else if (instructionToToggle.Length == 3)
        {
            instructionToToggle[0] = instructionToToggle[0] == "jnz" ? "cpy" : "jnz";
        }
    }

    // Console.WriteLine("Toggled " + toggleInstructionPointer);

    return 1;
}

int ExecuteOutInstruction(string[][] pInstructions, int pInstructionPointer, int[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

    globalOut.Add(GetOperandValue(instruction[1], pRegisters));

    return 1;
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

bool IsRegister (string pOperand)
{
    return !int.TryParse(pOperand, out _);
}

// ** Part 1 - Execute the code according to specs trying to find the lowest int to generate an alternating pattern

string[][] instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(
        x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    ).ToArray();

// Let's try and brute force it first...

bool RunProgram(int[] pRegisters, string[][] pInstructions)
{
    globalOut.Clear();

    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        int offset = ExecuteInstruction(pInstructions, instructionPointer, pRegisters);
        instructionPointer += offset;

        if (globalOut.Count == 8) break;
    }

    // Quick brute force check
    return
        globalOut[0] == 0 && globalOut[1] == 1 &&
        globalOut[2] == 0 && globalOut[3] == 1 &&
        globalOut[4] == 0 && globalOut[5] == 1 &&
        globalOut[6] == 0 && globalOut[7] == 1;
}

int startValue = 0;
int[] registersPart1 = [startValue, 0, 0, 0];

while (!RunProgram(registersPart1, instructions))
{
    startValue++;
    registersPart1 = [startValue, 0, 0, 0];
}

Console.WriteLine("Part 1:" + startValue);

// Easy peasy, probably not how it was supposed to work, but it works :)