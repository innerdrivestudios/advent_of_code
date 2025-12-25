//Solution for https://adventofcode.com/2021/day/24 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// First define the interpreter...
bool debug = true;
int maxInputs = 3;
int inputProcessed = 0;
string input = "123";

void RunProgram(long[] pRegisters, string[][] pInstructions)
{
    int instructionPointer = 0;

    //Console.WriteLine("Register values: [" + string.Join ("] [", pRegisters) + "]");

    while (instructionPointer < pInstructions.Length)
    {
        if (debug) Console.ReadKey();
        int offset = ExecuteInstruction(pInstructions, instructionPointer, pRegisters);

        if (debug)
        {
            string leftHalf = "Executed :" + string.Join(" ", pInstructions[instructionPointer]).PadRight(40);
            Console.Write(leftHalf);
            Console.WriteLine("Register values: [" + string.Join("] [", pRegisters) + "]");
        }


        instructionPointer += offset;
    }
}

int ExecuteInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string _operator = pInstructions[pInstructionPointer][0];

    // inp a   - Read an input value and write it to variable a.
    // add a b - Add the value of a to the value of b, then store the result in variable a.
    // mul a b - Multiply the value of a by the value of b, then store the result in variable a.
    // div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a. (Here, "truncate" means to round the value toward zero.)
    // mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)
    // eql a b - If the value of a and b are equal, then store the value 1 in variable a. Otherwise, store the value 0 in variable a.

	switch (_operator)
    {
		case "inp":
            if (inputProcessed >= maxInputs) return 1000;
            return ExecuteInputInstruction      (pInstructions, pInstructionPointer, pRegisters); 
		case "add": return ExecuteAddInstruction        (pInstructions, pInstructionPointer, pRegisters); 
		case "mul": return ExecuteMultiplyInstruction   (pInstructions, pInstructionPointer, pRegisters); 
		case "div": return ExecuteDivideInstruction     (pInstructions, pInstructionPointer, pRegisters); 
		case "mod": return ExecuteModuloInstruction     (pInstructions, pInstructionPointer, pRegisters); 
		case "eql": return ExecuteEqualInstruction      (pInstructions, pInstructionPointer, pRegisters); 

        default:
            throw new InvalidOperationException("Unknown operator " + _operator);
    }
}

// Registers are w, x, y, z
int GetRegisterIndex (string pRegisterIdentifier)
{
    int registerIndex = pRegisterIdentifier[0] - 'w';
    if (registerIndex < 0 || registerIndex > 3) throw new InvalidDataException("Invalid register id:" + pRegisterIdentifier);
    return registerIndex;
}

bool IsRegister(string pOperand)
{
	return !int.TryParse(pOperand, out _);
}

long GetOperandValue(string pOperand, long[] pRegisters)
{
    //Is pOperand a value? 
    if (!long.TryParse(pOperand, out long value))
    {
        //If not, interpret it as register value (take the first char of that string and subtract the a char)
        value = pRegisters[GetRegisterIndex(pOperand)];
    }

    return value;
}


// inp a   - Read an input value and write it to variable a.

int ExecuteInputInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
	string[] instruction = pInstructions[pInstructionPointer];

    int registerIndex = GetRegisterIndex(instruction[1]);

    int value = input[inputProcessed] - '0';
    /*
    int value = 0;

    do
    {
        Console.WriteLine("Enter a valid integer...");
    }
    while (!int.TryParse(Console.ReadLine(), out value));
    */


	pRegisters[registerIndex] = value;

    inputProcessed++;

	return 1;
}

// add a b - Add the value of a to the value of b, then store the result in variable a.

int ExecuteAddInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

	long value1 = GetOperandValue(instruction[1], pRegisters);
	long value2 = GetOperandValue(instruction[2], pRegisters);

	pRegisters[GetRegisterIndex(instruction[1])] = value1 + value2;

    return 1;
}

// mul a b - Multiply the value of a by the value of b, then store the result in variable a.

int ExecuteMultiplyInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

	long value1 = GetOperandValue(instruction[1], pRegisters);
	long value2 = GetOperandValue(instruction[2], pRegisters);

	pRegisters[GetRegisterIndex(instruction[1])] = value1 * value2;

    return 1;
}

// div a b - Divide the value of a by the value of b, truncate the result to an integer, then store the result in variable a.
// (Here, "truncate" means to round the value toward zero.)

int ExecuteDivideInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

	long value1 = GetOperandValue(instruction[1], pRegisters);
	long value2 = GetOperandValue(instruction[2], pRegisters);

	pRegisters[GetRegisterIndex(instruction[1])] = (long) (value1 / (double)value2);

    return 1;
}

// mod a b - Divide the value of a by the value of b, then store the remainder in variable a. (This is also called the modulo operation.)

int ExecuteModuloInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

	long value1 = GetOperandValue(instruction[1], pRegisters);
	long value2 = GetOperandValue(instruction[2], pRegisters);

	pRegisters[GetRegisterIndex(instruction[1])] = value1 % value2;

    return 1;
}

// eql a b - If the value of a and b are equal, then store the value 1 in variable a. Otherwise, store the value 0 in variable a.

int ExecuteEqualInstruction(string[][] pInstructions, int pInstructionPointer, long[] pRegisters)
{
    string[] instruction = pInstructions[pInstructionPointer];

    long value1 = GetOperandValue(instruction[1], pRegisters);
    long value2 = GetOperandValue(instruction[2], pRegisters);

    pRegisters[GetRegisterIndex(instruction[1])] = (value1 == value2) ? 1 : 0;

    return 1;
}


// ** Part 1 - Execute the code according to specs with four registers starting at 0,0,0,0
// ** Your input: a sequence of instructions

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[][] instructions = myInput
    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
    .Select(
        x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    ).ToArray();

// debug = false;
// maxInputs = 8;
// long[] registersPart1 = [0, 0, 0, 0];

// Ok so no code solution...
// In the end I migrated/converted the whole program into an excel sheet...
// and then I meddled around ENDLESSLY...
// There is a certain relation between the different numbers...

// See the excel sheet....

// Ok, after solving this the hard way, by trying to detect relations between numbers,
// I finally 'kinda' got the idea. It has nothing to do with bits
// (I definitely went down the wrong track thanks to the binary example)
// This is a base 26 kind of stack, where multiplying numbers by 26 puts them on the stack,
// and dividing by 26 gets things of the stack. 
// This approach, even if not solving it by code, at least gives you an idea of which pairs have to match.
