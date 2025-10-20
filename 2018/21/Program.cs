//Solution for https://adventofcode.com/2018/day/21 (Ctrl+Click in VS to follow link)

using System.Diagnostics;
using Instruction = (string op, long[] operands);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A program using the instructions as described in day 16
// Note that this code relies heavily on the code written for that day.

// ** Step 1: Parse the input 

string[] myInput = File.ReadAllLines(args[0]);

List<Instruction> instructions = new();
long[] register;
long registerInstructionPointerIndex = 0;

void InitializeProgram()
{
    instructions.Clear();

    foreach (string input in myInput)
    {
        string[] values = input.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        Instruction instruction = new Instruction(
                values[0],
                values.Skip(1).Select(long.Parse).ToArray()
            );

        instructions.Add(instruction);
    }

    // Important part: the first instruction is not actually part of the instructions!
    register = new long[6];

    Instruction registerSelectionInstruction = instructions[0];
    instructions.RemoveAt(0);
    ip(register, registerSelectionInstruction.operands);
}

// ** Step 2: Define a bunch of helper methods...

// Let's define some helper methods first and variables:

// Note these are not bounds checked we expect the input to be valid

const int INPUT_A = 0;
const int INPUT_B = 1;
const int OUTPUT_C = 2;

long GetRegisterValue (long[] pRegisters, long pRegisterIndex)
{
    //This was a tricky one ;), some operations ignore parameter A or B, but if we still
    //request a value from the registry based on param A or B while it should be ignored, 
    //we might be using faulty indices and cause out of bounds exceptions.
    //So we "fixed" it by just return 0 for the requested value, since it is going to be ignored after anyway...
    if (pRegisterIndex >= 0 && pRegisterIndex < pRegisters.Length) return pRegisters[pRegisterIndex];
    else return 0;
}

void SetRegisterValue(long[] pRegisters, long pRegisterIndex, long pValue)
{
    pRegisters[pRegisterIndex] = pValue;
}

// Now define all operations, we have 3 distinct formats:

void ApplyRegisterABOperation(long[] pRegister, long[] pOperands, Func<long, long, long> pOperation)
{
    SetRegisterValue(
        pRegister,
        pOperands[OUTPUT_C],
        pOperation(
            GetRegisterValue(pRegister, pOperands[INPUT_A]),
            GetRegisterValue(pRegister, pOperands[INPUT_B])
        )
    );
}

void ApplyRegisterAValueBOperation(long[] pRegister, long[] pOperands, Func<long, long, long> pOperation)
{
    SetRegisterValue(
        pRegister,
        pOperands[OUTPUT_C],
        pOperation(
            GetRegisterValue(pRegister, pOperands[INPUT_A]),
            pOperands[INPUT_B]
        )
    );
}

void ApplyValueARegisterBOperation(long[] pRegister, long[] pOperands, Func<long, long, long> pOperation)
{
    SetRegisterValue(
        pRegister,
        pOperands[OUTPUT_C],
        pOperation(
            pOperands[INPUT_A],
            GetRegisterValue(pRegister, pOperands[INPUT_B])
        )
    );
}

// Set register instruction pointer index
void ip(long[] pRegister, long[] pOperands) => registerInstructionPointerIndex = pOperands[0];

// Addition:
// addr(add register) stores into register C the result of adding register A and register B.
void addr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a + b);

// addi (add immediate) stores into register C the result of adding register A and value B.
void addi(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a + b);

// Multiplication:
// mulr(multiply register) stores into register C the result of multiplying register A and register B.
void mulr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a * b);

// muli (multiply immediate) stores into register C the result of multiplying register A and value B.
void muli(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a * b);

// Bitwise AND:
// banr(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
void banr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a & b);

// bani (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
void bani(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a & b);

// Bitwise OR:
// borr(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
void borr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a | b);

// bori (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
void bori(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a | b);

// Assignment:
// setr(set register) copies the contents of register A into register C. (Input B is ignored.)
void setr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a);

// seti (set immediate) stores value A into register C. (Input B is ignored.)
void seti(long[] pRegister, long[] pOperands) => ApplyValueARegisterBOperation(pRegister, pOperands, (a, b) => a);

// Greater-than testing:
// gtir(greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
void gtir(long[] pRegister, long[] pOperands) => ApplyValueARegisterBOperation(pRegister, pOperands, (a, b) => a > b ? 1 : 0);

// gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
void gtri(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a > b ? 1 : 0);

// gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
void gtrr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a > b ? 1 : 0);

// Equality testing:
// eqir(equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
void eqir(long[] pRegister, long[] pOperands) => ApplyValueARegisterBOperation(pRegister, pOperands, (a, b) => a == b ? 1 : 0);

// eqri (equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
void eqri(long[] pRegister, long[] pOperands) => ApplyRegisterAValueBOperation(pRegister, pOperands, (a, b) => a == b ? 1 : 0);

// eqrr (equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
void eqrr(long[] pRegister, long[] pOperands) => ApplyRegisterABOperation(pRegister, pOperands, (a, b) => a == b ? 1 : 0);

// no operation
void nop(long[] pRegister, long[] pOperands) { }

// ** Step 3: Map the operation names to the actual operations

Dictionary<string, Action<long[], long[]>> mappedOperations = new();
mappedOperations["setr"] = setr;
mappedOperations["eqrr"] = eqrr;
mappedOperations["gtri"] = gtri;
mappedOperations["muli"] = muli;
mappedOperations["eqir"] = eqir;
mappedOperations["borr"] = borr;
mappedOperations["bori"] = bori;
mappedOperations["mulr"] = mulr;
mappedOperations["gtrr"] = gtrr;
mappedOperations["seti"] = seti;
mappedOperations["banr"] = banr;
mappedOperations["eqri"] = eqri;
mappedOperations["addr"] = addr;
mappedOperations["gtir"] = gtir;
mappedOperations["addi"] = addi;
mappedOperations["bani"] = bani;
mappedOperations["nop"] = nop;

// ** Part 1: Find the value for register[0] that causes the program to stop after the minimum amount of repetitions:

InitializeProgram();
bool debug = false;

// Looking at the program (which is incomprehensible as usual), there is a key end comparison:
// "eqrr 5 0 3" which basically says, if register 0 equals register 5 store a 1 in register 3
// The next line "addr 3 4 4" adds register 3 to the instruction pointer causing the program to either repeat or stop.
// In other words for part 1, the first time the program hits the instruction eqrr 5 0 3, we should stop and print
// the value of register 5.
// The instruction is on line 30, but since we removed line 1 and everything is 0 based, it is actually line 28

// Also see assembly.xlsx

while (true)
{
    long instructionPointer = register[registerInstructionPointerIndex];

    if (instructionPointer == 28)
    {
        break;
    }

    if (instructionPointer < 0 || instructionPointer >= instructions.Count) break;

    Instruction instruction = instructions[(int)instructionPointer];

    if (debug) Console.Write(
        $"ip {instructionPointer} [{string.Join(",", register)}] {instruction.op} {string.Join(",", instruction.operands)} ");

    mappedOperations[instruction.op](register, instruction.operands);
    if (debug) Console.WriteLine($"[{string.Join(",", register)}]");
    register[registerInstructionPointerIndex]++;

    if (debug) Console.ReadKey();
}

Console.WriteLine("Part 1: " + register[5]);

// ** Part 2: This one is a little bit harder or a lot harder based on your approach.

// HARD:    TRY to figure out what the program is actually doing
// SIMPLE:  RUN the program until register[5] hits a duplicate and print the value before that.
//          This last approach takes a while, but way faster than me trying to find out what the program actually does.

HashSet<long> values = new();
InitializeProgram();
long last = 0;

while (true)
{
    long instructionPointer = register[registerInstructionPointerIndex];

    if (instructionPointer == 28) {
        if (!values.Add(register[5]))
        {
            break;
        }
        else { 
            last = register[5];
        }
    }

    if (instructionPointer < 0 || instructionPointer >= instructions.Count) break;

    Instruction instruction = instructions[(int)instructionPointer];
    mappedOperations[instruction.op](register, instruction.operands);
    register[registerInstructionPointerIndex]++;
}

Console.WriteLine("Part 2: " + last);
