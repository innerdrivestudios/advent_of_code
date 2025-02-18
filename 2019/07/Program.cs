// Solution for https://adventofcode.com/2019/day/7 (Ctrl+Click in VS to follow link)

// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings("");

int[] program = myInput.Split(",").Select(int.Parse).ToArray();

// ** Part 1: Run a modified version of the intcode computer from day 5
// Biggest change:
// - input now comes from a queue and output is a single return value
// - we need to run 5 of these programs in sequence, configuring them with both
//   - a fixed input determined by the provided configuration (a permutation of 0,1,2,3,4)
//   - an input resulting from the output of the previous program run (program is called an amplifier)
// - of course for the first program there is no pre-existing output from a previous program so we use 0

int ConfigureAmplifiersPart1(List<int> pConfigurations)
{
	// Set up the first input for each amplifier
	Queue<int> inputA = new Queue<int>(pConfigurations);

	// Set up the 2nd input for the first amplifier, rest will follow as each amplifier completes
	Queue<int> inputB = new();
	inputB.Enqueue(0);

	// We need to give each amplifier/run of the program 2 inputs, we'll do that with another queue
	Queue<int> programInput = new();

	// While we still have main amplifier input to process
	while (inputA.Count > 0)
	{
		programInput.Enqueue(inputA.Dequeue());	//Queue input 1
		programInput.Enqueue(inputB.Dequeue());	//Assume the previous amplifier also resulted in new input 

		int output = ModifiedIntCodeComputer_Part1.Run(program, programInput);
		inputB.Enqueue(output);
	}

    return inputB.Dequeue();
}

// Get all permutations of 0,1,2,3,4,
// use them to configure the "amplifiers" and
// calculate what the maximum output is:

List<List<int>> possibleConfigurations1 = new List<int>([0, 1, 2, 3, 4]).GetPermutations();
int maxOutput1 = possibleConfigurations1.Max(x => ConfigureAmplifiersPart1(x));
Console.WriteLine("Part 1 - Output:"+maxOutput1);

// ** Part 2
//
// Part 2 was maybe one of the most difficult descriptions to wrap my head around of all 
// puzzles I've done so far, even though we are only on day 7 for this year ;).

// Part of it also stems from the fact that it seems we need to run separate threads
// with producer / consumer queues feeding back into each other,
// which seems like a debugging nightmare...
// instead I opted for a more polling/coroutine like approach

// Instead of the static ModifiedIntCodeComputer_Part1,
// we'll create 5 instances of the ModifiedIntCodeComputer_Part2
// running one after the other, pausing only when waiting for more input...
// and stopping only when the program has exited with code 99

// After we've fixed that, we'll need to run THAT process over and over again for all
// permutations of [5,6,7,8,9] and try to find the highest value

int ConfigureAmplifiersPart2(List<int> pConfigurations)
{
	// First set up all amplifiers...
	Queue<ModifiedIntCodeComputer_Part2> amplifiers = new();

	foreach (int configuration in pConfigurations)
	{
		amplifiers.Enqueue (new ModifiedIntCodeComputer_Part2(program, configuration));
	}

	// Queue 0 for the first amplifier
	amplifiers.Peek().QueueInputs([0]);

	List<int> lastOutputs = new();

	while (amplifiers.Count > 0)
	{
		var amplifier = amplifiers.Dequeue();
		amplifier.QueueInputs(lastOutputs);
		ModifiedIntCodeComputer_Part2.ProgramState result = amplifier.Run();
        
		//Console.WriteLine("Amplifier result:" + result);

		if (result == ModifiedIntCodeComputer_Part2.ProgramState.WAITING_FOR_INPUT)
		{
			// Queue it to run again later...
			amplifiers.Enqueue(amplifier);
		}

		lastOutputs = amplifier.GetOutputs();
        
		//Console.WriteLine("Output count: " + lastOutputs.Count);
    }

	return lastOutputs[0];
}

// Get all permutations of 5,6,7,8,9,
// use them to configure the "amplifiers" and
// calculate what the maximum output is

List<List<int>> possibleConfigurations2 = new List<int>([5,6,7,8,9]).GetPermutations();
int maxOutput2 = possibleConfigurations2.Max(x => ConfigureAmplifiersPart2(x));
Console.WriteLine("Part 2 - Output:" + maxOutput2);
