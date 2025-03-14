// Solution for https://adventofcode.com/2019/day/2 (Ctrl+Click in VS to follow link)


// In visual studio you can modify what input file will be loaded by going to Debug/Debug Properties
// and specifying its path and filename as a command line argument, e.g. "$(SolutionDir)input" 
// This value will be processed and passed to the built-in args[0] variable

// ** Your input: a bunch of program that represent opcode and parameters

string myInput = File.ReadAllText(args[0]);
myInput = myInput.ReplaceLineEndings();

int[] program = myInput.Split(",").Select(int.Parse).ToArray();

// Part 1 - "Run" the program and return the value at position 0
//
// How do you run the progam?
// if program[i] == 1 => program[program[i+3]] = program[program[i+1]] + program[program[i+2]]
// if program[i] == 2 => program[program[i+3]] = program[program[i+1]] * program[program[i+2]]
// if program[i] == 99 => halt the program
// after i => i+4

// Noun and Verb terminology used in part 2
long RunProgram(int[] pProgram, int pNoun, int pVerb)
{
	pProgram = (int[])pProgram.Clone();

	int i = 0;
	pProgram[1] = pNoun;
	pProgram[2] = pVerb;

	while (pProgram[i] != 99)
	{
		if (pProgram[i] == 1)
		{
			pProgram[pProgram[i + 3]] = pProgram[pProgram[i + 1]] + pProgram[pProgram[i + 2]];
		}
		else if (pProgram[i] == 2)
		{
			pProgram[pProgram[i + 3]] = pProgram[pProgram[i + 1]] * pProgram[pProgram[i + 2]];
		}

		i += 4;
	}
	return pProgram[0];
}

Console.WriteLine("Part 1 - Result of program (12,2): " + RunProgram (program, 12, 2)) ;

// Part 2 - Find the verb and noun that make the "valueToAchieve"
// Alternatively we could just run this in a double loop

Console.WriteLine("Part 2:");
Console.WriteLine("Result of program (0,0): " + RunProgram (program, 0, 0));

long valueToAchieve = 19690720;
Console.WriteLine("Value to achieve: " + valueToAchieve);

long baseValue = valueToAchieve - RunProgram(program, 0, 0);
long diffA = RunProgram(program, 1, 0) - RunProgram(program, 0, 0);
long diffB = RunProgram(program, 0, 1) - RunProgram(program, 0, 0);

Console.WriteLine("Calculate the base value, verb stepsize, noun stepsize");
Console.WriteLine(baseValue + " / " + diffA + " / " + diffB);

int verb = (int) (baseValue / diffA);
int noun = (int) (baseValue - ((baseValue / diffA) * diffA));

Console.WriteLine("Verb:" + verb);
Console.WriteLine("Noun:" + noun);

Console.WriteLine($"Test RunProgram(program, {verb}, {noun}): " + RunProgram(program, verb, noun));
Console.WriteLine("100 * verb + noun: " + (100 * verb + noun));

Console.ReadKey();