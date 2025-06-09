//Solution for https://adventofcode.com/2018/day/16 (Ctrl+Click in VS to follow link)

using Sample = (int[] before, int[] operation, int[] after);

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input:

// The first section of your puzzle input contains samples.
// The second section includes a small test program - you can ignore it for now.
// The input samples describe 2 sets of register values (4 each),
// separated by the operation that transformed the 1st register set into the 2nd
// (as described in the puzzle description)
// (Issue is we don't know which opcode matches which operation)

// But first things first: parsing the input :).

string myInput = File.ReadAllText(args[0]).ReplaceLineEndings();

string[] samplesAndTests = myInput.Split (
    Environment.NewLine + Environment.NewLine + Environment.NewLine,
    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
);

// How are we going to store all samples? As 3 sets of int[] :)

Sample[] samples = 
    // Take the whole string:
    samplesAndTests[0]
    // Split it into only number strings (all given numbers in a row)
    .Split(["Before:", "[", ",", "]", "After:", " "], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    // Convert them to integers...
    .Select(int.Parse)
    // Chunk 'm in sets of int[] arrays of length 4
    .Chunk(4).Select(x => x.ToArray())
    // Chuck these into triplets 
    .Chunk(3).Select(x => (x[0], x[1], x[2]))
    .ToArray();

// In addition we need to define each operation, to see if that operation would result the expected outcome.
// In the end for part 1 we need to find how many samples in your puzzle input behave like three or more opcodes.
// For now that basically means, how many operations match the given sample output when applying that operation to the sample input.

// Let's define some helper methods first and variables:

// Note these are not bounds checked we expect the input to be valid

const int OPERATION = 0;
const int INPUT_A = 1;
const int INPUT_B = 2;
const int OUTPUT_C = 3;

int GetRegisterValue (int[] pRegisters, int pRegisterIndex)
{
    return pRegisters[pRegisterIndex];
}

void SetRegisterValue(int[] pRegisters, int pRegisterIndex, int pValue)
{
    pRegisters[pRegisterIndex] = pValue;
}

bool RegistersMatch(int[] pRegistersA, int[] pRegistersB)
{
    for (int i = 0; i < pRegistersA.Length; i++)
    {
        if (pRegistersA[i] != pRegistersB[i]) return false;
    }
    return true;
}

// Now define all operations, we have 3 distinct formats:

int[] ApplyRegisterABOperation(Sample pSample, Func<int, int, int> pOperation)
{
    int[] result = pSample.before.ToArray();

    SetRegisterValue(
        result,
        pSample.operation[OUTPUT_C],
        pOperation(
            GetRegisterValue(pSample.before, pSample.operation[INPUT_A]),
            GetRegisterValue(pSample.before, pSample.operation[INPUT_B])
        )
    );

    return result;
}

int[] ApplyRegisterAValueBOperation(Sample pSample, Func<int, int, int> pOperation)
{
    int[] result = pSample.before.ToArray();

    SetRegisterValue(
        result,
        pSample.operation[OUTPUT_C],
        pOperation(
            GetRegisterValue(pSample.before, pSample.operation[INPUT_A]),
            pSample.operation[INPUT_B]
        )
    );

    return result;
}

int[] ApplyValueARegisterBOperation(Sample pSample, Func<int, int, int> pOperation)
{
    int[] result = pSample.before.ToArray();

    SetRegisterValue(
        result,
        pSample.operation[OUTPUT_C],
        pOperation(
            pSample.operation[INPUT_A],
            GetRegisterValue(pSample.before, pSample.operation[INPUT_B])
        )
    );

    return result;
}

// Addition:
// addr(add register) stores into register C the result of adding register A and register B.
int[] addr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a + b);

// addi (add immediate) stores into register C the result of adding register A and value B.
int[] addi(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a + b);

// Multiplication:
// mulr(multiply register) stores into register C the result of multiplying register A and register B.
int[] mulr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a * b);

// muli (multiply immediate) stores into register C the result of multiplying register A and value B.
int[] muli(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a * b);

// Bitwise AND:
// banr(bitwise AND register) stores into register C the result of the bitwise AND of register A and register B.
int[] banr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a & b);

// bani (bitwise AND immediate) stores into register C the result of the bitwise AND of register A and value B.
int[] bani(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a & b);

// Bitwise OR:
// borr(bitwise OR register) stores into register C the result of the bitwise OR of register A and register B.
int[] borr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a | b);

// bori (bitwise OR immediate) stores into register C the result of the bitwise OR of register A and value B.
int[] bori(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a | b);

// Assignment:
// setr(set register) copies the contents of register A into register C. (Input B is ignored.)
int[] setr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a);

// seti (set immediate) stores value A into register C. (Input B is ignored.)
int[] seti(Sample pSample) => ApplyValueARegisterBOperation(pSample, (a, b) => a);

// Greater-than testing:
// gtir(greater - than immediate / register) sets register C to 1 if value A is greater than register B. Otherwise, register C is set to 0.
int[] gtir(Sample pSample) => ApplyValueARegisterBOperation(pSample, (a, b) => a > b ? 1 : 0);

// gtri (greater-than register/immediate) sets register C to 1 if register A is greater than value B. Otherwise, register C is set to 0.
int[] gtri(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a > b ? 1 : 0);

// gtrr (greater-than register/register) sets register C to 1 if register A is greater than register B. Otherwise, register C is set to 0.
int[] gtrr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a > b ? 1 : 0);

// Equality testing:
// eqir(equal immediate / register) sets register C to 1 if value A is equal to register B. Otherwise, register C is set to 0.
int[] eqir(Sample pSample) => ApplyValueARegisterBOperation(pSample, (a, b) => a == b ? 1 : 0);

// eqri (equal register/immediate) sets register C to 1 if register A is equal to value B. Otherwise, register C is set to 0.
int[] eqri(Sample pSample) => ApplyRegisterAValueBOperation(pSample, (a, b) => a == b ? 1 : 0);

// eqrr (equal register/register) sets register C to 1 if register A is equal to register B. Otherwise, register C is set to 0.
int[] eqrr(Sample pSample) => ApplyRegisterABOperation(pSample, (a, b) => a == b ? 1 : 0);

// Now that all operations have been defined, we'll store them in a list:

Func<Sample, int[]>[] operations = [ addr, addi, mulr, muli, banr, bani, borr, bori, setr, seti, gtir, gtri, gtrr, eqir, eqri, eqrr ];

// Last but not least for part 1, we run over all samples, count how many operations yield the expected output

int matchingOperationCount = samples.Count(sample => operations.Count(operation => RegistersMatch(operation(sample), sample.after)) >= 3);
Console.WriteLine("Part 1:" +matchingOperationCount);

// ** Part 2:
// Using the samples you collected, work out the number of each opcode and execute the test program (the second section of your puzzle input).
// What value is contained in register 0 after executing the test program?

// So first, we need to work out the number of each opcode...

Console.WriteLine();
Console.WriteLine("Part 2 ...");

(List<int> matchingOpcodes, Func<Sample, int[]> operation)[] operationsToMap = [
    (new(),addr), 
    (new(),addi), 
    (new(),mulr), 
    (new(),muli), 
    (new(),banr), 
    (new(),bani), 
    (new(),borr), 
    (new(),bori), 
    (new(),setr), 
    (new(),seti), 
    (new(),gtir), 
    (new(),gtri), 
    (new(),gtrr), 
    (new(),eqir), 
    (new(),eqri),
    (new(),eqrr)
];

// First derive all options of opcodes ...

Console.WriteLine("Deriving opcodes options ...");

for (int opcode = 0; opcode < 16; opcode++)
{
    //First all samples with a given opcode... (x.operation[0] equals the opcode
    Sample[] samplesWithThisOpcode = samples.Where (x => x.operation[OPERATION] == opcode).ToArray();
    Console.WriteLine("Testing opcode " + opcode + " " + samplesWithThisOpcode.Length + " samples...");

    //Then loop over all possible operations that could match this opcode...
    for (int operation = 0; operation < operationsToMap.Length; operation++)
    {
        //Matching sample count for this operation:
        int matchingSamples = samplesWithThisOpcode.Count(x => RegistersMatch(operationsToMap[operation].operation(x), x.after));

        if (samplesWithThisOpcode.Length == matchingSamples)
        {
            operationsToMap[operation].matchingOpcodes.Add(opcode);
        }
    }
}

Console.WriteLine();
Console.WriteLine("Options overview:");

foreach (var mappedOperation in operationsToMap)
{
    Console.WriteLine(String.Join(",", mappedOperation.matchingOpcodes) + " => " + mappedOperation.operation.Method.Name);
}

// In the output you can see this is a reducible list, 
// which means that there is one list item that only has 1 option.
// If we remove that item from the list (since we know which opcode it matches to)
// AND that item's opcode from the possible opcode option list for all other operations, 
// we'll end up with ANOTHER item with only one opcode
// So we'll repeat this process until there is no item left...

List<(List<int> matchingOpcodes, Func<Sample, int[]> operation)> optionsOverview = operationsToMap.ToList();

// After reducing the number of options to 1, we'll store the final 
Dictionary<int, Func<Sample, int[]>> mappedOperations = new();

Console.WriteLine();
Console.WriteLine("Reducing options:");

while (optionsOverview.Count > 0)
{
    Console.WriteLine(optionsOverview.Count + " items to reduce...");
    
    // Find all items with a single option
    var itemsWithSingleOption = optionsOverview.Where(x => x.matchingOpcodes.Count == 1);

    // Just pick the first one and remove that item from the list
    var itemWithSingleOption = itemsWithSingleOption.First();
    optionsOverview.Remove(itemWithSingleOption);

    //And remove that item's option from the option list of all other items...
    foreach (var item in optionsOverview)
    {
        item.matchingOpcodes.Remove(itemWithSingleOption.matchingOpcodes[0]);
    }

    mappedOperations[itemWithSingleOption.matchingOpcodes[OPERATION]] = itemWithSingleOption.operation;
}

Console.WriteLine();
Console.WriteLine("Deducted mapping table:");
foreach (var mappedOperation in mappedOperations.OrderBy(x => x.Key))
{
    Console.WriteLine(mappedOperation.Key + " => " + mappedOperation.Value.Method.Name);
}

Console.WriteLine();
Console.WriteLine("Executing program with mapped operations:");

List<int[]> instructions =
    // Take the whole string:
    samplesAndTests[1]
    // Split it into only number strings (all given numbers in a row)
    .Split([" ", Environment.NewLine], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    // Convert them to integers...
    .Select(int.Parse)
    // Chunk 'm in sets of int[] arrays of length 4
    .Chunk(4).Select(x => x.ToArray())
    .ToList();

// The next setup is a little bit convoluted, setting up and reusing the Sample type this way,
// but otherwise we have to rewrite the whole sample/function/mapping setup

                           //Registers          //Operation to overwrite       //Not used
Sample executionContext = ([0, 0, 0, 0],        [0, 0, 0, 0],                  [0, 0, 0, 0]);

foreach (var instruction in instructions)
{
    //Store the operation in the context
    executionContext.operation = instruction;
    //Pass the sample/context to the correctly mapped operation and store the result in before (we are ignoring the after part)
    executionContext.before = mappedOperations[instruction[OPERATION]](executionContext);
}

Console.WriteLine("Part 2:" + executionContext.before[0]);

