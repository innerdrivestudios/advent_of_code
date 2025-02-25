// Solution for https://adventofcode.com/2017/day/8 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of register instructions with conditions like this:

// b inc 5 if a > 1
// a inc 1 if b < 5
// c dec -10 if a >= 1
// c inc -20 if c == 10

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings(Environment.NewLine);

string[] instructions = myInput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

// Let's define some helper variables and functions first:

Dictionary<string, int> registers = new();

// for part 2
int highestValueEver = int.MinValue;	

// Returns the value of a register or zero if it wasn't defined yet...
int GetRegisterValue(string pRegister)
{
	registers.TryGetValue(pRegister, out int registerValue);
	return registerValue;
}

// Modify the value of a register positive or negative
void ModifyRegister (string pRegister, int pDelta)
{
	int newValue = GetRegisterValue(pRegister) + pDelta;
	registers[pRegister] = newValue;

	highestValueEver = int.Max(highestValueEver, newValue);
}

// Simply evaluate the condition part as is
bool EvaluateCondition (string pCondition)
{
	//c != 4
	string[] conditionParts = pCondition.Split(" ");
	int registerValue = GetRegisterValue(conditionParts[0]);
	int comparisonValue = int.Parse(conditionParts[2]);

	switch (conditionParts[1])
	{
		case "==": return registerValue == comparisonValue;
		case "!=": return registerValue != comparisonValue;
		case ">": return registerValue > comparisonValue;
		case ">=": return registerValue >= comparisonValue;
		case "<": return registerValue < comparisonValue;
		case "<=": return registerValue <= comparisonValue;
	}

	throw new NotImplementedException();
}

// Evaluate the register instruction
void EvaluateRegisterInstruction(string pRegisterInstruction)
{
	//b inc 5
	string[] registerInstructionParts = pRegisterInstruction.Split(" ");
	string register = registerInstructionParts[0];
	int multiplier = registerInstructionParts[1] == "inc" ? 1 : -1;
	int value = int.Parse(registerInstructionParts[2]);

	ModifyRegister(register, multiplier * value);
}

// ** Part 1 & 2: Execute all instructions and
// - print the biggest end value
// - print the biggest value ever

foreach (string instruction in instructions)
{
	string[] instructionParts = instruction.Split(" if ");

	if (EvaluateCondition(instructionParts[1])) EvaluateRegisterInstruction(instructionParts[0]);
}

Console.WriteLine("Part 1 - Biggest final register value:" + registers.Values.Max());
Console.WriteLine("Part 2 - Biggest register value ever:" + highestValueEver);
