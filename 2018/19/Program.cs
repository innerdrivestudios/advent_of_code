//Solution for https://adventofcode.com/2018/day/19 (Ctrl+Click in VS to follow link)

using Instruction = (string op, long[] operands);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: A program using the instructions as described in day 16
// Note that this code relies heavily on the code written for that day.

// ** Step 1: Parse the input 

string[] myInput = File.ReadAllLines(args[0]);

List<Instruction> instructions = new();

foreach (string input in myInput)
{
    string[] values = input.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    Instruction instruction = new Instruction(
            values[0],
            values.Skip(1).Select (long.Parse).ToArray()
        );

    instructions.Add(instruction);
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
long registerInstructionPointerIndex = 0;
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

// ** Step 3: Map the operation names to the actual operations

Dictionary<string, Action<long[], long[]>> mappedOperations = new();
mappedOperations["#ip"] = ip;
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

// ** Part 1: Execute the program and check the value of register 0

// Important part: the first instruction is not actually part of the instructions!
// SO:
long[] register = new long[6];

Instruction registerSelectionInstruction = instructions[0];
mappedOperations[registerSelectionInstruction.op](register, registerSelectionInstruction.operands);

instructions.RemoveAt(0);
register[registerInstructionPointerIndex] = 0;

while (true)
{
    long instructionPointer = register[registerInstructionPointerIndex];
    if (instructionPointer < 0 || instructionPointer >= instructions.Count) break;

    Instruction instruction = instructions[(int)instructionPointer];
    
    //Console.Write(
    //    $"ip {instructionPointer} [{string.Join (",", register)}] {instruction.op} {string.Join(",", instruction.operands)} ");

    mappedOperations[instruction.op](register, instruction.operands);
    //Console.WriteLine($"[{string.Join(",", register)}]");
    register[registerInstructionPointerIndex]++;

    //Console.ReadKey();
}

Console.WriteLine("Part 1: " + register[0]);

// ** Part 2: Run it again, without resetting any values, except the register 0 needs to start with value 1


/*
// Used for debugging...

long value = 0;

using (FileStream fs = new FileStream("d:\\test.txt", FileMode.Create))
{
    using (StreamWriter sw = new StreamWriter(fs))
    {

        Console.SetOut(sw);

        register = new long[6];
        register[registerInstructionPointerIndex] = 0;
        register[0] = 1;

        HashSet<string> visited = new HashSet<string>();

        while (true)
        {
            int instructionPointer = (int)register[registerInstructionPointerIndex];
            if (instructionPointer < 0 || instructionPointer >= instructions.Count) break;

            Instruction instruction = instructions[instructionPointer];
           
            //Console.Write(
            //   $"ip {instructionPointer}"                   .PadRight(8)       +
            //   $"[{string.Join("\t", register)}]\t"                            +
            //   $"{instruction.op} "+
            //   $"{string.Join(",", instruction.operands)}\t" 
            //);

            mappedOperations[instruction.op](register, instruction.operands);
            //Console.WriteLine($"[{string.Join("\t", register)}]");
            register[registerInstructionPointerIndex]++;

            if (register[0] != value)
            {
                Console.WriteLine(register[0]);
                value = register[0];
            }

            if (value > 1) break;
        }

        Console.WriteLine("Part 2: " + register[0]);
    }
}
*/

// Ok so after inspecting the program and its output for a long looong (looooong) time, this is what I discovered.
// The program is basically first generating a number:
// With the register set to 0 that was 892 in my case, 
// With the register set to 1 that was 10551292 in my case

// After that it runs, tries to find all factors of that numbers and sums them:

long GenerateFactorSumFor (long pNumber)
{
    long total = 0;

    for (long i = 1; i <= pNumber; i++)
    {
        if (pNumber % i == 0) total += i;
    }

    return total;
}

Console.WriteLine("Part 1 revisited:" + GenerateFactorSumFor(892));
Console.WriteLine("Part 2:" + GenerateFactorSumFor(10551292));







