//Solution for https://adventofcode.com/2017/day/23 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of instructions ...

string[] myInput = File.ReadAllLines(args[0]);
string[][] instructions = myInput.Select(x => x.Split(' ')).ToArray();

// ** Part 1: How many times in mul executed?

SimpleVM machine1 = new SimpleVM(instructions);

while (true)
{
    SimpleVM.VMState machine1State = machine1.Run();

    if (machine1State != SimpleVM.VMState.Stepped) break;
}

Console.WriteLine("Part 1:" + machine1.GetInstructionCount("mul"));

// ** Part 2: What is the value in register h?

// Check the xls, if we reverse engineer the program, we can see we are testing
// d * e for every combination of d and e from 2 to b
// If d * e, we set f to zero
// If f is zero we increase h
// In other words, we count the number of numbers that can be factorized between b and c

// First lets get values b and c, these should be set, once f is set to 1

SimpleVM machine2 = new SimpleVM(instructions);
machine2.SetRegisterValue("a", 1);
//machine2.log = true;

while (true)
{

    SimpleVM.VMState machine2State = machine2.Run();

    if (machine2State != SimpleVM.VMState.Stepped || machine2.GetRegisterValue("f") != 0) break;
}

long b = machine2.GetRegisterValue("b");
long c = machine2.GetRegisterValue("c");

int nonPrimeCount = 0;

for(long i = b; i <= c; i += 17)
{
    nonPrimeCount += IsPrime(i) ? 0 : 1;
}

bool IsPrime (long pValue)
{
    long top = (long)Math.Ceiling(Math.Sqrt(pValue));
    for (int i = 2; i <= top;i++)
    {
        if (pValue % i == 0) return false;
    }
    return true;
}

Console.WriteLine("Part 2:" + nonPrimeCount);