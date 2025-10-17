//Solution for https://adventofcode.com/2016/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a sequence of instructions
// Note this is a modification of the machine from day 12.

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

// First define the interpreter...

void RunProgram(int[] pRegisters, string[][] pInstructions)
{
    int instructionPointer = 0;

    while (instructionPointer < pInstructions.Length)
    {
        int offset = ExecuteInstruction(pInstructions, instructionPointer, pRegisters);
        instructionPointer += offset;
    }
}

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

// ** Part 1 - Execute the code according to specs with four registers starting at 7,0,0,0

string[][] instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(
        x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    ).ToArray();

int[] registersPart1 = [7, 0, 0, 0];
RunProgram(registersPart1, instructions);
Console.WriteLine("Part 1: Value of register A after executing the program:" + registersPart1[0]);


// ** Part 2 - Execute the code according to specs with four registers starting at 12,0,0,0

// Don't forget to reset the instructions :)

instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(
        x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    ).ToArray();

int[] registersPart2 = [12, 0, 0, 0];
RunProgram(registersPart2, instructions);
Console.WriteLine("Part 2: Value of register A after executing the program:" + registersPart2[0]);

// Or using the quick version:
Console.WriteLine(Fact(7) + 94 * 82);
Console.WriteLine(Fact(12) + 94 * 82);

long Fact (long pValue)
{
    return (pValue <= 1) ? 1 : pValue * Fact(pValue - 1); 
}

// I found this out by inspecting the log in detail for part 1.
// Note however that these numbers will differ based on your input,
// the program first calculates the factorial and then adds the numbers found here:
// cpy 94 c
// jnz 82 d

// The best solution would probably be to add the multiply instruction as part 2 hints at
// but I'll leave that to a future day when it turns out to be REALLY necessary :).
// Also note for that day, that the toggle instruction only changed code that concerns the 82*94 part.