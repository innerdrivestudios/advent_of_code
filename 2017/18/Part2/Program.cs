//Solution for https://adventofcode.com/2017/day/18 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a list of instructions for playing sounds...

string[] myInput = File.ReadAllLines(args[0]);
string[][] instructions = myInput.Select(x => x.Split(' ')).ToArray();

// ** Part 2: Since part 2 requires a different approach to keep it clean I implemented that part in a separate project

SimpleVM machine0 = new SimpleVM(instructions, 0);
SimpleVM machine1 = new SimpleVM(instructions, 1);

machine0.other = machine1;
machine1.other = machine0;

while (true)
{
    SimpleVM.VMState machine0State = machine0.Run();
    SimpleVM.VMState machine1State = machine1.Run();

    //Console.WriteLine(machine0State + "  " + machine1State);

    if (machine0State != SimpleVM.VMState.Stepped && machine1State != SimpleVM.VMState.Stepped) break;
}

Console.WriteLine("Part 2:" + machine1.sendCount);

